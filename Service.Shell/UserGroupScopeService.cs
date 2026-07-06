using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public class UserGroupScopeService : IUserGroupScopeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public UserGroupScopeService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<UserGroupScopeDto>> GetAllByUserGroupGuidAsync(Guid userGroupGuid, bool trackChanges)
        {
            var entities = await _repository.UserGroupScope.GetAllByUserGroupGuidAsync(userGroupGuid, trackChanges);
            return entities.Adapt<IEnumerable<UserGroupScopeDto>>();
        }

        public async Task<IEnumerable<UserGroupScopeDto>> GetAllByUserIdAsync(long userId)
        {
            var entities = await _repository.UserGroupScope.GetAllByUserIdAsync(userId);
            return entities.Adapt<IEnumerable<UserGroupScopeDto>>();
        }

        public async Task<UserGroupScopeDto?> GetByGuidAsync(Guid userGroupScopeGuid, bool trackChanges)
        {
            var entity = await _repository.UserGroupScope.GetAsync(userGroupScopeGuid, trackChanges);
            return entity is null ? null : entity.Adapt<UserGroupScopeDto>();
        }

        public async Task<UserGroupScopeDto?> GetByUserGroupAndGuidAsync(Guid userGroupGuid, Guid userGroupScopeGuid)
        {
            var entity = await _repository.UserGroupScope.GetByUserGroupAndGuidAsync(userGroupGuid, userGroupScopeGuid);
            return entity is null ? null : entity.Adapt<UserGroupScopeDto>();
        }

        public async Task<UserGroupScopeDto> CreateAsync(Guid userGroupGuid, UserGroupScopeForCreationDto input)
        {
            // Resolve userGroupGuid ? userGroupId
            var userGroup = await _repository.UserGroup.GetUserGroupAsync(userGroupGuid, false)
                ?? throw new KeyNotFoundException($"UserGroup '{userGroupGuid}' not found.");

            var entity = input.Adapt<UserGroupScope>();
            entity.UserGroupId = userGroup.UserGroupId;

            await _repository.UserGroupScope.CreateAsync(entity);
            return entity.Adapt<UserGroupScopeDto>();
        }

        public async Task<UserGroupScopeDto> CreateByUserAsync(Guid userGuid, UserGroupScopeForCreationDto input)
        {
            var user = await _repository.User.GetUserAsync(userGuid, false)
                ?? throw new KeyNotFoundException($"User '{userGuid}' not found.");

            var entity = input.Adapt<UserGroupScope>();
            entity.UserId = user.UserId;

            await _repository.UserGroupScope.CreateAsync(entity);
            return entity.Adapt<UserGroupScopeDto>();
        }

        public async Task DeleteAsync(Guid userGroupGuid, Guid userGroupScopeGuid, UserGroupScopeForDeleteDto input, bool trackChanges)
        {
            var entity = await _repository.UserGroupScope.GetByUserGroupAndGuidAsync(userGroupGuid, userGroupScopeGuid)
                ?? throw new KeyNotFoundException($"UserGroupScope '{userGroupScopeGuid}' not found for UserGroup '{userGroupGuid}'.");

            long deletedBy = input.DeletedById ?? entity.CreatedById;
            await _repository.UserGroupScope.SoftDeleteAsync(entity, deletedBy);
        }

        public async Task DeleteByUserAsync(Guid userGuid, Guid userGroupScopeGuid, UserGroupScopeForDeleteDto input, bool trackChanges)
        {
            var entity = await _repository.UserGroupScope.GetAsync(userGroupScopeGuid, false)
                ?? throw new KeyNotFoundException($"UserGroupScope '{userGroupScopeGuid}' not found.");

            var user = await _repository.User.GetUserAsync(userGuid, false)
                ?? throw new KeyNotFoundException($"User '{userGuid}' not found.");

            if (entity.UserId != user.UserId) 
                throw new UnauthorizedAccessException("Mismatch User.");

            long deletedBy = input.DeletedById ?? entity.CreatedById;
            await _repository.UserGroupScope.SoftDeleteAsync(entity, deletedBy);
        }

        public async Task DeleteByAdminAsync(Guid userGroupScopeGuid, bool trackChanges)
        {
            await _repository.UserGroupScope.DeleteAsync(userGroupScopeGuid);
        }

        public async Task<UserGroupScopeDto> SetDefaultAsync(Guid userGroupScopeGuid, UserGroupScopeSetDefaultDto input)
        {
            // Pastikan scope yang dimaksud ada dan aktif
            var existing = await _repository.UserGroupScope.GetAsync(userGroupScopeGuid, false)
                ?? throw new KeyNotFoundException($"UserGroupScope '{userGroupScopeGuid}' tidak ditemukan atau sudah tidak aktif.");

            var updatedEntity = await _repository.UserGroupScope.SetDefaultAsync(
                userGroupScopeGuid,
                existing.UserId,
                input.UpdatedById);

            return updatedEntity.Adapt<UserGroupScopeDto>();
        }
    }
}
