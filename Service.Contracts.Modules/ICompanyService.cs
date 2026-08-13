using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges);
        Task<CompanyDto?> GetCompanyByGuidAsync(Guid companyGuid, bool trackChanges);
        Task<CompanyDto> CreateCompanyAsync(Guid personGuid, CompanyForCreationDto input);
        Task UpdateCompanyAsync(Guid personGuid, Guid companyGuid, CompanyForUpdateDto input, bool trackChanges);
        Task DeleteCompanyAsync(Guid personGuid, Guid companyGuid, CompanyForDeleteDto input, bool trackChanges);
        Task DeleteCompanyByAdminAsync(Guid companyGuid, bool trackChanges);

        Task<IEnumerable<CompanyDto>> SearchCompanyAsync(
            string? companyName, string? companyNameSearchType, string? initialName, string? initialNameSearchType, string? taxCompulsionNo, string? taxCompulsionNoSearchType, string? registrationNo, string? registrationNoSearchType, string? defaultCurrency, string? defaultCurrencySearchType, string? companyLogo, string? companyLogoSearchType,
            Guid personGuid, Guid companyGuid);

        // Detail (child) helpers
        Task<IEnumerable<CompanyDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<CompanyDto?> GetByPersonGuidAndCompanyGuidAsync(Guid personGuid, Guid companyGuid);
    }
}
