using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("AwardCriteriaRule", Schema = "app")]
    public partial class AwardCriteriaRule
    {
        [Column("AwardCriteriaRuleId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AwardCriteriaRuleId { get; set; }

        [Key]
        [Column("AwardCriteriaRuleGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AwardCriteriaRuleGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long AwardCriteriaId { get; set; }

        [Required]
        public string RuleCode { get; set; } = string.Empty;

        [Required]
        public string RuleName { get; set; } = string.Empty;

        [Required]
        public string Operator { get; set; } = string.Empty;

        public decimal? ValueFrom { get; set; }

        public decimal? Valueto { get; set; }

        public decimal? Score { get; set; }

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
