using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public partial interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges);
        Task<CompanyDto> GetCompanyByGuidAsync(Guid companyGuid, bool trackChanges);
        Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto input);
        Task UpdateCompanyAsync(Guid companyGuid, CompanyForUpdateDto input, bool trackChanges);
        Task DeleteCompanyAsync(Guid companyGuid, CompanyForDeleteDto input, bool trackChanges);
        Task DeleteCompanyByAdminAsync(Guid companyGuid, bool trackChanges);

        Task<IEnumerable<CompanyDto>> SearchCompanyAsync(
            string? companyName, string? companyNameSearchType, string? initialName, string? initialNameSearchType, string? taxCompulsionNo, string? taxCompulsionNoSearchType, string? registrationNo, string? registrationNoSearchType, string? defaultCurrency, string? defaultCurrencySearchType
        );

    }
}
