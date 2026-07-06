using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Approval
{
    [Table("ApprovalTemplateLevel", Schema = "apv")]
    public class ApprovalTemplateLevel
    {
        [Column("ApprovalTemplateLevelId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ApprovalTemplateLevelId { get; set; }

        [Key]
        [Column("ApprovalTemplateLevelGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApprovalTemplateLevelGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long ApprovalTemplateId { get; set; }

        /// <summary>Urutan level: 1, 2, 3, ...</summary>
        [Required]
        public int LevelOrder { get; set; }

        [Required]
        [MaxLength(200)]
        public string LevelName { get; set; } = string.Empty;

        /// <summary>SPECIFIC_USER | USER_GROUP | DIRECT_MANAGER</summary>
        [Required]
        [MaxLength(50)]
        public string ApproverType { get; set; } = ApprovalConstants.ApproverType.SpecificUser;

        /// <summary>true = semua approver harus approve; false = cukup 1</summary>
        public bool RequireAllApprovers { get; set; } = false;

        /// <summary>true = level ini bisa di-skip jika level sebelumnya gagal/timeout</summary>
        public bool CanSkipIfPreviousNotApproved { get; set; } = false;

        /// <summary>SLA dalam jam; 0 = tidak ada SLA</summary>
        public int SlaHours { get; set; } = 0;

        /// <summary>LevelOrder tujuan eskalasi saat SLA habis; null = tidak ada eskalasi</summary>
        public int? EscalateToLevelOrder { get; set; }

        [Required]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Required]
        public long CreatedById { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }
}
