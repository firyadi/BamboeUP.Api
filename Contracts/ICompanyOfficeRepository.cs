using Entities.Models;
using System.Data;

namespace Contracts
{
    public partial interface ICompanyOfficeRepository
    {
        Task<CompanyOffice> GetCompanyOfficeAsync(Guid companyOfficeGuid, bool trackChanges);
        Task<CompanyOffice?> GetCompanyOfficeByIdAsync(long companyOfficeId, bool trackChanges);
        Task<IEnumerable<CompanyOffice>> GetAllCompanyOfficesAsync(bool trackChanges);

        Task CreateCompanyOfficeAsync(CompanyOffice companyOffice, IDbTransaction? transaction = null);
        Task UpdateCompanyOfficeAsync(CompanyOffice companyOffice, IDbTransaction? transaction = null);
        Task DeleteCompanyOfficeAsync(Guid companyOfficeGuid, IDbTransaction? transaction = null);
        Task SoftDeleteCompanyOfficeAsync(CompanyOffice companyOffice, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<CompanyOffice>> SearchCompanyOfficeAsync(
            string? companyOfficeName, string? companyOfficeNameSearchType,
            long? srAddressType,
            long? countryId, 
            long? stateId, 
            long? cityId, 
            string? postalCodeId, string? postalCodeIdSearchType,
            string? address, string? addressSearchType, 
            Guid companyGuid, Guid companyOfficeGuid,
            IDbTransaction? transaction = null);

        // Detail helpers (only if entity has a parent)
        Task<IEnumerable<CompanyOffice>> GetAllByCompanyGuidAsync(Guid companyGuid);
        Task<CompanyOffice> GetByCompanyGuidAndCompanyOfficeGuidAsync(Guid companyGuid, Guid companyOfficeGuid);
    }
}

