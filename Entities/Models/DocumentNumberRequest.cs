using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("DocumentNumberRequest", Schema = "app")]
    public class DocumentNumberRequest
    {
        [Column("DocumentNumberRequestId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DocumentNumberRequestId { get; set; }

        [Key]
        [Column("DocumentNumberRequestGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DocumentNumberRequestGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DocumentNo { get; set; } = string.Empty;

        public long AutoNumberLogId { get; set; }

        [MaxLength(100)]
        public string? ExternalReference { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Used"; // Draft / Used / Void

        public long? CompanyId { get; set; }
        public long? OfficeId { get; set; }
        public long? OrgUnitId { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; }

        [Column("UpdatedById")]
        public long? UpdatedById { get; set; }

        public DateTime? UpdatedTime { get; set; }
    }
}
