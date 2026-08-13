using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IAwardCriteriaRuleService
    {
        Task<IEnumerable<AwardCriteriaRuleDto>> GetAllAwardCriteriaRulesAsync(bool trackChanges);
        Task<AwardCriteriaRuleDto?> GetAwardCriteriaRuleByGuidAsync(Guid awardCriteriaRuleGuid, bool trackChanges);
        Task<AwardCriteriaRuleDto> CreateAwardCriteriaRuleAsync(Guid awardCriteriaGuid, AwardCriteriaRuleForCreationDto input);
        Task UpdateAwardCriteriaRuleAsync(Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid, AwardCriteriaRuleForUpdateDto input, bool trackChanges);
        Task DeleteAwardCriteriaRuleAsync(Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid, AwardCriteriaRuleForDeleteDto input, bool trackChanges);
        Task DeleteAwardCriteriaRuleByAdminAsync(Guid awardCriteriaRuleGuid, bool trackChanges);

        Task<IEnumerable<AwardCriteriaRuleDto>> SearchAwardCriteriaRuleAsync(
            string? ruleCode, string? ruleCodeSearchType, string? ruleName, string? ruleNameSearchType, string? @operator, string? @operatorSearchType, string? valueFrom, string? valueFromSearchType, string? valueto, string? valuetoSearchType, string? score, string? scoreSearchType, string? note, string? noteSearchType,
            Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid);

        // SubDetail (child) helpers
        Task<IEnumerable<AwardCriteriaRuleDto>> GetAllByAwardCriteriaGuidAsync(Guid awardCriteriaGuid);
        Task<AwardCriteriaRuleDto?> GetByAwardCriteriaGuidAndAwardCriteriaRuleGuidAsync(Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid);
    }
}
