using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class CostCenterRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : ICostCenterRepository
    {
        private readonly string _table = dialect.Qualify("app", "CostCenter");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<CostCenter?> GetCostCenterAsync(Guid costCenterGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a

                WHERE a.CostCenterGuid = @costCenterGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenter>(sql, new { costCenterGuid });
        }

        public async Task<CostCenter?> GetCostCenterByIdAsync(long costCenterId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a

                WHERE a.CostCenterId = @costCenterId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenter>(sql, new { costCenterId });
        }

        public async Task<IEnumerable<CostCenter>> GetAllCostCentersAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.CostCenterId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<CostCenter>(sql);
        }

        public async Task CreateCostCenterAsync(CostCenter costCenter, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(costCenter);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (CostCenterGuid, CreatedById, StatusId, CreatedTime, CostCenterCode, CostCenterName, CostCenterDescription, ParentCostCenterId, LevelDepth, HierarchyPath)
                VALUES
                (@CostCenterGuid, @CreatedById, @StatusId, @CreatedTime, @CostCenterCode, @CostCenterName, @CostCenterDescription, @ParentCostCenterId, @LevelDepth, @HierarchyPath);
                {dialect.ReturnIdentitySuffix("CostCenterId")}";
            costCenter.CostCenterId = await conn.QuerySingleAsync<long>(sql, costCenter, transaction);
        }

        public async Task UpdateCostCenterAsync(CostCenter costCenter, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(costCenter);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
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
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CostCenterGuid = @CostCenterGuid";

            await conn.ExecuteAsync(sql, costCenter, transaction);
        }

        public async Task DeleteCostCenterAsync(Guid costCenterGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE CostCenterGuid = @costCenterGuid";
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
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(costCenterCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostCenterCode", "@costCenterCode", costCenterCodeSearchType, parameters, "costCenterCode", costCenterCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(costCenterName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostCenterName", "@costCenterName", costCenterNameSearchType, parameters, "costCenterName", costCenterName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(costCenterDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostCenterDescription", "@costCenterDescription", costCenterDescriptionSearchType, parameters, "costCenterDescription", costCenterDescription, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(parentCostCenterId))
            {
                var param = SqlFilterHelper.BuildFilter("a.ParentCostCenterId", "@parentCostCenterId", parentCostCenterIdSearchType, parameters, "parentCostCenterId", parentCostCenterId, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(levelDepth))
            {
                var param = SqlFilterHelper.BuildFilter("a.LevelDepth", "@levelDepth", levelDepthSearchType, parameters, "levelDepth", levelDepth, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(hierarchyPath))
            {
                var param = SqlFilterHelper.BuildFilter("a.HierarchyPath", "@hierarchyPath", hierarchyPathSearchType, parameters, "hierarchyPath", hierarchyPath, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CostCenterId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<CostCenter>(sql, parameters, transaction);
        }
    }
}
