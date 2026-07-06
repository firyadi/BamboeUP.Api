using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IVwStandardReferenceItemService
    {
        Task<IEnumerable<VwStandardReferenceItemDto>> GetAllAsync(string? standardReferenceInitial, bool trackChanges);
        Task<VwStandardReferenceItemDto> GetOneAsync(long id, bool trackChanges);
    }
}
