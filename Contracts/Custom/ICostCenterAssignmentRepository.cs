using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ICostCenterAssignmentRepository
    {
        Task<IEnumerable<CostCenterAssignment>> GetAllByCostCenterGuidAsync(Guid costCenterGuid);
        Task<CostCenterAssignment?> GetByCostCenterGuidAndCostCenterAssignmentGuidAsync(Guid costCenterGuid, Guid costCenterAssignmentGuid);
        Task<IEnumerable<CostCenterAssignment>> SearchCostCenterAssignmentAsync(
            string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? profitCenterId, string? profitCenterIdSearchType, string? costCenterManagerEmployeeId, string? costCenterManagerEmployeeIdSearchType, string? budgetControlType, string? budgetControlTypeSearchType, string? effectiveDate, string? effectiveDateSearchType, string? expiredDate, string? expiredDateSearchType,
            Guid costCenterGuid, Guid costCenterAssignmentGuid,
            IDbTransaction? transaction = null);
    }
}
