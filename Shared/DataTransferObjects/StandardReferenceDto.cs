using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record StandardReferenceDto
    {
        public long StandardReferenceId { get; set; }
        public Guid StandardReferenceGuid { get; init; }
        public string StandardReferenceName { get; set; } = string.Empty;
        public string StandardReferenceInitial { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Child lists
        public IEnumerable<StandardReferenceItemDto>? StandardReferenceItems { get; set; }
        public IEnumerable<StandardReferenceScopeDto>? StandardReferenceScopes { get; set; }

        public int DefaultItemsCount { get; set; }
        public int ScopesCount { get; set; }

        public int StatusId { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
        public byte[]? RowVersion { get; set; }
    }

    public record StandardReferenceForCreationDto
    {
        public string StandardReferenceName { get; set; } = string.Empty;
        public string StandardReferenceInitial { get; set; } = string.Empty;
        public string? Description { get; set; }

        public IEnumerable<StandardReferenceItemForCreationDto>? StandardReferenceItems { get; set; }
        public IEnumerable<StandardReferenceScopeForCreationDto>? StandardReferenceScopes { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record StandardReferenceForUpdateDto
    {
        public string StandardReferenceName { get; set; } = string.Empty;
        public string StandardReferenceInitial { get; set; } = string.Empty;
        public string? Description { get; set; }

        public IEnumerable<StandardReferenceItemForUpdateDto>? StandardReferenceItems { get; set; }
        public IEnumerable<StandardReferenceScopeForUpdateDto>? StandardReferenceScopes { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record StandardReferenceForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class StandardReferenceSearchDto
    {
        public string? StandardReferenceInitial { get; set; }
        public SearchType StandardReferenceInitialSearchType { get; set; } = SearchType.Contains;

        public string? StandardReferenceName { get; set; }
        public SearchType StandardReferenceNameSearchType { get; set; } = SearchType.Contains;

        public string? Description { get; set; }
        public SearchType DescriptionSearchType { get; set; } = SearchType.Contains;
    }
}
