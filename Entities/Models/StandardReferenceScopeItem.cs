using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("StandardReferenceScopeItem", Schema = "app")]
    public class StandardReferenceScopeItem
    {
        [Column("StandardReferenceScopeItemId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StandardReferenceScopeItemId { get; set; }

        [Key]
        [Column("StandardReferenceScopeItemGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid StandardReferenceScopeItemGuid { get; set; } = Guid.NewGuid();

        [Column("StandardReferenceScopeId")]
        public long StandardReferenceScopeId { get; set; }

        [Column("StandardReferenceScopeGuid")]
        public Guid StandardReferenceScopeGuid { get; set; }

        [Required]
        [MaxLength(50)]
        public string StandardReferenceScopeItemInitial { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string StandardReferenceScopeItemName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? StandardReferenceScopeItemValue { get; set; }

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
