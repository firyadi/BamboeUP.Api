using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public interface IProvinceService
    {
        Task<IEnumerable<ProvinceDto>> GetAllProvincesAsync(Guid countryGuid, bool trackChanges);
        Task<ProvinceDto?> GetProvinceByGuidAsync(Guid countryGuid, Guid provinceGuid, bool trackChanges);
        Task<ProvinceDto> CreateProvinceAsync(Guid countryGuid, ProvinceForCreationDto provinceForCreation);
        Task UpdateProvinceAsync(Guid countryGuid, Guid provinceGuid, ProvinceForUpdateDto provinceForUpdate, bool trackChanges);
        Task DeleteProvinceAsync(Guid countryGuid, Guid provinceGuid, ProvinceForDeleteDto provinceForDelete, bool trackChanges);
        Task DeleteProvinceByAdminAsync(Guid countryGuid, Guid provinceGuid, bool trackChanges);

        Task<IEnumerable<ProvinceDto>> SearchProvinceAsync(
            Guid countryGuid, string? provinceName, string? provinceNameSearchType
        );
    }
}
