using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record OrganizationUnitScopeDto
    {
        public long OrganizationUnitScopeId { get; init; }
        public Guid OrganizationUnitScopeGuid { get; init; }
        public long OrganizationUnitId { get; set; }
        public long CompanyId { get; init; }
        public long? CompanyOfficeId { get; init; }
        public string ScopeType { get; set; } = string.Empty;
public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? OrganizationUnitName { get; set; }

        public int StatusId { get; init; }
        public byte[]? RowVersion { get; init; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; init; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; init; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; init; }
    }

    public record OrganizationUnitScopeForCreationDto
    {
        public long OrganizationUnitId { get; init; }
        public long CompanyId { get; init; }
        public long? CompanyOfficeId { get; init; }
        public string ScopeType { get; set; } = string.Empty;
        public long CreatedById { get; init; } = 0;
        public int StatusId { get; init; } = 1;
    }

    public record OrganizationUnitScopeForUpdateDto
    {
        public Guid OrganizationUnitScopeGuid { get; set; }
        public long OrganizationUnitId { get; init; }
        public long CompanyId { get; init; }
        public long? CompanyOfficeId { get; init; }
        public string ScopeType { get; set; } = string.Empty;
        public long UpdatedById { get; init; }
        public int StatusId { get; init; }
        public byte[]? RowVersion { get; init; }
    }

    public record OrganizationUnitScopeForDeleteDto
    {
        public long DeletedById { get; init; }
    }

    public class OrganizationUnitScopeSearchDto
    {
        public string? ScopeType { get; set; }
        public SearchType ScopeTypeSearchType { get; set; } = SearchType.Contains;
    }
}
