using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryDto>> GetAllCountriesAsync(bool trackChanges);
        Task<CountryDto> GetCountryByGuidAsync(Guid countryGuid, bool trackChanges);
        Task<CountryDto> CreateCountryAsync(CountryForCreationDto countryForCreation);
        Task UpdateCountryAsync(Guid countryGuid, CountryForUpdateDto countryForUpdate, bool trackChanges);
        Task DeleteCountryAsync(Guid countryGuid, CountryForDeleteDto countryForDelete, bool trackChanges);
        Task DeleteCountryByAdminAsync(Guid countryGuid, bool trackChanges);

        Task<IEnumerable<CountryDto>> SearchCountryAsync(
            string? countryName, string? countryNameSearchType, string? countryIso, string? countryIsoSearchType
        );
    }
}
