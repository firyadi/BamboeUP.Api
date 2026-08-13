using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IEmployeeLanguageService
    {
        Task<IEnumerable<EmployeeLanguageDto>> GetAllEmployeeLanguagesAsync(bool trackChanges);
        Task<EmployeeLanguageDto?> GetEmployeeLanguageByGuidAsync(Guid employeeLanguageGuid, bool trackChanges);
        Task<EmployeeLanguageDto> CreateEmployeeLanguageAsync(Guid employeeGuid, EmployeeLanguageForCreationDto input);
        Task UpdateEmployeeLanguageAsync(Guid employeeGuid, Guid employeeLanguageGuid, EmployeeLanguageForUpdateDto input, bool trackChanges);
        Task DeleteEmployeeLanguageAsync(Guid employeeGuid, Guid employeeLanguageGuid, EmployeeLanguageForDeleteDto input, bool trackChanges);
        Task DeleteEmployeeLanguageByAdminAsync(Guid employeeLanguageGuid, bool trackChanges);

        Task<IEnumerable<EmployeeLanguageDto>> SearchEmployeeLanguageAsync(
            string? certificationName, string? certificationNameSearchType, string? score, string? scoreSearchType, string? issuedDate, string? issuedDateSearchType, string? expiredDate, string? expiredDateSearchType, string? isPrimary, string? isPrimarySearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeLanguageGuid);

        // Detail (child) helpers
        Task<IEnumerable<EmployeeLanguageDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid);
        Task<EmployeeLanguageDto?> GetByEmployeeGuidAndEmployeeLanguageGuidAsync(Guid employeeGuid, Guid employeeLanguageGuid);
    }
}
