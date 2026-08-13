using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IEmployeeCareerService
    {
        Task<IEnumerable<EmployeeCareerDto>> GetAllEmployeeCareersAsync(bool trackChanges);
        Task<EmployeeCareerDto?> GetEmployeeCareerByGuidAsync(Guid employeeCareerGuid, bool trackChanges);
        Task<EmployeeCareerDto> CreateEmployeeCareerAsync(Guid employeeGuid, EmployeeCareerForCreationDto input);
        Task UpdateEmployeeCareerAsync(Guid employeeGuid, Guid employeeCareerGuid, EmployeeCareerForUpdateDto input, bool trackChanges);
        Task DeleteEmployeeCareerAsync(Guid employeeGuid, Guid employeeCareerGuid, EmployeeCareerForDeleteDto input, bool trackChanges);
        Task DeleteEmployeeCareerByAdminAsync(Guid employeeCareerGuid, bool trackChanges);

        Task<IEnumerable<EmployeeCareerDto>> SearchEmployeeCareerAsync(
            string? effectiveDate, string? effectiveDateSearchType, string? referenceNo, string? referenceNoSearchType, string? referenceDate, string? referenceDateSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeCareerGuid);

        // Detail (child) helpers
        Task<IEnumerable<EmployeeCareerDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid);
        Task<EmployeeCareerDto?> GetByEmployeeGuidAndEmployeeCareerGuidAsync(Guid employeeGuid, Guid employeeCareerGuid);
    }
}
