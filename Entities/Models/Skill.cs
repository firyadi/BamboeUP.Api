using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Skill", Schema = "app")]
    public partial class Skill
    {
        [Column("SkillId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SkillId { get; set; }

        [Key]
        [Column("SkillGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SkillGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string SkillCode { get; set; } = string.Empty;

        [Required]
        public string SkillName { get; set; } = string.Empty;

        public string? SkillShortName { get; set; }

        public long SrSkillCategory { get; set; }

        public long? SrSkillType { get; set; }

        public long? SrDefaultSkillLevel { get; set; }

        public bool IsCertificationRequired { get; set; }

        public bool IsMandatory { get; set; }

        public long? FileStorageId { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }



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
