using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IEmployeeSkillService
    {
        Task<IEnumerable<EmployeeSkillDto>> GetAllEmployeeSkillsAsync(bool trackChanges);
        Task<EmployeeSkillDto?> GetEmployeeSkillByGuidAsync(Guid employeeSkillGuid, bool trackChanges);
        Task<EmployeeSkillDto> CreateEmployeeSkillAsync(Guid employeeGuid, EmployeeSkillForCreationDto input);
        Task UpdateEmployeeSkillAsync(Guid employeeGuid, Guid employeeSkillGuid, EmployeeSkillForUpdateDto input, bool trackChanges);
        Task DeleteEmployeeSkillAsync(Guid employeeGuid, Guid employeeSkillGuid, EmployeeSkillForDeleteDto input, bool trackChanges);
        Task DeleteEmployeeSkillByAdminAsync(Guid employeeSkillGuid, bool trackChanges);

        Task<IEnumerable<EmployeeSkillDto>> SearchEmployeeSkillAsync(
            string? yearsofExperience, string? yearsofExperienceSearchType, string? lastUsedDate, string? lastUsedDateSearchType, string? isPrimary, string? isPrimarySearchType, string? isCertified, string? isCertifiedSearchType, string? certificateNumber, string? certificateNumberSearchType, string? certificateIssueDate, string? certificateIssueDateSearchType, string? certificateExpiredDate, string? certificateExpiredDateSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeSkillGuid);

        // Detail (child) helpers
        Task<IEnumerable<EmployeeSkillDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid);
        Task<EmployeeSkillDto?> GetByEmployeeGuidAndEmployeeSkillGuidAsync(Guid employeeGuid, Guid employeeSkillGuid);
    }
}
