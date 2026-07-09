using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public partial interface IOrganizationUnitService
    {
        Task<IEnumerable<OrganizationUnitDto>> GetAllOrganizationUnitsAsync(bool trackChanges);
        Task<OrganizationUnitDto> GetOrganizationUnitByGuidAsync(Guid organizationUnitGuid, bool trackChanges);
        Task<OrganizationUnitDto> CreateOrganizationUnitAsync(OrganizationUnitForCreationDto input);
        Task UpdateOrganizationUnitAsync(Guid organizationUnitGuid, OrganizationUnitForUpdateDto input, bool trackChanges);
        Task DeleteOrganizationUnitAsync(Guid organizationUnitGuid, OrganizationUnitForDeleteDto input, bool trackChanges);
        Task DeleteOrganizationUnitByAdminAsync(Guid organizationUnitGuid, bool trackChanges);

        Task<IEnumerable<OrganizationUnitDto>> SearchOrganizationUnitAsync(
            string? organizationUnitCode, string? organizationUnitCodeSearchType, string? organizationUnitName, string? organizationUnitNameSearchType, string? srOrganizationLevel, string? srOrganizationLevelSearchType, string? levelDepth, string? levelDepthSearchType, string? hierarchyPath, string? hierarchyPathSearchType

        );
    }
}
