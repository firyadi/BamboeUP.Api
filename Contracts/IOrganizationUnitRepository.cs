using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IOrganizationUnitRepository
    {
        Task<OrganizationUnit> GetOrganizationUnitAsync(Guid organizationUnitGuid, bool trackChanges);
        Task<OrganizationUnit?> GetOrganizationUnitByIdAsync(long organizationUnitId, bool trackChanges);
        Task<IEnumerable<OrganizationUnit>> GetAllOrganizationUnitsAsync(bool trackChanges);

        Task CreateOrganizationUnitAsync(OrganizationUnit organizationUnit, IDbTransaction? transaction = null);
        Task UpdateOrganizationUnitAsync(OrganizationUnit organizationUnit, IDbTransaction? transaction = null);
        Task DeleteOrganizationUnitAsync(Guid organizationUnitGuid, IDbTransaction? transaction = null);
        Task SoftDeleteOrganizationUnitAsync(OrganizationUnit organizationUnit, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<OrganizationUnit>> SearchOrganizationUnitAsync(
            string? organizationUnitCode, string? organizationUnitCodeSearchType,
            string? organizationUnitName, string? organizationUnitNameSearchType,
string? parentOrganizationUnitName, string? parentOrganizationUnitNameSearchType,

            IDbTransaction? transaction = null);
    }
}
