using System;
using System.Text.Json.Serialization;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record StandardReferenceScopeItemDto
    {
        public long StandardReferenceScopeItemId { get; set; }

        [JsonPropertyName("standardReferenceScopeItemGuid")]
        public Guid StandardReferenceScopeItemGuid { get; init; }
        public long StandardReferenceScopeId { get; set; }
        public Guid StandardReferenceScopeGuid { get; set; }
        public string StandardReferenceScopeItemInitial { get; set; } = string.Empty;
        public string StandardReferenceScopeItemName { get; set; } = string.Empty;
        public string? StandardReferenceScopeItemValue { get; set; }
        public int DisplayOrder { get; set; }

        public int StatusId { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[]? RowVersion { get; set; }
    }

    public record StandardReferenceScopeItemForCreationDto
    {
        public Guid StandardReferenceScopeGuid { get; set; }
        public string StandardReferenceScopeItemInitial { get; set; } = string.Empty;
        public string StandardReferenceScopeItemName { get; set; } = string.Empty;
        public string? StandardReferenceScopeItemValue { get; set; }
        public int DisplayOrder { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record StandardReferenceScopeItemForUpdateDto
    {
        [JsonPropertyName("standardReferenceScopeItemGuid")]
        public Guid StandardReferenceScopeItemGuid { get; set; }
        public Guid StandardReferenceScopeGuid { get; set; }
        public string StandardReferenceScopeItemInitial { get; set; } = string.Empty;
        public string StandardReferenceScopeItemName { get; set; } = string.Empty;
        public string? StandardReferenceScopeItemValue { get; set; }
        public int DisplayOrder { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record StandardReferenceScopeItemForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class StandardReferenceScopeItemSearchDto
    {
        public string? StandardReferenceScopeItemInitial { get; set; }
        public SearchType StandardReferenceScopeItemInitialSearchType { get; set; } = SearchType.Contains;

        public string? StandardReferenceScopeItemName { get; set; }
        public SearchType StandardReferenceScopeItemNameSearchType { get; set; } = SearchType.Contains;
    }
}
