using System;

namespace Shared.DataTransferObjects
{
    /// <summary>
    /// DTO for a single item returned by app.fn_GetStandardReferenceItems.
    /// </summary>
    public record StandardReferenceItemDisplayDto
    {
        public long StandardReferenceId { get; init; }
        public Guid StandardReferenceGuid { get; init; }
        public string StandardReferenceInitial { get; init; } = string.Empty;
        public string StandardReferenceName { get; init; } = string.Empty;

        public long StandardReferenceItemId { get; init; }
        public Guid StandardReferenceItemGuid { get; init; }
        public string StandardReferenceItemInitial { get; init; } = string.Empty;
        public string StandardReferenceItemName { get; init; } = string.Empty;
        public string StandardReferenceItemValue { get; init; } = string.Empty;

        public int DisplayOrder { get; init; }

        /// <summary>
        /// "Scope" or "Template" — indicates which resolution path was used.
        /// </summary>
        public string DataSource { get; init; } = string.Empty;
    }
}
