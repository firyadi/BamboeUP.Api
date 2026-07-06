using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public interface ICityService
    {
        Task<IEnumerable<CityDto>> GetAllCitiesAsync(Guid provinceGuid, bool trackChanges);
        Task<CityDto> GetCityByGuidAsync(Guid provinceGuid, Guid cityGuid, bool trackChanges);
        Task<CityDto> CreateCityAsync(Guid provinceGuid, CityForCreationDto cityForCreation);
        Task UpdateCityAsync(Guid provinceGuid, Guid cityGuid, CityForUpdateDto cityForUpdate, bool trackChanges);
        Task DeleteCityAsync(Guid provinceGuid, Guid cityGuid, CityForDeleteDto cityForDelete, bool trackChanges);
        Task DeleteCityByAdminAsync(Guid provinceGuid, Guid cityGuid, bool trackChanges);

        Task<IEnumerable<CityDto>> SearchCityAsync(
            Guid provinceGuid, string? cityName, string? cityNameSearchType
        );
    }
}
