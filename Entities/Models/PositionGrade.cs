using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PositionGrade", Schema = "app")]
    public partial class PositionGrade
    {
        [Column("PositionGradeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PositionGradeId { get; set; }

        [Key]
        [Column("PositionGradeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PositionGradeGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string PositionGradeCode { get; set; } = string.Empty;

        [Required]
        public string PositionGradeName { get; set; } = string.Empty;

        [Required]
        public string Interval { get; set; } = string.Empty;

        public short Ranking { get; set; }

        public string? PositionGradeDescription { get; set; }



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
