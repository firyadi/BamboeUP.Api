using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public class UserGroupService : IUserGroupService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public UserGroupService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<UserGroupDto>> GetAllUserGroupsAsync(bool trackChanges)
        {
            var entities = await _repository.UserGroup.GetAllUserGroupsAsync(trackChanges);
            return entities.Adapt<IEnumerable<UserGroupDto>>();
        }

        public async Task<UserGroupDto> GetUserGroupAsync(Guid userGroupGuid, bool trackChanges)
        {
            var entity = await _repository.UserGroup.GetUserGroupAsync(userGroupGuid, trackChanges)
                ?? throw new KeyNotFoundException($"UserGroup with Guid '{userGroupGuid}' not found.");
            return entity.Adapt<UserGroupDto>();
        }

        public async Task<UserGroupDto> CreateUserGroupAsync(UserGroupForCreationDto input)
        {
            var model = input.Adapt<UserGroup>();
            model.StatusId = 1;
            await _repository.UserGroup.CreateUserGroupAsync(model);
            return model.Adapt<UserGroupDto>();
        }

        public async Task UpdateUserGroupAsync(Guid userGroupGuid, UserGroupForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<UserGroup>();
            model.UserGroupGuid = userGroupGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.UserGroup.UpdateUserGroupAsync(model);
        }

        public async Task DeleteUserGroupAsync(Guid userGroupGuid, UserGroupForDeleteDto input, bool trackChanges)
        {
            var model = new UserGroup
            {
                UserGroupGuid = userGroupGuid,
                StatusId = 0,
                DeletedById = input.DeletedById ?? 0,
                DeletedTime = DateTime.UtcNow
            };

            await _repository.UserGroup.SoftDeleteUserGroupAsync(model, input.DeletedById ?? 0);
        }

        public async Task DeleteUserGroupByAdminAsync(Guid userGroupGuid, bool trackChanges)
        {
            await _repository.UserGroup.DeleteUserGroupAsync(userGroupGuid);
        }

        public async Task<IEnumerable<UserGroupDto>> SearchUserGroupAsync(
            string? userGroupName, string? userGroupNameSearchType)
        {
            var data = await _repository.UserGroup.SearchUserGroupAsync(
                userGroupName, userGroupNameSearchType);
            return data.Adapt<IEnumerable<UserGroupDto>>();
        }
    }
}
