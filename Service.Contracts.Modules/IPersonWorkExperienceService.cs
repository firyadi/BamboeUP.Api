using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonWorkExperienceService
    {
        Task<IEnumerable<PersonWorkExperienceDto>> GetAllPersonWorkExperiencesAsync(bool trackChanges);
        Task<PersonWorkExperienceDto?> GetPersonWorkExperienceByGuidAsync(Guid personWorkExperienceGuid, bool trackChanges);
        Task<PersonWorkExperienceDto> CreatePersonWorkExperienceAsync(Guid personGuid, PersonWorkExperienceForCreationDto input);
        Task UpdatePersonWorkExperienceAsync(Guid personGuid, Guid personWorkExperienceGuid, PersonWorkExperienceForUpdateDto input, bool trackChanges);
        Task DeletePersonWorkExperienceAsync(Guid personGuid, Guid personWorkExperienceGuid, PersonWorkExperienceForDeleteDto input, bool trackChanges);
        Task DeletePersonWorkExperienceByAdminAsync(Guid personWorkExperienceGuid, bool trackChanges);

        Task<IEnumerable<PersonWorkExperienceDto>> SearchPersonWorkExperienceAsync(
            string? srIndustry, string? srIndustrySearchType, string? srEmploymentType, string? srEmploymentTypeSearchType, string? companyName, string? companyNameSearchType, string? jobTitle, string? jobTitleSearchType, string? department, string? departmentSearchType, string? location, string? locationSearchType, string? supervisor, string? supervisorSearchType, string? jobDescription, string? jobDescriptionSearchType, string? startDate, string? startDateSearchType, string? endDate, string? endDateSearchType, string? isCurrentEmployment, string? isCurrentEmploymentSearchType, string? lastSalary, string? lastSalarySearchType, string? reasonforLeaving, string? reasonforLeavingSearchType, string? remarks, string? remarksSearchType,
            Guid personGuid, Guid personWorkExperienceGuid);

        // Detail (child) helpers
        Task<IEnumerable<PersonWorkExperienceDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<PersonWorkExperienceDto?> GetByPersonGuidAndPersonWorkExperienceGuidAsync(Guid personGuid, Guid personWorkExperienceGuid);
    }
}
