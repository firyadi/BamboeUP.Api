using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public interface IUserGroupService
    {
        Task<IEnumerable<UserGroupDto>> GetAllUserGroupsAsync(bool trackChanges);
        Task<UserGroupDto> GetUserGroupAsync(Guid userGroupGuid, bool trackChanges);
        Task<UserGroupDto> CreateUserGroupAsync(UserGroupForCreationDto userGroup);
        Task UpdateUserGroupAsync(Guid userGroupGuid, UserGroupForUpdateDto userGroup, bool trackChanges);
        Task DeleteUserGroupAsync(Guid userGroupGuid, UserGroupForDeleteDto userGroup, bool trackChanges);
        Task DeleteUserGroupByAdminAsync(Guid userGroupGuid, bool trackChanges);
        Task<IEnumerable<UserGroupDto>> SearchUserGroupAsync(string? userGroupName, string? userGroupNameSearchType);
    }
}
