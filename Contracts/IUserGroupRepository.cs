using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserGroupRepository
    {
        Task<UserGroup?> GetUserGroupAsync(Guid userGroupGuid, bool trackChanges);
        Task<IEnumerable<UserGroup>> GetAllUserGroupsAsync(bool trackChanges);
        Task CreateUserGroupAsync(UserGroup userGroup, IDbTransaction? transaction = null);
        Task UpdateUserGroupAsync(UserGroup userGroup, IDbTransaction? transaction = null);
        Task SoftDeleteUserGroupAsync(UserGroup userGroup, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteUserGroupAsync(Guid userGroupGuid, IDbTransaction? transaction = null);
        Task<IEnumerable<UserGroup>> SearchUserGroupAsync(string? userGroupName, string? userGroupNameSearchType, IDbTransaction? transaction = null);
    }
}
