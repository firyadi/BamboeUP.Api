using Entities.Models;
using System.Data;

namespace Contracts
{
    public partial interface ICompanyRepository
    {
        Task<Company?> GetCompanyAsync(Guid companyGuid, bool trackChanges);
        Task<Company?> GetCompanyByIdAsync(long companyId, bool trackChanges);
        Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);

        Task CreateCompanyAsync(Company company, IDbTransaction? transaction = null);
        Task UpdateCompanyAsync(Company company, IDbTransaction? transaction = null);
        Task DeleteCompanyAsync(Guid companyGuid, IDbTransaction? transaction = null);
        Task SoftDeleteCompanyAsync(Company company, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Company>> SearchCompanyAsync(
            string? companyName, string? companyNameSearchType,
string? initialName, string? initialNameSearchType,
string? taxCompulsionNo, string? taxCompulsionNoSearchType,
string? registrationNo, string? registrationNoSearchType,
string? defaultCurrency, string? defaultCurrencySearchType,
IDbTransaction? transaction = null);

        // Detail helpers (only if entity has a parent)
        
    }
}
