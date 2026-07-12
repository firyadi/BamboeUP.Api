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
    public partial class CostCenterAssignmentRepository : ICostCenterAssignmentRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public CostCenterAssignmentRepository(RepositoryContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<CostCenterAssignment?> GetCostCenterAssignmentAsync(Guid costCenterAssignmentGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CostCenterId.CostCenterName AS CostCenterName

                FROM [app].[CostCenterAssignment] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId

                WHERE a.CostCenterAssignmentGuid = @costCenterAssignmentGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenterAssignment>(sql, new { costCenterAssignmentGuid });
        }

        public async Task<CostCenterAssignment?> GetByCostCenterGuidAndCostCenterAssignmentGuidAsync(Guid costCenterGuid, Guid costCenterAssignmentGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CostCenterId.CostCenterName AS CostCenterName

                FROM [app].[CostCenterAssignment] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId

                WHERE j_CostCenterId.CostCenterGuid = @costCenterGuid
                  AND a.CostCenterAssignmentGuid = @costCenterAssignmentGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenterAssignment>(sql, new { costCenterGuid, costCenterAssignmentGuid });
        }

        public async Task<IEnumerable<CostCenterAssignment>> GetAllByCostCenterGuidAsync(Guid costCenterGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CostCenterId.CostCenterName AS CostCenterName

                FROM [app].[CostCenterAssignment] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId

                WHERE j_CostCenterId.CostCenterGuid = @costCenterGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.CostCenterAssignmentId DESC";
            return await connection.QueryAsync<CostCenterAssignment>(sql, new { costCenterGuid });
        }

        public async Task CreateCostCenterAssignmentAsync(CostCenterAssignment costCenterAssignment, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(costCenterAssignment);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[CostCenterAssignment]
                (CostCenterAssignmentGuid, CostCenterId, CreatedById, StatusId, CreatedTime, CompanyId, CompanyOfficeId, ProfitCenterId, CostCenterManagerEmployeeId, BudgetControlType, EffectiveDate, ExpiredDate)
                VALUES
                (@CostCenterAssignmentGuid, @CostCenterId, @CreatedById, @StatusId, @CreatedTime, @CompanyId, @CompanyOfficeId, @ProfitCenterId, @CostCenterManagerEmployeeId, @BudgetControlType, @EffectiveDate, @ExpiredDate);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            costCenterAssignment.CostCenterAssignmentId = await conn.QuerySingleAsync<long>(sql, costCenterAssignment, transaction);
        }

        public async Task UpdateCostCenterAssignmentAsync(CostCenterAssignment costCenterAssignment, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(costCenterAssignment);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[CostCenterAssignment]
                SET                     CostCenterId = @CostCenterId,
                    CompanyId = @CompanyId,
                    CompanyOfficeId = @CompanyOfficeId,
                    ProfitCenterId = @ProfitCenterId,
                    CostCenterManagerEmployeeId = @CostCenterManagerEmployeeId,
                    BudgetControlType = @BudgetControlType,
                    EffectiveDate = @EffectiveDate,
                    ExpiredDate = @ExpiredDate,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE CostCenterAssignmentGuid = @CostCenterAssignmentGuid";
            await conn.ExecuteAsync(sql, costCenterAssignment, transaction);
        }

        public async Task SoftDeleteCostCenterAssignmentAsync(CostCenterAssignment costCenterAssignment, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(costCenterAssignment, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[CostCenterAssignment]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CostCenterAssignmentGuid = @CostCenterAssignmentGuid";

            await conn.ExecuteAsync(sql, costCenterAssignment, transaction);
        }

        public async Task DeleteCostCenterAssignmentAsync(Guid costCenterAssignmentGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[CostCenterAssignment] WHERE CostCenterAssignmentGuid = @costCenterAssignmentGuid";
            await conn.ExecuteAsync(sql, new { costCenterAssignmentGuid }, transaction);
        }

        public async Task DeleteByCostCenterGuidAsync(Guid costCenterGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[CostCenterAssignment] WHERE CostCenterId IN (SELECT CostCenterId FROM [app].[CostCenter] WHERE CostCenterGuid = @costCenterGuid)";
            await conn.ExecuteAsync(sql, new { costCenterGuid }, transaction);
        }

        public async Task<IEnumerable<CostCenterAssignment>> SearchCostCenterAssignmentAsync(
            string? companyId,
            string? companyIdSearchType,
            string? companyOfficeId,
            string? companyOfficeIdSearchType,
            string? profitCenterId,
            string? profitCenterIdSearchType,
            string? costCenterManagerEmployeeId,
            string? costCenterManagerEmployeeIdSearchType,
            string? budgetControlType,
            string? budgetControlTypeSearchType,
            string? effectiveDate,
            string? effectiveDateSearchType,
            string? expiredDate,
            string? expiredDateSearchType,
            Guid costCenterGuid, Guid costCenterAssignmentGuid,
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
            if (costCenterAssignmentGuid != Guid.Empty)
            {
                whereClauses.Add("a.CostCenterAssignmentGuid = @costCenterAssignmentGuid");
                parameters.Add("@costCenterAssignmentGuid", costCenterAssignmentGuid);
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

            if (!string.IsNullOrWhiteSpace(profitCenterId))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProfitCenterId", "@profitCenterId", profitCenterIdSearchType, parameters, "profitCenterId", profitCenterId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(costCenterManagerEmployeeId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostCenterManagerEmployeeId", "@costCenterManagerEmployeeId", costCenterManagerEmployeeIdSearchType, parameters, "costCenterManagerEmployeeId", costCenterManagerEmployeeId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(budgetControlType))
            {
                var param = SqlFilterHelper.BuildFilter("a.BudgetControlType", "@budgetControlType", budgetControlTypeSearchType, parameters, "budgetControlType", budgetControlType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(effectiveDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.EffectiveDate", "@effectiveDate", effectiveDateSearchType, parameters, "effectiveDate", effectiveDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(expiredDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ExpiredDate", "@expiredDate", expiredDateSearchType, parameters, "expiredDate", expiredDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CostCenterId.CostCenterName AS CostCenterName

                FROM [app].[CostCenterAssignment] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CostCenterAssignmentId DESC";

            return await connection.QueryAsync<CostCenterAssignment>(sql, parameters, transaction);
        }
    }
}
