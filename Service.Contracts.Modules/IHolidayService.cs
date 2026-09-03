using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IHolidayService
    {
        Task<IEnumerable<HolidayDto>> GetAllHolidaysAsync(bool trackChanges);
        Task<HolidayDto?> GetHolidayByGuidAsync(Guid holidayGuid, bool trackChanges);
        Task<HolidayDto> CreateHolidayAsync(HolidayForCreationDto input);
        Task UpdateHolidayAsync(Guid holidayGuid, HolidayForUpdateDto input, bool trackChanges);
        Task DeleteHolidayAsync(Guid holidayGuid, HolidayForDeleteDto input, bool trackChanges);
        Task DeleteHolidayByAdminAsync(Guid holidayGuid, bool trackChanges);

        Task<IEnumerable<HolidayDto>> SearchHolidayAsync(
            string? holidayDate, string? holidayDateSearchType, string? holidayName, string? holidayNameSearchType, string? holidayDescription, string? holidayDescriptionSearchType, string? srHolidayType, string? srHolidayTypeSearchType, string? isNational, string? isNationalSearchType, string? isCompanyHoliday, string? isCompanyHolidaySearchType

        );

        Task<IReadOnlyList<CalendarEventDto>> GetCalendarEventsAsync(DateTime start, DateTime end, string? category = null, string? search = null);
    }
}
