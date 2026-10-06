using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PositionLevel", Schema = "app")]
    public partial class PositionLevel
    {
        [Column("PositionLevelId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PositionLevelId { get; set; }

        [Key]
        [Column("PositionLevelGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PositionLevelGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string PositionLevelCode { get; set; } = string.Empty;

        [Required]
        public string PositionLevelName { get; set; } = string.Empty;

        public short Ranking { get; set; }

        public string? PositionLevelDescription { get; set; }



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
