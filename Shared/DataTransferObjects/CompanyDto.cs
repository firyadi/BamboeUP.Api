using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record CompanyDto
    {
        public long CompanyId { get; set; }
        public Guid CompanyGuid { get; init; }
        public string CompanyName { get; set; } = string.Empty;
        public string InitialName { get; set; } = string.Empty;
        public string TaxCompulsionNo { get; set; } = string.Empty;
        public string RegistrationNo { get; set; } = string.Empty;
        public long? ParentCompanyId { get; set; }

        public string? ParentProgramName { get; set; }
        public string? ParentCompanyName { get; set; }
        public string DefaultCurrency { get; set; } = string.Empty;
        public byte[]? CompanyLogo { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record CompanyForCreationDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string InitialName { get; set; } = string.Empty;
        public string TaxCompulsionNo { get; set; } = string.Empty;
        public string RegistrationNo { get; set; } = string.Empty;
        public long? ParentCompanyId { get; set; }
        public string DefaultCurrency { get; set; } = string.Empty;
        public byte[]? CompanyLogo { get; set; }
        public long CreatedById { get; set; } = 0;
        public IEnumerable<CompanyOfficeForCreationDto>? Offices { get; set; }
    }

    public record CompanyForUpdateDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string InitialName { get; set; } = string.Empty;
        public string TaxCompulsionNo { get; set; } = string.Empty;
        public string RegistrationNo { get; set; } = string.Empty;
        public long? ParentCompanyId { get; set; }
        public string DefaultCurrency { get; set; } = string.Empty;
        public byte[]? CompanyLogo { get; set; }
        public long UpdatedById { get; set; }
        public IEnumerable<CompanyOfficeForUpdateDto>? Offices { get; set; }
    }

    public record CompanyForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    public class CompanySearchDto
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
    }
}
