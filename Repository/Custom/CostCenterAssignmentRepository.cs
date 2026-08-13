using Dapper;
using Entities.Models;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class CostCenterAssignmentRepository
    {
        public async Task<IEnumerable<CostCenterAssignment>> GetAllByCostCenterGuidAsync(Guid costCenterGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CostCenterId.CostCenterName AS CostCenterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterManagerEmployeeId.CostCenterManagerEmployeeName AS CostCenterManagerEmployeeName
, j_ProfitCenterId.ProfitCenterName AS ProfitCenterName
                FROM [app].[CostCenterAssignment] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[Person] j_CostCenterManagerEmployeeId ON a.CostCenterManagerEmployeeId = j_CostCenterManagerEmployeeId.PersonId
LEFT JOIN [app].[ProfitCenter] j_ProfitCenterId ON a.ProfitCenterId = j_ProfitCenterId.ProfitCenterId
                WHERE (@costCenterGuid = '00000000-0000-0000-0000-000000000000' OR j_CostCenterId.CostCenterGuid = @costCenterGuid)
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.CostCenterAssignmentId DESC";
            return await connection.QueryAsync<CostCenterAssignment>(sql, new { costCenterGuid });
        }

        public async Task<CostCenterAssignment?> GetByCostCenterGuidAndCostCenterAssignmentGuidAsync(Guid costCenterGuid, Guid costCenterAssignmentGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP 1 a.*
, j_CostCenterId.CostCenterName AS CostCenterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterManagerEmployeeId.CostCenterManagerEmployeeName AS CostCenterManagerEmployeeName
, j_ProfitCenterId.ProfitCenterName AS ProfitCenterName
                FROM [app].[CostCenterAssignment] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[Person] j_CostCenterManagerEmployeeId ON a.CostCenterManagerEmployeeId = j_CostCenterManagerEmployeeId.PersonId
LEFT JOIN [app].[ProfitCenter] j_ProfitCenterId ON a.ProfitCenterId = j_ProfitCenterId.ProfitCenterId
                WHERE j_CostCenterId.CostCenterGuid = @costCenterGuid
                  AND a.CostCenterAssignmentGuid = @costCenterAssignmentGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CostCenterAssignment>(sql, new { costCenterGuid, costCenterAssignmentGuid });
        }

        public async Task<IEnumerable<CostCenterAssignment>> SearchCostCenterAssignmentAsync(
            string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? profitCenterId, string? profitCenterIdSearchType, string? costCenterManagerEmployeeId, string? costCenterManagerEmployeeIdSearchType, string? budgetControlType, string? budgetControlTypeSearchType, string? effectiveDate, string? effectiveDateSearchType, string? expiredDate, string? expiredDateSearchType,
            Guid costCenterGuid, Guid costCenterAssignmentGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
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
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterManagerEmployeeId.CostCenterManagerEmployeeName AS CostCenterManagerEmployeeName
, j_ProfitCenterId.ProfitCenterName AS ProfitCenterName
                FROM [app].[CostCenterAssignment] a
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[Person] j_CostCenterManagerEmployeeId ON a.CostCenterManagerEmployeeId = j_CostCenterManagerEmployeeId.PersonId
LEFT JOIN [app].[ProfitCenter] j_ProfitCenterId ON a.ProfitCenterId = j_ProfitCenterId.ProfitCenterId
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CostCenterAssignmentId DESC";

            return await connection.QueryAsync<CostCenterAssignment>(sql, parameters, transaction);
        }
    }
}
