using Mapster;
using Contracts;
using Entities.Exceptions;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public class StandardReferenceDisplayService : IStandardReferenceDisplayService
    {
        private readonly IRepositoryManager _repository;
        public StandardReferenceDisplayService(IRepositoryManager repository)
        {
            _repository = repository;
            }

        public async Task<IEnumerable<StandardReferenceDisplayDto>> GetAllDisplaysAsync(Shared.RequestFeatures.StandardReferenceDisplayParameters parameters, bool trackChanges)
        {
            var entities = await _repository.StandardReferenceDisplay.GetAllDisplaysAsync(parameters, trackChanges);
            return entities.Adapt<IEnumerable<StandardReferenceDisplayDto>>();
        }

        public async Task<StandardReferenceDisplayDto> GetOneAsync(long id, bool trackChanges)
        {
            var entity = await _repository.StandardReferenceDisplay.GetOneAsync(id, trackChanges);
            if (entity is null)
                throw new StandardReferenceDisplayNotFoundException(id);

            return entity.Adapt<StandardReferenceDisplayDto>();
        }
    }
}
