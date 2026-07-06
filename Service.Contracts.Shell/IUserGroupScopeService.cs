using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IUserGroupScopeService
    {
        /// <summary>Semua users dalam satu UserGroup (lintas company/office)</summary>
        Task<IEnumerable<UserGroupScopeDto>> GetAllByUserGroupGuidAsync(Guid userGroupGuid, bool trackChanges);

        /// <summary>Semua UserGroup yang dimiliki user (lintas company)</summary>
        Task<IEnumerable<UserGroupScopeDto>> GetAllByUserIdAsync(long userId);

        Task<UserGroupScopeDto?> GetByGuidAsync(Guid userGroupScopeGuid, bool trackChanges);
        Task<UserGroupScopeDto?> GetByUserGroupAndGuidAsync(Guid userGroupGuid, Guid userGroupScopeGuid);

        Task<UserGroupScopeDto> CreateAsync(Guid userGroupGuid, UserGroupScopeForCreationDto input);
        Task<UserGroupScopeDto> CreateByUserAsync(Guid userGuid, UserGroupScopeForCreationDto input);
        Task DeleteAsync(Guid userGroupGuid, Guid userGroupScopeGuid, UserGroupScopeForDeleteDto input, bool trackChanges);
        Task DeleteByUserAsync(Guid userGuid, Guid userGroupScopeGuid, UserGroupScopeForDeleteDto input, bool trackChanges);
        Task DeleteByAdminAsync(Guid userGroupScopeGuid, bool trackChanges);

        /// <summary>
        /// Set scope sebagai default untuk user. Semua scope default lain milik user akan di-unset secara atomik.
        /// </summary>
        Task<UserGroupScopeDto> SetDefaultAsync(Guid userGroupScopeGuid, UserGroupScopeSetDefaultDto input);
    }
}
