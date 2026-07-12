using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IAwardService
    {
        Task<IEnumerable<AwardDto>> GetAllAwardsAsync(bool trackChanges);
        Task<AwardDto?> GetAwardByGuidAsync(Guid awardGuid, bool trackChanges);
        Task<AwardDto> CreateAwardAsync(AwardForCreationDto input);
        Task UpdateAwardAsync(Guid awardGuid, AwardForUpdateDto input, bool trackChanges);
        Task DeleteAwardAsync(Guid awardGuid, AwardForDeleteDto input, bool trackChanges);
        Task DeleteAwardByAdminAsync(Guid awardGuid, bool trackChanges);

        Task<IEnumerable<AwardDto>> SearchAwardAsync(
            string? awardCode, string? awardCodeSearchType, string? awardName, string? awardNameSearchType, string? srAwardCriteria, string? srAwardCriteriaSearchType, string? srAwardType, string? srAwardTypeSearchType, string? validFrom, string? validFromSearchType, string? validto, string? validtoSearchType, string? awardPrize, string? awardPrizeSearchType, string? note, string? noteSearchType

        );
    }
}
