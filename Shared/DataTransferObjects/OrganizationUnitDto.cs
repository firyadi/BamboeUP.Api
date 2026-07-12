using System;
using System.Collections.Generic;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record OrganizationUnitDto
    {
        public long OrganizationUnitId { get; set; }
        public Guid OrganizationUnitGuid { get; init; }
        public string OrganizationUnitCode { get; set; } = string.Empty;
        public string OrganizationUnitName { get; set; } = string.Empty;
        public int? ParentOrganizationUnitId { get; set; }
        public long SrOrganizationLevel { get; set; }
        public int LevelDepth { get; set; }
        public string? HierarchyPath { get; set; }
public string? ParentOrganizationUnitName { get; set; }

        public IEnumerable<OrganizationUnitScopeDto>? OrganizationUnitScopes { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record OrganizationUnitForCreationDto
    {
        public string OrganizationUnitCode { get; set; } = string.Empty;
        public string OrganizationUnitName { get; set; } = string.Empty;
        public int? ParentOrganizationUnitId { get; set; }
        public long SrOrganizationLevel { get; set; }
        public int LevelDepth { get; set; }
        public string? HierarchyPath { get; set; }
        public IEnumerable<OrganizationUnitScopeForCreationDto>? OrganizationUnitScopes { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record OrganizationUnitForUpdateDto
    {
        public string OrganizationUnitCode { get; set; } = string.Empty;
        public string OrganizationUnitName { get; set; } = string.Empty;
        public int? ParentOrganizationUnitId { get; set; }
        public long SrOrganizationLevel { get; set; }
        public int LevelDepth { get; set; }
        public string? HierarchyPath { get; set; }
        public IEnumerable<OrganizationUnitScopeForUpdateDto>? OrganizationUnitScopes { get; set; }
        public long UpdatedById { get; set; }
    }

    public record OrganizationUnitForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class OrganizationUnitSearchDto
    {
        public string? OrganizationUnitCode { get; set; }
        public SearchType OrganizationUnitCodeSearchType { get; set; } = SearchType.Contains;

        public string? OrganizationUnitName { get; set; }
        public SearchType OrganizationUnitNameSearchType { get; set; } = SearchType.Contains;

        public string? SrOrganizationLevel { get; set; }
        public SearchType SrOrganizationLevelSearchType { get; set; } = SearchType.Contains;

        public string? LevelDepth { get; set; }
        public SearchType LevelDepthSearchType { get; set; } = SearchType.Contains;

        public string? HierarchyPath { get; set; }
        public SearchType HierarchyPathSearchType { get; set; } = SearchType.Contains;
public string? ParentOrganizationUnitName { get; set; }
public SearchType ParentOrganizationUnitNameSearchType { get; set; } = SearchType.Contains;

    }
}
