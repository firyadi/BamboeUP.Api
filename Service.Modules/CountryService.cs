using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace Service.Modules
{
    public class CountryService : ICountryService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public CountryService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync(bool trackChanges)
        {
            var entities = await _repository.Country.GetAllCountriesAsync(trackChanges);
            return entities.Adapt<IEnumerable<CountryDto>>();
        }

        public async Task<CountryDto?> GetCountryByGuidAsync(Guid countryGuid, bool trackChanges)
        {
            var entity = await _repository.Country.GetCountryAsync(countryGuid, trackChanges);
            return entity.Adapt<CountryDto>();
        }

        public async Task<CountryDto> CreateCountryAsync(CountryForCreationDto input)
        {
            var model = input.Adapt<Country>();
            await _repository.Country.CreateCountryAsync(model);
            return model.Adapt<CountryDto>();
        }      

        public async Task UpdateCountryAsync(Guid countryGuid, CountryForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<Country>();
            model.CountryGuid = countryGuid;
            await _repository.Country.UpdateCountryAsync(model);
        }

        public async Task DeleteCountryAsync(Guid countryGuid, CountryForDeleteDto input, bool trackChanges)
        {
            var country = new Country
            {
                CountryGuid = countryGuid
            };

            await _repository.Country.SoftDeleteCountryAsync(country, input.DeletedById);
        }

        public async Task DeleteCountryByAdminAsync(Guid countryGuid, bool trackChanges)
        {
            await _repository.Country.DeleteCountryAsync(countryGuid);
        }

        public async Task<IEnumerable<CountryDto>> SearchCountryAsync(
            string? countryName, string? countryNameSearchType, string? countryIso, string? countryIsoSearchType)
        {
            var data = await _repository.Country.SearchCountryAsync(
                countryName, countryNameSearchType, countryIso, countryIsoSearchType);
            return data.Adapt<IEnumerable<CountryDto>>();
        }
    }
}
