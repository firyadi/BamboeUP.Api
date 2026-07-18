using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PersonIdentificationDto
    {
        public long PersonIdentificationId { get; set; }
        public Guid PersonIdentificationGuid { get; init; }
        public long PersonId { get; set; }
        public long SrIdentificationType { get; set; }
        public string IdentificationValue { get; set; } = string.Empty;
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

    public partial record PersonIdentificationForCreationDto
    {
        public long PersonId { get; set; }
        public long SrIdentificationType { get; set; }
        public string IdentificationValue { get; set; } = string.Empty;
        public long CreatedById { get; set; } = 0;
    }

    public partial record PersonIdentificationForUpdateDto
    {
        public Guid PersonIdentificationGuid { get; set; }
        public long PersonId { get; set; }
        public long SrIdentificationType { get; set; }
        public string IdentificationValue { get; set; } = string.Empty;
        public long UpdatedById { get; set; }
    }

    public partial record PersonIdentificationForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PersonIdentificationSearchDto
    {
        public string? SrIdentificationType { get; set; }
        public SearchType SrIdentificationTypeSearchType { get; set; } = SearchType.Contains;

        public string? IdentificationValue { get; set; }
        public SearchType IdentificationValueSearchType { get; set; } = SearchType.Contains;
    }
}
