using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Approval
{
    [Table("ApprovalRequest", Schema = "apv")]
    public class ApprovalRequest
    {
        [Column("ApprovalRequestId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ApprovalRequestId { get; set; }

        [Key]
        [Column("ApprovalRequestGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApprovalRequestGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long ApprovalTemplateId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RequestNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ModuleCode { get; set; } = string.Empty;

        /// <summary>GUID dokumen asal (Leave.LeaveGuid, Overtime.OvertimeGuid, dll)</summary>
        [Required]
        public Guid ReferenceGuid { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [Required]
        public long RequestedByUserId { get; set; }

        /// <summary>Level approval yang sedang aktif saat ini</summary>
        public int CurrentLevelOrder { get; set; } = 1;

        /// <summary>0=Draft, 1=Pending, 2=InProgress, 3=Approved, 4=Rejected, 5=Cancelled</summary>
        [Required]
        public int StatusId { get; set; } = ApprovalConstants.RequestStatus.Pending;

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime RequestedTime { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedTime { get; set; }

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
