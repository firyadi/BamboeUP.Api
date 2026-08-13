using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IEmployeeAssetService
    {
        Task<IEnumerable<EmployeeAssetDto>> GetAllEmployeeAssetsAsync(bool trackChanges);
        Task<EmployeeAssetDto?> GetEmployeeAssetByGuidAsync(Guid employeeAssetGuid, bool trackChanges);
        Task<EmployeeAssetDto> CreateEmployeeAssetAsync(Guid employeeGuid, EmployeeAssetForCreationDto input);
        Task UpdateEmployeeAssetAsync(Guid employeeGuid, Guid employeeAssetGuid, EmployeeAssetForUpdateDto input, bool trackChanges);
        Task DeleteEmployeeAssetAsync(Guid employeeGuid, Guid employeeAssetGuid, EmployeeAssetForDeleteDto input, bool trackChanges);
        Task DeleteEmployeeAssetByAdminAsync(Guid employeeAssetGuid, bool trackChanges);

        Task<IEnumerable<EmployeeAssetDto>> SearchEmployeeAssetAsync(
            string? assignedDate, string? assignedDateSearchType, string? returnedDate, string? returnedDateSearchType, string? expectedReturnDate, string? expectedReturnDateSearchType, string? isCurrent, string? isCurrentSearchType, string? conditionWhenAssigned, string? conditionWhenAssignedSearchType, string? conditionWhenReturned, string? conditionWhenReturnedSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeAssetGuid);

        // Detail (child) helpers
        Task<IEnumerable<EmployeeAssetDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid);
        Task<EmployeeAssetDto?> GetByEmployeeGuidAndEmployeeAssetGuidAsync(Guid employeeGuid, Guid employeeAssetGuid);
    }
}
