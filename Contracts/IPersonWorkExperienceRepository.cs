using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonWorkExperienceRepository
    {
        Task<PersonWorkExperience?> GetPersonWorkExperienceAsync(Guid personWorkExperienceGuid, bool trackChanges);
        Task<PersonWorkExperience?> GetByPersonGuidAndPersonWorkExperienceGuidAsync(Guid personGuid, Guid personWorkExperienceGuid);
        Task<IEnumerable<PersonWorkExperience>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreatePersonWorkExperienceAsync(PersonWorkExperience personWorkExperience, IDbTransaction? transaction = null);
        Task UpdatePersonWorkExperienceAsync(PersonWorkExperience personWorkExperience, IDbTransaction? transaction = null);
        Task SoftDeletePersonWorkExperienceAsync(PersonWorkExperience personWorkExperience, long deletedBy, IDbTransaction? transaction = null);
        Task DeletePersonWorkExperienceAsync(Guid personWorkExperienceGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<PersonWorkExperience>> SearchPersonWorkExperienceAsync(
            string? srIndustry,
            string? srIndustrySearchType,
            string? srEmploymentType,
            string? srEmploymentTypeSearchType,
            string? companyName,
            string? companyNameSearchType,
            string? jobTitle,
            string? jobTitleSearchType,
            string? department,
            string? departmentSearchType,
            string? location,
            string? locationSearchType,
            string? supervisor,
            string? supervisorSearchType,
            string? jobDescription,
            string? jobDescriptionSearchType,
            string? startDate,
            string? startDateSearchType,
            string? endDate,
            string? endDateSearchType,
            string? isCurrentEmployment,
            string? isCurrentEmploymentSearchType,
            string? lastSalary,
            string? lastSalarySearchType,
            string? reasonforLeaving,
            string? reasonforLeavingSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid personGuid, Guid personWorkExperienceGuid,
            IDbTransaction? transaction = null);
    }
}
