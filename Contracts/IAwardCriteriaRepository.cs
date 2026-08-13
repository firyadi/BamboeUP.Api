using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IAwardCriteriaRepository
    {
        Task<AwardCriteria?> GetAwardCriteriaAsync(Guid awardCriteriaGuid, bool trackChanges);
        Task<AwardCriteria?> GetByAwardGuidAndAwardCriteriaGuidAsync(Guid awardGuid, Guid awardCriteriaGuid);
        Task<IEnumerable<AwardCriteria>> GetAllByAwardGuidAsync(Guid awardGuid);
        Task<IEnumerable<AwardCriteria>> GetAllByAwardIdAsync(long awardId);

        Task CreateAwardCriteriaAsync(AwardCriteria awardCriteria, IDbTransaction? transaction = null);
        Task UpdateAwardCriteriaAsync(AwardCriteria awardCriteria, IDbTransaction? transaction = null);
        Task SoftDeleteAwardCriteriaAsync(AwardCriteria awardCriteria, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteAwardCriteriaAsync(Guid awardCriteriaGuid, IDbTransaction? transaction = null);
        Task DeleteByAwardGuidAsync(Guid awardGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<AwardCriteria>> SearchAwardCriteriaAsync(
            string? lineNo,
            string? lineNoSearchType,
            string? criteriaCode,
            string? criteriaCodeSearchType,
            string? criteriaName,
            string? criteriaNameSearchType,
            string? minimumScore,
            string? minimumScoreSearchType,
            string? maximumScore,
            string? maximumScoreSearchType,
            string? weight,
            string? weightSearchType,
            string? note,
            string? noteSearchType,
            Guid awardGuid, Guid awardCriteriaGuid,
            IDbTransaction? transaction = null);
    }
}
