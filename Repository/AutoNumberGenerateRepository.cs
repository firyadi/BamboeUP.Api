using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    /// <summary>
    /// Repository khusus untuk Generate Number Engine.
    /// Semua method di sini HARUS dipanggil di dalam transaction aktif
    /// karena menggunakan SQL locking hints (UPDLOCK, ROWLOCK) untuk mencegah race condition.
    /// </summary>
    public class AutoNumberGenerateRepository : IAutoNumberGenerateRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public AutoNumberGenerateRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        /// <summary>
        /// Ambil counter dengan ROW LOCK, atau buat baru jika belum ada.
        /// 
        /// Strategi UPSERT yang aman untuk concurrent access:
        /// 1. Coba INSERT dengan IF NOT EXISTS (mencegah duplicate)
        /// 2. SELECT with row lock for the duration of the transaction
        /// 
        /// Dengan pola ini, dua request concurrent yang sama-sama tidak menemukan counter
        /// akan berlomba di INSERT. Salah satu menang, satunya tidak (tapi tidak error).
        /// Keduanya kemudian SELECT baris yang sama dengan lock, lalu menunggu giliran increment.
        /// </summary>
        public async Task<AutoNumberCounter> GetOrCreateCounterWithLockAsync(
            long templateId,
            long? companyId,
            long? companyOfficeId,
            long? organizationUnitId,
            int? yearNo,
            int? monthNo,
            int? dayNo,
            IDbTransaction transaction)
        {
            var conn = transaction.Connection!;

            var createdTime = DateTime.UtcNow;
            // Step 1: INSERT jika belum ada (menggunakan NOT EXISTS agar idempoten)
            var insertSql = $@"
                INSERT INTO {_dialect.Qualify("app", "AutoNumberCounter")}
                    (AutoNumberCounterGuid, AutoNumberTemplateId,
                     CompanyId, CompanyOfficeId, OrganizationUnitId,
                     YearNo, MonthNo, DayNo,
                     LastNumber, StatusId, CreatedById, CreatedTime)
                SELECT
                    {_dialect.NewGuidExpression}, @TemplateId,
                    @CompanyId, @CompanyOfficeId, @OrganizationUnitId,
                    @YearNo, @MonthNo, @DayNo,
                    0, 1, 0, @CreatedTime
                WHERE NOT EXISTS (
                    SELECT 1 FROM {_dialect.Qualify("app", "AutoNumberCounter")}
                    WHERE AutoNumberTemplateId = @TemplateId
                      AND (CompanyId         = @CompanyId         OR (CompanyId         IS NULL AND @CompanyId         IS NULL))
                      AND (CompanyOfficeId   = @CompanyOfficeId   OR (CompanyOfficeId   IS NULL AND @CompanyOfficeId   IS NULL))
                      AND (OrganizationUnitId= @OrganizationUnitId OR (OrganizationUnitId IS NULL AND @OrganizationUnitId IS NULL))
                      AND (YearNo            = @YearNo            OR (YearNo            IS NULL AND @YearNo            IS NULL))
                      AND (MonthNo           = @MonthNo           OR (MonthNo           IS NULL AND @MonthNo           IS NULL))
                      AND (DayNo             = @DayNo             OR (DayNo             IS NULL AND @DayNo             IS NULL))
                      AND StatusId > 0
                      AND DeletedTime IS NULL
                )";

            await conn.ExecuteAsync(insertSql, new
            {
                TemplateId        = templateId,
                CompanyId         = companyId,
                CompanyOfficeId   = companyOfficeId,
                OrganizationUnitId= organizationUnitId,
                YearNo            = yearNo,
                MonthNo           = monthNo,
                DayNo             = dayNo,
                CreatedTime       = createdTime
            }, transaction);

            // Step 2: lock row for the duration of the transaction
            var selectSql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")}
                FROM {_dialect.Qualify("app", "AutoNumberCounter")}{_dialect.TableLockHint}
                WHERE AutoNumberTemplateId = @TemplateId
                  AND (CompanyId         = @CompanyId         OR (CompanyId         IS NULL AND @CompanyId         IS NULL))
                  AND (CompanyOfficeId   = @CompanyOfficeId   OR (CompanyOfficeId   IS NULL AND @CompanyOfficeId   IS NULL))
                  AND (OrganizationUnitId= @OrganizationUnitId OR (OrganizationUnitId IS NULL AND @OrganizationUnitId IS NULL))
                  AND (YearNo            = @YearNo            OR (YearNo            IS NULL AND @YearNo            IS NULL))
                  AND (MonthNo           = @MonthNo           OR (MonthNo           IS NULL AND @MonthNo           IS NULL))
                  AND (DayNo             = @DayNo             OR (DayNo             IS NULL AND @DayNo             IS NULL))
                  AND StatusId > 0
                  AND DeletedTime IS NULL{_dialect.ForUpdateSuffix}";

            var counter = await conn.QueryFirstOrDefaultAsync<AutoNumberCounter>(selectSql, new
            {
                TemplateId        = templateId,
                CompanyId         = companyId,
                CompanyOfficeId   = companyOfficeId,
                OrganizationUnitId= organizationUnitId,
                YearNo            = yearNo,
                MonthNo           = monthNo,
                DayNo             = dayNo
            }, transaction);

            if (counter is null)
                throw new InvalidOperationException(
                    $"Failed to get or create AutoNumberCounter for TemplateId={templateId}. " +
                    "Pastikan tabel AutoNumberCounter sudah ada dan transaction aktif.");

            return counter;
        }

        /// <summary>
        /// Increment LastNumber counter secara atomic di dalam transaction.
        /// Mengembalikan nilai newValue yang sama (untuk digunakan build number).
        /// </summary>
        public async Task<int> IncrementCounterAsync(
            long counterId,
            int newValue,
            IDbTransaction transaction)
        {
            var conn = transaction.Connection!;

            var sql = $@"
                UPDATE {_dialect.Qualify("app", "AutoNumberCounter")}
                SET LastNumber  = @NewValue,
                    UpdatedTime = @UpdatedTime
                WHERE AutoNumberCounterId = @CounterId";

            await conn.ExecuteAsync(sql, new
            {
                CounterId = counterId,
                NewValue  = newValue,
                UpdatedTime = DateTime.UtcNow
            }, transaction);

            return newValue;
        }
        /// <summary>
        /// Buat koneksi baru tanpa membukanya — caller bertanggung jawab Open() dan Dispose().
        /// Digunakan oleh service agar tidak perlu referensi langsung ke RepositoryContext.
        /// </summary>
        public IDbConnection CreateConnection() => _context.CreateConnection();
    }
}
