using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record PersonIdentificationDto
    {
        public long PersonIdentificationId { get; set; }
        public Guid PersonIdentificationGuid { get; init; }
        public long PersonId { get; set; }
        public long SrIdentificationTypeId { get; set; }
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

    public record PersonIdentificationForCreationDto
    {
        public long PersonId { get; set; }
        public long SrIdentificationTypeId { get; set; }
        public string IdentificationValue { get; set; } = string.Empty;
        public long CreatedById { get; set; } = 0;
    }

    public record PersonIdentificationForUpdateDto
    {
        public Guid PersonIdentificationGuid { get; set; }
        public long PersonId { get; set; }
        public long SrIdentificationTypeId { get; set; }
        public string IdentificationValue { get; set; } = string.Empty;
        public long UpdatedById { get; set; }
    }

    public record PersonIdentificationForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class PersonIdentificationSearchDto
    {
        public string? SrIdentificationTypeId { get; set; }
        public SearchType SrIdentificationTypeIdSearchType { get; set; } = SearchType.Contains;

        public string? IdentificationValue { get; set; }
        public SearchType IdentificationValueSearchType { get; set; } = SearchType.Contains;
    }
}
