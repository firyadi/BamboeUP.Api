using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IHolidayRepository
    {
        Task<Holiday> GetHolidayAsync(Guid holidayGuid, bool trackChanges);
        Task<IEnumerable<Holiday>> GetAllHolidaysAsync(bool trackChanges);

        Task CreateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null);
        Task UpdateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null);
        Task DeleteHolidayAsync(Guid holidayGuid, IDbTransaction? transaction = null);
        Task SoftDeleteHolidayAsync(Holiday holiday, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Holiday>> SearchHolidayAsync(
            string? yearPeriode, string? yearPeriodeSearchType,
string? note, string? noteSearchType,
DateTime? holidayDatesFrom, DateTime? holidayDatesTo,
IDbTransaction? transaction = null);

        // Detail helpers (only if entity has a parent)
        
    }
}
