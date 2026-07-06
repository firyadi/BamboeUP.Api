using Entities.Models;

namespace Contracts
{
    public interface IAdministrativeAreaDisplayRepository
    {
        Task<IEnumerable<AdministrativeAreaDisplay>> GetAllAsync(Shared.RequestFeatures.AdministrativeAreaParameters parameters, bool trackChanges);
        Task<AdministrativeAreaDisplay> GetOneAsync(string id, bool trackChanges);
    }
}
