using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record HolidayDto
    {
        public long HolidayId { get; set; }
        public Guid HolidayGuid { get; init; }
        public string YearPeriode { get; set; }
        public DateTime HolidayDates { get; set; }
        public string Note { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public byte[] RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record HolidayForCreationDto
    {
        public string YearPeriode { get; set; }
        public DateTime HolidayDates { get; set; }
        public string Note { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record HolidayForUpdateDto
    {
        public string YearPeriode { get; set; }
        public DateTime HolidayDates { get; set; }
        public string Note { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record HolidayForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    public class HolidaySearchDto
    {
        public string? YearPeriode { get; set; }
        public SearchType YearPeriodeSearchType { get; set; } = SearchType.Contains;

        public DateTime? HolidayDatesFrom { get; set; }
        public DateTime? HolidayDatesTo { get; set; }

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;
    }
}
