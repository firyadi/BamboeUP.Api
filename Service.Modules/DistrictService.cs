using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace Service.Modules
{
    public class DistrictService : IDistrictService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public DistrictService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<DistrictDto>> GetAllDistrictsAsync(Guid cityGuid, bool trackChanges)
        {
            var entities = await _repository.District.GetAllDistrictsAsync(cityGuid, trackChanges);
            return entities.Adapt<IEnumerable<DistrictDto>>();
        }

        public async Task<DistrictDto?> GetDistrictByGuidAsync(Guid cityGuid, Guid districtGuid, bool trackChanges)
        {
            var entity = await _repository.District.GetDistrictAsync(cityGuid, districtGuid, trackChanges);
            return entity.Adapt<DistrictDto>();
        }

        public async Task<DistrictDto> CreateDistrictAsync(Guid cityGuid, DistrictForCreationDto input)
        {
            var cityId = await _repository.City.GetCityIdByGuidAsync(cityGuid);
            if (cityId == null || cityId == 0) throw new Exception("City not found"); 

            var model = input.Adapt<District>();
            model.CityId = cityId.Value;

            await _repository.District.CreateDistrictAsync(model);
            return model.Adapt<DistrictDto>();
        }      

        public async Task UpdateDistrictAsync(Guid cityGuid, Guid districtGuid, DistrictForUpdateDto input, bool trackChanges)
        {
            var entity = await _repository.District.GetDistrictAsync(cityGuid, districtGuid, false);
            if (entity == null) throw new Exception("District not found in this City");

            input.Adapt(entity); 
            
            await _repository.District.UpdateDistrictAsync(entity);
        }

        public async Task DeleteDistrictAsync(Guid cityGuid, Guid districtGuid, DistrictForDeleteDto input, bool trackChanges)
        {
            var entity = await _repository.District.GetDistrictAsync(cityGuid, districtGuid, false);
            if (entity == null) throw new Exception("District not found in this City");

            await _repository.District.SoftDeleteDistrictAsync(entity, input.DeletedById);
        }

        public async Task DeleteDistrictByAdminAsync(Guid cityGuid, Guid districtGuid, bool trackChanges)
        {
            var entity = await _repository.District.GetDistrictAsync(cityGuid, districtGuid, false);
            if (entity == null) throw new Exception("District not found in this City");

            await _repository.District.DeleteDistrictAsync(districtGuid);
        }

        public async Task<IEnumerable<DistrictDto>> SearchDistrictAsync(
            Guid cityGuid, string? districtName, string? districtNameSearchType)
        {
            var data = await _repository.District.SearchDistrictAsync(
                cityGuid, districtName, districtNameSearchType);
            return data.Adapt<IEnumerable<DistrictDto>>();
        }
    }
}
