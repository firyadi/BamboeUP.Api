using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("vw_StandardReferenceItem", Schema = "app")]
    public class VwStandardReferenceItem
    {
        [Column("StandardReferenceItemId")]
        public long StandardReferenceItemId { get; set; }

        [Column("StandardReferenceItemGuid")]
        public Guid StandardReferenceItemGuid { get; set; }

        [Column("CompanyId")]
        public long CompanyId { get; set; }

        [Column("CompanyGuid")]
        public Guid CompanyGuid { get; set; }

        [Column("CompanyOfficeId")]
        public long CompanyOfficeId { get; set; }

        [Column("CompanyOfficeGuid")]
        public Guid CompanyOfficeGuid { get; set; }

        [Column("StandardReferenceId")]
        public long StandardReferenceId { get; set; }

        [Column("StandardReferenceGuid")]
        public Guid StandardReferenceGuid { get; set; }

        [Column("StandardReferenceInitial")]
        public string StandardReferenceInitial { get; set; } = string.Empty;

        [Column("StandardReferenceItemInitial")]
        public string StandardReferenceItemInitial { get; set; } = string.Empty;

        [Column("StandardReferenceItemName")]
        public string StandardReferenceItemName { get; set; } = string.Empty;

        [Column("Note")]
        public string Note { get; set; } = string.Empty;
    }
}
