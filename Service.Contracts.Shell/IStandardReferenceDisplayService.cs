using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IStandardReferenceDisplayService
    {
        Task<IEnumerable<StandardReferenceDisplayDto>> GetAllDisplaysAsync(Shared.RequestFeatures.StandardReferenceDisplayParameters parameters, bool trackChanges);
        Task<StandardReferenceDisplayDto> GetOneAsync(long id, bool trackChanges);
    }
}
