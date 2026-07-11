using BCrypt.Net;
using Contracts;
using Entities.ConfigurationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service.Shell
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepositoryManager _repository;
        private readonly IConfiguration _configuration;
        private readonly IAdminRegistryService _adminRegistry;

        public AuthenticationService(
            IRepositoryManager repository,
            IConfiguration configuration,
            IAdminRegistryService adminRegistry)
        {
            _repository = repository;
            _configuration = configuration;
            _adminRegistry = adminRegistry;
        }

        public async Task<TokenDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _repository.User.GetUserByUserNameAsync(loginRequest.Username, false);
            if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!user.PasswordHash.StartsWith("$2") || string.IsNullOrEmpty(user.PasswordSalt))
            {
                var (upgradedHash, upgradedSalt) = HashPassword(loginRequest.Password);
                await UpgradePasswordHashAsync(user.UserId, upgradedHash, upgradedSalt);
                user.PasswordHash = upgradedHash;
                user.PasswordSalt = upgradedSalt;
            }

            var isAdmin = _adminRegistry.IsAdministrator(user.UserId);
            var dbScopes = await GetDbScopesAsync(user.UserId);

            List<LoginScopeDto> availableScopes;
            (long? activeCompanyId, Guid? activeCompanyGuid, string? activeCompanyName,
             long? activeOfficeId, Guid? activeOfficeGuid, string? activeOfficeName,
             long? activeUserGroupScopeId) activeScope;

            if (loginRequest.CompanyId.HasValue)
            {
                if (isAdmin)
                {
                    var adminScopes = await BuildAdminScopesAsync();
                    activeScope = await ResolveScopeDetailsAsync(
                        loginRequest.CompanyId.Value,
                        loginRequest.OfficeId,
                        adminScopes,
                        isSyntheticAdminScopes: true);
                    availableScopes = BuildAvailableScopes(adminScopes);
                }
                else
                {
                    if (!HasScopeAccess(dbScopes, loginRequest.CompanyId.Value, loginRequest.OfficeId))
                        throw new UnauthorizedAccessException("User does not have access to this company or office.");

                    var selectedScope = SelectScopeFromRequest(
                        loginRequest.CompanyId.Value,
                        loginRequest.OfficeId,
                        dbScopes);

                    if (selectedScope == null)
                        throw new UnauthorizedAccessException("Selected company or office is not in your assigned scopes.");

                    activeScope = await MapDbScopeToActiveAsync(selectedScope);
                    availableScopes = BuildAvailableScopes(dbScopes);
                }
            }
            else
            {
                var defaultScope = dbScopes.FirstOrDefault(s => s.IsDefault) ?? dbScopes.FirstOrDefault();
                if (defaultScope == null)
                {
                    throw new UnauthorizedAccessException(
                        "No scope is assigned to your account. Please contact your administrator.");
                }

                activeScope = await MapDbScopeToActiveAsync(defaultScope);
                availableScopes = BuildAvailableScopes(
                    isAdmin ? await BuildAdminScopesAsync() : dbScopes);
            }

            var (activeCompanyId, activeCompanyGuid, activeCompanyName,
                 activeOfficeId, activeOfficeGuid, activeOfficeName,
                 activeUserGroupScopeId) = activeScope;

            var token = GenerateToken(user, isAdmin, activeCompanyId, activeOfficeId, activeUserGroupScopeId, out var expired);

            return new TokenDto
            {
                Token = token,
                Expired = expired,
                UserId = user.UserId,
                UserGuid = user.UserGuid,
                UserName = user.UserName,
                FullName = user.FullName ?? user.UserName,
                Role = isAdmin ? "Admin" : "User",
                IsAdmin = isAdmin,
                ActiveCompanyId = activeCompanyId,
                ActiveCompanyGuid = activeCompanyGuid,
                ActiveCompanyName = activeCompanyName,
                ActiveOfficeId = activeOfficeId,
                ActiveOfficeGuid = activeOfficeGuid,
                ActiveOfficeName = activeOfficeName,
                ActiveUserGroupScopeId = activeUserGroupScopeId,
                AvailableScopes = availableScopes
            };
        }

        public async Task<TokenDto> SwitchScopeAsync(string userName, SwitchScopeRequestDto request)
        {
            var user = await _repository.User.GetUserByUserNameAsync(userName, false);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var isAdmin = _adminRegistry.IsAdministrator(user.UserId);
            var selectableScopes = await GetSelectableScopesAsync(user.UserId, isAdmin);
            var isSyntheticAdminScopes = isAdmin;

            if (!isSyntheticAdminScopes)
            {
                var dbScopes = await GetDbScopesAsync(user.UserId);
                if (!HasScopeAccess(dbScopes, request.CompanyId, request.OfficeId))
                    throw new UnauthorizedAccessException("User does not have access to this company or office.");
            }

            var (activeCompanyId, activeCompanyGuid, activeCompanyName,
                 activeOfficeId, activeOfficeGuid, activeOfficeName,
                 activeUserGroupScopeId) = await ResolveScopeDetailsAsync(
                request.CompanyId, request.OfficeId, selectableScopes, isSyntheticAdminScopes);

            var availableScopes = BuildAvailableScopes(selectableScopes);

            var token = GenerateToken(user, isAdmin, activeCompanyId, activeOfficeId, activeUserGroupScopeId, out var expired);

            return new TokenDto
            {
                Token = token,
                Expired = expired,
                UserId = user.UserId,
                UserGuid = user.UserGuid,
                UserName = user.UserName,
                FullName = user.FullName ?? user.UserName,
                Role = isAdmin ? "Admin" : "User",
                IsAdmin = isAdmin,
                ActiveCompanyId = activeCompanyId,
                ActiveCompanyGuid = activeCompanyGuid,
                ActiveCompanyName = activeCompanyName,
                ActiveOfficeId = activeOfficeId,
                ActiveOfficeGuid = activeOfficeGuid,
                ActiveOfficeName = activeOfficeName,
                ActiveUserGroupScopeId = activeUserGroupScopeId,
                AvailableScopes = availableScopes
            };
        }

        public async Task<LoginScopesPreviewDto> GetLoginScopesPreviewAsync(LoginRequestDto loginRequest)
        {
            var user = await _repository.User.GetUserByUserNameAsync(loginRequest.Username, false);
            if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid username or password.");

            var isAdmin = _adminRegistry.IsAdministrator(user.UserId);
            var dbScopes = await GetDbScopesAsync(user.UserId);
            var selectableScopes = await GetSelectableScopesAsync(user.UserId, isAdmin);
            var (defaultCompanyId, defaultOfficeId) = ResolveDefaultScopeIdsFromDb(dbScopes);

            return new LoginScopesPreviewDto
            {
                IsAdmin = isAdmin,
                DefaultCompanyId = defaultCompanyId,
                DefaultOfficeId = defaultOfficeId,
                AvailableScopes = BuildAvailableScopes(selectableScopes)
            };
        }

        private async Task<List<Entities.Models.UserGroupScope>> GetDbScopesAsync(long userId)
        {
            return (await _repository.UserGroupScope.GetAllByUserIdAsync(userId)).ToList();
        }

        private async Task<List<Entities.Models.UserGroupScope>> GetSelectableScopesAsync(long userId, bool isAdmin)
        {
            if (isAdmin)
                return await BuildAdminScopesAsync();

            return await GetDbScopesAsync(userId);
        }

        private static (long? CompanyId, long? OfficeId) ResolveDefaultScopeIdsFromDb(
            List<Entities.Models.UserGroupScope> dbScopes)
        {
            var userDefault = dbScopes.FirstOrDefault(s => s.IsDefault) ?? dbScopes.FirstOrDefault();
            if (userDefault == null)
                return (null, null);

            return (userDefault.CompanyId, userDefault.CompanyOfficeId);
        }

        private static bool HasScopeAccess(
            List<Entities.Models.UserGroupScope> scopes,
            long companyId,
            long? officeId)
        {
            return scopes.Any(s =>
                s.CompanyId == companyId &&
                (!officeId.HasValue ||
                 s.CompanyOfficeId == officeId.Value ||
                 s.CompanyOfficeId == null));
        }

        private async Task<(long? CompanyId, Guid? CompanyGuid, string? CompanyName,
            long? OfficeId, Guid? OfficeGuid, string? OfficeName, long? UserGroupScopeId)> MapDbScopeToActiveAsync(
            Entities.Models.UserGroupScope selectedScope)
        {
            var scopeComp = await _repository.Company.GetCompanyByIdAsync(selectedScope.CompanyId, false);
            Guid? scopeOfficeGuid = null;
            if (selectedScope.CompanyOfficeId.HasValue)
            {
                var scopeOffice = await _repository.CompanyOffice.GetCompanyOfficeByIdAsync(
                    selectedScope.CompanyOfficeId.Value, false);
                scopeOfficeGuid = scopeOffice?.CompanyOfficeGuid;
            }

            return (
                selectedScope.CompanyId,
                scopeComp?.CompanyGuid,
                selectedScope.CompanyName ?? scopeComp?.CompanyName,
                selectedScope.CompanyOfficeId,
                scopeOfficeGuid,
                selectedScope.CompanyOfficeName,
                selectedScope.UserGroupScopeId);
        }

        private static List<LoginScopeDto> BuildAvailableScopes(List<Entities.Models.UserGroupScope> rawScopes)
        {
            return rawScopes
                .GroupBy(s => new { s.CompanyId, s.CompanyOfficeId })
                .Select(g =>
                {
                    var first = g.OrderByDescending(x => x.IsDefault).ThenBy(x => x.UserGroupScopeId).First();
                    return new LoginScopeDto
                    {
                        CompanyId = g.Key.CompanyId,
                        CompanyName = first.CompanyName ?? string.Empty,
                        OfficeId = g.Key.CompanyOfficeId,
                        OfficeName = first.CompanyOfficeName,
                        UserGroupScopeId = first.UserGroupScopeId,
                        UserGroupName = first.UserGroupName
                    };
                })
                .OrderBy(s => s.CompanyName)
                .ThenBy(s => s.OfficeName)
                .ToList();
        }

        private async Task<(long? CompanyId, Guid? CompanyGuid, string? CompanyName,
            long? OfficeId, Guid? OfficeGuid, string? OfficeName, long? UserGroupScopeId)> ResolveScopeDetailsAsync(
            long companyId,
            long? officeId,
            List<Entities.Models.UserGroupScope> rawScopes,
            bool isSyntheticAdminScopes)
        {
            if (isSyntheticAdminScopes)
            {
                string? activeCompanyName = null;
                Guid? activeCompanyGuid = null;
                string? activeOfficeName = null;
                Guid? activeOfficeGuid = null;

                var comp = await _repository.Company.GetCompanyByIdAsync(companyId, false);
                activeCompanyName = comp?.CompanyName;
                activeCompanyGuid = comp?.CompanyGuid;

                if (officeId.HasValue)
                {
                    var office = await _repository.CompanyOffice.GetCompanyOfficeByIdAsync(officeId.Value, false);
                    activeOfficeName = office?.CompanyOfficeName;
                    activeOfficeGuid = office?.CompanyOfficeGuid;
                }

                return (companyId, activeCompanyGuid, activeCompanyName, officeId, activeOfficeGuid, activeOfficeName, null);
            }

            var selectedScope = SelectScopeFromRequest(companyId, officeId, rawScopes);
            if (selectedScope == null)
                throw new UnauthorizedAccessException("Selected company or office is not in your assigned scopes.");

            var scopeComp = await _repository.Company.GetCompanyByIdAsync(selectedScope.CompanyId, false);
            Guid? scopeOfficeGuid = null;
            if (selectedScope.CompanyOfficeId.HasValue)
            {
                var scopeOffice = await _repository.CompanyOffice.GetCompanyOfficeByIdAsync(selectedScope.CompanyOfficeId.Value, false);
                scopeOfficeGuid = scopeOffice?.CompanyOfficeGuid;
            }

            return (
                selectedScope.CompanyId,
                scopeComp?.CompanyGuid,
                selectedScope.CompanyName,
                selectedScope.CompanyOfficeId,
                scopeOfficeGuid,
                selectedScope.CompanyOfficeName,
                selectedScope.UserGroupScopeId);
        }

        private static Entities.Models.UserGroupScope? SelectScopeFromRequest(
            long companyId,
            long? officeId,
            List<Entities.Models.UserGroupScope> rawScopes)
        {
            if (officeId.HasValue)
            {
                return rawScopes.FirstOrDefault(s =>
                           s.CompanyId == companyId && s.CompanyOfficeId == officeId.Value)
                       ?? rawScopes.FirstOrDefault(s =>
                           s.CompanyId == companyId && s.CompanyOfficeId == null);
            }

            return rawScopes.FirstOrDefault(s =>
                       s.CompanyId == companyId && s.CompanyOfficeId == null)
                   ?? rawScopes.FirstOrDefault(s => s.CompanyId == companyId);
        }

        private async Task<List<Entities.Models.UserGroupScope>> BuildAdminScopesAsync()
        {
            var rawScopes = new List<Entities.Models.UserGroupScope>();
            var allCompanies = (await _repository.Company.GetAllCompaniesAsync(false)).OrderBy(c => c.CompanyName).ToList();
            var allOffices = (await _repository.CompanyOffice.GetAllCompanyOfficesAsync(false)).ToList();
            var defaultCompanyId = allCompanies.FirstOrDefault()?.CompanyId ?? 0;

            foreach (var c in allCompanies)
            {
                rawScopes.Add(new Entities.Models.UserGroupScope
                {
                    UserGroupScopeId = c.CompanyId * 1000,
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    CompanyOfficeId = null,
                    IsDefault = c.CompanyId == defaultCompanyId
                });

                long officeIdx = 1;
                foreach (var o in allOffices.Where(o => o.CompanyId == c.CompanyId).OrderBy(o => o.CompanyOfficeName))
                {
                    rawScopes.Add(new Entities.Models.UserGroupScope
                    {
                        UserGroupScopeId = (c.CompanyId * 1000) + officeIdx++,
                        CompanyId = c.CompanyId,
                        CompanyName = c.CompanyName,
                        CompanyOfficeId = o.CompanyOfficeId,
                        CompanyOfficeName = o.CompanyOfficeName
                    });
                }
            }

            return rawScopes;
        }

        public (string Hash, string Salt) HashPassword(string password)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(32);
            var salt = Convert.ToBase64String(saltBytes);
            var saltedPassword = salt + password;
            var hash = BCrypt.Net.BCrypt.HashPassword(saltedPassword, workFactor: 12);
            return (hash, salt);
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            if (!hash.StartsWith("$2") && password == hash)
                return true;

            try
            {
                if (string.IsNullOrEmpty(salt))
                    return BCrypt.Net.BCrypt.Verify(password, hash);

                var saltedPassword = salt + password;
                return BCrypt.Net.BCrypt.Verify(saltedPassword, hash);
            }
            catch
            {
                return false;
            }
        }

        public async Task UpgradePasswordHashAsync(long userId, string newHash, string newSalt)
        {
            await _repository.User.UpgradePasswordHashAsync(userId, newHash, newSalt);
        }

        private string GenerateToken(
            Entities.Models.User user,
            bool isAdmin,
            long? companyId,
            long? officeId,
            long? userGroupScopeId,
            out DateTime expired)
        {
            var jwtConfig = _configuration.GetSection("JwtSettings").Get<JwtConfiguration>()
                ?? throw new InvalidOperationException("JWT configuration tidak ditemukan.");

            var secretKey = jwtConfig.SecretKey;
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
                throw new InvalidOperationException("JWT SecretKey tidak valid atau terlalu pendek (min 32 karakter).");

            expired = DateTime.UtcNow.AddMinutes(jwtConfig.Expires);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, isAdmin ? "Admin" : "User"),
                new("UserId", user.UserId.ToString()),
                new("UserGuid", user.UserGuid.ToString())
            };

            if (companyId.HasValue)
                claims.Add(new Claim("CompanyId", companyId.Value.ToString()));

            if (officeId.HasValue)
                claims.Add(new Claim("OfficeId", officeId.Value.ToString()));

            if (userGroupScopeId.HasValue)
                claims.Add(new Claim("UserGroupScopeId", userGroupScopeId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtConfig.ValidIssuer,
                audience: jwtConfig.ValidAudience,
                claims: claims,
                expires: expired,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
