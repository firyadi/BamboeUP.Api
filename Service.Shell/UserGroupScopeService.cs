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
            entity.PersonId = input.PersonId is > 0 ? input.PersonId : null;
            await EnsurePersonIsFreeAsync(entity.CompanyId, entity.PersonId, entity.UserId);

            await _repository.UserGroupScope.CreateAsync(entity);
            return entity.Adapt<UserGroupScopeDto>();
        }

        public async Task UpdateByUserAsync(Guid userGuid, Guid userGroupScopeGuid, UserGroupScopeForUpdateDto input)
        {
            var user = await _repository.User.GetUserAsync(userGuid, false)
                ?? throw new KeyNotFoundException($"User '{userGuid}' not found.");
            var entity = await _repository.UserGroupScope.GetAsync(userGroupScopeGuid, false)
                ?? throw new KeyNotFoundException($"UserGroupScope '{userGroupScopeGuid}' not found.");
            if (entity.UserId != user.UserId)
                throw new InvalidOperationException("Scope ini bukan milik user tersebut.");

            var userGroup = await _repository.UserGroup.GetUserGroupAsync(input.UserGroupGuid, false)
                ?? throw new KeyNotFoundException($"UserGroup '{input.UserGroupGuid}' not found.");

            var personId = input.PersonId is > 0 ? input.PersonId : null;
            await EnsurePersonIsFreeAsync(input.CompanyId, personId, user.UserId);

            entity.UserGroupId = userGroup.UserGroupId;
            entity.UserGroupGuid = input.UserGroupGuid;
            entity.CompanyId = input.CompanyId;
            entity.CompanyGuid = input.CompanyGuid;
            entity.CompanyOfficeId = input.CompanyOfficeId is > 0 ? input.CompanyOfficeId : null;
            entity.CompanyOfficeGuid = input.CompanyOfficeId is > 0 ? input.CompanyOfficeGuid : null;
            entity.PersonId = personId;
            entity.UpdatedById = input.UpdatedById;

            await _repository.UserGroupScope.UpdateAsync(entity);
        }

        public async Task UpdatePersonByUserAsync(Guid userGuid, Guid userGroupScopeGuid, UserGroupScopePersonDto input)
        {
            var user = await _repository.User.GetUserAsync(userGuid, false)
                ?? throw new KeyNotFoundException($"User '{userGuid}' not found.");
            var entity = await _repository.UserGroupScope.GetAsync(userGroupScopeGuid, false)
                ?? throw new KeyNotFoundException($"UserGroupScope '{userGroupScopeGuid}' not found.");
            if (entity.UserId != user.UserId)
                throw new InvalidOperationException("Scope ini bukan milik user tersebut.");

            var personId = input.PersonId is > 0 ? input.PersonId : null;
            await EnsurePersonIsFreeAsync(entity.CompanyId, personId, user.UserId);
            await _repository.UserGroupScope.UpdatePersonAsync(userGroupScopeGuid, personId, input.UpdatedById);
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

        private async Task EnsurePersonIsFreeAsync(long companyId, long? personId, long userId)
        {
            if (personId is not > 0 || companyId <= 0)
                return;

            var person = await _repository.Person.GetPersonByIdAsync(personId.Value, false);
            if (person == null)
                throw new InvalidOperationException("Person tidak ditemukan.");

            var owner = await _repository.UserGroupScope.GetOtherUserByCompanyPersonAsync(companyId, personId.Value, userId);
            if (owner != null)
                throw new InvalidOperationException($"Person ini sudah terikat ke user {owner.UserName} di company yang sama.");
        }
    }
}
