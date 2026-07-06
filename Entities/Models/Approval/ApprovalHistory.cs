using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Approval
{
    [Table("ApprovalHistory", Schema = "apv")]
    public class ApprovalHistory
    {
        [Column("ApprovalHistoryId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ApprovalHistoryId { get; set; }

        [Key]
        [Column("ApprovalHistoryGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApprovalHistoryGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long ApprovalRequestId { get; set; }

        public long? ApprovalStepId { get; set; }

        /// <summary>SUBMITTED, APPROVED, REJECTED, DELEGATED, ESCALATED, CANCELLED</summary>
        [Required]
        [MaxLength(50)]
        public string ActionType { get; set; } = string.Empty;

        [Required]
        public long ActionByUserId { get; set; }

        [Required]
        public DateTime ActionTime { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Comment { get; set; }

        [MaxLength(50)]
        public string? FromStatus { get; set; }

        [MaxLength(50)]
        public string? ToStatus { get; set; }

        public int? LevelOrder { get; set; }
    }
}
