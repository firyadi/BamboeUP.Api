using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    // Old-style: does NOT inherit RepositoryBase<T>
    public class AutoNumberRepository : IAutoNumberRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public AutoNumberRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<AutoNumber?> GetAutoNumberAsync(Guid autoNumberGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumber] a
                WHERE a.AutoNumberGuid = @autoNumberGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<AutoNumber>(sql, new { autoNumberGuid });
        }

        public async Task<IEnumerable<AutoNumber>> GetAllAutoNumbersAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumber] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.AutoNumberId DESC";
            return await connection.QueryAsync<AutoNumber>(sql);
        }

        public async Task CreateAutoNumberAsync(AutoNumber autoNumber, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(autoNumber);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[AutoNumber]
                (AutoNumberGuid, CreatedById, StatusId, CreatedTime, Prefik, SeparatorAfterPrefik, IsUsedDepartment, SeparatorAfterDept, IsUsedYear, YearDigit, SeparatorAfterYear, IsUsedMonth, IsMonthInRomawi, SeparatorAfterMonth, IsUsedDay, SeparatorAfterDay, NumberLength, NumberGroupLength, NumberGroupSeparator, NumberFormat)
                VALUES
                (@AutoNumberGuid, @CreatedById, @StatusId, @CreatedTime, @Prefik, @SeparatorAfterPrefik, @IsUsedDepartment, @SeparatorAfterDept, @IsUsedYear, @YearDigit, @SeparatorAfterYear, @IsUsedMonth, @IsMonthInRomawi, @SeparatorAfterMonth, @IsUsedDay, @SeparatorAfterDay, @NumberLength, @NumberGroupLength, @NumberGroupSeparator, @NumberFormat)";
            await conn.ExecuteAsync(sql, autoNumber, transaction);

            await _audit.LogAsync(
                actionType: "CREATE",
                tableName: "AutoNumber",
                primaryKey: autoNumber.AutoNumberGuid.ToString(),
                userId: autoNumber.CreatedById.ToString(),
                oldEntity: null,
                newEntity: autoNumber);
        }

        public async Task UpdateAutoNumberAsync(AutoNumber autoNumber, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(autoNumber);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberAsync(autoNumber.AutoNumberGuid, false);

            const string sql = @"
                UPDATE [app].[AutoNumber]
                SET Prefik = @Prefik,
                                      SeparatorAfterPrefik = @SeparatorAfterPrefik,
                                      IsUsedDepartment = @IsUsedDepartment,
                                      SeparatorAfterDept = @SeparatorAfterDept,
                                      IsUsedYear = @IsUsedYear,
                                      YearDigit = @YearDigit,
                                      SeparatorAfterYear = @SeparatorAfterYear,
                                      IsUsedMonth = @IsUsedMonth,
                                      IsMonthInRomawi = @IsMonthInRomawi,
                                      SeparatorAfterMonth = @SeparatorAfterMonth,
                                      IsUsedDay = @IsUsedDay,
                                      SeparatorAfterDay = @SeparatorAfterDay,
                                      NumberLength = @NumberLength,
                                      NumberGroupLength = @NumberGroupLength,
                                      NumberGroupSeparator = @NumberGroupSeparator,
                                      NumberFormat = @NumberFormat,
                                      StatusId = @StatusId,
                                      UpdatedById = @UpdatedById,
                                      UpdatedTime = @UpdatedTime
                WHERE AutoNumberGuid = @AutoNumberGuid";
            await conn.ExecuteAsync(sql, autoNumber, transaction);

            await _audit.LogAsync(
                actionType: "UPDATE",
                tableName: "AutoNumber",
                primaryKey: autoNumber.AutoNumberGuid.ToString(),
                userId: autoNumber.UpdatedById.ToString(),
                oldEntity: oldData,
                newEntity: autoNumber);
        }

        public async Task SoftDeleteAutoNumberAsync(AutoNumber autoNumber, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(autoNumber, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberAsync(autoNumber.AutoNumberGuid, false);

            const string sql = @"
                UPDATE [app].[AutoNumber]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AutoNumberGuid = @AutoNumberGuid";

            await conn.ExecuteAsync(sql, autoNumber, transaction);

            await _audit.LogAsync(
                actionType: "DELETE",
                tableName: "AutoNumber",
                primaryKey: autoNumber.AutoNumberGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task DeleteAutoNumberAsync(Guid autoNumberGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberAsync(autoNumberGuid, false);

            const string sql = @"DELETE FROM [app].[AutoNumber] WHERE AutoNumberGuid = @autoNumberGuid";
            await conn.ExecuteAsync(sql, new { autoNumberGuid }, transaction);

            await _audit.LogAsync(
                actionType: "DELETE_ADMIN",
                tableName: "AutoNumber",
                primaryKey: autoNumberGuid.ToString(),
                userId: oldData?.DeletedById.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<AutoNumber>> SearchAutoNumberAsync(
            string? prefik,
            string? prefikSearchType,
            string? separatorAfterPrefik,
            string? separatorAfterPrefikSearchType,
            string? separatorAfterDept,
            string? separatorAfterDeptSearchType,
            string? separatorAfterYear,
            string? separatorAfterYearSearchType,
            string? separatorAfterMonth,
            string? separatorAfterMonthSearchType,
            string? separatorAfterDay,
            string? separatorAfterDaySearchType,
            string? numberGroupSeparator,
            string? numberGroupSeparatorSearchType,
            string? numberFormat,
            string? numberFormatSearchType,
            
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            // 🔍 Prefik
if (!string.IsNullOrWhiteSpace(prefik))
{
    var param = SqlFilterHelper.BuildFilter("a.Prefik", "@prefik", prefikSearchType, parameters, "Prefik", prefik);
    if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
}

// 🔍 SeparatorAfterPrefik
if (!string.IsNullOrWhiteSpace(separatorAfterPrefik))
{
    var param = SqlFilterHelper.BuildFilter("a.SeparatorAfterPrefik", "@separatorAfterPrefik", separatorAfterPrefikSearchType, parameters, "SeparatorAfterPrefik", separatorAfterPrefik);
    if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
}

// 🔍 SeparatorAfterDept
if (!string.IsNullOrWhiteSpace(separatorAfterDept))
{
    var param = SqlFilterHelper.BuildFilter("a.SeparatorAfterDept", "@separatorAfterDept", separatorAfterDeptSearchType, parameters, "SeparatorAfterDept", separatorAfterDept);
    if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
}

// 🔍 SeparatorAfterYear
if (!string.IsNullOrWhiteSpace(separatorAfterYear))
{
    var param = SqlFilterHelper.BuildFilter("a.SeparatorAfterYear", "@separatorAfterYear", separatorAfterYearSearchType, parameters, "SeparatorAfterYear", separatorAfterYear);
    if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
}

// 🔍 SeparatorAfterMonth
if (!string.IsNullOrWhiteSpace(separatorAfterMonth))
{
    var param = SqlFilterHelper.BuildFilter("a.SeparatorAfterMonth", "@separatorAfterMonth", separatorAfterMonthSearchType, parameters, "SeparatorAfterMonth", separatorAfterMonth);
    if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
}

// 🔍 SeparatorAfterDay
if (!string.IsNullOrWhiteSpace(separatorAfterDay))
{
    var param = SqlFilterHelper.BuildFilter("a.SeparatorAfterDay", "@separatorAfterDay", separatorAfterDaySearchType, parameters, "SeparatorAfterDay", separatorAfterDay);
    if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
}

// 🔍 NumberGroupSeparator
if (!string.IsNullOrWhiteSpace(numberGroupSeparator))
{
    var param = SqlFilterHelper.BuildFilter("a.NumberGroupSeparator", "@numberGroupSeparator", numberGroupSeparatorSearchType, parameters, "NumberGroupSeparator", numberGroupSeparator);
    if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
}

// 🔍 NumberFormat
if (!string.IsNullOrWhiteSpace(numberFormat))
{
    var param = SqlFilterHelper.BuildFilter("a.NumberFormat", "@numberFormat", numberFormatSearchType, parameters, "NumberFormat", numberFormat);
    if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
}
            

            var sql = $@"
                SELECT a.*
                
                FROM [app].[AutoNumber] a
                
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AutoNumberId DESC";

            return await connection.QueryAsync<AutoNumber>(sql, parameters, transaction);
        }

        // Detail helpers (only emitted if entity has a parent)
        
    }
}
