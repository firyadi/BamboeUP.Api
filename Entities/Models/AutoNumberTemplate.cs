using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("AutoNumberTemplate", Schema = "app")]
    public class AutoNumberTemplate
    {
        [Column("AutoNumberTemplateId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AutoNumberTemplateId { get; set; }

        [Key]
        [Column("AutoNumberTemplateGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AutoNumberTemplateGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string TemplateName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EffectiveDate { get; set; }

        // Multi-Company / Multi-Office Scope
        [Required]
        [MaxLength(20)]
        [Column("TemplateScopeType")]
        public string TemplateScopeType { get; set; } = "GLOBAL";
        // GLOBAL / COMPANY / OFFICE / DEPARTMENT

        [Required]
        public long SrFormMappingNumbering { get; set; }

        [Column("CompanyId")]
        public long? CompanyId { get; set; }

        [Column("CompanyOfficeId")]
        public long? CompanyOfficeId { get; set; }

        // Standard BamboeUP Audit Fields
        [Column("StatusId")]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; } 

        [Column("UpdatedById")]
        public long? UpdatedById { get; set; }

        [Column("UpdatedTime")]
        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long? DeletedById { get; set; }

        [Column("DeletedTime")]
        public DateTime? DeletedTime { get; set; }
    }
}
