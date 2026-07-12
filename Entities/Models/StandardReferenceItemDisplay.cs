using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    /// <summary>
    /// Read-only projection of app.fn_GetStandardReferenceItems result set.
    /// Represents a single reference item resolved from either a Scope override or the global Template.
    /// </summary>
    public class StandardReferenceItemDisplay
    {
        [Column("StandardReferenceId")]
        public long StandardReferenceId { get; set; }

        [Column("StandardReferenceGuid")]
        public Guid StandardReferenceGuid { get; set; }

        [Column("StandardReferenceInitial")]
        public string StandardReferenceInitial { get; set; } = string.Empty;

        [Column("StandardReferenceName")]
        public string StandardReferenceName { get; set; } = string.Empty;

        [Column("StandardReferenceItemId")]
        public long StandardReferenceItemId { get; set; }

        [Column("StandardReferenceItemGuid")]
        public Guid StandardReferenceItemGuid { get; set; }

        [Column("StandardReferenceItemInitial")]
        public string StandardReferenceItemInitial { get; set; } = string.Empty;

        [Column("StandardReferenceItemName")]
        public string StandardReferenceItemName { get; set; } = string.Empty;

        [Column("StandardReferenceItemValue")]
        public string StandardReferenceItemValue { get; set; } = string.Empty;

        [Column("DisplayOrder")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// "Scope" when resolved from a company/office scope override, "Template" when using the global template.
        /// </summary>
        [Column("DataSource")]
        public string DataSource { get; set; } = string.Empty;
    }
}
