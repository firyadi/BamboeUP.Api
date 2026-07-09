using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public partial interface ICostCenterAssignmentService
    {
        Task<IEnumerable<CostCenterAssignmentDto>> GetAllCostCenterAssignmentsAsync(bool trackChanges);
        Task<CostCenterAssignmentDto> GetCostCenterAssignmentByGuidAsync(Guid costCenterAssignmentGuid, bool trackChanges);
        Task<CostCenterAssignmentDto> CreateCostCenterAssignmentAsync(Guid costCenterGuid, CostCenterAssignmentForCreationDto input);
        Task UpdateCostCenterAssignmentAsync(Guid costCenterGuid, Guid costCenterAssignmentGuid, CostCenterAssignmentForUpdateDto input, bool trackChanges);
        Task DeleteCostCenterAssignmentAsync(Guid costCenterGuid, Guid costCenterAssignmentGuid, CostCenterAssignmentForDeleteDto input, bool trackChanges);
        Task DeleteCostCenterAssignmentByAdminAsync(Guid costCenterAssignmentGuid, bool trackChanges);

        Task<IEnumerable<CostCenterAssignmentDto>> SearchCostCenterAssignmentAsync(
            string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? profitCenterId, string? profitCenterIdSearchType, string? costCenterManagerEmployeeId, string? costCenterManagerEmployeeIdSearchType, string? budgetControlType, string? budgetControlTypeSearchType, string? effectiveDate, string? effectiveDateSearchType, string? expiredDate, string? expiredDateSearchType,
            Guid costCenterGuid, Guid costCenterAssignmentGuid);

        // Detail (child) helpers
        Task<IEnumerable<CostCenterAssignmentDto>> GetAllByCostCenterGuidAsync(Guid costCenterGuid);
        Task<CostCenterAssignmentDto> GetByCostCenterGuidAndCostCenterAssignmentGuidAsync(Guid costCenterGuid, Guid costCenterAssignmentGuid);
    }
}
