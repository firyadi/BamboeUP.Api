using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class CertificationRepository(RepositoryContext context, ISqlDialect dialect) : ICertificationRepository
    {
        private readonly string _table = dialect.Qualify("app", "Certification");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        private string BuildSelectColumns() => $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCertificationCategory.StandardReferenceItemName")} AS CertificationCategoryName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCertificationProvider.StandardReferenceItemName")} AS CertificationProviderName";

        private string BuildStandardReferenceJoins() => $@"
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCertificationCategory", "SrCertificationCategory", "CertificationCategory")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCertificationProvider", "SrCertificationProvider", "CertificationProvider")}";

        public async Task<Certification?> GetCertificationAsync(Guid certificationGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {BuildSelectColumns()}
                FROM {_table} a
{BuildStandardReferenceJoins()}
                WHERE a.CertificationGuid = @certificationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Certification>(sql, new { certificationGuid });
        }

        public async Task<IEnumerable<Certification>> GetAllCertificationsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {BuildSelectColumns()}
                FROM {_table} a
{BuildStandardReferenceJoins()}
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.CertificationId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Certification>(sql);
        }

        public async Task CreateCertificationAsync(Certification certification, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (CertificationGuid, CreatedById, StatusId, CreatedTime, CertificationCode, CertificationName, CertificationShortName, SrCertificationCategory, SrCertificationProvider, DefaultValidityMonth, IsLifetime, IsRenewable, LogoFileStorageId, Description, IsActive
                )
                VALUES
                (@CertificationGuid, @CreatedById, @StatusId, @CreatedTime, @CertificationCode, @CertificationName, @CertificationShortName, @SrCertificationCategory, @SrCertificationProvider, @DefaultValidityMonth, @IsLifetime, @IsRenewable, @LogoFileStorageId, @Description, @IsActive
                ){dialect.ReturnIdentitySuffix("CertificationId")}";
            certification.CertificationId = await conn.QuerySingleAsync<long>(sql, certification, transaction);
        }

        public async Task UpdateCertificationAsync(Certification certification, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     CertificationCode = @CertificationCode,
                    CertificationName = @CertificationName,
                    CertificationShortName = @CertificationShortName,
                    SrCertificationCategory = @SrCertificationCategory,
                    SrCertificationProvider = @SrCertificationProvider,
                    DefaultValidityMonth = @DefaultValidityMonth,
                    IsLifetime = @IsLifetime,
                    IsRenewable = @IsRenewable,
                    LogoFileStorageId = @LogoFileStorageId,
                    Description = @Description,
                    IsActive = @IsActive,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE CertificationGuid = @CertificationGuid";
            await conn.ExecuteAsync(sql, certification, transaction);
        }

        public async Task SoftDeleteCertificationAsync(Certification certification, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CertificationGuid = @CertificationGuid";

            await conn.ExecuteAsync(sql, certification, transaction);
        }

        public async Task DeleteCertificationAsync(Guid certificationGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE CertificationGuid = @certificationGuid";
            await conn.ExecuteAsync(sql, new { certificationGuid }, transaction);
        }

        public async Task<IEnumerable<Certification>> SearchCertificationAsync(
            string? certificationCode,
            string? certificationCodeSearchType,
            string? certificationName,
            string? certificationNameSearchType,
            string? certificationShortName,
            string? certificationShortNameSearchType,
            string? defaultValidityMonth,
            string? defaultValidityMonthSearchType,
            string? isLifetime,
            string? isLifetimeSearchType,
            string? isRenewable,
            string? isRenewableSearchType,
            string? description,
            string? descriptionSearchType,
            string? isActive,
            string? isActiveSearchType,
string? certificationCategoryName, string? certificationCategoryNameSearchType,
string? certificationProviderName, string? certificationProviderNameSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(certificationCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificationCode", "@certificationCode", certificationCodeSearchType, parameters, "certificationCode", certificationCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(certificationName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificationName", "@certificationName", certificationNameSearchType, parameters, "certificationName", certificationName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(certificationShortName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificationShortName", "@certificationShortName", certificationShortNameSearchType, parameters, "certificationShortName", certificationShortName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(defaultValidityMonth))
            {
                var param = SqlFilterHelper.BuildFilter("a.DefaultValidityMonth", "@defaultValidityMonth", defaultValidityMonthSearchType, parameters, "defaultValidityMonth", defaultValidityMonth, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isLifetime))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsLifetime", "@isLifetime", isLifetimeSearchType, parameters, "isLifetime", isLifetime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isRenewable))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsRenewable", "@isRenewable", isRenewableSearchType, parameters, "isRenewable", isRenewable, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isActive))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsActive", "@isActive", isActiveSearchType, parameters, "isActive", isActive, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }
if (!string.IsNullOrWhiteSpace(certificationCategoryName)) { var param = SqlFilterHelper.BuildFilter("v_SrCertificationCategory.StandardReferenceItemName", "@certificationCategoryName", certificationCategoryNameSearchType, parameters, "certificationCategoryName", certificationCategoryName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }
if (!string.IsNullOrWhiteSpace(certificationProviderName)) { var param = SqlFilterHelper.BuildFilter("v_SrCertificationProvider.StandardReferenceItemName", "@certificationProviderName", certificationProviderNameSearchType, parameters, "certificationProviderName", certificationProviderName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }


            var sql = $@"
                {BuildSelectColumns()}
                FROM {_table} a
{BuildStandardReferenceJoins()}
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CertificationId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Certification>(sql, parameters, transaction);
        }
    }
}
