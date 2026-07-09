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
    public partial class CostCenterRepository : ICostCenterRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public CostCenterRepository(RepositoryContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<CostCenter> GetCostCenterAsync(Guid costCenterGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[CostCenter] a

                WHERE a.CostCenterGuid = @costCenterGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenter>(sql, new { costCenterGuid });
        }

        public async Task<CostCenter?> GetCostCenterByIdAsync(long costCenterId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[CostCenter] a

                WHERE a.CostCenterId = @costCenterId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenter>(sql, new { costCenterId });
        }

        public async Task<IEnumerable<CostCenter>> GetAllCostCentersAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[CostCenter] a

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.CostCenterId DESC";
            return await connection.QueryAsync<CostCenter>(sql);
        }

        public async Task CreateCostCenterAsync(CostCenter costCenter, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(costCenter);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[CostCenter]
                (CostCenterGuid, CreatedById, StatusId, CreatedTime, CostCenterCode, CostCenterName, CostCenterDescription, ParentCostCenterId, LevelDepth, HierarchyPath)
                VALUES
                (@CostCenterGuid, @CreatedById, @StatusId, @CreatedTime, @CostCenterCode, @CostCenterName, @CostCenterDescription, @ParentCostCenterId, @LevelDepth, @HierarchyPath);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            costCenter.CostCenterId = await conn.QuerySingleAsync<long>(sql, costCenter, transaction);
        }

        public async Task UpdateCostCenterAsync(CostCenter costCenter, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(costCenter);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[CostCenter]
                SET                     CostCenterCode = @CostCenterCode,
                    CostCenterName = @CostCenterName,
                    CostCenterDescription = @CostCenterDescription,
                    ParentCostCenterId = @ParentCostCenterId,
                    LevelDepth = @LevelDepth,
                    HierarchyPath = @HierarchyPath,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE CostCenterGuid = @CostCenterGuid";
            await conn.ExecuteAsync(sql, costCenter, transaction);
        }

        public async Task SoftDeleteCostCenterAsync(CostCenter costCenter, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(costCenter, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[CostCenter]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CostCenterGuid = @CostCenterGuid";

            await conn.ExecuteAsync(sql, costCenter, transaction);
        }

        public async Task DeleteCostCenterAsync(Guid costCenterGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[CostCenter] WHERE CostCenterGuid = @costCenterGuid";
            await conn.ExecuteAsync(sql, new { costCenterGuid }, transaction);
        }

        public async Task<IEnumerable<CostCenter>> SearchCostCenterAsync(
            string? costCenterCode,
            string? costCenterCodeSearchType,
            string? costCenterName,
            string? costCenterNameSearchType,
            string? costCenterDescription,
            string? costCenterDescriptionSearchType,
            string? parentCostCenterId,
            string? parentCostCenterIdSearchType,
            string? levelDepth,
            string? levelDepthSearchType,
            string? hierarchyPath,
            string? hierarchyPathSearchType,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(costCenterCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostCenterCode", "@costCenterCode", costCenterCodeSearchType, parameters, "costCenterCode", costCenterCode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(costCenterName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostCenterName", "@costCenterName", costCenterNameSearchType, parameters, "costCenterName", costCenterName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(costCenterDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostCenterDescription", "@costCenterDescription", costCenterDescriptionSearchType, parameters, "costCenterDescription", costCenterDescription);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(parentCostCenterId))
            {
                var param = SqlFilterHelper.BuildFilter("a.ParentCostCenterId", "@parentCostCenterId", parentCostCenterIdSearchType, parameters, "parentCostCenterId", parentCostCenterId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(levelDepth))
            {
                var param = SqlFilterHelper.BuildFilter("a.LevelDepth", "@levelDepth", levelDepthSearchType, parameters, "levelDepth", levelDepth);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(hierarchyPath))
            {
                var param = SqlFilterHelper.BuildFilter("a.HierarchyPath", "@hierarchyPath", hierarchyPathSearchType, parameters, "hierarchyPath", hierarchyPath);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[CostCenter] a

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CostCenterId DESC";

            return await connection.QueryAsync<CostCenter>(sql, parameters, transaction);
        }
    }
}
