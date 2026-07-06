using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(bool trackChanges);
        Task<UserDto> GetUserByGuidAsync(Guid userGuid, bool trackChanges);
        Task<UserDto> CreateUserAsync(UserForCreationDto userForCreation);
        Task UpdateUserAsync(Guid userGuid, UserForUpdateDto userForUpdate, bool trackChanges);
        Task DeleteUserAsync(Guid userGuid, UserForDeleteDto userForDelete, bool trackChanges);
        Task ResetPasswordAsync(UserResetPasswordDto dto);
        Task DeleteUserByAdminAsync(Guid userGuid, bool trackChanges);

        Task<IEnumerable<UserDto>> SearchUserAsync(
            string? userName, string? userNameSearchType,
            string? fullName, string? fullNameSearchType,
            string? email, string? emailSearchType
        );

        Task<IEnumerable<ProgramDto>> GetAllowedMenusAsync(Guid userGuid, string? companyId, string? officeId);
    }
}
