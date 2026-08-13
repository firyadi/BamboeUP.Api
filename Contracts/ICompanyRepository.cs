using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ICompanyRepository
    {
        Task<Company?> GetCompanyAsync(Guid companyGuid, bool trackChanges);
        Task<Company?> GetByPersonGuidAndCompanyGuidAsync(Guid personGuid, Guid companyGuid);
        Task<IEnumerable<Company>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreateCompanyAsync(Company company, IDbTransaction? transaction = null);
        Task UpdateCompanyAsync(Company company, IDbTransaction? transaction = null);
        Task SoftDeleteCompanyAsync(Company company, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteCompanyAsync(Guid companyGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<Company>> SearchCompanyAsync(
            string? companyName,
            string? companyNameSearchType,
            string? initialName,
            string? initialNameSearchType,
            string? taxCompulsionNo,
            string? taxCompulsionNoSearchType,
            string? registrationNo,
            string? registrationNoSearchType,
            string? defaultCurrency,
            string? defaultCurrencySearchType,
            string? companyLogo,
            string? companyLogoSearchType,
            Guid personGuid, Guid companyGuid,
            IDbTransaction? transaction = null);
    }
}
