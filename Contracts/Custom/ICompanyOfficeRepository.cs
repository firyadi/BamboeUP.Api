using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ICompanyOfficeRepository
    {
        Task<CompanyOffice?> GetCompanyOfficeByIdAsync(long companyOfficeId, bool trackChanges);
        Task<IEnumerable<CompanyOffice>> GetAllCompanyOfficesAsync(bool trackChanges);

        Task<IEnumerable<CompanyOffice>> SearchCompanyOfficeAsync(
            string? companyOfficeName,
            string? companyOfficeNameSearchType,
            long? srAddressType,
            long? countryId,
            long? stateId,
            long? cityId,
            string? postalCodeId,
            string? postalCodeIdSearchType,
            string? address,
            string? addressSearchType,
            Guid companyGuid,
            Guid companyOfficeGuid);
    }
}
