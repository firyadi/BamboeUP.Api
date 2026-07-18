using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PersonContactDto
    {
        public long PersonContactId { get; set; }
        public Guid PersonContactGuid { get; init; }
        public long PersonId { get; set; }
        public long SrContactType { get; set; }
        public string ContactValue { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public string? Remark { get; set; }
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

    public partial record PersonContactForCreationDto
    {
        public long PersonId { get; set; }
        public long SrContactType { get; set; }
        public string ContactValue { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public string? Remark { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record PersonContactForUpdateDto
    {
        public Guid PersonContactGuid { get; set; }
        public long PersonId { get; set; }
        public long SrContactType { get; set; }
        public string ContactValue { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public string? Remark { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record PersonContactForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PersonContactSearchDto
    {
        public string? SrContactType { get; set; }
        public SearchType SrContactTypeSearchType { get; set; } = SearchType.Contains;

        public string? ContactValue { get; set; }
        public SearchType ContactValueSearchType { get; set; } = SearchType.Contains;

        public string? IsPrimary { get; set; }
        public SearchType IsPrimarySearchType { get; set; } = SearchType.Contains;

        public string? Remark { get; set; }
        public SearchType RemarkSearchType { get; set; } = SearchType.Contains;
    }
}
