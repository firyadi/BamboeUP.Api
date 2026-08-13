using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeAssignmentDto
    {
        public long EmployeeAssignmentId { get; set; }
        public Guid EmployeeAssignmentGuid { get; init; }
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

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record EmployeeAssignmentForCreationDto
    {
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
        public long CreatedById { get; set; } = 0;
    }

    public partial record EmployeeAssignmentForUpdateDto
    {
        public Guid EmployeeAssignmentGuid { get; set; }
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
        public long UpdatedById { get; set; }
    }

    public partial record EmployeeAssignmentForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class EmployeeAssignmentSearchDto
    {
        public string? EffectiveFrom { get; set; }
        public SearchType EffectiveFromSearchType { get; set; } = SearchType.Contains;

        public string? Effectiveto { get; set; }
        public SearchType EffectivetoSearchType { get; set; } = SearchType.Contains;

        public string? IsPrimary { get; set; }
        public SearchType IsPrimarySearchType { get; set; } = SearchType.Contains;

        public string? IsCurrent { get; set; }
        public SearchType IsCurrentSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
