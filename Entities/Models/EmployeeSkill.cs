using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("EmployeeSkill", Schema = "app")]
    public partial class EmployeeSkill
    {
        [Column("EmployeeSkillId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeSkillId { get; set; }

        [Key]
        [Column("EmployeeSkillGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeSkillGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long EmployeeId { get; set; }

        public long SkillId { get; set; }

        public long? SrSkillLevel { get; set; }

        public decimal? YearsofExperience { get; set; }

        public DateTime? LastUsedDate { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsCertified { get; set; }

        public string? CertificateNumber { get; set; }

        public DateTime? CertificateIssueDate { get; set; }

        public DateTime? CertificateExpiredDate { get; set; }

        public long? FileStorageId { get; set; }

        public string? Remarks { get; set; }

public string? SkillName { get; set; }
public string? SkillLevelName { get; set; }


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
