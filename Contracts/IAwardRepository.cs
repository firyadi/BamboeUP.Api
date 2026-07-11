using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IAwardRepository
    {
        Task<Award> GetAwardAsync(Guid awardGuid, bool trackChanges);
        Task<IEnumerable<Award>> GetAllAwardsAsync(bool trackChanges);

        Task CreateAwardAsync(Award award, IDbTransaction? transaction = null);
        Task UpdateAwardAsync(Award award, IDbTransaction? transaction = null);
        Task DeleteAwardAsync(Guid awardGuid, IDbTransaction? transaction = null);
        Task SoftDeleteAwardAsync(Award award, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Award>> SearchAwardAsync(
            string? awardCode,
            string? awardCodeSearchType,
            string? awardName,
            string? awardNameSearchType,
            string? srAwardCriteria,
            string? srAwardCriteriaSearchType,
            string? srAwardType,
            string? srAwardTypeSearchType,
            string? validFrom,
            string? validFromSearchType,
            string? validto,
            string? validtoSearchType,
            string? awardPrize,
            string? awardPrizeSearchType,
            string? note,
            string? noteSearchType,

            IDbTransaction? transaction = null);
    }
}
