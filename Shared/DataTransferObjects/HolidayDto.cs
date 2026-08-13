using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record HolidayDto
    {
        public long HolidayId { get; set; }
        public Guid HolidayGuid { get; init; }
        
        public DateTime HolidayDate { get; set; }
        public string HolidayName { get; set; } = string.Empty;
        public string? HolidayDescription { get; set; }
        public long? SrHolidayType { get; set; }
        public bool IsNational { get; set; }
        public bool IsCompanyHoliday { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record HolidayForCreationDto
    {
        public DateTime HolidayDate { get; set; }
        public string HolidayName { get; set; } = string.Empty;
        public string? HolidayDescription { get; set; }
        public long? SrHolidayType { get; set; }
        public bool IsNational { get; set; }
        public bool IsCompanyHoliday { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record HolidayForUpdateDto
    {
        public DateTime HolidayDate { get; set; }
        public string HolidayName { get; set; } = string.Empty;
        public string? HolidayDescription { get; set; }
        public long? SrHolidayType { get; set; }
        public bool IsNational { get; set; }
        public bool IsCompanyHoliday { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record HolidayForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class HolidaySearchDto
    {
        public string? HolidayDate { get; set; }
        public SearchType HolidayDateSearchType { get; set; } = SearchType.Contains;

        public string? HolidayName { get; set; }
        public SearchType HolidayNameSearchType { get; set; } = SearchType.Contains;

        public string? HolidayDescription { get; set; }
        public SearchType HolidayDescriptionSearchType { get; set; } = SearchType.Contains;

        public string? SrHolidayType { get; set; }
        public SearchType SrHolidayTypeSearchType { get; set; } = SearchType.Contains;

        public string? IsNational { get; set; }
        public SearchType IsNationalSearchType { get; set; } = SearchType.Contains;

        public string? IsCompanyHoliday { get; set; }
        public SearchType IsCompanyHolidaySearchType { get; set; } = SearchType.Contains;

    }
}
