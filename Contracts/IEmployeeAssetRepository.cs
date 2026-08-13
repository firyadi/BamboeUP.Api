using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IEmployeeAssetRepository
    {
        Task<EmployeeAsset?> GetEmployeeAssetAsync(Guid employeeAssetGuid, bool trackChanges);
        Task<EmployeeAsset?> GetByEmployeeGuidAndEmployeeAssetGuidAsync(Guid employeeGuid, Guid employeeAssetGuid);
        Task<IEnumerable<EmployeeAsset>> GetAllByEmployeeGuidAsync(Guid employeeGuid);

        Task CreateEmployeeAssetAsync(EmployeeAsset employeeAsset, IDbTransaction? transaction = null);
        Task UpdateEmployeeAssetAsync(EmployeeAsset employeeAsset, IDbTransaction? transaction = null);
        Task SoftDeleteEmployeeAssetAsync(EmployeeAsset employeeAsset, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteEmployeeAssetAsync(Guid employeeAssetGuid, IDbTransaction? transaction = null);
        Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<EmployeeAsset>> SearchEmployeeAssetAsync(
            string? assignedDate,
            string? assignedDateSearchType,
            string? returnedDate,
            string? returnedDateSearchType,
            string? expectedReturnDate,
            string? expectedReturnDateSearchType,
            string? isCurrent,
            string? isCurrentSearchType,
            string? conditionWhenAssigned,
            string? conditionWhenAssignedSearchType,
            string? conditionWhenReturned,
            string? conditionWhenReturnedSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeAssetGuid,
            IDbTransaction? transaction = null);
    }
}
