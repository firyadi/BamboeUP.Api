using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Approval
{
    [Table("ApprovalTemplateLevelApprover", Schema = "apv")]
    public class ApprovalTemplateLevelApprover
    {
        [Column("ApprovalTemplateLevelApproverId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ApprovalTemplateLevelApproverId { get; set; }

        [Key]
        [Column("ApprovalTemplateLevelApproverGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApprovalTemplateLevelApproverGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long ApprovalTemplateLevelId { get; set; }

        /// <summary>Diisi jika ApproverType = SPECIFIC_USER</summary>
        public long? UserId { get; set; }

        /// <summary>Diisi jika ApproverType = USER_GROUP</summary>
        public long? UserGroupId { get; set; }

        [Required]
        public int StatusId { get; set; } = 1;

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
