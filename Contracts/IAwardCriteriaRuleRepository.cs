using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IAwardCriteriaRuleRepository
    {
        Task<AwardCriteriaRule?> GetAwardCriteriaRuleAsync(Guid awardCriteriaRuleGuid, bool trackChanges);
        Task<AwardCriteriaRule?> GetByAwardCriteriaGuidAndAwardCriteriaRuleGuidAsync(Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid);
        Task<IEnumerable<AwardCriteriaRule>> GetAllByAwardCriteriaGuidAsync(Guid awardCriteriaGuid);
        Task<IEnumerable<AwardCriteriaRule>> GetAllByAwardCriteriaIdAsync(long awardCriteriaId);

        Task CreateAwardCriteriaRuleAsync(AwardCriteriaRule awardCriteriaRule, IDbTransaction? transaction = null);
        Task UpdateAwardCriteriaRuleAsync(AwardCriteriaRule awardCriteriaRule, IDbTransaction? transaction = null);
        Task SoftDeleteAwardCriteriaRuleAsync(AwardCriteriaRule awardCriteriaRule, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteAwardCriteriaRuleAsync(Guid awardCriteriaRuleGuid, IDbTransaction? transaction = null);
        Task DeleteByAwardCriteriaGuidAsync(Guid awardCriteriaGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<AwardCriteriaRule>> SearchAwardCriteriaRuleAsync(
            string? ruleCode,
            string? ruleCodeSearchType,
            string? ruleName,
            string? ruleNameSearchType,
            string? @operator,
            string? @operatorSearchType,
            string? valueFrom,
            string? valueFromSearchType,
            string? valueto,
            string? valuetoSearchType,
            string? score,
            string? scoreSearchType,
            string? note,
            string? noteSearchType,
            Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid,
            IDbTransaction? transaction = null);
    }
}
