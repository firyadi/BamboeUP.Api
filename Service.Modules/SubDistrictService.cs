using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace Service.Modules
{
    public class SubDistrictService : ISubDistrictService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public SubDistrictService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<SubDistrictDto>> GetAllSubDistrictsAsync(Guid districtGuid, bool trackChanges)
        {
            var entities = await _repository.SubDistrict.GetAllSubDistrictsAsync(districtGuid, trackChanges);
            return entities.Adapt<IEnumerable<SubDistrictDto>>();
        }

        public async Task<SubDistrictDto?> GetSubDistrictByGuidAsync(Guid districtGuid, Guid subDistrictGuid, bool trackChanges)
        {
            var entity = await _repository.SubDistrict.GetSubDistrictAsync(districtGuid, subDistrictGuid, trackChanges);
            return entity.Adapt<SubDistrictDto>();
        }

        public async Task<SubDistrictDto> CreateSubDistrictAsync(Guid districtGuid, SubDistrictForCreationDto input)
        {
            var districtId = await _repository.District.GetDistrictIdByGuidAsync(districtGuid);
            if (districtId == null || districtId == 0) throw new Exception("District not found"); 

            var model = input.Adapt<SubDistrict>();
            model.DistrictId = districtId.Value;

            await _repository.SubDistrict.CreateSubDistrictAsync(model);
            return model.Adapt<SubDistrictDto>();
        }      

        public async Task UpdateSubDistrictAsync(Guid districtGuid, Guid subDistrictGuid, SubDistrictForUpdateDto input, bool trackChanges)
        {
            var entity = await _repository.SubDistrict.GetSubDistrictAsync(districtGuid, subDistrictGuid, false);
            if (entity == null) throw new Exception("SubDistrict not found in this District");

            input.Adapt(entity); 
            
            await _repository.SubDistrict.UpdateSubDistrictAsync(entity);
        }

        public async Task DeleteSubDistrictAsync(Guid districtGuid, Guid subDistrictGuid, SubDistrictForDeleteDto input, bool trackChanges)
        {
            var entity = await _repository.SubDistrict.GetSubDistrictAsync(districtGuid, subDistrictGuid, false);
            if (entity == null) throw new Exception("SubDistrict not found in this District");

            await _repository.SubDistrict.SoftDeleteSubDistrictAsync(entity, input.DeletedById);
        }

        public async Task DeleteSubDistrictByAdminAsync(Guid districtGuid, Guid subDistrictGuid, bool trackChanges)
        {
            var entity = await _repository.SubDistrict.GetSubDistrictAsync(districtGuid, subDistrictGuid, false);
            if (entity == null) throw new Exception("SubDistrict not found in this District");

            await _repository.SubDistrict.DeleteSubDistrictAsync(subDistrictGuid);
        }

        public async Task<IEnumerable<SubDistrictDto>> SearchSubDistrictAsync(
            Guid districtGuid, string? subDistrictName, string? subDistrictNameSearchType)
        {
            var data = await _repository.SubDistrict.SearchSubDistrictAsync(
                districtGuid, subDistrictName, subDistrictNameSearchType);
            return data.Adapt<IEnumerable<SubDistrictDto>>();
        }
    }
}
