using Shared.Settings.Enums;
using System;

namespace Shared.DataTransferObjects
{
    public record DocumentNumberRequestDto
    {
        public long DocumentNumberRequestId { get; set; }
        public Guid DocumentNumberRequestGuid { get; init; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNo { get; set; } = string.Empty;
        public long AutoNumberLogId { get; set; }
        public string? ExternalReference { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public long? CompanyId { get; set; }
        public long? OfficeId { get; set; }
        public long? OrgUnitId { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

    public record DocumentNumberRequestForCreationDto
    {
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNo { get; set; } = string.Empty;
        public long AutoNumberLogId { get; set; }
        public string? ExternalReference { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "Used";
        public long? CompanyId { get; set; }
        public long? OfficeId { get; set; }
        public long? OrgUnitId { get; set; }
        public long CreatedById { get; set; }
    }

    public record DocumentNumberRequestForUpdateDto
    {
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNo { get; set; } = string.Empty;
        public long AutoNumberLogId { get; set; }
        public string? ExternalReference { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public long? CompanyId { get; set; }
        public long? OfficeId { get; set; }
        public long? OrgUnitId { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record DocumentNumberRequestForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    public class DocumentNumberRequestSearchDto
    {
        public string? DocumentType { get; set; }
        public SearchType DocumentTypeSearchType { get; set; } = SearchType.Contains;

        public string? DocumentNo { get; set; }
        public SearchType DocumentNoSearchType { get; set; } = SearchType.Contains;

        public string? ExternalReference { get; set; }
        public SearchType ExternalReferenceSearchType { get; set; } = SearchType.Contains;

        public string? Description { get; set; }
        public SearchType DescriptionSearchType { get; set; } = SearchType.Contains;

        public string? Status { get; set; }
        public SearchType StatusSearchType { get; set; } = SearchType.Equals;

        public long? CompanyId { get; set; }
        public long? OfficeId { get; set; }
        public long? OrgUnitId { get; set; }
    }
}
