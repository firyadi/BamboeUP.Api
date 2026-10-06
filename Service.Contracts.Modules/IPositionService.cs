using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPositionService
    {
        Task<IEnumerable<PositionDto>> GetAllPositionsAsync(bool trackChanges);
        Task<PositionDto?> GetPositionByGuidAsync(Guid positionGuid, bool trackChanges);
        Task<PositionDto> CreatePositionAsync(PositionForCreationDto input);
        Task UpdatePositionAsync(Guid positionGuid, PositionForUpdateDto input, bool trackChanges);
        Task DeletePositionAsync(Guid positionGuid, PositionForDeleteDto input, bool trackChanges);
        Task DeletePositionByAdminAsync(Guid positionGuid, bool trackChanges);

        Task<IEnumerable<PositionDto>> SearchPositionAsync(
            string? positionCode, string? positionCodeSearchType, string? positionName, string? positionNameSearchType, string? summary, string? summarySearchType, string? validFrom, string? validFromSearchType, string? validto, string? validtoSearchType, string? positionDescription, string? positionDescriptionSearchType
, string? positionGradeName, string? positionGradeNameSearchType
, string? positionLevelName, string? positionLevelNameSearchType

        );
    }
}
