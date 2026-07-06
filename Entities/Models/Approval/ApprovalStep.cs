using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Approval
{
    [Table("ApprovalStep", Schema = "apv")]
    public class ApprovalStep
    {
        [Column("ApprovalStepId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ApprovalStepId { get; set; }

        [Key]
        [Column("ApprovalStepGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApprovalStepGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long ApprovalRequestId { get; set; }

        [Required]
        public int LevelOrder { get; set; }

        [Required]
        [MaxLength(200)]
        public string LevelName { get; set; } = string.Empty;

        [Required]
        public long ApproverUserId { get; set; }

        public long? DelegatedFromUserId { get; set; }

        /// <summary>0=Waiting, 1=Pending, 2=Approved, 3=Rejected, 4=Skipped, 5=Escalated, 6=Delegated</summary>
        [Required]
        public int StatusId { get; set; } = ApprovalConstants.StepStatus.Waiting;

        public DateTime? ActionTime { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime? SlaDeadline { get; set; }

        public bool IsEscalated { get; set; } = false;

        [Required]
        public long CreatedById { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
