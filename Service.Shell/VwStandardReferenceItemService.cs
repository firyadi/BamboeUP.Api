using Mapster;
using Contracts;
using Entities.Exceptions;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public class VwStandardReferenceItemService : IVwStandardReferenceItemService
    {
        private readonly IRepositoryManager _repository;
        public VwStandardReferenceItemService(IRepositoryManager repository)
        {
            _repository = repository;
            }

        public async Task<IEnumerable<VwStandardReferenceItemDto>> GetAllAsync(string? standardReferenceInitial, bool trackChanges)
        {
            var entities = await _repository.VwStandardReferenceItem.GetAllAsync(standardReferenceInitial, trackChanges);
            return entities.Adapt<IEnumerable<VwStandardReferenceItemDto>>();
        }

        public async Task<VwStandardReferenceItemDto> GetOneAsync(long id, bool trackChanges)
        {
            var entity = await _repository.VwStandardReferenceItem.GetOneAsync(id, trackChanges);
            if (entity is null)
                throw new VwStandardReferenceItemNotFoundException(id);

            return entity.Adapt<VwStandardReferenceItemDto>();
        }
    }
}
