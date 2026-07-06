using System;
using System.Text.Json.Serialization;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record StandardReferenceItemDto
    {
        public long StandardReferenceItemId { get; set; }

        [JsonPropertyName("standardReferenceItemGuid")]
        public Guid StandardReferenceItemGuid { get; init; }
        public long StandardReferenceId { get; set; }
        public Guid StandardReferenceGuid { get; set; }
        public string StandardReferenceItemInitial { get; set; } = string.Empty;
        public string StandardReferenceItemName { get; set; } = string.Empty;
        public string? StandardReferenceItemValue { get; set; }
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

    public record StandardReferenceItemForCreationDto
    {
        public Guid StandardReferenceGuid { get; set; }
        public string StandardReferenceItemInitial { get; set; } = string.Empty;
        public string StandardReferenceItemName { get; set; } = string.Empty;
        public string? StandardReferenceItemValue { get; set; }
        public int DisplayOrder { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record StandardReferenceItemForUpdateDto
    {
        [JsonPropertyName("standardReferenceItemGuid")]
        public Guid StandardReferenceItemGuid { get; set; }
        public Guid StandardReferenceGuid { get; set; }
        public string StandardReferenceItemInitial { get; set; } = string.Empty;
        public string StandardReferenceItemName { get; set; } = string.Empty;
        public string? StandardReferenceItemValue { get; set; }
        public int DisplayOrder { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record StandardReferenceItemForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class StandardReferenceItemSearchDto
    {
        public string? StandardReferenceItemInitial { get; set; }
        public SearchType StandardReferenceItemInitialSearchType { get; set; } = SearchType.Contains;

        public string? StandardReferenceItemName { get; set; }
        public SearchType StandardReferenceItemNameSearchType { get; set; } = SearchType.Contains;
    }
}
