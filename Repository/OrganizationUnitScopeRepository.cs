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
    public partial class OrganizationUnitScopeRepository : IOrganizationUnitScopeRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public OrganizationUnitScopeRepository(RepositoryContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<OrganizationUnitScope> GetOrganizationUnitScopeAsync(Guid organizationUnitScopeGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_OrganizationUnitId.OrganizationUnitName AS OrganizationUnitName

                FROM [app].[OrganizationUnitScope] a
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[OrganizationUnit] j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId

                WHERE a.OrganizationUnitScopeGuid = @organizationUnitScopeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<OrganizationUnitScope>(sql, new { organizationUnitScopeGuid });
        }

        public async Task<OrganizationUnitScope> GetByOrganizationUnitGuidAndOrganizationUnitScopeGuidAsync(Guid organizationUnitGuid, Guid organizationUnitScopeGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_OrganizationUnitId.OrganizationUnitName AS OrganizationUnitName

                FROM [app].[OrganizationUnitScope] a
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[OrganizationUnit] j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId

                WHERE j_OrganizationUnitId.OrganizationUnitGuid = @organizationUnitGuid
                  AND a.OrganizationUnitScopeGuid = @organizationUnitScopeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<OrganizationUnitScope>(sql, new { organizationUnitGuid, organizationUnitScopeGuid });
        }

        public async Task<IEnumerable<OrganizationUnitScope>> GetAllByOrganizationUnitGuidAsync(Guid organizationUnitGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_OrganizationUnitId.OrganizationUnitName AS OrganizationUnitName

                FROM [app].[OrganizationUnitScope] a
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[OrganizationUnit] j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId

                WHERE j_OrganizationUnitId.OrganizationUnitGuid = @organizationUnitGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.OrganizationUnitScopeId DESC";
            return await connection.QueryAsync<OrganizationUnitScope>(sql, new { organizationUnitGuid });
        }

        public async Task CreateOrganizationUnitScopeAsync(OrganizationUnitScope organizationUnitScope, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(organizationUnitScope);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[OrganizationUnitScope]
                (OrganizationUnitScopeGuid, OrganizationUnitId, CompanyId, CompanyOfficeId, CreatedById, StatusId, CreatedTime, ScopeType)
                VALUES
                (@OrganizationUnitScopeGuid, @OrganizationUnitId, @CompanyId, @CompanyOfficeId, @CreatedById, @StatusId, @CreatedTime, @ScopeType);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            organizationUnitScope.OrganizationUnitScopeId = await conn.QuerySingleAsync<long>(sql, organizationUnitScope, transaction);
        }

        public async Task UpdateOrganizationUnitScopeAsync(OrganizationUnitScope organizationUnitScope, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(organizationUnitScope);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[OrganizationUnitScope]
                SET OrganizationUnitId = @OrganizationUnitId,
                    CompanyId = @CompanyId,
                    CompanyOfficeId = @CompanyOfficeId,
                    ScopeType = @ScopeType,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE OrganizationUnitScopeGuid = @OrganizationUnitScopeGuid";
            await conn.ExecuteAsync(sql, organizationUnitScope, transaction);
        }

        public async Task SoftDeleteOrganizationUnitScopeAsync(OrganizationUnitScope organizationUnitScope, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(organizationUnitScope, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[OrganizationUnitScope]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE OrganizationUnitScopeGuid = @OrganizationUnitScopeGuid";

            await conn.ExecuteAsync(sql, organizationUnitScope, transaction);
        }

        public async Task DeleteOrganizationUnitScopeAsync(Guid organizationUnitScopeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[OrganizationUnitScope] WHERE OrganizationUnitScopeGuid = @organizationUnitScopeGuid";
            await conn.ExecuteAsync(sql, new { organizationUnitScopeGuid }, transaction);
        }

        public async Task DeleteByOrganizationUnitGuidAsync(Guid organizationUnitGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[OrganizationUnitScope] WHERE OrganizationUnitId IN (SELECT OrganizationUnitId FROM [app].[OrganizationUnit] WHERE OrganizationUnitGuid = @organizationUnitGuid)";
            await conn.ExecuteAsync(sql, new { organizationUnitGuid }, transaction);
        }

        public async Task<IEnumerable<OrganizationUnitScope>> SearchOrganizationUnitScopeAsync(
            string? scopeType, string? scopeTypeSearchType,
            Guid organizationUnitGuid, Guid organizationUnitScopeGuid,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (organizationUnitGuid != Guid.Empty)
            {
                whereClauses.Add("j_OrganizationUnitId.OrganizationUnitGuid = @organizationUnitGuid");
                parameters.Add("@organizationUnitGuid", organizationUnitGuid);
            }
            if (organizationUnitScopeGuid != Guid.Empty)
            {
                whereClauses.Add("a.OrganizationUnitScopeGuid = @organizationUnitScopeGuid");
                parameters.Add("@organizationUnitScopeGuid", organizationUnitScopeGuid);
            }

            if (!string.IsNullOrWhiteSpace(scopeType))
            {
                var param = SqlFilterHelper.BuildFilter("a.ScopeType", "@scopeType", scopeTypeSearchType, parameters, "scopeType", scopeType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_OrganizationUnitId.OrganizationUnitName AS OrganizationUnitName

                FROM [app].[OrganizationUnitScope] a
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[OrganizationUnit] j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.OrganizationUnitScopeId DESC";

            return await connection.QueryAsync<OrganizationUnitScope>(sql, parameters, transaction);
        }
    }
}
