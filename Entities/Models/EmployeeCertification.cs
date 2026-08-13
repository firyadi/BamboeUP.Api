using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("EmployeeCertification", Schema = "app")]
    public partial class EmployeeCertification
    {
        [Column("EmployeeCertificationId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeCertificationId { get; set; }

        [Key]
        [Column("EmployeeCertificationGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeCertificationGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long EmployeeId { get; set; }

        public long CertificationId { get; set; }

        public string? CertificateNumber { get; set; }

        public DateTime? IssuedDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public string? Issuedby { get; set; }

        public decimal? Score { get; set; }

        public long? FileStorageId { get; set; }

        public bool IsLifetime { get; set; }

        public bool IsCurrent { get; set; }

        public string? Remarks { get; set; }

public string? CertificationName { get; set; }


        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long UpdatedById { get; set; } = 0;

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long DeletedById { get; set; } = 0;

        public DateTime? DeletedTime { get; set; }
    }
}
