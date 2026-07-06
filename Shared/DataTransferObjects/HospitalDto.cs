using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record HospitalDto
    {
        public long HospitalId { get; set; }
        public Guid HospitalGuid { get; init; }
        
        public string HospitalName { get; set; }
        public string? HospitalCode { get; set; }
        public string? ShortName { get; set; }
        public string? LicenseNo { get; set; }
        public string? HospitalType { get; set; }
        public string? HospitalClass { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public int StatusId { get; set; }
        public byte[] RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record HospitalForCreationDto
    {
        public string HospitalName { get; set; }
        public string? HospitalCode { get; set; }
        public string? ShortName { get; set; }
        public string? LicenseNo { get; set; }
        public string? HospitalType { get; set; }
        public string? HospitalClass { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record HospitalForUpdateDto
    {
        public string HospitalName { get; set; }
        public string? HospitalCode { get; set; }
        public string? ShortName { get; set; }
        public string? LicenseNo { get; set; }
        public string? HospitalType { get; set; }
        public string? HospitalClass { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public long UpdatedById { get; set; }
    }

    public record HospitalForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class HospitalSearchDto
    {
        public string? HospitalName { get; set; }
        public SearchType HospitalNameSearchType { get; set; } = SearchType.Contains;

        public string? HospitalCode { get; set; }
        public SearchType HospitalCodeSearchType { get; set; } = SearchType.Contains;

        public string? ShortName { get; set; }
        public SearchType ShortNameSearchType { get; set; } = SearchType.Contains;

        public string? LicenseNo { get; set; }
        public SearchType LicenseNoSearchType { get; set; } = SearchType.Contains;

        public string? HospitalType { get; set; }
        public SearchType HospitalTypeSearchType { get; set; } = SearchType.Contains;

        public string? HospitalClass { get; set; }
        public SearchType HospitalClassSearchType { get; set; } = SearchType.Contains;

        public string? PhoneNo { get; set; }
        public SearchType PhoneNoSearchType { get; set; } = SearchType.Contains;

        public string? Email { get; set; }
        public SearchType EmailSearchType { get; set; } = SearchType.Contains;

        public string? Website { get; set; }
        public SearchType WebsiteSearchType { get; set; } = SearchType.Contains;
    }
}
