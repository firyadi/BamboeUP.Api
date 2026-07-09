using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ICostCenterScopeRepository
    {
        Task<CostCenterScope> GetCostCenterScopeAsync(Guid costCenterScopeGuid, bool trackChanges);
        Task<CostCenterScope> GetByCostCenterGuidAndCostCenterScopeGuidAsync(Guid costCenterGuid, Guid costCenterScopeGuid);
        Task<IEnumerable<CostCenterScope>> GetAllByCostCenterGuidAsync(Guid costCenterGuid);

        Task CreateCostCenterScopeAsync(CostCenterScope costCenterScope, IDbTransaction? transaction = null);
        Task UpdateCostCenterScopeAsync(CostCenterScope costCenterScope, IDbTransaction? transaction = null);
        Task SoftDeleteCostCenterScopeAsync(CostCenterScope costCenterScope, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteCostCenterScopeAsync(Guid costCenterScopeGuid, IDbTransaction? transaction = null);
        Task DeleteByCostCenterGuidAsync(Guid costCenterGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<CostCenterScope>> SearchCostCenterScopeAsync(
            string? companyId,
            string? companyIdSearchType,
            string? companyOfficeId,
            string? companyOfficeIdSearchType,
            string? scopeType,
            string? scopeTypeSearchType,
            Guid costCenterGuid, Guid costCenterScopeGuid,
            IDbTransaction? transaction = null);
    }
}
