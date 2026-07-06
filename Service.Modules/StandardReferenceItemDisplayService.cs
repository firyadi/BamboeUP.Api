using Mapster;
using Contracts;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace Service.Modules
{
    public partial class StandardReferenceItemDisplayService : IStandardReferenceItemDisplayService
    {
        private readonly IRepositoryManager _repository;

        public StandardReferenceItemDisplayService(IRepositoryManager repository)
            => _repository = repository;

        /// <inheritdoc />
        public async Task<IEnumerable<StandardReferenceItemDisplayDto>> GetItemsAsync(
            long companyId,
            long companyOfficeId,
            long? departmentId,
            string standardReferenceInitial)
        {
            var entities = await _repository.StandardReferenceItemDisplay.GetItemsAsync(
                companyId,
                companyOfficeId,
                departmentId,
                standardReferenceInitial);

            return entities.Adapt<IEnumerable<StandardReferenceItemDisplayDto>>();
        }
    }
}
