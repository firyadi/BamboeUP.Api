using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("EmployeeCareer", Schema = "app")]
    public partial class EmployeeCareer
    {
        [Column("EmployeeCareerId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeCareerId { get; set; }

        [Key]
        [Column("EmployeeCareerGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeCareerGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long EmployeeId { get; set; }

        public long? EmployeeAssignmentId { get; set; }

        public long? EmployeeContractId { get; set; }

        public long SrCareerEvent { get; set; }

        public long? SrCareerReason { get; set; }

        public DateTime EffectiveDate { get; set; }

        public long? FromCompanyId { get; set; }

        public long? FromOrganizationUnitId { get; set; }

        public long? FromPositionId { get; set; }

        public long? FromJobTitleId { get; set; }

        public long? FromJobGradeId { get; set; }

        public long? FromJobLevelId { get; set; }

        public long? FromManagerEmployeeId { get; set; }

        public long? ToCompanyId { get; set; }

        public long? ToOrganizationUnitId { get; set; }

        public long? ToPositionId { get; set; }

        public long? ToJobTitleId { get; set; }

        public long? ToJobGradeId { get; set; }

        public long? ToJobLevelId { get; set; }

        public long? ToManagerEmployeeId { get; set; }

        public string? ReferenceNo { get; set; }

        public DateTime? ReferenceDate { get; set; }

        public string? Remarks { get; set; }

public string? EmployeeAssignmentName { get; set; }
public string? EmployeeContractName { get; set; }
public string? FromCompanyName { get; set; }
public string? FromJobGradeName { get; set; }
public string? FromJobLevelName { get; set; }
public string? FromJobTitleName { get; set; }
public string? FromManagerEmployeeName { get; set; }
public string? FromOrganizationUnitName { get; set; }
public string? FromPositionName { get; set; }
public string? CareerEventName { get; set; }
public string? CareerReasonName { get; set; }
public string? ToCompanyName { get; set; }
public string? ToJobGradeName { get; set; }
public string? ToJobLevelName { get; set; }
public string? ToJobTitleName { get; set; }
public string? ToManagerEmployeeName { get; set; }
public string? ToOrganizationUnitName { get; set; }
public string? ToPositionName { get; set; }


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
