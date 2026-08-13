using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class CompanyRepository(RepositoryContext context, IAuditService audit) : ICompanyRepository
    {

        public async Task<Company?> GetCompanyAsync(Guid companyGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_LeaderId.LeaderName AS LeaderName
, j_ParentCompanyId.ParentCompanyName AS ParentCompanyName


                FROM [app].[Company] a
LEFT JOIN [app].[Person] j_LeaderId ON a.LeaderId = j_LeaderId.PersonId
LEFT JOIN [app].[Company] j_ParentCompanyId ON a.ParentCompanyId = j_ParentCompanyId.CompanyId

                WHERE a.CompanyGuid = @companyGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { companyGuid });
        }

        public async Task<Company?> GetByPersonGuidAndCompanyGuidAsync(Guid personGuid, Guid companyGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_LeaderId.LeaderName AS LeaderName
, j_ParentCompanyId.ParentCompanyName AS ParentCompanyName


                FROM [app].[Company] a
LEFT JOIN [app].[Person] j_LeaderId ON a.LeaderId = j_LeaderId.PersonId
LEFT JOIN [app].[Company] j_ParentCompanyId ON a.ParentCompanyId = j_ParentCompanyId.CompanyId

                WHERE j_LeaderId.PersonGuid = @personGuid
                  AND a.CompanyGuid = @companyGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { personGuid, companyGuid });
        }

        public async Task<IEnumerable<Company>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_LeaderId.LeaderName AS LeaderName
, j_ParentCompanyId.ParentCompanyName AS ParentCompanyName


                FROM [app].[Company] a
LEFT JOIN [app].[Person] j_LeaderId ON a.LeaderId = j_LeaderId.PersonId
LEFT JOIN [app].[Company] j_ParentCompanyId ON a.ParentCompanyId = j_ParentCompanyId.CompanyId

                WHERE j_LeaderId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.CompanyId DESC";
            return await connection.QueryAsync<Company>(sql, new { personGuid });
        }

        public async Task CreateCompanyAsync(Company company, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(company);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Company]
                (CompanyGuid, LeaderId, CreatedById, StatusId, CreatedTime, [CompanyName], [InitialName], [TaxCompulsionNo], [RegistrationNo], [ParentCompanyId], [DefaultCurrency], [CompanyLogo])
                VALUES
                (@CompanyGuid, @LeaderId, @CreatedById, @StatusId, @CreatedTime, @CompanyName, @InitialName, @TaxCompulsionNo, @RegistrationNo, @ParentCompanyId, @DefaultCurrency, @CompanyLogo);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            company.CompanyId = await conn.QuerySingleAsync<long>(sql, company, transaction);
        }

        public async Task UpdateCompanyAsync(Company company, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(company);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Company]
                SET                     [LeaderId] = @LeaderId,
                    [CompanyName] = @CompanyName,
                    [InitialName] = @InitialName,
                    [TaxCompulsionNo] = @TaxCompulsionNo,
                    [RegistrationNo] = @RegistrationNo,
                    [ParentCompanyId] = @ParentCompanyId,
                    [DefaultCurrency] = @DefaultCurrency,
                    [CompanyLogo] = @CompanyLogo,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE CompanyGuid = @CompanyGuid";
            await conn.ExecuteAsync(sql, company, transaction);
        }

        public async Task SoftDeleteCompanyAsync(Company company, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(company, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Company]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CompanyGuid = @CompanyGuid";

            await conn.ExecuteAsync(sql, company, transaction);
        }

        public async Task DeleteCompanyAsync(Guid companyGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Company] WHERE CompanyGuid = @companyGuid";
            await conn.ExecuteAsync(sql, new { companyGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Company] WHERE LeaderId IN (SELECT PersonId FROM [app].[Person] WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<Company>> SearchCompanyAsync(
            string? companyName,
            string? companyNameSearchType,
            string? initialName,
            string? initialNameSearchType,
            string? taxCompulsionNo,
            string? taxCompulsionNoSearchType,
            string? registrationNo,
            string? registrationNoSearchType,
            string? defaultCurrency,
            string? defaultCurrencySearchType,
            string? companyLogo,
            string? companyLogoSearchType,
            Guid personGuid, Guid companyGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (personGuid != Guid.Empty)
            {
                whereClauses.Add("j_LeaderId.PersonGuid = @personGuid");
                parameters.Add("@personGuid", personGuid);
            }
            if (companyGuid != Guid.Empty)
            {
                whereClauses.Add("a.CompanyGuid = @companyGuid");
                parameters.Add("@companyGuid", companyGuid);
            }

            if (!string.IsNullOrWhiteSpace(companyName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyName", "@companyName", companyNameSearchType, parameters, "companyName", companyName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(initialName))
            {
                var param = SqlFilterHelper.BuildFilter("a.InitialName", "@initialName", initialNameSearchType, parameters, "initialName", initialName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(taxCompulsionNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.TaxCompulsionNo", "@taxCompulsionNo", taxCompulsionNoSearchType, parameters, "taxCompulsionNo", taxCompulsionNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(registrationNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.RegistrationNo", "@registrationNo", registrationNoSearchType, parameters, "registrationNo", registrationNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(defaultCurrency))
            {
                var param = SqlFilterHelper.BuildFilter("a.DefaultCurrency", "@defaultCurrency", defaultCurrencySearchType, parameters, "defaultCurrency", defaultCurrency);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(companyLogo))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyLogo", "@companyLogo", companyLogoSearchType, parameters, "companyLogo", companyLogo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_LeaderId.LeaderName AS LeaderName
, j_ParentCompanyId.ParentCompanyName AS ParentCompanyName


                FROM [app].[Company] a
LEFT JOIN [app].[Person] j_LeaderId ON a.LeaderId = j_LeaderId.PersonId
LEFT JOIN [app].[Company] j_ParentCompanyId ON a.ParentCompanyId = j_ParentCompanyId.CompanyId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CompanyId DESC";

            return await connection.QueryAsync<Company>(sql, parameters, transaction);
        }
    }
}
