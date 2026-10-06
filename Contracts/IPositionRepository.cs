using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPositionRepository
    {
        Task<Position?> GetPositionAsync(Guid positionGuid, bool trackChanges);
        Task<IEnumerable<Position>> GetAllPositionsAsync(bool trackChanges);

        Task CreatePositionAsync(Position position, IDbTransaction? transaction = null);
        Task UpdatePositionAsync(Position position, IDbTransaction? transaction = null);
        Task DeletePositionAsync(Guid positionGuid, IDbTransaction? transaction = null);
        Task SoftDeletePositionAsync(Position position, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Position>> SearchPositionAsync(
            string? positionCode,
            string? positionCodeSearchType,
            string? positionName,
            string? positionNameSearchType,
            string? summary,
            string? summarySearchType,
            string? validFrom,
            string? validFromSearchType,
            string? validto,
            string? validtoSearchType,
            string? positionDescription,
            string? positionDescriptionSearchType,
string? positionGradeName, string? positionGradeNameSearchType,
string? positionLevelName, string? positionLevelNameSearchType,

            IDbTransaction? transaction = null);
    }
}
