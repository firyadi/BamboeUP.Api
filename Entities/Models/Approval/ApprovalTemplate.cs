using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Approval
{
    [Table("ApprovalTemplate", Schema = "apv")]
    public class ApprovalTemplate
    {
        [Column("ApprovalTemplateId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ApprovalTemplateId { get; set; }

        [Key]
        [Column("ApprovalTemplateGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApprovalTemplateGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;

        /// <summary>Module code: LEAVE, OVERTIME, PURCHASE, etc.</summary>
        [Required]
        [MaxLength(100)]
        public string ModuleCode { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

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
