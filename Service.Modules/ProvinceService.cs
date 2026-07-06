using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace Service.Modules
{
    public class ProvinceService : IProvinceService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public ProvinceService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<ProvinceDto>> GetAllProvincesAsync(Guid countryGuid, bool trackChanges)
        {
            var entities = await _repository.Province.GetAllProvincesAsync(countryGuid, trackChanges);
            return entities.Adapt<IEnumerable<ProvinceDto>>();
        }

        public async Task<ProvinceDto> GetProvinceByGuidAsync(Guid countryGuid, Guid provinceGuid, bool trackChanges)
        {
            var entity = await _repository.Province.GetProvinceAsync(countryGuid, provinceGuid, trackChanges);
            return entity.Adapt<ProvinceDto>();
        }

        public async Task<ProvinceDto> CreateProvinceAsync(Guid countryGuid, ProvinceForCreationDto input)
        {
            var country = await _repository.Country.GetCountryAsync(countryGuid, false);
            if (country == null) throw new Exception("Country not found"); 

            var model = input.Adapt<Province>();
            model.CountryId = country.CountryId;

            await _repository.Province.CreateProvinceAsync(model);
            return model.Adapt<ProvinceDto>();
        }      

        public async Task UpdateProvinceAsync(Guid countryGuid, Guid provinceGuid, ProvinceForUpdateDto input, bool trackChanges)
        {
            var entity = await _repository.Province.GetProvinceAsync(countryGuid, provinceGuid, false);
            if (entity == null) throw new Exception("Province not found in this Country");

            input.Adapt(entity); 
            
            await _repository.Province.UpdateProvinceAsync(entity);
        }

        public async Task DeleteProvinceAsync(Guid countryGuid, Guid provinceGuid, ProvinceForDeleteDto input, bool trackChanges)
        {
            var entity = await _repository.Province.GetProvinceAsync(countryGuid, provinceGuid, false);
            if (entity == null) throw new Exception("Province not found in this Country");

            await _repository.Province.SoftDeleteProvinceAsync(entity, input.DeletedById);
        }

        public async Task DeleteProvinceByAdminAsync(Guid countryGuid, Guid provinceGuid, bool trackChanges)
        {
            var entity = await _repository.Province.GetProvinceAsync(countryGuid, provinceGuid, false);
            if (entity == null) throw new Exception("Province not found in this Country");

            await _repository.Province.DeleteProvinceAsync(provinceGuid);
        }

        public async Task<IEnumerable<ProvinceDto>> SearchProvinceAsync(
            Guid countryGuid, string? provinceName, string? provinceNameSearchType)
        {
            var data = await _repository.Province.SearchProvinceAsync(
                countryGuid, provinceName, provinceNameSearchType);
            return data.Adapt<IEnumerable<ProvinceDto>>();
        }
    }
}
