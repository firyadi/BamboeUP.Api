using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class CertificationRepository(RepositoryContext context) : ICertificationRepository
    {
        public async Task<Certification?> GetCertificationAsync(Guid certificationGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrCertificationCategory.StandardReferenceItemName, '') AS CertificationCategoryName
, ISNULL(v_SrCertificationProvider.StandardReferenceItemName, '') AS CertificationProviderName


                FROM [app].[Certification] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CertificationCategory')
    WHERE StandardReferenceItemId = a.SrCertificationCategory
) v_SrCertificationCategory
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CertificationProvider')
    WHERE StandardReferenceItemId = a.SrCertificationProvider
) v_SrCertificationProvider


                WHERE a.CertificationGuid = @certificationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Certification>(sql, new { certificationGuid });
        }

        public async Task<IEnumerable<Certification>> GetAllCertificationsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrCertificationCategory.StandardReferenceItemName, '') AS CertificationCategoryName
, ISNULL(v_SrCertificationProvider.StandardReferenceItemName, '') AS CertificationProviderName



                FROM [app].[Certification] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CertificationCategory')
    WHERE StandardReferenceItemId = a.SrCertificationCategory
) v_SrCertificationCategory
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CertificationProvider')
    WHERE StandardReferenceItemId = a.SrCertificationProvider
) v_SrCertificationProvider


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.CertificationId DESC";
            return await connection.QueryAsync<Certification>(sql);
        }

        public async Task CreateCertificationAsync(Certification certification, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Certification]
                (CertificationGuid, CreatedById, StatusId, CreatedTime, CertificationCode, CertificationName, CertificationShortName, SrCertificationCategory, SrCertificationProvider, DefaultValidityMonth, IsLifetime, IsRenewable, LogoFileStorageId, Description, IsActive
                )
                VALUES
                (@CertificationGuid, @CreatedById, @StatusId, @CreatedTime, @CertificationCode, @CertificationName, @CertificationShortName, @SrCertificationCategory, @SrCertificationProvider, @DefaultValidityMonth, @IsLifetime, @IsRenewable, @LogoFileStorageId, @Description, @IsActive
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            certification.CertificationId = await conn.QuerySingleAsync<long>(sql, certification, transaction);
        }

        public async Task UpdateCertificationAsync(Certification certification, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Certification]
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
            const string sql = @"
                UPDATE [app].[Certification]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CertificationGuid = @CertificationGuid";

            await conn.ExecuteAsync(sql, certification, transaction);
        }

        public async Task DeleteCertificationAsync(Guid certificationGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Certification] WHERE CertificationGuid = @certificationGuid";
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
                var param = SqlFilterHelper.BuildFilter("a.CertificationCode", "@certificationCode", certificationCodeSearchType, parameters, "certificationCode", certificationCode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(certificationName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificationName", "@certificationName", certificationNameSearchType, parameters, "certificationName", certificationName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(certificationShortName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificationShortName", "@certificationShortName", certificationShortNameSearchType, parameters, "certificationShortName", certificationShortName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(defaultValidityMonth))
            {
                var param = SqlFilterHelper.BuildFilter("a.DefaultValidityMonth", "@defaultValidityMonth", defaultValidityMonthSearchType, parameters, "defaultValidityMonth", defaultValidityMonth);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isLifetime))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsLifetime", "@isLifetime", isLifetimeSearchType, parameters, "isLifetime", isLifetime);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isRenewable))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsRenewable", "@isRenewable", isRenewableSearchType, parameters, "isRenewable", isRenewable);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isActive))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsActive", "@isActive", isActiveSearchType, parameters, "isActive", isActive);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }
if (!string.IsNullOrWhiteSpace(certificationCategoryName)) { var param = SqlFilterHelper.BuildFilter("v_SrCertificationCategory.StandardReferenceItemName", "@certificationCategoryName", certificationCategoryNameSearchType, parameters, "certificationCategoryName", certificationCategoryName); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }
if (!string.IsNullOrWhiteSpace(certificationProviderName)) { var param = SqlFilterHelper.BuildFilter("v_SrCertificationProvider.StandardReferenceItemName", "@certificationProviderName", certificationProviderNameSearchType, parameters, "certificationProviderName", certificationProviderName); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }


            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrCertificationCategory.StandardReferenceItemName, '') AS CertificationCategoryName
, ISNULL(v_SrCertificationProvider.StandardReferenceItemName, '') AS CertificationProviderName


                FROM [app].[Certification] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CertificationCategory')
    WHERE StandardReferenceItemId = a.SrCertificationCategory
) v_SrCertificationCategory
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CertificationProvider')
    WHERE StandardReferenceItemId = a.SrCertificationProvider
) v_SrCertificationProvider


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CertificationId DESC";

            return await connection.QueryAsync<Certification>(sql, parameters, transaction);
        }
    }
}
