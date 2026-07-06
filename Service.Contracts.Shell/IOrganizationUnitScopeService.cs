using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public partial interface IOrganizationUnitScopeService
    {
        Task<IEnumerable<OrganizationUnitScopeDto>> GetAllOrganizationUnitScopesAsync(bool trackChanges);
        Task<OrganizationUnitScopeDto> GetOrganizationUnitScopeByGuidAsync(Guid organizationUnitScopeGuid, bool trackChanges);
        Task<OrganizationUnitScopeDto> CreateOrganizationUnitScopeAsync(OrganizationUnitScopeForCreationDto input);
        Task UpdateOrganizationUnitScopeAsync(Guid organizationUnitScopeGuid, OrganizationUnitScopeForUpdateDto input, bool trackChanges);
        Task DeleteOrganizationUnitScopeAsync(Guid organizationUnitScopeGuid, OrganizationUnitScopeForDeleteDto input, bool trackChanges);
        Task DeleteOrganizationUnitScopeByAdminAsync(Guid organizationUnitScopeGuid, bool trackChanges);

        Task<IEnumerable<OrganizationUnitScopeDto>> SearchOrganizationUnitScopeAsync(
            string? scopeType, string? scopeTypeSearchType,
            Guid organizationUnitGuid, Guid organizationUnitScopeGuid);

        // Detail (child) helpers
        Task<IEnumerable<OrganizationUnitScopeDto>> GetAllByOrganizationUnitGuidAsync(Guid organizationUnitGuid);
        Task<OrganizationUnitScopeDto> GetByOrganizationUnitGuidAndOrganizationUnitScopeGuidAsync(Guid organizationUnitGuid, Guid organizationUnitScopeGuid);
    }
}
