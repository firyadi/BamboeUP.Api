using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IAwardCriteriaService
    {
        Task<IEnumerable<AwardCriteriaDto>> GetAllAwardCriteriaAsync(bool trackChanges);
        Task<AwardCriteriaDto?> GetAwardCriteriaByGuidAsync(Guid awardCriteriaGuid, bool trackChanges);
        Task<AwardCriteriaDto> CreateAwardCriteriaAsync(Guid awardGuid, AwardCriteriaForCreationDto input);
        Task UpdateAwardCriteriaAsync(Guid awardGuid, Guid awardCriteriaGuid, AwardCriteriaForUpdateDto input, bool trackChanges);
        Task DeleteAwardCriteriaAsync(Guid awardGuid, Guid awardCriteriaGuid, AwardCriteriaForDeleteDto input, bool trackChanges);
        Task DeleteAwardCriteriaByAdminAsync(Guid awardCriteriaGuid, bool trackChanges);

        Task<IEnumerable<AwardCriteriaDto>> SearchAwardCriteriaAsync(
            string? lineNo, string? lineNoSearchType, string? criteriaCode, string? criteriaCodeSearchType, string? criteriaName, string? criteriaNameSearchType, string? minimumScore, string? minimumScoreSearchType, string? maximumScore, string? maximumScoreSearchType, string? weight, string? weightSearchType, string? note, string? noteSearchType,
            Guid awardGuid, Guid awardCriteriaGuid);

        // Detail (child) helpers
        Task<IEnumerable<AwardCriteriaDto>> GetAllByAwardGuidAsync(Guid awardGuid);
        Task<AwardCriteriaDto?> GetByAwardGuidAndAwardCriteriaGuidAsync(Guid awardGuid, Guid awardCriteriaGuid);
    }
}
