using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserGroupProgramRepository
    {
        Task<UserGroupProgram?> GetUserGroupProgramAsync(Guid userGroupProgramGuid, bool trackChanges);
        Task<IEnumerable<UserGroupProgram>> GetAllUserGroupProgramsAsync(bool trackChanges);
        Task<IEnumerable<UserGroupProgram>> GetUserGroupProgramsByUserGroupGuidAsync(Guid userGroupGuid, bool trackChanges);
        Task CreateUserGroupProgramAsync(UserGroupProgram userGroupProgram, IDbTransaction? transaction = null);
        Task UpdateUserGroupProgramAsync(UserGroupProgram userGroupProgram, IDbTransaction? transaction = null);
        Task SoftDeleteUserGroupProgramAsync(UserGroupProgram userGroupProgram, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteUserGroupProgramAsync(Guid userGroupProgramGuid, IDbTransaction? transaction = null);
    }
}
