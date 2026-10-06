using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPositionLevelRepository
    {
        Task<PositionLevel?> GetPositionLevelAsync(Guid positionLevelGuid, bool trackChanges);
        Task<IEnumerable<PositionLevel>> GetAllPositionLevelsAsync(bool trackChanges);

        Task CreatePositionLevelAsync(PositionLevel positionLevel, IDbTransaction? transaction = null);
        Task UpdatePositionLevelAsync(PositionLevel positionLevel, IDbTransaction? transaction = null);
        Task DeletePositionLevelAsync(Guid positionLevelGuid, IDbTransaction? transaction = null);
        Task SoftDeletePositionLevelAsync(PositionLevel positionLevel, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<PositionLevel>> SearchPositionLevelAsync(
            string? positionLevelCode,
            string? positionLevelCodeSearchType,
            string? positionLevelName,
            string? positionLevelNameSearchType,
            string? ranking,
            string? rankingSearchType,
            string? positionLevelDescription,
            string? positionLevelDescriptionSearchType,

            IDbTransaction? transaction = null);
    }
}
