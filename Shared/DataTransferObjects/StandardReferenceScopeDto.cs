using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record StandardReferenceScopeDto
    {
        public long StandardReferenceScopeId { get; set; }

        [JsonPropertyName("standardReferenceScopeGuid")]
        public Guid StandardReferenceScopeGuid { get; init; }
        public long StandardReferenceId { get; set; }
        public Guid StandardReferenceGuid { get; set; }
        public string? StandardReferenceName { get; set; }

        public long? CompanyId { get; set; }
        public Guid? CompanyGuid { get; set; }
        public string? CompanyName { get; set; }

        public long? CompanyOfficeId { get; set; }
        public Guid? CompanyOfficeGuid { get; set; }
        public string? CompanyOfficeName { get; set; }

        // Child list
        public IEnumerable<StandardReferenceScopeItemDto>? StandardReferenceScopeItems { get; set; }

        public int StatusId { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[]? RowVersion { get; set; }
    }

    public record StandardReferenceScopeForCreationDto
    {
        public Guid StandardReferenceGuid { get; set; }
        public Guid? CompanyGuid { get; set; }
        public Guid? CompanyOfficeGuid { get; set; }

        public IEnumerable<StandardReferenceScopeItemForCreationDto>? StandardReferenceScopeItems { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record StandardReferenceScopeForUpdateDto
    {
        [JsonPropertyName("standardReferenceScopeGuid")]
        public Guid StandardReferenceScopeGuid { get; set; }
        public Guid StandardReferenceGuid { get; set; }
        public Guid? CompanyGuid { get; set; }
        public Guid? CompanyOfficeGuid { get; set; }

        public IEnumerable<StandardReferenceScopeItemForUpdateDto>? StandardReferenceScopeItems { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record StandardReferenceScopeForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class StandardReferenceScopeSearchDto
    {
        public Guid? CompanyGuid { get; set; }
        public Guid? CompanyOfficeGuid { get; set; }
    }
}
