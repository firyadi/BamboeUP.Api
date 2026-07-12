using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public interface IHolidayService
    {
        Task<IEnumerable<HolidayDto>> GetAllHolidaysAsync(bool trackChanges);
        Task<HolidayDto?> GetHolidayByGuidAsync(Guid holidayGuid, bool trackChanges);
        Task<HolidayDto> CreateHolidayAsync(HolidayForCreationDto input);
        Task UpdateHolidayAsync(Guid holidayGuid, HolidayForUpdateDto input, bool trackChanges);
        Task DeleteHolidayAsync(Guid holidayGuid, HolidayForDeleteDto input, bool trackChanges);
        Task DeleteHolidayByAdminAsync(Guid holidayGuid, bool trackChanges);

        Task<IEnumerable<HolidayDto>> SearchHolidayAsync(
            string? yearPeriode, string? yearPeriodeSearchType, string? note, string? noteSearchType, DateTime? holidayDatesFrom, DateTime? holidayDatesTo
        );

    }
}
