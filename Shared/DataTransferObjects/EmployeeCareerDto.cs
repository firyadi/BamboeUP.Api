using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeCareerDto
    {
        public long EmployeeCareerId { get; set; }
        public Guid EmployeeCareerGuid { get; init; }
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

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record EmployeeCareerForCreationDto
    {
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
        public long CreatedById { get; set; } = 0;
    }

    public partial record EmployeeCareerForUpdateDto
    {
        public Guid EmployeeCareerGuid { get; set; }
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
        public long UpdatedById { get; set; }
    }

    public partial record EmployeeCareerForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class EmployeeCareerSearchDto
    {
        public string? EffectiveDate { get; set; }
        public SearchType EffectiveDateSearchType { get; set; } = SearchType.Contains;

        public string? ReferenceNo { get; set; }
        public SearchType ReferenceNoSearchType { get; set; } = SearchType.Contains;

        public string? ReferenceDate { get; set; }
        public SearchType ReferenceDateSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
