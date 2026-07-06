using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("StandardReferenceItem", Schema = "app")]
    public class StandardReferenceItem
    {
        [Column("StandardReferenceItemId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StandardReferenceItemId { get; set; }

        [Key]
        [Column("StandardReferenceItemGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid StandardReferenceItemGuid { get; set; } = Guid.NewGuid();

        [Column("StandardReferenceId")]
        public long StandardReferenceId { get; set; }

        [Column("StandardReferenceGuid")]
        public Guid StandardReferenceGuid { get; set; }

        [Required]
        [MaxLength(50)]
        public string StandardReferenceItemInitial { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string StandardReferenceItemName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? StandardReferenceItemValue { get; set; }

        public int DisplayOrder { get; set; }

        [Column("StatusId")]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long? UpdatedById { get; set; }

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long? DeletedById { get; set; }

        public DateTime? DeletedTime { get; set; }
    }
}
