using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("AutoNumberLog", Schema = "app")]
    public class AutoNumberLog
    {
        [Column("AutoNumberLogId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AutoNumberLogId { get; set; }

        [Key]
        [Column("AutoNumberLogGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AutoNumberLogGuid { get; set; } = Guid.NewGuid();

        [Column("AutoNumberTemplateId")]
        public long AutoNumberTemplateId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("GeneratedNumber")]
        public string GeneratedNumber { get; set; } = string.Empty;

        [Column("CounterValue")]
        public int CounterValue { get; set; }

        [MaxLength(100)]
        [Column("ReferenceId")]
        public string ReferenceId { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("Status")]
        public string Status { get; set; } = "Used";

        // Scope snapshot saat generate
        [Column("CompanyId")]
        public long? CompanyId { get; set; }

        [Column("CompanyOfficeId")]
        public long? CompanyOfficeId { get; set; }

        [Column("OrganizationUnitId")]
        public long? OrganizationUnitId { get; set; }

        [Column("YearNo")]
        public int? YearNo { get; set; }

        [Column("MonthNo")]
        public int? MonthNo { get; set; }

        [Column("DayNo")]
        public int? DayNo { get; set; }

        // Audit
        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}
