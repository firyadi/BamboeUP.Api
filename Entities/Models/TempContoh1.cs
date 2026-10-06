using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("TempContoh1", Schema = "app")]
    public partial class TempContoh1
    {
        [Column("TempContoh1Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TempContoh1Id { get; set; }

        [Key]
        [Column("TempContoh1Guid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TempContoh1Guid { get; set; } = Guid.NewGuid();

        public long PersonId { get; set; }

        public long? SrAwardType { get; set; }
public string? PersonName { get; set; }
public string? AwardTypeName { get; set; }



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
