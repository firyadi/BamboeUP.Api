using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public partial interface ICompanyOfficeService
    {
        Task<IEnumerable<CompanyOfficeDto>> GetAllCompanyOfficesAsync(bool trackChanges);
        Task<CompanyOfficeDto> GetCompanyOfficeByGuidAsync(Guid companyOfficeGuid, bool trackChanges);
        Task<CompanyOfficeDto> CreateCompanyOfficeAsync(CompanyOfficeForCreationDto input);
        Task UpdateCompanyOfficeAsync(Guid companyOfficeGuid, CompanyOfficeForUpdateDto input, bool trackChanges);
        Task DeleteCompanyOfficeAsync(Guid companyOfficeGuid, CompanyOfficeForDeleteDto input, bool trackChanges);
        Task DeleteCompanyOfficeByAdminAsync(Guid companyOfficeGuid, bool trackChanges);

        Task<IEnumerable<CompanyOfficeDto>> SearchCompanyOfficeAsync(
            string? companyOfficeName, string? companyOfficeNameSearchType,
            long? srAddressType,
            long? countryId, 
            long? stateId, 
            long? cityId, 
            string? postalCodeId, string? postalCodeIdSearchType,
            string? address, string? addressSearchType, 
            Guid companyGuid, Guid companyOfficeGuid
        );

        // Detail helpers
        Task<IEnumerable<CompanyOfficeDto>> GetAllByCompanyGuidAsync(Guid companyGuid);
        Task<CompanyOfficeDto> GetByCompanyGuidAndCompanyOfficeGuidAsync(Guid companyGuid, Guid companyOfficeGuid);
    }
}

