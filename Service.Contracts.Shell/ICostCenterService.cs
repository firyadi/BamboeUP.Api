using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public partial interface ICostCenterService
    {
        Task<IEnumerable<CostCenterDto>> GetAllCostCentersAsync(bool trackChanges);
        Task<CostCenterDto> GetCostCenterByGuidAsync(Guid costCenterGuid, bool trackChanges);
        Task<CostCenterDto> CreateCostCenterAsync(CostCenterForCreationDto input);
        Task UpdateCostCenterAsync(Guid costCenterGuid, CostCenterForUpdateDto input, bool trackChanges);
        Task DeleteCostCenterAsync(Guid costCenterGuid, CostCenterForDeleteDto input, bool trackChanges);
        Task DeleteCostCenterByAdminAsync(Guid costCenterGuid, bool trackChanges);

        Task<IEnumerable<CostCenterDto>> SearchCostCenterAsync(
            string? costCenterCode, string? costCenterCodeSearchType, string? costCenterName, string? costCenterNameSearchType, string? costCenterDescription, string? costCenterDescriptionSearchType, string? parentCostCenterId, string? parentCostCenterIdSearchType, string? levelDepth, string? levelDepthSearchType, string? hierarchyPath, string? hierarchyPathSearchType

        );
    }
}
