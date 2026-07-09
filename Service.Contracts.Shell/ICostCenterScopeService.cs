using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public partial interface ICostCenterScopeService
    {
        Task<IEnumerable<CostCenterScopeDto>> GetAllCostCenterScopesAsync(bool trackChanges);
        Task<CostCenterScopeDto> GetCostCenterScopeByGuidAsync(Guid costCenterScopeGuid, bool trackChanges);
        Task<CostCenterScopeDto> CreateCostCenterScopeAsync(Guid costCenterGuid, CostCenterScopeForCreationDto input);
        Task UpdateCostCenterScopeAsync(Guid costCenterGuid, Guid costCenterScopeGuid, CostCenterScopeForUpdateDto input, bool trackChanges);
        Task DeleteCostCenterScopeAsync(Guid costCenterGuid, Guid costCenterScopeGuid, CostCenterScopeForDeleteDto input, bool trackChanges);
        Task DeleteCostCenterScopeByAdminAsync(Guid costCenterScopeGuid, bool trackChanges);

        Task<IEnumerable<CostCenterScopeDto>> SearchCostCenterScopeAsync(
            string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? scopeType, string? scopeTypeSearchType,
            Guid costCenterGuid, Guid costCenterScopeGuid);

        // Detail (child) helpers
        Task<IEnumerable<CostCenterScopeDto>> GetAllByCostCenterGuidAsync(Guid costCenterGuid);
        Task<CostCenterScopeDto> GetByCostCenterGuidAndCostCenterScopeGuidAsync(Guid costCenterGuid, Guid costCenterScopeGuid);
    }
}
