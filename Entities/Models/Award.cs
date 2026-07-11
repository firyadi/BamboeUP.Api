using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Award", Schema = "app")]
    public class Award
    {
        [Column("AwardId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AwardId { get; set; }

        [Key]
        [Column("AwardGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AwardGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string AwardCode { get; set; }

        [Required]
        public string AwardName { get; set; }

        public long SrAwardCriteria { get; set; }

        public long SrAwardType { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime Validto { get; set; }

        public string? AwardPrize { get; set; }

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
