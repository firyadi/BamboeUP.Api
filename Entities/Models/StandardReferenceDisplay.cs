using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("vw_StandardReference_Display", Schema = "app")]
    public class StandardReferenceDisplay
    {
        [Column("StandardReferenceItemId")]
        public long StandardReferenceItemId { get; set; }

        [Column("CompanyId")]
        public long CompanyId { get; set; }

        [Column("CompanyOfficeId")]
        public long CompanyOfficeId { get; set; }

        [Column("StandardReferenceGroupId")]
        public long? StandardReferenceGroupId { get; set; }

        [Column("StandardReferenceInitial")]
        public string StandardReferenceInitial { get; set; } = string.Empty;

        [Column("StandardReferenceItemName")]
        public string StandardReferenceItemName { get; set; } = string.Empty;
    }
}
