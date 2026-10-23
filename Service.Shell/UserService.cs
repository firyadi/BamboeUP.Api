using Mapster;
using BCrypt.Net;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System.Security.Cryptography;

namespace Service.Shell
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAdminRegistryService _adminRegistry;

        public UserService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAdminRegistryService adminRegistry)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
            _adminRegistry = adminRegistry;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(bool trackChanges)
        {
            var entities = await _repository.User.GetAllUsersAsync(trackChanges);
            var dtos = new List<UserDto>();
            foreach (var entity in entities)
                dtos.Add(await MapToDtoAsync(entity));
            return dtos;
        }

        public async Task<UserDto> GetUserByGuidAsync(Guid userGuid, bool trackChanges)
        {
            var entity = await _repository.User.GetUserAsync(userGuid, trackChanges);
            if (entity == null)
                throw new KeyNotFoundException($"User with GUID {userGuid} not found.");
            return await MapToDtoAsync(entity);
        }

        public async Task<UserDto> CreateUserAsync(UserForCreationDto input)
        {
            var model = input.Adapt<User>();
            model.IsAdmin = false;
            var saltBytes = RandomNumberGenerator.GetBytes(32);
            var salt = Convert.ToBase64String(saltBytes);
            model.PasswordSalt = salt;
            model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(salt + input.PasswordHash, workFactor: 12);
            model.StatusId = 1;
            model.PersonId = input.PersonId is > 0 ? input.PersonId : null;
            await EnsurePersonIsFreeAsync(model.PersonId, null);
            await _repository.User.CreateUserAsync(model);
            return await MapToDtoAsync(model);
        }

        public async Task UpdateUserAsync(Guid userGuid, UserForUpdateDto input, bool trackChanges)
        {
            var existingUser = await _repository.User.GetUserAsync(userGuid, false);
            if (existingUser == null)
                throw new Exception($"User with GUID {userGuid} not found.");

            var model = input.Adapt<User>();
            model.UserGuid = userGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            model.IsAdmin = false;
            model.PasswordHash = existingUser.PasswordHash;
            model.PasswordSalt = existingUser.PasswordSalt;

            if (!string.IsNullOrEmpty(input.PasswordHash) && !input.PasswordHash.StartsWith("$2"))
            {
                var saltBytes = RandomNumberGenerator.GetBytes(32);
                var salt = Convert.ToBase64String(saltBytes);
                model.PasswordSalt = salt;
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(salt + input.PasswordHash, workFactor: 12);
            }

            model.PersonId = input.PersonId is > 0 ? input.PersonId : null;
            await EnsurePersonIsFreeAsync(model.PersonId, userGuid);
            await _repository.User.UpdateUserAsync(model);
        }

        public async Task DeleteUserAsync(Guid userGuid, UserForDeleteDto input, bool trackChanges)
        {
            var model = new User
            {
                UserGuid = userGuid,
                StatusId = 0,
                DeletedById = input.DeletedById ?? 0,
                DeletedTime = DateTime.UtcNow
            };

            await _repository.User.SoftDeleteUserAsync(model, input.DeletedById ?? 0);
        }

        public async Task DeleteUserByAdminAsync(Guid userGuid, bool trackChanges)
        {
            await _repository.User.DeleteUserAsync(userGuid);
        }

        public async Task<IEnumerable<UserDto>> SearchUserAsync(
            string? userName, string? userNameSearchType,
            string? fullName, string? fullNameSearchType,
            string? email, string? emailSearchType)
        {
            var data = await _repository.User.SearchUserAsync(
                userName, userNameSearchType,
                fullName, fullNameSearchType,
                email, emailSearchType);
            var dtos = new List<UserDto>();
            foreach (var entity in data)
                dtos.Add(await MapToDtoAsync(entity));
            return dtos;
        }

        public async Task ResetPasswordAsync(UserResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new Exception("New password and confirmation password do not match.");

            var user = await _repository.User.GetUserByUserNameAsync(dto.UserName, trackChanges: false);
            if (user == null)
                throw new Exception($"User '{dto.UserName}' not found.");

            var saltBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
            var newSalt = Convert.ToBase64String(saltBytes);
            var newHash = BCrypt.Net.BCrypt.HashPassword(newSalt + dto.NewPassword, workFactor: 12);

            await _repository.User.UpgradePasswordHashAsync(user.UserId, newHash, newSalt);
        }

        public async Task<IEnumerable<ProgramDto>> GetAllowedMenusAsync(Guid userGuid, string? companyId, string? officeId)
        {
            var user = await _repository.User.GetUserAsync(userGuid, trackChanges: false);
            if (user == null)
                throw new KeyNotFoundException($"User dengan Guid '{userGuid}' tidak ditemukan.");

            var programs = _adminRegistry.IsAdministrator(user.UserId)
                ? await _repository.Program.GetAllProgramsAsync(trackChanges: false)
                : await _repository.Program.GetAllowedProgramsAsync(userGuid, companyId, officeId);

            // Administrator privileges bypass group grants, not the program's active state.
            // Keep the master program listing unchanged so inactive programs can still be managed.
            return programs
                .Where(program => program.IsActive == true && program.StatusId > 0 && program.DeletedTime is null)
                .Adapt<List<ProgramDto>>();
        }

        private async Task EnsurePersonIsFreeAsync(long? personId, Guid? exceptUserGuid)
        {
            if (personId is not > 0)
                return;

            var person = await _repository.Person.GetPersonByIdAsync(personId.Value, false);
            if (person == null)
                throw new InvalidOperationException("Person tidak ditemukan.");

            var owner = await _repository.User.GetActiveUserByPersonIdAsync(personId.Value);
            if (owner != null && owner.UserGuid != exceptUserGuid)
                throw new InvalidOperationException($"Person ini sudah terikat ke user {owner.UserName}.");
        }

        private async Task<UserDto> MapToDtoAsync(User entity)
        {
            var dto = entity.Adapt<UserDto>();
            dto.IsAdmin = _adminRegistry.IsAdministrator(entity.UserId);
            if (entity.PersonId is > 0)
            {
                var person = await _repository.Person.GetPersonByIdAsync(entity.PersonId.Value, false);
                dto.PersonName = person?.PersonName;
            }
            return dto;
        }
    }
}
