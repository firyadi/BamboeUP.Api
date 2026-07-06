using Entities.Models;

namespace Contracts
{
    public interface IStandardReferenceDisplayRepository
    {
        Task<IEnumerable<StandardReferenceDisplay>> GetAllDisplaysAsync(Shared.RequestFeatures.StandardReferenceDisplayParameters parameters, bool trackChanges);
        Task<StandardReferenceDisplay> GetOneAsync(long id, bool trackChanges);
    }
}
