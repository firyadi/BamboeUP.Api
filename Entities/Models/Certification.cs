using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Certification", Schema = "app")]
    public partial class Certification
    {
        [Column("CertificationId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CertificationId { get; set; }

        [Key]
        [Column("CertificationGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CertificationGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string CertificationCode { get; set; } = string.Empty;

        [Required]
        public string CertificationName { get; set; } = string.Empty;

        public string? CertificationShortName { get; set; }

        public long SrCertificationCategory { get; set; }

        public long SrCertificationProvider { get; set; }

        public int? DefaultValidityMonth { get; set; }

        public bool IsLifetime { get; set; }

        public bool IsRenewable { get; set; }

        public long? LogoFileStorageId { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
public string? CertificationCategoryName { get; set; }
public string? CertificationProviderName { get; set; }



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
