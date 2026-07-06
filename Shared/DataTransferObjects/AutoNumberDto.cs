using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record AutoNumberDto
    {
        public long AutoNumberId { get; set; }
        public Guid AutoNumberGuid { get; init; }
        public string Prefik { get; set; }
        public string SeparatorAfterPrefik { get; set; }
        public bool? IsUsedDepartment { get; set; }
        public string SeparatorAfterDept { get; set; }
        public bool? IsUsedYear { get; set; }
        public byte? YearDigit { get; set; }
        public string SeparatorAfterYear { get; set; }
        public bool? IsUsedMonth { get; set; }
        public bool? IsMonthInRomawi { get; set; }
        public string SeparatorAfterMonth { get; set; }
        public bool? IsUsedDay { get; set; }
        public string SeparatorAfterDay { get; set; }
        public byte? NumberLength { get; set; }
        public byte? NumberGroupLength { get; set; }
        public string NumberGroupSeparator { get; set; }
        public string NumberFormat { get; set; }
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

    public record AutoNumberForCreationDto
    {
        public string Prefik { get; set; }
        public string SeparatorAfterPrefik { get; set; }
        public bool? IsUsedDepartment { get; set; }
        public string SeparatorAfterDept { get; set; }
        public bool? IsUsedYear { get; set; }
        public byte? YearDigit { get; set; }
        public string SeparatorAfterYear { get; set; }
        public bool? IsUsedMonth { get; set; }
        public bool? IsMonthInRomawi { get; set; }
        public string SeparatorAfterMonth { get; set; }
        public bool? IsUsedDay { get; set; }
        public string SeparatorAfterDay { get; set; }
        public byte? NumberLength { get; set; }
        public byte? NumberGroupLength { get; set; }
        public string NumberGroupSeparator { get; set; }
        public string NumberFormat { get; set; }
        public long CreatedById { get; set; } = 0;
        public DateTime CreatedTime { get; set; }
    }

    public record AutoNumberForUpdateDto
    {
        public string Prefik { get; set; }
        public string SeparatorAfterPrefik { get; set; }
        public bool? IsUsedDepartment { get; set; }
        public string SeparatorAfterDept { get; set; }
        public bool? IsUsedYear { get; set; }
        public byte? YearDigit { get; set; }
        public string SeparatorAfterYear { get; set; }
        public bool? IsUsedMonth { get; set; }
        public bool? IsMonthInRomawi { get; set; }
        public string SeparatorAfterMonth { get; set; }
        public bool? IsUsedDay { get; set; }
        public string SeparatorAfterDay { get; set; }
        public byte? NumberLength { get; set; }
        public byte? NumberGroupLength { get; set; }
        public string NumberGroupSeparator { get; set; }
        public string NumberFormat { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record AutoNumberForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    public class AutoNumberSearchDto
    {
        public string Prefik { get; set; }
        public SearchType PrefikSearchType { get; set; } = SearchType.Contains;
        public string SeparatorAfterPrefik { get; set; }
        public SearchType SeparatorAfterPrefikSearchType { get; set; } = SearchType.Contains;
        public string SeparatorAfterDept { get; set; }
        public SearchType SeparatorAfterDeptSearchType { get; set; } = SearchType.Contains;
        public byte? YearDigit { get; set; }
        public string SeparatorAfterYear { get; set; }
        public SearchType SeparatorAfterYearSearchType { get; set; } = SearchType.Contains;
        public string SeparatorAfterMonth { get; set; }
        public SearchType SeparatorAfterMonthSearchType { get; set; } = SearchType.Contains;
        public string SeparatorAfterDay { get; set; }
        public SearchType SeparatorAfterDaySearchType { get; set; } = SearchType.Contains;
        public byte? NumberLength { get; set; }
        public byte? NumberGroupLength { get; set; }
        public string NumberGroupSeparator { get; set; }
        public SearchType NumberGroupSeparatorSearchType { get; set; } = SearchType.Contains;
        public string NumberFormat { get; set; }
        public SearchType NumberFormatSearchType { get; set; } = SearchType.Contains;
    }
}
