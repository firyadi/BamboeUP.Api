using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public class UserCompanyScopeService : IUserCompanyScopeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public UserCompanyScopeService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<UserCompanyScopeDto>> GetAllAsync(bool trackChanges)
        {
            var entities = await _repository.UserCompanyScope.GetAllAsync(trackChanges);
            return entities.Adapt<IEnumerable<UserCompanyScopeDto>>();
        }

        public async Task<IEnumerable<UserCompanyScopeDto>> GetAllByUserGuidAsync(Guid userGuid)
        {
            // Resolve user GUID ? userId
            var user = await _repository.User.GetUserAsync(userGuid, false);
            if (user is null)
                return Enumerable.Empty<UserCompanyScopeDto>();

            var entities = await _repository.UserCompanyScope.GetAllByUserIdAsync(user.UserId);
            return entities.Adapt<IEnumerable<UserCompanyScopeDto>>();
        }

        public async Task<UserCompanyScopeDto?> GetByGuidAsync(Guid userCompanyScopeGuid, bool trackChanges)
        {
            var entity = await _repository.UserCompanyScope.GetAsync(userCompanyScopeGuid, trackChanges);
            return entity is null ? null : entity.Adapt<UserCompanyScopeDto>();
        }

        public async Task<UserCompanyScopeDto?> GetByUserAndGuidAsync(Guid userGuid, Guid userCompanyScopeGuid)
        {
            var user = await _repository.User.GetUserAsync(userGuid, false);
            if (user is null) return null;

            var entity = await _repository.UserCompanyScope.GetByUserAndGuidAsync(user.UserId, userCompanyScopeGuid);
            return entity is null ? null : entity.Adapt<UserCompanyScopeDto>();
        }

        public async Task<UserCompanyScopeDto> CreateAsync(Guid userGuid, UserCompanyScopeForCreationDto input)
        {
            var user = await _repository.User.GetUserAsync(userGuid, false)
                ?? throw new KeyNotFoundException($"User '{userGuid}' not found.");

            var entity = input.Adapt<UserCompanyScope>();
            entity.UserId = user.UserId;

            await _repository.UserCompanyScope.CreateAsync(entity);
            return entity.Adapt<UserCompanyScopeDto>();
        }

        public async Task UpdateAsync(Guid userGuid, Guid userCompanyScopeGuid, UserCompanyScopeForUpdateDto input, bool trackChanges)
        {
            var user = await _repository.User.GetUserAsync(userGuid, false)
                ?? throw new KeyNotFoundException($"User '{userGuid}' not found.");

            var entity = await _repository.UserCompanyScope.GetByUserAndGuidAsync(user.UserId, userCompanyScopeGuid)
                ?? throw new KeyNotFoundException($"UserCompanyScope '{userCompanyScopeGuid}' not found for user.");

            entity.CompanyOfficeId  = input.CompanyOfficeId;
            entity.IsDefaultCompany = input.IsDefaultCompany;
            entity.IsDefaultOffice  = input.IsDefaultOffice;
            entity.UpdatedById      = input.UpdatedById;

            await _repository.UserCompanyScope.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid userGuid, Guid userCompanyScopeGuid, UserCompanyScopeForDeleteDto input, bool trackChanges)
        {
            var user = await _repository.User.GetUserAsync(userGuid, false)
                ?? throw new KeyNotFoundException($"User '{userGuid}' not found.");

            var entity = await _repository.UserCompanyScope.GetByUserAndGuidAsync(user.UserId, userCompanyScopeGuid)
                ?? throw new KeyNotFoundException($"UserCompanyScope '{userCompanyScopeGuid}' not found for user.");

            long deletedBy = input.DeletedById ?? user.UserId;
            await _repository.UserCompanyScope.SoftDeleteAsync(entity, deletedBy);
        }

        public async Task DeleteByAdminAsync(Guid userCompanyScopeGuid, bool trackChanges)
        {
            await _repository.UserCompanyScope.DeleteAsync(userCompanyScopeGuid);
        }
    }
}
