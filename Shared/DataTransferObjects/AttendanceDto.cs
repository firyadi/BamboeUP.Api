using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record AttendanceDto
    {
        public long AttendanceId { get; set; }
        public Guid AttendanceGuid { get; init; }
        
        public long CompanyId { get; set; }
        public long CompanyOfficeId { get; set; }
        public long PersonId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? CheckinChannel { get; set; }
        public string? CheckOutChannel { get; set; }
        public long? CheckinFileStorageId { get; set; }
        public long? CheckOutFileStorageId { get; set; }
        public decimal? CheckinLatitude { get; set; }
        public decimal? CheckinLongitude { get; set; }
        public int? CheckinAccuracyMeters { get; set; }
        public decimal? CheckOutLatitude { get; set; }
        public decimal? CheckOutLongitude { get; set; }
        public int? CheckOutAccuracyMeters { get; set; }
        public string? Note { get; set; }
        public Guid? CheckinFileStorageGuid { get; set; }
public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? PersonName { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long SyncVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record AttendanceForCreationDto
    {
        public long CompanyId { get; set; }
        public long CompanyOfficeId { get; set; }
        public long PersonId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? CheckinChannel { get; set; }
        public string? CheckOutChannel { get; set; }
        public long? CheckinFileStorageId { get; set; }
        public long? CheckOutFileStorageId { get; set; }
        public decimal? CheckinLatitude { get; set; }
        public decimal? CheckinLongitude { get; set; }
        public int? CheckinAccuracyMeters { get; set; }
        public decimal? CheckOutLatitude { get; set; }
        public decimal? CheckOutLongitude { get; set; }
        public int? CheckOutAccuracyMeters { get; set; }
        public string? Note { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record AttendanceForUpdateDto
    {
        public long CompanyId { get; set; }
        public long CompanyOfficeId { get; set; }
        public long PersonId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? CheckinChannel { get; set; }
        public string? CheckOutChannel { get; set; }
        public long? CheckinFileStorageId { get; set; }
        public long? CheckOutFileStorageId { get; set; }
        public decimal? CheckinLatitude { get; set; }
        public decimal? CheckinLongitude { get; set; }
        public int? CheckinAccuracyMeters { get; set; }
        public decimal? CheckOutLatitude { get; set; }
        public decimal? CheckOutLongitude { get; set; }
        public int? CheckOutAccuracyMeters { get; set; }
        public string? Note { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record AttendanceForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class AttendanceSearchDto
    {
        public string? AttendanceDate { get; set; }
        public SearchType AttendanceDateSearchType { get; set; } = SearchType.Contains;

        public string? CheckinTime { get; set; }
        public SearchType CheckinTimeSearchType { get; set; } = SearchType.Contains;

        public string? CheckOutTime { get; set; }
        public SearchType CheckOutTimeSearchType { get; set; } = SearchType.Contains;

        public string? CheckinChannel { get; set; }
        public SearchType CheckinChannelSearchType { get; set; } = SearchType.Contains;

        public string? CheckOutChannel { get; set; }
        public SearchType CheckOutChannelSearchType { get; set; } = SearchType.Contains;

        public string? CheckinLatitude { get; set; }
        public SearchType CheckinLatitudeSearchType { get; set; } = SearchType.Contains;

        public string? CheckinLongitude { get; set; }
        public SearchType CheckinLongitudeSearchType { get; set; } = SearchType.Contains;

        public string? CheckinAccuracyMeters { get; set; }
        public SearchType CheckinAccuracyMetersSearchType { get; set; } = SearchType.Contains;

        public string? CheckOutLatitude { get; set; }
        public SearchType CheckOutLatitudeSearchType { get; set; } = SearchType.Contains;

        public string? CheckOutLongitude { get; set; }
        public SearchType CheckOutLongitudeSearchType { get; set; } = SearchType.Contains;

        public string? CheckOutAccuracyMeters { get; set; }
        public SearchType CheckOutAccuracyMetersSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;
public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? PersonName { get; set; }
public SearchType CompanyNameSearchType { get; set; } = SearchType.Contains;
public SearchType CompanyOfficeNameSearchType { get; set; } = SearchType.Contains;
public SearchType PersonNameSearchType { get; set; } = SearchType.Contains;

    }
}
