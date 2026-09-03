using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ICompanyOfficeRepository
    {
        Task<CompanyOffice?> GetCompanyOfficeAsync(Guid companyOfficeGuid, bool trackChanges);
        Task<CompanyOffice?> GetByCompanyGuidAndCompanyOfficeGuidAsync(Guid companyGuid, Guid companyOfficeGuid);
        Task<IEnumerable<CompanyOffice>> GetAllByCompanyGuidAsync(Guid companyGuid);

        Task CreateCompanyOfficeAsync(CompanyOffice companyOffice, IDbTransaction? transaction = null);
        Task UpdateCompanyOfficeAsync(CompanyOffice companyOffice, IDbTransaction? transaction = null);
        Task SoftDeleteCompanyOfficeAsync(CompanyOffice companyOffice, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteCompanyOfficeAsync(Guid companyOfficeGuid, IDbTransaction? transaction = null);
        Task DeleteByCompanyGuidAsync(Guid companyGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<CompanyOffice>> SearchCompanyOfficeAsync(
            string? companyOfficeName,
            string? companyOfficeNameSearchType,
            string? address,
            string? addressSearchType,
            Guid companyGuid, Guid companyOfficeGuid,
            IDbTransaction? transaction = null);
    }
}
