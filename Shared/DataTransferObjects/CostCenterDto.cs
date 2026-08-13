using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record CostCenterDto
    {
        public long CostCenterId { get; set; }
        public Guid CostCenterGuid { get; init; }
        public string CostCenterCode { get; set; } = string.Empty;
        public string CostCenterName { get; set; } = string.Empty;
        public string? CostCenterDescription { get; set; }
        public long? ParentCostCenterId { get; set; }
        public int LevelDepth { get; set; }
        public string? HierarchyPath { get; set; }


                public IEnumerable<CostCenterScopeDto>? CostCenterScopes { get; set; }
        // ##HeaderDetailCollections##
        public IEnumerable<CostCenterAssignmentDto>? CostCenterAssignments { get; set; }
        public int StatusId { get; init; }
        public byte[]? RowVersion { get; init; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record CostCenterForCreationDto
    {
        public string CostCenterCode { get; set; } = string.Empty;
        public string CostCenterName { get; set; } = string.Empty;
        public string? CostCenterDescription { get; set; }
        public long? ParentCostCenterId { get; set; }
        public int LevelDepth { get; set; }
        public string? HierarchyPath { get; set; }
                public IEnumerable<CostCenterScopeForCreationDto>? CostCenterScopes { get; set; }
        // ##HeaderDetailCreateCollections##
        public IEnumerable<CostCenterAssignmentForCreationDto>? CostCenterAssignments { get; set; }
        public long CreatedById { get; init; } = 0;
        public int StatusId { get; init; } = 1;
    }

    public partial record CostCenterForUpdateDto
    {
        public string CostCenterCode { get; set; } = string.Empty;
        public string CostCenterName { get; set; } = string.Empty;
        public string? CostCenterDescription { get; set; }
        public long? ParentCostCenterId { get; set; }
        public int LevelDepth { get; set; }
        public string? HierarchyPath { get; set; }
                public IEnumerable<CostCenterScopeForUpdateDto>? CostCenterScopes { get; set; }
        // ##HeaderDetailUpdateCollections##
        public IEnumerable<CostCenterAssignmentForUpdateDto>? CostCenterAssignments { get; set; }
        public long UpdatedById { get; init; }
        public int StatusId { get; init; }
        public byte[]? RowVersion { get; init; }
    }

    public partial record CostCenterForDeleteDto
    {
        public long DeletedById { get; init; }
    }

    public partial class CostCenterSearchDto
    {
        public string? CostCenterCode { get; set; }
        public SearchType CostCenterCodeSearchType { get; set; } = SearchType.Contains;

        public string? CostCenterName { get; set; }
        public SearchType CostCenterNameSearchType { get; set; } = SearchType.Contains;

        public string? CostCenterDescription { get; set; }
        public SearchType CostCenterDescriptionSearchType { get; set; } = SearchType.Contains;

        public string? ParentCostCenterId { get; set; }
        public SearchType ParentCostCenterIdSearchType { get; set; } = SearchType.Contains;

        public string? LevelDepth { get; set; }
        public SearchType LevelDepthSearchType { get; set; } = SearchType.Contains;

        public string? HierarchyPath { get; set; }
        public SearchType HierarchyPathSearchType { get; set; } = SearchType.Contains;

    }
}
