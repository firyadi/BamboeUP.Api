using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Position", Schema = "app")]
    public partial class Position
    {
        [Column("PositionId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PositionId { get; set; }

        [Key]
        [Column("PositionGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PositionGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string PositionCode { get; set; } = string.Empty;

        [Required]
        public string PositionName { get; set; } = string.Empty;

        [Required]
        public string Summary { get; set; } = string.Empty;

        public long PositionGradeId { get; set; }

        public long PositionLevelId { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime Validto { get; set; }

        public string? PositionDescription { get; set; }
public string? PositionGradeName { get; set; }
public string? PositionLevelName { get; set; }



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
