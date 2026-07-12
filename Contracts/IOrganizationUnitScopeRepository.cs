using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IOrganizationUnitScopeRepository
    {
        Task<OrganizationUnitScope?> GetOrganizationUnitScopeAsync(Guid organizationUnitScopeGuid, bool trackChanges);
        Task<OrganizationUnitScope?> GetByOrganizationUnitGuidAndOrganizationUnitScopeGuidAsync(Guid organizationUnitGuid, Guid organizationUnitScopeGuid);
        Task<IEnumerable<OrganizationUnitScope>> GetAllByOrganizationUnitGuidAsync(Guid organizationUnitGuid);

        Task CreateOrganizationUnitScopeAsync(OrganizationUnitScope organizationUnitScope, IDbTransaction? transaction = null);
        Task UpdateOrganizationUnitScopeAsync(OrganizationUnitScope organizationUnitScope, IDbTransaction? transaction = null);
        Task SoftDeleteOrganizationUnitScopeAsync(OrganizationUnitScope organizationUnitScope, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteOrganizationUnitScopeAsync(Guid organizationUnitScopeGuid, IDbTransaction? transaction = null);
        Task DeleteByOrganizationUnitGuidAsync(Guid organizationUnitGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<OrganizationUnitScope>> SearchOrganizationUnitScopeAsync(
            string? scopeType,
            string? scopeTypeSearchType,
            Guid organizationUnitGuid, Guid organizationUnitScopeGuid,
            IDbTransaction? transaction = null);
    }
}
