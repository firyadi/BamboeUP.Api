using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("EmployeeAssignment", Schema = "app")]
    public partial class EmployeeAssignment
    {
        [Column("EmployeeAssignmentId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeAssignmentId { get; set; }

        [Key]
        [Column("EmployeeAssignmentGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeAssignmentGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long EmployeeId { get; set; }

        public long CompanyId { get; set; }

        public long? CompanyOfficeId { get; set; }

        public long OrganizationUnitId { get; set; }

        public long? PositionId { get; set; }

        public long? JobTitleId { get; set; }

        public long? JobGradeId { get; set; }

        public long? JobLevelId { get; set; }

        public long? ManagerEmployeeId { get; set; }

        public long? SupervisorEmployeeId { get; set; }

        public long? CostCenterId { get; set; }

        public long? PayrollGroupId { get; set; }

        public long? ShiftId { get; set; }

        public long? WorkLocationId { get; set; }

        public long SrEmploymentStatus { get; set; }

        public long SrEmploymentType { get; set; }

        public long SrAssignmentType { get; set; }

        public long? SrAssignmentReason { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? Effectiveto { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsCurrent { get; set; }

        public string? Remarks { get; set; }

public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? CostCenterName { get; set; }
public string? JobGradeName { get; set; }
public string? JobLevelName { get; set; }
public string? JobTitleName { get; set; }
public string? ManagerEmployeeName { get; set; }
public string? OrganizationUnitName { get; set; }
public string? PayrollGroupName { get; set; }
public string? PositionName { get; set; }
public string? ShiftName { get; set; }
public string? AssignmentReasonName { get; set; }
public string? AssignmentTypeName { get; set; }
public string? EmploymentStatusName { get; set; }
public string? EmploymentTypeName { get; set; }
public string? SupervisorEmployeeName { get; set; }
public string? WorkLocationName { get; set; }


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
