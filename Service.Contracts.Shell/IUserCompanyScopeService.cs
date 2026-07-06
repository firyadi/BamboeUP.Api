using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IUserCompanyScopeService
    {
        Task<IEnumerable<UserCompanyScopeDto>> GetAllAsync(bool trackChanges);
        Task<IEnumerable<UserCompanyScopeDto>> GetAllByUserGuidAsync(Guid userGuid);
        Task<UserCompanyScopeDto?> GetByGuidAsync(Guid userCompanyScopeGuid, bool trackChanges);
        Task<UserCompanyScopeDto?> GetByUserAndGuidAsync(Guid userGuid, Guid userCompanyScopeGuid);

        Task<UserCompanyScopeDto> CreateAsync(Guid userGuid, UserCompanyScopeForCreationDto input);
        Task UpdateAsync(Guid userGuid, Guid userCompanyScopeGuid, UserCompanyScopeForUpdateDto input, bool trackChanges);
        Task DeleteAsync(Guid userGuid, Guid userCompanyScopeGuid, UserCompanyScopeForDeleteDto input, bool trackChanges);
        Task DeleteByAdminAsync(Guid userCompanyScopeGuid, bool trackChanges);
    }
}
