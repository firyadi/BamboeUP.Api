using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("WorkItem", Schema = "app")]
    public partial class WorkItem
    {
        [Column("WorkItemId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long WorkItemId { get; set; }

        [Key]
        [Column("WorkItemGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid WorkItemGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long OwnerUserId { get; set; }

        [Required]
        public string WorkItemCode { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Module { get; set; }

        public string? WorkItemType { get; set; }

        public string? OwnerName { get; set; }

        [Required]
        public string WorkflowStatus { get; set; } = string.Empty;

        public string? Priority { get; set; }

        public int? CompletionPercent { get; set; }

        public string? Tags { get; set; }

        public string? Framework { get; set; }

        public DateTime? DueDate { get; set; }

        public string? Note { get; set; }



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
