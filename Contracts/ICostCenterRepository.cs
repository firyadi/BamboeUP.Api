using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ICostCenterRepository
    {
        Task<CostCenter?> GetCostCenterAsync(Guid costCenterGuid, bool trackChanges);
        Task<CostCenter?> GetCostCenterByIdAsync(long costCenterId, bool trackChanges);
        Task<IEnumerable<CostCenter>> GetAllCostCentersAsync(bool trackChanges);

        Task CreateCostCenterAsync(CostCenter costCenter, IDbTransaction? transaction = null);
        Task UpdateCostCenterAsync(CostCenter costCenter, IDbTransaction? transaction = null);
        Task DeleteCostCenterAsync(Guid costCenterGuid, IDbTransaction? transaction = null);
        Task SoftDeleteCostCenterAsync(CostCenter costCenter, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<CostCenter>> SearchCostCenterAsync(
            string? costCenterCode,
            string? costCenterCodeSearchType,
            string? costCenterName,
            string? costCenterNameSearchType,
            string? costCenterDescription,
            string? costCenterDescriptionSearchType,
            string? parentCostCenterId,
            string? parentCostCenterIdSearchType,
            string? levelDepth,
            string? levelDepthSearchType,
            string? hierarchyPath,
            string? hierarchyPathSearchType,
            IDbTransaction? transaction = null);
    }
}
