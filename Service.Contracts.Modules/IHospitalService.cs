using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IHospitalService
    {
        Task<IEnumerable<HospitalDto>> GetAllHospitalsAsync(bool trackChanges);
        Task<HospitalDto?> GetHospitalByGuidAsync(Guid hospitalGuid, bool trackChanges);
        Task<HospitalDto> CreateHospitalAsync(HospitalForCreationDto input);
        Task UpdateHospitalAsync(Guid hospitalGuid, HospitalForUpdateDto input, bool trackChanges);
        Task DeleteHospitalAsync(Guid hospitalGuid, HospitalForDeleteDto input, bool trackChanges);
        Task DeleteHospitalByAdminAsync(Guid hospitalGuid, bool trackChanges);

        Task<IEnumerable<HospitalDto>> SearchHospitalAsync(
            string? hospitalName, string? hospitalNameSearchType, string? hospitalCode, string? hospitalCodeSearchType, string? shortName, string? shortNameSearchType, string? licenseNo, string? licenseNoSearchType, string? hospitalType, string? hospitalTypeSearchType, string? hospitalClass, string? hospitalClassSearchType, string? phoneNo, string? phoneNoSearchType, string? email, string? emailSearchType, string? website, string? websiteSearchType

        );
    }
}
