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

        public UserService(
           IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(bool trackChanges)
        {
            var entities = await _repository.User.GetAllUsersAsync(trackChanges);
            return entities.Adapt<IEnumerable<UserDto>>();
        }

        public async Task<UserDto> GetUserByGuidAsync(Guid userGuid, bool trackChanges)
        {
            var entity = await _repository.User.GetUserAsync(userGuid, trackChanges);
            return entity.Adapt<UserDto>();
        }

        public async Task<UserDto> CreateUserAsync(UserForCreationDto input)
        {
            var model = input.Adapt<User>();
            // Generate salt dan hash password sebelum disimpan ke database
            var saltBytes = RandomNumberGenerator.GetBytes(32);
            var salt = Convert.ToBase64String(saltBytes);
            model.PasswordSalt = salt;
            model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(salt + input.PasswordHash, workFactor: 12);
            model.StatusId = 1;
            await _repository.User.CreateUserAsync(model);
            return model.Adapt<UserDto>();
        }

        public async Task UpdateUserAsync(Guid userGuid, UserForUpdateDto input, bool trackChanges)
        {
            // Fetch the existing entity to preserve unchanged fields like PasswordHash and PasswordSalt
            var existingUser = await _repository.User.GetUserAsync(userGuid, false);
            if (existingUser == null)
            {
                throw new Exception($"User with GUID {userGuid} not found.");
            }

            var model = input.Adapt<User>();
            model.UserGuid = userGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;

            // Preserve old PasswordHash and PasswordSalt by default
            model.PasswordHash = existingUser.PasswordHash;
            model.PasswordSalt = existingUser.PasswordSalt;

            // Hash password jika diberikan dan bukan sudah BCrypt
            if (!string.IsNullOrEmpty(input.PasswordHash) && !input.PasswordHash.StartsWith("$2"))
            {
                var saltBytes = RandomNumberGenerator.GetBytes(32);
                var salt = Convert.ToBase64String(saltBytes);
                model.PasswordSalt = salt;
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(salt + input.PasswordHash, workFactor: 12);
            }

            await _repository.User.UpdateUserAsync(model);
        }

        //Soft delete method
        
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
            return data.Adapt<IEnumerable<UserDto>>();
        }

        public async Task ResetPasswordAsync(UserResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new Exception("New password and confirmation password do not match.");

            var user = await _repository.User.GetUserByUserNameAsync(dto.UserName, trackChanges: false);
            if (user == null)
                throw new Exception($"User '{dto.UserName}' not found.");

            // Hash password dengan BCrypt (pola sama seperti CreateUserAsync)
            var saltBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
            var newSalt = Convert.ToBase64String(saltBytes);
            var newHash = BCrypt.Net.BCrypt.HashPassword(newSalt + dto.NewPassword, workFactor: 12);

            await _repository.User.UpgradePasswordHashAsync(user.UserId, newHash, newSalt);
        }

        public async Task<IEnumerable<ProgramDto>> GetAllowedMenusAsync(Guid userGuid, string? companyId, string? officeId)
        {
            var user = await _repository.User.GetUserAsync(userGuid, trackChanges: false);
            if (user == null)
            {
                throw new KeyNotFoundException($"User dengan Guid '{userGuid}' tidak ditemukan.");
            }

            if (user.IsAdmin)
            {
                // Admin has access to all active programs/menus
                var allPrograms = await _repository.Program.GetAllProgramsAsync(trackChanges: false);
                return allPrograms.Adapt<IEnumerable<ProgramDto>>();
            }
            else
            {
                // Non-admin gets menus based on scopes and user group permissions
                var allowedPrograms = await _repository.Program.GetAllowedProgramsAsync(userGuid, companyId, officeId);
                return allowedPrograms.Adapt<IEnumerable<ProgramDto>>();
            }
        }
    }
}
