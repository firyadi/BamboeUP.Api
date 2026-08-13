using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface ISkillService
    {
        Task<IEnumerable<SkillDto>> GetAllSkillsAsync(bool trackChanges);
        Task<SkillDto?> GetSkillByGuidAsync(Guid skillGuid, bool trackChanges);
        Task<SkillDto> CreateSkillAsync(SkillForCreationDto input);
        Task UpdateSkillAsync(Guid skillGuid, SkillForUpdateDto input, bool trackChanges);
        Task DeleteSkillAsync(Guid skillGuid, SkillForDeleteDto input, bool trackChanges);
        Task DeleteSkillByAdminAsync(Guid skillGuid, bool trackChanges);

        Task<IEnumerable<SkillDto>> SearchSkillAsync(
            string? skillCode, string? skillCodeSearchType, string? skillName, string? skillNameSearchType, string? skillShortName, string? skillShortNameSearchType, string? srSkillCategory, string? srSkillCategorySearchType, string? srSkillType, string? srSkillTypeSearchType, string? srDefaultSkillLevel, string? srDefaultSkillLevelSearchType, string? isCertificationRequired, string? isCertificationRequiredSearchType, string? isMandatory, string? isMandatorySearchType, string? description, string? descriptionSearchType, string? isActive, string? isActiveSearchType

        );
    }
}
