using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPositionGradeService
    {
        Task<IEnumerable<PositionGradeDto>> GetAllPositionGradesAsync(bool trackChanges);
        Task<PositionGradeDto?> GetPositionGradeByGuidAsync(Guid positionGradeGuid, bool trackChanges);
        Task<PositionGradeDto> CreatePositionGradeAsync(PositionGradeForCreationDto input);
        Task UpdatePositionGradeAsync(Guid positionGradeGuid, PositionGradeForUpdateDto input, bool trackChanges);
        Task DeletePositionGradeAsync(Guid positionGradeGuid, PositionGradeForDeleteDto input, bool trackChanges);
        Task DeletePositionGradeByAdminAsync(Guid positionGradeGuid, bool trackChanges);

        Task<IEnumerable<PositionGradeDto>> SearchPositionGradeAsync(
            string? positionGradeCode, string? positionGradeCodeSearchType, string? positionGradeName, string? positionGradeNameSearchType, string? interval, string? intervalSearchType, string? ranking, string? rankingSearchType, string? positionGradeDescription, string? positionGradeDescriptionSearchType

        );
    }
}
