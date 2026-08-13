using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record CostCenterScopeDto
    {
        public long CostCenterScopeId { get; set; }
        public Guid CostCenterScopeGuid { get; init; }
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public string ScopeType { get; set; } = string.Empty;
public string? CostCenterName { get; set; }

        public int StatusId { get; init; }
        public byte[]? RowVersion { get; init; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record CostCenterScopeForCreationDto
    {
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public string ScopeType { get; set; } = string.Empty;
        public long CreatedById { get; init; } = 0;
        public int StatusId { get; init; } = 1;
    }

    public partial record CostCenterScopeForUpdateDto
    {
        public Guid CostCenterScopeGuid { get; set; }
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public string ScopeType { get; set; } = string.Empty;
        public long UpdatedById { get; init; }
        public int StatusId { get; init; }
        public byte[]? RowVersion { get; init; }
    }

    public partial record CostCenterScopeForDeleteDto
    {
        public long DeletedById { get; init; }
    }

    public partial class CostCenterScopeSearchDto
    {
        public string? CompanyId { get; set; }
        public SearchType CompanyIdSearchType { get; set; } = SearchType.Contains;

        public string? CompanyOfficeId { get; set; }
        public SearchType CompanyOfficeIdSearchType { get; set; } = SearchType.Contains;

        public string? ScopeType { get; set; }
        public SearchType ScopeTypeSearchType { get; set; } = SearchType.Contains;
        public string? CostCenterId { get; set; }
        public SearchType CostCenterIdSearchType { get; set; } = SearchType.Contains;
    }
}
