using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IAuthenticationService
    {
        Task<TokenDto> LoginAsync(LoginRequestDto loginRequest);
        Task<TokenDto> SwitchScopeAsync(string userName, SwitchScopeRequestDto request);
        /// <summary>Hash password dan generate salt. Returns (hash, salt).</summary>
        (string Hash, string Salt) HashPassword(string password);
        bool VerifyPassword(string password, string hash, string salt);
        /// <summary>Upgrade hash plaintext ke BCrypt setelah login berhasil.</summary>
        Task UpgradePasswordHashAsync(long userId, string newHash, string newSalt);
    }
}
