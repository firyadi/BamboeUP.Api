using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ISkillRepository
    {
        Task<Skill?> GetSkillAsync(Guid skillGuid, bool trackChanges);
        Task<IEnumerable<Skill>> GetAllSkillsAsync(bool trackChanges);

        Task CreateSkillAsync(Skill skill, IDbTransaction? transaction = null);
        Task UpdateSkillAsync(Skill skill, IDbTransaction? transaction = null);
        Task DeleteSkillAsync(Guid skillGuid, IDbTransaction? transaction = null);
        Task SoftDeleteSkillAsync(Skill skill, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Skill>> SearchSkillAsync(
            string? skillCode,
            string? skillCodeSearchType,
            string? skillName,
            string? skillNameSearchType,
            string? skillShortName,
            string? skillShortNameSearchType,
            string? srSkillCategory,
            string? srSkillCategorySearchType,
            string? srSkillType,
            string? srSkillTypeSearchType,
            string? srDefaultSkillLevel,
            string? srDefaultSkillLevelSearchType,
            string? isCertificationRequired,
            string? isCertificationRequiredSearchType,
            string? isMandatory,
            string? isMandatorySearchType,
            string? description,
            string? descriptionSearchType,
            string? isActive,
            string? isActiveSearchType,

            IDbTransaction? transaction = null);
    }
}
