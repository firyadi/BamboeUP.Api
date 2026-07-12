using Entities.Models;

namespace Contracts
{
    public interface IVwStandardReferenceItemRepository
    {
        Task<IEnumerable<VwStandardReferenceItem>> GetAllAsync(string? standardReferenceInitial, bool trackChanges);
        Task<VwStandardReferenceItem?> GetOneAsync(long id, bool trackChanges);
    }
}
