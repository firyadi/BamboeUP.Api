using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record CostCenterAssignmentDto
    {
        public long CostCenterAssignmentId { get; set; }
        public Guid CostCenterAssignmentGuid { get; init; }
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public long? ProfitCenterId { get; set; }
        public long? CostCenterManagerEmployeeId { get; set; }
        public byte BudgetControlType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
public string? CostCenterName { get; set; }

        public int StatusId { get; set; }
        public byte[] RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record CostCenterAssignmentForCreationDto
    {
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public long? ProfitCenterId { get; set; }
        public long? CostCenterManagerEmployeeId { get; set; }
        public byte BudgetControlType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record CostCenterAssignmentForUpdateDto
    {
        public Guid CostCenterAssignmentGuid { get; set; }
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public long? ProfitCenterId { get; set; }
        public long? CostCenterManagerEmployeeId { get; set; }
        public byte BudgetControlType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long UpdatedById { get; set; }
    }

    public record CostCenterAssignmentForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class CostCenterAssignmentSearchDto
    {
        public string? CompanyId { get; set; }
        public SearchType CompanyIdSearchType { get; set; } = SearchType.Contains;

        public string? CompanyOfficeId { get; set; }
        public SearchType CompanyOfficeIdSearchType { get; set; } = SearchType.Contains;

        public string? ProfitCenterId { get; set; }
        public SearchType ProfitCenterIdSearchType { get; set; } = SearchType.Contains;

        public string? CostCenterManagerEmployeeId { get; set; }
        public SearchType CostCenterManagerEmployeeIdSearchType { get; set; } = SearchType.Contains;

        public string? BudgetControlType { get; set; }
        public SearchType BudgetControlTypeSearchType { get; set; } = SearchType.Contains;

        public string? EffectiveDate { get; set; }
        public SearchType EffectiveDateSearchType { get; set; } = SearchType.Contains;

        public string? ExpiredDate { get; set; }
        public SearchType ExpiredDateSearchType { get; set; } = SearchType.Contains;
    }
}
