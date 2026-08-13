using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record CompanyDto
    {
        public long CompanyId { get; set; }
        public Guid CompanyGuid { get; init; }
        public long LeaderId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? InitialName { get; set; }
        public string? TaxCompulsionNo { get; set; }
        public string? RegistrationNo { get; set; }
        public long? ParentCompanyId { get; set; }
        public string? DefaultCurrency { get; set; }
        public string? CompanyLogo { get; set; }
public string? LeaderName { get; set; }
public string? ParentCompanyName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record CompanyForCreationDto
    {
        public long LeaderId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? InitialName { get; set; }
        public string? TaxCompulsionNo { get; set; }
        public string? RegistrationNo { get; set; }
        public long? ParentCompanyId { get; set; }
        public string? DefaultCurrency { get; set; }
        public string? CompanyLogo { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record CompanyForUpdateDto
    {
        public Guid CompanyGuid { get; set; }
        public long LeaderId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? InitialName { get; set; }
        public string? TaxCompulsionNo { get; set; }
        public string? RegistrationNo { get; set; }
        public long? ParentCompanyId { get; set; }
        public string? DefaultCurrency { get; set; }
        public string? CompanyLogo { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record CompanyForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class CompanySearchDto
    {
        public string? CompanyName { get; set; }
        public SearchType CompanyNameSearchType { get; set; } = SearchType.Contains;

        public string? InitialName { get; set; }
        public SearchType InitialNameSearchType { get; set; } = SearchType.Contains;

        public string? TaxCompulsionNo { get; set; }
        public SearchType TaxCompulsionNoSearchType { get; set; } = SearchType.Contains;

        public string? RegistrationNo { get; set; }
        public SearchType RegistrationNoSearchType { get; set; } = SearchType.Contains;

        public string? DefaultCurrency { get; set; }
        public SearchType DefaultCurrencySearchType { get; set; } = SearchType.Contains;

        public string? CompanyLogo { get; set; }
        public SearchType CompanyLogoSearchType { get; set; } = SearchType.Contains;
    }
}
