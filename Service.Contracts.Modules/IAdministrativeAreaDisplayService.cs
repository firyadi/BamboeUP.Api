using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public interface IAdministrativeAreaDisplayService
    {
        Task<IEnumerable<AdministrativeAreaDisplayDto>> GetAllAsync(Shared.RequestFeatures.AdministrativeAreaParameters parameters, bool trackChanges);
        Task<AdministrativeAreaDisplayDto?> GetOneAsync(string id, bool trackChanges);
    }
}
