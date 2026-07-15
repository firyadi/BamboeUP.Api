using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record PersonEmergencyContactDto
    {
        public long PersonEmergencyContactId { get; set; }
        public Guid PersonEmergencyContactGuid { get; init; }
        public long PersonId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public long SrRelationship { get; set; }
        public string? Phone { get; set; }
        public bool IsPrimary { get; set; }
public string? PersonName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record PersonEmergencyContactForCreationDto
    {
        public long PersonId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public long SrRelationship { get; set; }
        public string? Phone { get; set; }
        public bool IsPrimary { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record PersonEmergencyContactForUpdateDto
    {
        public Guid PersonEmergencyContactGuid { get; set; }
        public long PersonId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public long SrRelationship { get; set; }
        public string? Phone { get; set; }
        public bool IsPrimary { get; set; }
        public long UpdatedById { get; set; }
    }

    public record PersonEmergencyContactForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class PersonEmergencyContactSearchDto
    {
        public string? ContactName { get; set; }
        public SearchType ContactNameSearchType { get; set; } = SearchType.Contains;

        public string? SrRelationship { get; set; }
        public SearchType SrRelationshipSearchType { get; set; } = SearchType.Contains;

        public string? Phone { get; set; }
        public SearchType PhoneSearchType { get; set; } = SearchType.Contains;

        public string? IsPrimary { get; set; }
        public SearchType IsPrimarySearchType { get; set; } = SearchType.Contains;
    }
}
