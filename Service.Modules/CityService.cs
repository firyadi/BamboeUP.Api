using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace Service
{
    public class CityService : ICityService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public CityService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<CityDto>> GetAllCitiesAsync(Guid provinceGuid, bool trackChanges)
        {
            var entities = await _repository.City.GetAllCitiesAsync(provinceGuid, trackChanges);
            return entities.Adapt<IEnumerable<CityDto>>();
        }

        public async Task<CityDto> GetCityByGuidAsync(Guid provinceGuid, Guid cityGuid, bool trackChanges)
        {
            var entity = await _repository.City.GetCityAsync(provinceGuid, cityGuid, trackChanges);
            return entity.Adapt<CityDto>();
        }

        public async Task<CityDto> CreateCityAsync(Guid provinceGuid, CityForCreationDto input)
        {
            var provinceId = await _repository.City.GetProvinceIdByGuidAsync(provinceGuid);
            if (provinceId == null || provinceId == 0) throw new Exception("Province not found"); 

            var model = input.Adapt<City>();
            model.ProvinceId = provinceId.Value;

            await _repository.City.CreateCityAsync(model);
            return model.Adapt<CityDto>();
        }      

        public async Task UpdateCityAsync(Guid provinceGuid, Guid cityGuid, CityForUpdateDto input, bool trackChanges)
        {
            var entity = await _repository.City.GetCityAsync(provinceGuid, cityGuid, false);
            if (entity == null) throw new Exception("City not found in this Province");

            input.Adapt(entity); 
            
            await _repository.City.UpdateCityAsync(entity);
        }

        public async Task DeleteCityAsync(Guid provinceGuid, Guid cityGuid, CityForDeleteDto input, bool trackChanges)
        {
            var entity = await _repository.City.GetCityAsync(provinceGuid, cityGuid, false);
            if (entity == null) throw new Exception("City not found in this Province");

            await _repository.City.SoftDeleteCityAsync(entity, input.DeletedById);
        }

        public async Task DeleteCityByAdminAsync(Guid provinceGuid, Guid cityGuid, bool trackChanges)
        {
            var entity = await _repository.City.GetCityAsync(provinceGuid, cityGuid, false);
            if (entity == null) throw new Exception("City not found in this Province");

            await _repository.City.DeleteCityAsync(cityGuid);
        }

        public async Task<IEnumerable<CityDto>> SearchCityAsync(
            Guid provinceGuid, string? cityName, string? cityNameSearchType)
        {
            var data = await _repository.City.SearchCityAsync(
                provinceGuid, cityName, cityNameSearchType);
            return data.Adapt<IEnumerable<CityDto>>();
        }
    }
}
