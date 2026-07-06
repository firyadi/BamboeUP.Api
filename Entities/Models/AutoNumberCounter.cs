using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("AutoNumberCounter", Schema = "app")]
    public class AutoNumberCounter
    {
        [Column("AutoNumberCounterId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AutoNumberCounterId { get; set; }

        [Key]
        [Column("AutoNumberCounterGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AutoNumberCounterGuid { get; set; } = Guid.NewGuid();

        [Column("AutoNumberTemplateId")]
        public long AutoNumberTemplateId { get; set; }

        // Dynamic Scope Key
        [Column("CompanyId")]
        public long? CompanyId { get; set; }

        [Column("CompanyOfficeId")]
        public long? CompanyOfficeId { get; set; }

        [Column("OrganizationUnitId")]
        public int? OrganizationUnitId { get; set; }

        // Time Dimension Key
        [Column("YearNo")]
        public int? YearNo { get; set; }

        [Column("MonthNo")]
        public int? MonthNo { get; set; }

        [Column("DayNo")]
        public int? DayNo { get; set; }

        // Counter Value
        [Required]
        [Column("LastNumber")]
        public int LastNumber { get; set; } = 0;

        // Standard BamboeUP Audit Fields
        [Column("StatusId")]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

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
