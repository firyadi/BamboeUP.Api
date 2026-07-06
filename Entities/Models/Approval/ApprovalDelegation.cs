using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Approval
{
    [Table("ApprovalDelegation", Schema = "apv")]
    public class ApprovalDelegation
    {
        [Column("ApprovalDelegationId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ApprovalDelegationId { get; set; }

        [Key]
        [Column("ApprovalDelegationGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApprovalDelegationGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long DelegatorUserId { get; set; }

        [Required]
        public long DelegateUserId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string? Notes { get; set; }

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
