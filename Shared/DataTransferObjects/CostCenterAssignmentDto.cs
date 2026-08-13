using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record CostCenterAssignmentDto
    {
        public long CostCenterAssignmentId { get; set; }
        public Guid CostCenterAssignmentGuid { get; init; }
        public long CostCenterManagerEmployeeId { get; set; }
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public long? ProfitCenterId { get; set; }
        public byte BudgetControlType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
public string? CostCenterName { get; set; }
public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? CostCenterManagerEmployeeName { get; set; }
public string? ProfitCenterName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record CostCenterAssignmentForCreationDto
    {
        public long CostCenterManagerEmployeeId { get; set; }
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public long? ProfitCenterId { get; set; }
        public byte BudgetControlType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record CostCenterAssignmentForUpdateDto
    {
        public Guid CostCenterAssignmentGuid { get; set; }
        public long CostCenterManagerEmployeeId { get; set; }
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public long? ProfitCenterId { get; set; }
        public byte BudgetControlType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record CostCenterAssignmentForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class CostCenterAssignmentSearchDto
    {
        public string? BudgetControlType { get; set; }
        public SearchType BudgetControlTypeSearchType { get; set; } = SearchType.Contains;

        public string? EffectiveDate { get; set; }
        public SearchType EffectiveDateSearchType { get; set; } = SearchType.Contains;

        public string? ExpiredDate { get; set; }
        public SearchType ExpiredDateSearchType { get; set; } = SearchType.Contains;
    }
}
