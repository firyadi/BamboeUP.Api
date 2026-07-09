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
    public partial class CostCenterScopeRepository : ICostCenterScopeRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public CostCenterScopeRepository(RepositoryContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<CostCenterScope> GetCostCenterScopeAsync(Guid costCenterScopeGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CostCenterId.CostCenterName AS CostCenterName

                FROM [app].[CostCenterScope] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId

                WHERE a.CostCenterScopeGuid = @costCenterScopeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenterScope>(sql, new { costCenterScopeGuid });
        }

        public async Task<CostCenterScope> GetByCostCenterGuidAndCostCenterScopeGuidAsync(Guid costCenterGuid, Guid costCenterScopeGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CostCenterId.CostCenterName AS CostCenterName

                FROM [app].[CostCenterScope] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId

                WHERE j_CostCenterId.CostCenterGuid = @costCenterGuid
                  AND a.CostCenterScopeGuid = @costCenterScopeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenterScope>(sql, new { costCenterGuid, costCenterScopeGuid });
        }

        public async Task<IEnumerable<CostCenterScope>> GetAllByCostCenterGuidAsync(Guid costCenterGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CostCenterId.CostCenterName AS CostCenterName

                FROM [app].[CostCenterScope] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId

                WHERE j_CostCenterId.CostCenterGuid = @costCenterGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.CostCenterScopeId DESC";
            return await connection.QueryAsync<CostCenterScope>(sql, new { costCenterGuid });
        }

        public async Task CreateCostCenterScopeAsync(CostCenterScope costCenterScope, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(costCenterScope);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[CostCenterScope]
                (CostCenterScopeGuid, CostCenterId, CreatedById, StatusId, CreatedTime, CompanyId, CompanyOfficeId, ScopeType)
                VALUES
                (@CostCenterScopeGuid, @CostCenterId, @CreatedById, @StatusId, @CreatedTime, @CompanyId, @CompanyOfficeId, @ScopeType);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            costCenterScope.CostCenterScopeId = await conn.QuerySingleAsync<long>(sql, costCenterScope, transaction);
        }

        public async Task UpdateCostCenterScopeAsync(CostCenterScope costCenterScope, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(costCenterScope);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[CostCenterScope]
                SET                     CostCenterId = @CostCenterId,
                    CompanyId = @CompanyId,
                    CompanyOfficeId = @CompanyOfficeId,
                    ScopeType = @ScopeType,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE CostCenterScopeGuid = @CostCenterScopeGuid";
            await conn.ExecuteAsync(sql, costCenterScope, transaction);
        }

        public async Task SoftDeleteCostCenterScopeAsync(CostCenterScope costCenterScope, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(costCenterScope, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[CostCenterScope]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CostCenterScopeGuid = @CostCenterScopeGuid";

            await conn.ExecuteAsync(sql, costCenterScope, transaction);
        }

        public async Task DeleteCostCenterScopeAsync(Guid costCenterScopeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[CostCenterScope] WHERE CostCenterScopeGuid = @costCenterScopeGuid";
            await conn.ExecuteAsync(sql, new { costCenterScopeGuid }, transaction);
        }

        public async Task DeleteByCostCenterGuidAsync(Guid costCenterGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[CostCenterScope] WHERE CostCenterId IN (SELECT CostCenterId FROM [app].[CostCenter] WHERE CostCenterGuid = @costCenterGuid)";
            await conn.ExecuteAsync(sql, new { costCenterGuid }, transaction);
        }

        public async Task<IEnumerable<CostCenterScope>> SearchCostCenterScopeAsync(
            string? companyId,
            string? companyIdSearchType,
            string? companyOfficeId,
            string? companyOfficeIdSearchType,
            string? scopeType,
            string? scopeTypeSearchType,
            Guid costCenterGuid, Guid costCenterScopeGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (costCenterGuid != Guid.Empty)
            {
                whereClauses.Add("j_CostCenterId.CostCenterGuid = @costCenterGuid");
                parameters.Add("@costCenterGuid", costCenterGuid);
            }
            if (costCenterScopeGuid != Guid.Empty)
            {
                whereClauses.Add("a.CostCenterScopeGuid = @costCenterScopeGuid");
                parameters.Add("@costCenterScopeGuid", costCenterScopeGuid);
            }

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyId", "@companyId", companyIdSearchType, parameters, "companyId", companyId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(companyOfficeId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyOfficeId", "@companyOfficeId", companyOfficeIdSearchType, parameters, "companyOfficeId", companyOfficeId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(scopeType))
            {
                var param = SqlFilterHelper.BuildFilter("a.ScopeType", "@scopeType", scopeTypeSearchType, parameters, "scopeType", scopeType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CostCenterId.CostCenterName AS CostCenterName

                FROM [app].[CostCenterScope] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CostCenterScopeId DESC";

            return await connection.QueryAsync<CostCenterScope>(sql, parameters, transaction);
        }
    }
}
