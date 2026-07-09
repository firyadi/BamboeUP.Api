using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record OrganizationUnitScopeDto
    {
        public long OrganizationUnitScopeId { get; set; }
        public Guid OrganizationUnitScopeGuid { get; init; }
        public long OrganizationUnitId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public string ScopeType { get; set; }
public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? OrganizationUnitName { get; set; }

        public int StatusId { get; set; }
        public byte[] RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record OrganizationUnitScopeForCreationDto
    {
        public int OrganizationUnitId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public string ScopeType { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record OrganizationUnitScopeForUpdateDto
    {
        public Guid OrganizationUnitScopeGuid { get; set; }
        public int OrganizationUnitId { get; set; }
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public string ScopeType { get; set; }
        public long UpdatedById { get; set; }
    }

    public record OrganizationUnitScopeForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class OrganizationUnitScopeSearchDto
    {
        public string? ScopeType { get; set; }
        public SearchType ScopeTypeSearchType { get; set; } = SearchType.Contains;
    }
}
