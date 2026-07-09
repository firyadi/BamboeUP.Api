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
            var rawScopes = isAdmin
                ? await BuildAdminScopesAsync()
                : (await _repository.UserGroupScope.GetAllByUserIdAsync(user.UserId)).ToList();

            if (loginRequest.CompanyId.HasValue && !isAdmin)
            {
                var hasAccess = rawScopes.Any(s =>
                    s.CompanyId == loginRequest.CompanyId.Value &&
                    (!loginRequest.OfficeId.HasValue ||
                     s.CompanyOfficeId == loginRequest.OfficeId.Value ||
                     s.CompanyOfficeId == null));

                if (!hasAccess)
                    throw new UnauthorizedAccessException("User does not have access to this company or office.");
            }

            var (activeCompanyId, activeCompanyGuid, activeCompanyName,
                 activeOfficeId, activeOfficeGuid, activeOfficeName,
                 activeUserGroupScopeId) = await ResolveActiveScopeAsync(
                loginRequest, isAdmin, rawScopes);

            var availableScopes = rawScopes
                .GroupBy(s => new { s.CompanyId, s.CompanyOfficeId })
                .Select(g => new LoginScopeDto
                {
                    CompanyId = g.Key.CompanyId,
                    CompanyName = g.First().CompanyName ?? string.Empty,
                    OfficeId = g.Key.CompanyOfficeId,
                    OfficeName = g.First().CompanyOfficeName
                })
                .OrderBy(s => s.CompanyName)
                .ToList();

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
            var rawScopes = isAdmin
                ? await BuildAdminScopesAsync()
                : (await _repository.UserGroupScope.GetAllByUserIdAsync(user.UserId)).ToList();

            if (!isAdmin)
            {
                var hasAccess = rawScopes.Any(s =>
                    s.CompanyId == request.CompanyId &&
                    (!request.OfficeId.HasValue ||
                     s.CompanyOfficeId == request.OfficeId.Value ||
                     s.CompanyOfficeId == null));

                if (!hasAccess)
                    throw new UnauthorizedAccessException("User does not have access to this company or office.");
            }

            long? activeCompanyId = request.CompanyId;
            Guid? activeCompanyGuid = null;
            string? activeCompanyName = null;
            long? activeOfficeId = request.OfficeId;
            Guid? activeOfficeGuid = null;
            string? activeOfficeName = null;
            long? activeUserGroupScopeId = null;

            if (isAdmin)
            {
                if (activeCompanyId.HasValue)
                {
                    var comp = await _repository.Company.GetCompanyByIdAsync(activeCompanyId.Value, false);
                    activeCompanyName = comp?.CompanyName;
                    activeCompanyGuid = comp?.CompanyGuid;
                }

                if (activeOfficeId.HasValue)
                {
                    var office = await _repository.CompanyOffice.GetCompanyOfficeByIdAsync(activeOfficeId.Value, false);
                    activeOfficeName = office?.CompanyOfficeName;
                    activeOfficeGuid = office?.CompanyOfficeGuid;
                }
            }
            else
            {
                var selectedScope = request.OfficeId.HasValue
                    ? rawScopes.FirstOrDefault(s =>
                        s.CompanyId == request.CompanyId &&
                        s.CompanyOfficeId == request.OfficeId.Value)
                      ?? rawScopes.FirstOrDefault(s =>
                        s.CompanyId == request.CompanyId &&
                        s.CompanyOfficeId == null)
                    : rawScopes.FirstOrDefault(s =>
                        s.CompanyId == request.CompanyId &&
                        s.CompanyOfficeId == null)
                      ?? rawScopes.FirstOrDefault(s => s.CompanyId == request.CompanyId);

                if (selectedScope != null)
                {
                    activeCompanyName = selectedScope.CompanyName;
                    activeOfficeName = selectedScope.CompanyOfficeName;
                    activeUserGroupScopeId = selectedScope.UserGroupScopeId;

                    var scopeComp = await _repository.Company.GetCompanyByIdAsync(selectedScope.CompanyId, false);
                    activeCompanyGuid = scopeComp?.CompanyGuid;

                    if (selectedScope.CompanyOfficeId.HasValue)
                    {
                        var scopeOffice = await _repository.CompanyOffice.GetCompanyOfficeByIdAsync(selectedScope.CompanyOfficeId.Value, false);
                        activeOfficeGuid = scopeOffice?.CompanyOfficeGuid;
                    }
                }
            }

            var availableScopes = rawScopes
                .GroupBy(s => new { s.CompanyId, s.CompanyOfficeId })
                .Select(g => new LoginScopeDto
                {
                    CompanyId = g.Key.CompanyId,
                    CompanyName = g.First().CompanyName ?? string.Empty,
                    OfficeId = g.Key.CompanyOfficeId,
                    OfficeName = g.First().CompanyOfficeName
                })
                .OrderBy(s => s.CompanyName)
                .ToList();

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

        private async Task<(long? CompanyId, Guid? CompanyGuid, string? CompanyName,
            long? OfficeId, Guid? OfficeGuid, string? OfficeName, long? UserGroupScopeId)> ResolveActiveScopeAsync(
            LoginRequestDto loginRequest,
            bool isAdmin,
            List<Entities.Models.UserGroupScope> rawScopes)
        {
            long? activeCompanyId = null;
            Guid? activeCompanyGuid = null;
            string? activeCompanyName = null;
            long? activeOfficeId = null;
            Guid? activeOfficeGuid = null;
            string? activeOfficeName = null;
            long? activeUserGroupScopeId = null;

            if (isAdmin)
            {
                activeCompanyId = loginRequest.CompanyId;
                activeOfficeId = loginRequest.OfficeId;

                if (!activeCompanyId.HasValue)
                {
                    if (rawScopes.Any())
                    {
                        var defaultScope = rawScopes.FirstOrDefault(s => s.IsDefault) ?? rawScopes.First();
                        activeCompanyId = defaultScope.CompanyId;
                        activeOfficeId = defaultScope.CompanyOfficeId;
                    }
                    else
                    {
                        var firstComp = (await _repository.Company.GetAllCompaniesAsync(false))
                            .OrderBy(c => c.CompanyName)
                            .FirstOrDefault();
                        if (firstComp != null)
                        {
                            activeCompanyId = firstComp.CompanyId;
                            var firstOffice = (await _repository.CompanyOffice.GetAllCompanyOfficesAsync(false))
                                .Where(o => o.CompanyId == firstComp.CompanyId)
                                .OrderBy(o => o.CompanyOfficeName)
                                .FirstOrDefault();
                            activeOfficeId = firstOffice?.CompanyOfficeId;
                        }
                    }
                }

                if (activeCompanyId.HasValue)
                {
                    var comp = await _repository.Company.GetCompanyByIdAsync(activeCompanyId.Value, false);
                    activeCompanyName = comp?.CompanyName;
                    activeCompanyGuid = comp?.CompanyGuid;
                }

                if (activeOfficeId.HasValue)
                {
                    var office = await _repository.CompanyOffice.GetCompanyOfficeByIdAsync(activeOfficeId.Value, false);
                    activeOfficeName = office?.CompanyOfficeName;
                    activeOfficeGuid = office?.CompanyOfficeGuid;
                }
            }
            else if (rawScopes.Count > 0)
            {
                Entities.Models.UserGroupScope? selectedScope = null;

                if (loginRequest.CompanyId.HasValue)
                {
                    selectedScope = loginRequest.OfficeId.HasValue
                        ? rawScopes.FirstOrDefault(s =>
                            s.CompanyId == loginRequest.CompanyId.Value &&
                            s.CompanyOfficeId == loginRequest.OfficeId.Value)
                          ?? rawScopes.FirstOrDefault(s =>
                            s.CompanyId == loginRequest.CompanyId.Value &&
                            s.CompanyOfficeId == null)
                        : rawScopes.FirstOrDefault(s =>
                            s.CompanyId == loginRequest.CompanyId.Value &&
                            s.CompanyOfficeId == null)
                          ?? rawScopes.FirstOrDefault(s => s.CompanyId == loginRequest.CompanyId.Value);
                }

                if (selectedScope == null)
                {
                    var uniqueCompanies = rawScopes.Select(s => s.CompanyId).Distinct().ToList();
                    if (uniqueCompanies.Count == 1)
                    {
                        selectedScope = rawScopes.FirstOrDefault(s => s.IsDefault)
                                        ?? rawScopes.First(s => s.CompanyId == uniqueCompanies[0]);
                    }
                    else
                    {
                        selectedScope = rawScopes.FirstOrDefault(s => s.IsDefault);
                        if (selectedScope == null)
                        {
                            throw new UnauthorizedAccessException(
                                "No default company is configured for your account. " +
                                "Please select a company and office, or contact your administrator.");
                        }
                    }
                }

                activeCompanyId = selectedScope.CompanyId;
                activeCompanyName = selectedScope.CompanyName;
                activeOfficeId = selectedScope.CompanyOfficeId;
                activeOfficeName = selectedScope.CompanyOfficeName;
                activeUserGroupScopeId = selectedScope.UserGroupScopeId;

                var scopeComp = await _repository.Company.GetCompanyByIdAsync(selectedScope.CompanyId, false);
                activeCompanyGuid = scopeComp?.CompanyGuid;

                if (selectedScope.CompanyOfficeId.HasValue)
                {
                    var scopeOffice = await _repository.CompanyOffice.GetCompanyOfficeByIdAsync(selectedScope.CompanyOfficeId.Value, false);
                    activeOfficeGuid = scopeOffice?.CompanyOfficeGuid;
                }
            }

            return (activeCompanyId, activeCompanyGuid, activeCompanyName,
                activeOfficeId, activeOfficeGuid, activeOfficeName, activeUserGroupScopeId);
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
