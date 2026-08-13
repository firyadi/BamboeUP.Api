using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("AwardCriteria", Schema = "app")]
    public partial class AwardCriteria
    {
        [Column("AwardCriteriaId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AwardCriteriaId { get; set; }

        [Key]
        [Column("AwardCriteriaGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AwardCriteriaGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long AwardId { get; set; }

        public int LineNo { get; set; }

        [Required]
        public string CriteriaCode { get; set; } = string.Empty;

        [Required]
        public string CriteriaName { get; set; } = string.Empty;

        public decimal? MinimumScore { get; set; }

        public decimal? MaximumScore { get; set; }

        public decimal? Weight { get; set; }

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
