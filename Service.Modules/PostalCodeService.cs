using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace Service.Modules
{
    public class PostalCodeService : IPostalCodeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public PostalCodeService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<PostalCodeDto>> GetAllPostalCodesAsync(Guid subDistrictGuid, bool trackChanges)
        {
            var entities = await _repository.PostalCode.GetAllPostalCodesAsync(subDistrictGuid, trackChanges);
            return entities.Adapt<IEnumerable<PostalCodeDto>>();
        }

        public async Task<PostalCodeDto> GetPostalCodeByGuidAsync(Guid subDistrictGuid, Guid postalCodeGuid, bool trackChanges)
        {
            var entity = await _repository.PostalCode.GetPostalCodeAsync(subDistrictGuid, postalCodeGuid, trackChanges);
            return entity.Adapt<PostalCodeDto>();
        }

        public async Task<PostalCodeDto> CreatePostalCodeAsync(Guid subDistrictGuid, PostalCodeForCreationDto input)
        {
            var subDistrictId = await _repository.SubDistrict.GetSubDistrictIdByGuidAsync(subDistrictGuid);
            if (subDistrictId == null || subDistrictId == 0) throw new Exception("SubDistrict not found"); 

            var model = input.Adapt<PostalCode>();
            model.SubDistrictId = subDistrictId.Value;

            await _repository.PostalCode.CreatePostalCodeAsync(model);
            return model.Adapt<PostalCodeDto>();
        }      

        public async Task UpdatePostalCodeAsync(Guid subDistrictGuid, Guid postalCodeGuid, PostalCodeForUpdateDto input, bool trackChanges)
        {
            var entity = await _repository.PostalCode.GetPostalCodeAsync(subDistrictGuid, postalCodeGuid, false);
            if (entity == null) throw new Exception("PostalCode not found in this SubDistrict");

            input.Adapt(entity); 
            
            await _repository.PostalCode.UpdatePostalCodeAsync(entity);
        }

        public async Task DeletePostalCodeAsync(Guid subDistrictGuid, Guid postalCodeGuid, PostalCodeForDeleteDto input, bool trackChanges)
        {
            var entity = await _repository.PostalCode.GetPostalCodeAsync(subDistrictGuid, postalCodeGuid, false);
            if (entity == null) throw new Exception("PostalCode not found in this SubDistrict");

            await _repository.PostalCode.SoftDeletePostalCodeAsync(entity, input.DeletedById);
        }

        public async Task DeletePostalCodeByAdminAsync(Guid subDistrictGuid, Guid postalCodeGuid, bool trackChanges)
        {
            var entity = await _repository.PostalCode.GetPostalCodeAsync(subDistrictGuid, postalCodeGuid, false);
            if (entity == null) throw new Exception("PostalCode not found in this SubDistrict");

            await _repository.PostalCode.DeletePostalCodeAsync(postalCodeGuid);
        }

        public async Task<IEnumerable<PostalCodeDto>> SearchPostalCodeAsync(
            Guid subDistrictGuid, string? postalCodeValue, string? postalCodeValueSearchType)
        {
            var data = await _repository.PostalCode.SearchPostalCodeAsync(
                subDistrictGuid, postalCodeValue, postalCodeValueSearchType);
            return data.Adapt<IEnumerable<PostalCodeDto>>();
        }
    }
}
