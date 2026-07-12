using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("CostCenterScope", Schema = "app")]
    public class CostCenterScope
    {
        [Column("CostCenterScopeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CostCenterScopeId { get; set; }

        [Key]
        [Column("CostCenterScopeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CostCenterScopeGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long CostCenterId { get; set; }

        public long CompanyId { get; set; }

        public long? CompanyOfficeId { get; set; }

        [Required]
        public string ScopeType { get; set; } = string.Empty;



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
