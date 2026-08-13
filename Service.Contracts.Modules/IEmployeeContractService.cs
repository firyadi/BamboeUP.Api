using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IEmployeeContractService
    {
        Task<IEnumerable<EmployeeContractDto>> GetAllEmployeeContractsAsync(bool trackChanges);
        Task<EmployeeContractDto?> GetEmployeeContractByGuidAsync(Guid employeeContractGuid, bool trackChanges);
        Task<EmployeeContractDto> CreateEmployeeContractAsync(Guid employeeGuid, EmployeeContractForCreationDto input);
        Task UpdateEmployeeContractAsync(Guid employeeGuid, Guid employeeContractGuid, EmployeeContractForUpdateDto input, bool trackChanges);
        Task DeleteEmployeeContractAsync(Guid employeeGuid, Guid employeeContractGuid, EmployeeContractForDeleteDto input, bool trackChanges);
        Task DeleteEmployeeContractByAdminAsync(Guid employeeContractGuid, bool trackChanges);

        Task<IEnumerable<EmployeeContractDto>> SearchEmployeeContractAsync(
            string? contractNo, string? contractNoSearchType, string? contractStartDate, string? contractStartDateSearchType, string? contractEndDate, string? contractEndDateSearchType, string? effectiveDate, string? effectiveDateSearchType, string? renewalSequence, string? renewalSequenceSearchType, string? isCurrent, string? isCurrentSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeContractGuid);

        // Detail (child) helpers
        Task<IEnumerable<EmployeeContractDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid);
        Task<EmployeeContractDto?> GetByEmployeeGuidAndEmployeeContractGuidAsync(Guid employeeGuid, Guid employeeContractGuid);
    }
}
