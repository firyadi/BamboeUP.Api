using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("CostCenter", Schema = "app")]
    public class CostCenter
    {
        [Column("CostCenterId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CostCenterId { get; set; }

        [Key]
        [Column("CostCenterGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CostCenterGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string CostCenterCode { get; set; }

        [Required]
        public string CostCenterName { get; set; }

        public string? CostCenterDescription { get; set; }

        public long? ParentCostCenterId { get; set; }

        public int LevelDepth { get; set; }

        public string? HierarchyPath { get; set; }



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
