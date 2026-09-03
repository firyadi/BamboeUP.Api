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
    public partial class CostCenterAssignmentRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : ICostCenterAssignmentRepository
    {
        private readonly string _table = dialect.Qualify("app", "CostCenterAssignment");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<CostCenterAssignment?> GetCostCenterAssignmentAsync(Guid costCenterAssignmentGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CostCenterId.CostCenterName AS CostCenterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterManagerEmployeeId.PersonName AS CostCenterManagerEmployeeName
, j_ProfitCenterId.ProfitCenterName AS ProfitCenterName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "CostCenter")} j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_CostCenterManagerEmployeeId ON a.CostCenterManagerEmployeeId = j_CostCenterManagerEmployeeId.PersonId
LEFT JOIN {dialect.Qualify("app", "ProfitCenter")} j_ProfitCenterId ON a.ProfitCenterId = j_ProfitCenterId.ProfitCenterId

                WHERE a.CostCenterAssignmentGuid = @costCenterAssignmentGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenterAssignment>(sql, new { costCenterAssignmentGuid });
        }

        public async Task<CostCenterAssignment?> GetByPersonGuidAndCostCenterAssignmentGuidAsync(Guid personGuid, Guid costCenterAssignmentGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CostCenterId.CostCenterName AS CostCenterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterManagerEmployeeId.PersonName AS CostCenterManagerEmployeeName
, j_ProfitCenterId.ProfitCenterName AS ProfitCenterName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "CostCenter")} j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_CostCenterManagerEmployeeId ON a.CostCenterManagerEmployeeId = j_CostCenterManagerEmployeeId.PersonId
LEFT JOIN {dialect.Qualify("app", "ProfitCenter")} j_ProfitCenterId ON a.ProfitCenterId = j_ProfitCenterId.ProfitCenterId

                WHERE j_CostCenterManagerEmployeeId.PersonGuid = @personGuid
                  AND a.CostCenterAssignmentGuid = @costCenterAssignmentGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenterAssignment>(sql, new { personGuid, costCenterAssignmentGuid });
        }

        public async Task<IEnumerable<CostCenterAssignment>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CostCenterId.CostCenterName AS CostCenterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterManagerEmployeeId.PersonName AS CostCenterManagerEmployeeName
, j_ProfitCenterId.ProfitCenterName AS ProfitCenterName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "CostCenter")} j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_CostCenterManagerEmployeeId ON a.CostCenterManagerEmployeeId = j_CostCenterManagerEmployeeId.PersonId
LEFT JOIN {dialect.Qualify("app", "ProfitCenter")} j_ProfitCenterId ON a.ProfitCenterId = j_ProfitCenterId.ProfitCenterId

                WHERE j_CostCenterManagerEmployeeId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.CostCenterAssignmentId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<CostCenterAssignment>(sql, new { personGuid });
        }

        public async Task CreateCostCenterAssignmentAsync(CostCenterAssignment costCenterAssignment, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(costCenterAssignment);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (CostCenterAssignmentGuid, CostCenterManagerEmployeeId, CreatedById, StatusId, CreatedTime, [CostCenterId], [CompanyId], [CompanyOfficeId], [ProfitCenterId], [BudgetControlType], [EffectiveDate], [ExpiredDate])
                VALUES
                (@CostCenterAssignmentGuid, @CostCenterManagerEmployeeId, @CreatedById, @StatusId, @CreatedTime, @CostCenterId, @CompanyId, @CompanyOfficeId, @ProfitCenterId, @BudgetControlType, @EffectiveDate, @ExpiredDate){dialect.ReturnIdentitySuffix("CostCenterAssignmentId")}";
            costCenterAssignment.CostCenterAssignmentId = await conn.QuerySingleAsync<long>(sql, costCenterAssignment, transaction);
        }

        public async Task UpdateCostCenterAssignmentAsync(CostCenterAssignment costCenterAssignment, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(costCenterAssignment);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [CostCenterManagerEmployeeId] = @CostCenterManagerEmployeeId,
                    [CostCenterId] = @CostCenterId,
                    [CompanyId] = @CompanyId,
                    [CompanyOfficeId] = @CompanyOfficeId,
                    [ProfitCenterId] = @ProfitCenterId,
                    [BudgetControlType] = @BudgetControlType,
                    [EffectiveDate] = @EffectiveDate,
                    [ExpiredDate] = @ExpiredDate,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE CostCenterAssignmentGuid = @CostCenterAssignmentGuid";
            await conn.ExecuteAsync(sql, costCenterAssignment, transaction);
        }

        public async Task SoftDeleteCostCenterAssignmentAsync(CostCenterAssignment costCenterAssignment, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(costCenterAssignment, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CostCenterAssignmentGuid = @CostCenterAssignmentGuid";

            await conn.ExecuteAsync(sql, costCenterAssignment, transaction);
        }

        public async Task DeleteCostCenterAssignmentAsync(Guid costCenterAssignmentGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE CostCenterAssignmentGuid = @costCenterAssignmentGuid";
            await conn.ExecuteAsync(sql, new { costCenterAssignmentGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE CostCenterManagerEmployeeId IN (SELECT PersonId FROM {dialect.Qualify("app", "Person")} WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<CostCenterAssignment>> SearchCostCenterAssignmentAsync(
            string? budgetControlType,
            string? budgetControlTypeSearchType,
            string? effectiveDate,
            string? effectiveDateSearchType,
            string? expiredDate,
            string? expiredDateSearchType,
            Guid personGuid, Guid costCenterAssignmentGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (personGuid != Guid.Empty)
            {
                whereClauses.Add("j_CostCenterManagerEmployeeId.PersonGuid = @personGuid");
                parameters.Add("@personGuid", personGuid);
            }
            if (costCenterAssignmentGuid != Guid.Empty)
            {
                whereClauses.Add("a.CostCenterAssignmentGuid = @costCenterAssignmentGuid");
                parameters.Add("@costCenterAssignmentGuid", costCenterAssignmentGuid);
            }

            if (!string.IsNullOrWhiteSpace(budgetControlType))
            {
                var param = SqlFilterHelper.BuildFilter("a.BudgetControlType", "@budgetControlType", budgetControlTypeSearchType, parameters, "budgetControlType", budgetControlType, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(effectiveDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.EffectiveDate", "@effectiveDate", effectiveDateSearchType, parameters, "effectiveDate", effectiveDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(expiredDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ExpiredDate", "@expiredDate", expiredDateSearchType, parameters, "expiredDate", expiredDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CostCenterId.CostCenterName AS CostCenterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterManagerEmployeeId.PersonName AS CostCenterManagerEmployeeName
, j_ProfitCenterId.ProfitCenterName AS ProfitCenterName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "CostCenter")} j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_CostCenterManagerEmployeeId ON a.CostCenterManagerEmployeeId = j_CostCenterManagerEmployeeId.PersonId
LEFT JOIN {dialect.Qualify("app", "ProfitCenter")} j_ProfitCenterId ON a.ProfitCenterId = j_ProfitCenterId.ProfitCenterId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CostCenterAssignmentId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<CostCenterAssignment>(sql, parameters, transaction);
        }
    }
}
