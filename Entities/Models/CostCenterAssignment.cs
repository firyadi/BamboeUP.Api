using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("CostCenterAssignment", Schema = "app")]
    public class CostCenterAssignment
    {
        [Column("CostCenterAssignmentId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CostCenterAssignmentId { get; set; }

        [Key]
        [Column("CostCenterAssignmentGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CostCenterAssignmentGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long CostCenterId { get; set; }

        public long CompanyId { get; set; }

        public long? CompanyOfficeId { get; set; }

        public long? ProfitCenterId { get; set; }

        public long? CostCenterManagerEmployeeId { get; set; }

        public byte BudgetControlType { get; set; }

        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiredDate { get; set; }



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
