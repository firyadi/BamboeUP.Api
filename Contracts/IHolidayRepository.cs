using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IHolidayRepository
    {
        Task<Holiday?> GetHolidayAsync(Guid holidayGuid, bool trackChanges);
        Task<IEnumerable<Holiday>> GetAllHolidaysAsync(bool trackChanges);

        Task CreateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null);
        Task UpdateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null);
        Task DeleteHolidayAsync(Guid holidayGuid, IDbTransaction? transaction = null);
        Task SoftDeleteHolidayAsync(Holiday holiday, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Holiday>> SearchHolidayAsync(
            string? holidayDate,
            string? holidayDateSearchType,
            string? holidayName,
            string? holidayNameSearchType,
            string? holidayDescription,
            string? holidayDescriptionSearchType,
            string? srHolidayType,
            string? srHolidayTypeSearchType,
            string? isNational,
            string? isNationalSearchType,
            string? isCompanyHoliday,
            string? isCompanyHolidaySearchType,

            IDbTransaction? transaction = null);
    }
}
