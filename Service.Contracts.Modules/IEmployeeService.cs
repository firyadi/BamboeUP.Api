using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(bool trackChanges);
        Task<EmployeeDto?> GetEmployeeByGuidAsync(Guid employeeGuid, bool trackChanges);
        Task<EmployeeDto> CreateEmployeeAsync(EmployeeForCreationDto input);
        Task UpdateEmployeeAsync(Guid employeeGuid, EmployeeForUpdateDto input, bool trackChanges);
        Task DeleteEmployeeAsync(Guid employeeGuid, EmployeeForDeleteDto input, bool trackChanges);
        Task DeleteEmployeeByAdminAsync(Guid employeeGuid, bool trackChanges);

        Task<IEnumerable<EmployeeDto>> SearchEmployeeAsync(
            string? employeeNo, string? employeeNoSearchType, string? hireDate, string? hireDateSearchType, string? probationEndDate, string? probationEndDateSearchType, string? confirmationDate, string? confirmationDateSearchType, string? terminationDate, string? terminationDateSearchType

        );
    }
}
