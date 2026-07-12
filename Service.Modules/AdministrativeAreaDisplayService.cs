using Mapster;
using Contracts;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace Service.Modules
{
    public class AdministrativeAreaDisplayService : IAdministrativeAreaDisplayService
    {
        private readonly IRepositoryManager _repository;
        public AdministrativeAreaDisplayService(IRepositoryManager repository)
        {
            _repository = repository;
            }

        public async Task<IEnumerable<AdministrativeAreaDisplayDto>> GetAllAsync(Shared.RequestFeatures.AdministrativeAreaParameters parameters, bool trackChanges)
        {
            var entities = await _repository.AdministrativeAreaDisplay.GetAllAsync(parameters, trackChanges);
            var dtos = entities.Adapt<IEnumerable<AdministrativeAreaDisplayDto>>();
            return dtos;
        }

        public async Task<AdministrativeAreaDisplayDto?> GetOneAsync(string id, bool trackChanges)
        {
            var entity = await _repository.AdministrativeAreaDisplay.GetOneAsync(id, trackChanges);
            if (entity == null)
                throw new Entities.Exceptions.AdministrativeAreaDisplayNotFoundException(id);

            var dto = entity.Adapt<AdministrativeAreaDisplayDto>();
            return dto;
        }
    }
}
