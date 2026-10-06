using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPositionGradeRepository
    {
        Task<PositionGrade?> GetPositionGradeAsync(Guid positionGradeGuid, bool trackChanges);
        Task<IEnumerable<PositionGrade>> GetAllPositionGradesAsync(bool trackChanges);

        Task CreatePositionGradeAsync(PositionGrade positionGrade, IDbTransaction? transaction = null);
        Task UpdatePositionGradeAsync(PositionGrade positionGrade, IDbTransaction? transaction = null);
        Task DeletePositionGradeAsync(Guid positionGradeGuid, IDbTransaction? transaction = null);
        Task SoftDeletePositionGradeAsync(PositionGrade positionGrade, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<PositionGrade>> SearchPositionGradeAsync(
            string? positionGradeCode,
            string? positionGradeCodeSearchType,
            string? positionGradeName,
            string? positionGradeNameSearchType,
            string? interval,
            string? intervalSearchType,
            string? ranking,
            string? rankingSearchType,
            string? positionGradeDescription,
            string? positionGradeDescriptionSearchType,

            IDbTransaction? transaction = null);
    }
}
