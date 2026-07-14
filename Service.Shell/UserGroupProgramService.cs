using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public class UserGroupProgramService : IUserGroupProgramService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public UserGroupProgramService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<UserGroupProgramDto>> GetAllUserGroupProgramsAsync(bool trackChanges)
        {
            var entities = await _repository.UserGroupProgram.GetAllUserGroupProgramsAsync(trackChanges);
            return entities.Adapt<IEnumerable<UserGroupProgramDto>>();
        }

        public async Task<IEnumerable<UserGroupProgramDto>> GetUserGroupProgramsByUserGroupAsync(Guid userGroupGuid, bool trackChanges)
        {
            var entities = await _repository.UserGroupProgram.GetUserGroupProgramsByUserGroupGuidAsync(userGroupGuid, trackChanges);
            return entities.Adapt<IEnumerable<UserGroupProgramDto>>();
        }

        public async Task<UserGroupProgramDto> GetUserGroupProgramAsync(Guid userGroupProgramGuid, bool trackChanges)
        {
            var entity = await _repository.UserGroupProgram.GetUserGroupProgramAsync(userGroupProgramGuid, trackChanges)
                ?? throw new KeyNotFoundException($"UserGroupProgram with Guid '{userGroupProgramGuid}' not found.");
            return entity.Adapt<UserGroupProgramDto>();
        }

        public async Task<UserGroupProgramDto> CreateUserGroupProgramAsync(UserGroupProgramForCreationDto input)
        {
            var model = input.Adapt<UserGroupProgram>();
            model.StatusId = 1;
            await _repository.UserGroupProgram.CreateUserGroupProgramAsync(model);
            return model.Adapt<UserGroupProgramDto>();
        }

        public async Task UpdateUserGroupProgramAsync(Guid userGroupProgramGuid, UserGroupProgramForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<UserGroupProgram>();
            model.UserGroupProgramGuid = userGroupProgramGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.UserGroupProgram.UpdateUserGroupProgramAsync(model);
        }

        public async Task DeleteUserGroupProgramAsync(Guid userGroupProgramGuid, UserGroupProgramForDeleteDto input, bool trackChanges)
        {
            var model = new UserGroupProgram
            {
                UserGroupProgramGuid = userGroupProgramGuid,
                StatusId = 0,
                DeletedById = input.DeletedById ?? 0,
                DeletedTime = DateTime.UtcNow
            };

            await _repository.UserGroupProgram.SoftDeleteUserGroupProgramAsync(model, input.DeletedById ?? 0);
        }

        public async Task DeleteUserGroupProgramByAdminAsync(Guid userGroupProgramGuid, bool trackChanges)
        {
            await _repository.UserGroupProgram.DeleteUserGroupProgramAsync(userGroupProgramGuid);
        }
    }
}
