using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PersonContact", Schema = "app")]
    public partial class PersonContact
    {
        [Column("PersonContactId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PersonContactId { get; set; }

        [Key]
        [Column("PersonContactGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonContactGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long PersonId { get; set; }

        public long SrContactType { get; set; }

        [Required]
        public string ContactValue { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }

        public string? Remark { get; set; }



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
