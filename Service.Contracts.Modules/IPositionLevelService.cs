using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPositionLevelService
    {
        Task<IEnumerable<PositionLevelDto>> GetAllPositionLevelsAsync(bool trackChanges);
        Task<PositionLevelDto?> GetPositionLevelByGuidAsync(Guid positionLevelGuid, bool trackChanges);
        Task<PositionLevelDto> CreatePositionLevelAsync(PositionLevelForCreationDto input);
        Task UpdatePositionLevelAsync(Guid positionLevelGuid, PositionLevelForUpdateDto input, bool trackChanges);
        Task DeletePositionLevelAsync(Guid positionLevelGuid, PositionLevelForDeleteDto input, bool trackChanges);
        Task DeletePositionLevelByAdminAsync(Guid positionLevelGuid, bool trackChanges);

        Task<IEnumerable<PositionLevelDto>> SearchPositionLevelAsync(
            string? positionLevelCode, string? positionLevelCodeSearchType, string? positionLevelName, string? positionLevelNameSearchType, string? ranking, string? rankingSearchType, string? positionLevelDescription, string? positionLevelDescriptionSearchType

        );
    }
}
