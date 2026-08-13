using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface ICostCenterAssignmentService
    {
        Task<IEnumerable<CostCenterAssignmentDto>> GetAllCostCenterAssignmentsAsync(bool trackChanges);
        Task<CostCenterAssignmentDto?> GetCostCenterAssignmentByGuidAsync(Guid costCenterAssignmentGuid, bool trackChanges);
        Task<CostCenterAssignmentDto> CreateCostCenterAssignmentAsync(Guid personGuid, CostCenterAssignmentForCreationDto input);
        Task UpdateCostCenterAssignmentAsync(Guid personGuid, Guid costCenterAssignmentGuid, CostCenterAssignmentForUpdateDto input, bool trackChanges);
        Task DeleteCostCenterAssignmentAsync(Guid personGuid, Guid costCenterAssignmentGuid, CostCenterAssignmentForDeleteDto input, bool trackChanges);
        Task DeleteCostCenterAssignmentByAdminAsync(Guid costCenterAssignmentGuid, bool trackChanges);

        Task<IEnumerable<CostCenterAssignmentDto>> SearchCostCenterAssignmentAsync(
            string? budgetControlType, string? budgetControlTypeSearchType, string? effectiveDate, string? effectiveDateSearchType, string? expiredDate, string? expiredDateSearchType,
            Guid personGuid, Guid costCenterAssignmentGuid);

        // Detail (child) helpers
        Task<IEnumerable<CostCenterAssignmentDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<CostCenterAssignmentDto?> GetByPersonGuidAndCostCenterAssignmentGuidAsync(Guid personGuid, Guid costCenterAssignmentGuid);
    }
}
