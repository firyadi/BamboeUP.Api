using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public interface IUserGroupProgramService
    {
        Task<IEnumerable<UserGroupProgramDto>> GetAllUserGroupProgramsAsync(bool trackChanges);
        Task<IEnumerable<UserGroupProgramDto>> GetUserGroupProgramsByUserGroupAsync(Guid userGroupGuid, bool trackChanges);
        Task<UserGroupProgramDto> GetUserGroupProgramAsync(Guid userGroupProgramGuid, bool trackChanges);
        Task<UserGroupProgramDto> CreateUserGroupProgramAsync(UserGroupProgramForCreationDto userGroupProgram);
        Task UpdateUserGroupProgramAsync(Guid userGroupProgramGuid, UserGroupProgramForUpdateDto userGroupProgram, bool trackChanges);
        Task DeleteUserGroupProgramAsync(Guid userGroupProgramGuid, UserGroupProgramForDeleteDto userGroupProgram, bool trackChanges);
        Task DeleteUserGroupProgramByAdminAsync(Guid userGroupProgramGuid, bool trackChanges);
    }
}
