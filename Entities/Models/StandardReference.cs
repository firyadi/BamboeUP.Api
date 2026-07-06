using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("StandardReference", Schema = "app")]
    public class StandardReference
    {
        [Column("StandardReferenceId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StandardReferenceId { get; set; }

        [Key]
        [Column("StandardReferenceGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid StandardReferenceGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string StandardReferenceName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string StandardReferenceInitial { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

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

        [NotMapped]
        public int DefaultItemsCount { get; set; }

        [NotMapped]
        public int ScopesCount { get; set; }
    }
}
