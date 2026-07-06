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

        public AuthenticationService(IRepositoryManager repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<TokenDto> LoginAsync(LoginRequestDto loginRequest)
        {
            Entities.Models.User? user = null;
            bool isHardcodedAdmin = false;

            // --- Hardcoded AdminRoy ---
            if (loginRequest.Username == "AdminRoy" && loginRequest.Password == "roy123")
            {
                user = new Entities.Models.User
                {
                    UserId = 999999, // Dummy ID
                    UserGuid = Guid.NewGuid(),
                    UserName = "AdminRoy",
                    FullName = "Super Admin Roy",
                    IsAdmin = true
                };
                isHardcodedAdmin = true;
            }
            else
            {
                // 1. Cari user by username — query menggunakan parameterized (aman dari SQLi)
                user = await _repository.User.GetUserByUserNameAsync(loginRequest.Username, false);
                if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash, user.PasswordSalt))
                {
                    // Pesan generik — tidak membedakan "user not found" vs "wrong password"
                    throw new UnauthorizedAccessException("Invalid username or password.");
                }

                // 2. Auto-upgrade: jika password masih plaintext atau belum punya salt, upgrade sekarang
                if (!user.PasswordHash.StartsWith("$2") || string.IsNullOrEmpty(user.PasswordSalt))
                {
                    var (upgradedHash, upgradedSalt) = HashPassword(loginRequest.Password);
                    await UpgradePasswordHashAsync(user.UserId, upgradedHash, upgradedSalt);
                    user.PasswordHash = upgradedHash; // update in-memory
                    user.PasswordSalt = upgradedSalt;
                }
            }

            // 3. Ambil semua scope user (sudah include CompanyName & CompanyOfficeName via JOIN)
            List<Entities.Models.UserGroupScope> rawScopes = new();

            if (isHardcodedAdmin)
            {
                var allCompanies = (await _repository.Company.GetAllCompaniesAsync(false)).OrderBy(c => c.CompanyName).ToList();
                var allOffices = (await _repository.CompanyOffice.GetAllCompanyOfficesAsync(false)).ToList();

                long defaultCompanyId = allCompanies.FirstOrDefault()?.CompanyId ?? 0;

                foreach (var c in allCompanies)
                {
                    // Tambahkan scope all office (null)
                    rawScopes.Add(new Entities.Models.UserGroupScope
                    {
                        UserGroupScopeId = c.CompanyId * 1000, // Dummy ID
                        CompanyId = c.CompanyId,
                        CompanyName = c.CompanyName,
                        CompanyOfficeId = null,
                        IsDefault = c.CompanyId == defaultCompanyId
                    });

                    var compOffices = allOffices.Where(o => o.CompanyId == c.CompanyId).OrderBy(o => o.CompanyOfficeName);
                    long officeIdx = 1;
                    foreach (var o in compOffices)
                    {
                        rawScopes.Add(new Entities.Models.UserGroupScope
                        {
                            UserGroupScopeId = (c.CompanyId * 1000) + officeIdx++, // Dummy ID
                            CompanyId = c.CompanyId,
                            CompanyName = c.CompanyName,
                            CompanyOfficeId = o.CompanyOfficeId,
                            CompanyOfficeName = o.CompanyOfficeName
                        });
                    }
                }
            }
            else
            {
                rawScopes = (await _repository.UserGroupScope.GetAllByUserIdAsync(user.UserId)).ToList();
            }

            // 4. Validasi scope jika user meminta company/office tertentu
            if (loginRequest.CompanyId.HasValue && !user.IsAdmin)
            {
                var hasAccess = rawScopes.Any(s =>
                    s.CompanyId == loginRequest.CompanyId.Value &&
                    (!loginRequest.OfficeId.HasValue ||
                     s.CompanyOfficeId == loginRequest.OfficeId.Value ||
                     s.CompanyOfficeId == null));

                if (!hasAccess)
                    throw new UnauthorizedAccessException("User does not have access to this company or office.");
            }

            // 5. Tentukan active scope
            long? activeCompanyId = null;
            Guid? activeCompanyGuid = null;
            string? activeCompanyName = null;
            long? activeOfficeId = null;
            Guid? activeOfficeGuid = null;
            string? activeOfficeName = null;
            long? activeUserGroupScopeId = null;

            if (user.IsAdmin)
            {
                // Admin: gunakan company/office yang diminta, atau jika tidak diminta, coba fallback ke default/first scope dari rawScopes
                activeCompanyId = loginRequest.CompanyId;
                activeOfficeId = loginRequest.OfficeId;

                if (!activeCompanyId.HasValue)
                {
                    if (rawScopes.Any())
                    {
                        var defaultScope = rawScopes.FirstOrDefault(s => s.IsDefault) ?? rawScopes.FirstOrDefault();
                        if (defaultScope != null)
                        {
                            activeCompanyId = defaultScope.CompanyId;
                            activeOfficeId = defaultScope.CompanyOfficeId;
                        }
                    }
                    else
                    {
                        // Fallback ke company pertama di system jika rawScopes kosong
                        var allComp = await _repository.Company.GetAllCompaniesAsync(false);
                        var firstComp = allComp.OrderBy(c => c.CompanyName).FirstOrDefault();
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

                // Admin name & guid fetch: ambil langsung dari repository
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
                    // Cari scope yang cocok dengan company (dan office jika diminta)
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
                          ?? rawScopes.FirstOrDefault(s =>
                            s.CompanyId == loginRequest.CompanyId.Value);
                }

                // Fallback ke default atau single company jika tidak ada yang dipilih di request
                if (selectedScope == null)
                {
                    var uniqueCompanies = rawScopes.Select(s => s.CompanyId).Distinct().ToList();
                    if (uniqueCompanies.Count == 1)
                    {
                        // Hanya punya 1 company akses, gunakan yang default jika ada, atau fallback ke scope pertama.
                        selectedScope = rawScopes.FirstOrDefault(s => s.IsDefault) 
                                        ?? rawScopes.FirstOrDefault(s => s.CompanyId == uniqueCompanies[0]);
                    }
                    else if (uniqueCompanies.Count > 1)
                    {
                        // Lebih dari 1 company akses, pilih yang default
                        selectedScope = rawScopes.FirstOrDefault(s => s.IsDefault);
                        if (selectedScope == null)
                        {
                            throw new UnauthorizedAccessException(
                                "No default company is configured for your account. " +
                                "Please select a company and office, or contact your administrator.");
                        }
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("User does not have access to any company.");
                    }
                }

                activeCompanyId = selectedScope.CompanyId;
                activeCompanyName = selectedScope.CompanyName;
                activeOfficeId = selectedScope.CompanyOfficeId;
                activeOfficeName = selectedScope.CompanyOfficeName;

                // [Gap 2 Fix] Simpan UserGroupScopeId agar bisa dimasukkan ke JWT claim
                activeUserGroupScopeId = selectedScope.UserGroupScopeId;

                // Fetch GUIDs dari repository (UserGroupScope tidak menyimpan GUID)
                var scopeComp = await _repository.Company.GetCompanyByIdAsync(selectedScope.CompanyId, false);
                activeCompanyGuid = scopeComp?.CompanyGuid;

                if (selectedScope.CompanyOfficeId.HasValue)
                {
                    var scopeOffice = await _repository.CompanyOffice.GetCompanyOfficeByIdAsync(selectedScope.CompanyOfficeId.Value, false);
                    activeOfficeGuid = scopeOffice?.CompanyOfficeGuid;
                }
            }

            // 6. Build available scopes untuk frontend dropdown (distinct per company+office)
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

            // 7. Generate JWT
            var token = GenerateToken(user, activeCompanyId, activeOfficeId, activeUserGroupScopeId, out DateTime expired);

            return new TokenDto
            {
                Token = token,
                Expired = expired,
                UserId = user.UserId,
                UserGuid = user.UserGuid,
                UserName = user.UserName,
                FullName = user.FullName ?? user.UserName,
                Role = user.IsAdmin ? "Admin" : "User",
                IsAdmin = user.IsAdmin,
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
            Entities.Models.User? user = null;
            bool isHardcodedAdmin = false;

            if (userName == "AdminRoy")
            {
                user = new Entities.Models.User
                {
                    UserId = 999999, // Dummy ID
                    UserGuid = Guid.NewGuid(),
                    UserName = "AdminRoy",
                    FullName = "Super Admin Roy",
                    IsAdmin = true
                };
                isHardcodedAdmin = true;
            }
            else
            {
                user = await _repository.User.GetUserByUserNameAsync(userName, false);
                if (user == null)
                    throw new UnauthorizedAccessException("User not found.");
            }

            List<Entities.Models.UserGroupScope> rawScopes = new();

            if (isHardcodedAdmin)
            {
                var allCompanies = (await _repository.Company.GetAllCompaniesAsync(false)).OrderBy(c => c.CompanyName).ToList();
                var allOffices = (await _repository.CompanyOffice.GetAllCompanyOfficesAsync(false)).ToList();

                long defaultCompanyId = allCompanies.FirstOrDefault()?.CompanyId ?? 0;

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

                    var compOffices = allOffices.Where(o => o.CompanyId == c.CompanyId).OrderBy(o => o.CompanyOfficeName);
                    long officeIdx = 1;
                    foreach (var o in compOffices)
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
            }
            else
            {
                rawScopes = (await _repository.UserGroupScope.GetAllByUserIdAsync(user.UserId)).ToList();
            }

            if (!user.IsAdmin)
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

            if (user.IsAdmin)
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
                Entities.Models.UserGroupScope? selectedScope = null;
                selectedScope = request.OfficeId.HasValue
                    ? rawScopes.FirstOrDefault(s =>
                        s.CompanyId == request.CompanyId &&
                        s.CompanyOfficeId == request.OfficeId.Value)
                        ?? rawScopes.FirstOrDefault(s =>
                        s.CompanyId == request.CompanyId &&
                        s.CompanyOfficeId == null)
                    : rawScopes.FirstOrDefault(s =>
                        s.CompanyId == request.CompanyId &&
                        s.CompanyOfficeId == null)
                        ?? rawScopes.FirstOrDefault(s =>
                        s.CompanyId == request.CompanyId);

                if (selectedScope != null)
                {
                    activeCompanyName = selectedScope.CompanyName;
                    activeOfficeName = selectedScope.CompanyOfficeName;

                    // [Gap 2 Fix] Ambil UserGroupScopeId dari scope yang dipilih
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

            var token = GenerateToken(user, activeCompanyId, activeOfficeId, activeUserGroupScopeId, out DateTime expired);

            return new TokenDto
            {
                Token = token,
                Expired = expired,
                UserId = user.UserId,
                UserGuid = user.UserGuid,
                UserName = user.UserName,
                FullName = user.FullName ?? user.UserName,
                Role = user.IsAdmin ? "Admin" : "User",
                IsAdmin = user.IsAdmin,
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

        public (string Hash, string Salt) HashPassword(string password)
        {
            // Generate 32-byte cryptographic random salt
            var saltBytes = RandomNumberGenerator.GetBytes(32);
            var salt = Convert.ToBase64String(saltBytes);

            // Gabungkan salt + password sebelum di-hash BCrypt
            var saltedPassword = salt + password;
            var hash = BCrypt.Net.BCrypt.HashPassword(saltedPassword, workFactor: 12);

            return (hash, salt);
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            // Fallback untuk password lama yang belum di-hash (migration period)
            if (!hash.StartsWith("$2") && password == hash)
                return true;

            try
            {
                // Jika salt kosong (data lama), verifikasi tanpa salt
                if (string.IsNullOrEmpty(salt))
                    return BCrypt.Net.BCrypt.Verify(password, hash);

                // Verifikasi dengan salt: salt + password
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

        private string GenerateToken(Entities.Models.User user, long? companyId, long? officeId, long? userGroupScopeId, out DateTime expired)
        {
            var jwtConfig = _configuration.GetSection("JwtSettings").Get<JwtConfiguration>()
                ?? throw new InvalidOperationException("JWT configuration tidak ditemukan.");

            var secretKey = jwtConfig.SecretKey;
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
                throw new InvalidOperationException("JWT SecretKey tidak valid atau terlalu pendek (min 32 karakter).");

            expired = DateTime.UtcNow.AddMinutes(jwtConfig.Expires);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("UserGuid", user.UserGuid.ToString())
            };

            if (companyId.HasValue)
                claims.Add(new Claim("CompanyId", companyId.Value.ToString()));

            if (officeId.HasValue)
                claims.Add(new Claim("OfficeId", officeId.Value.ToString()));

            // [Gap 2 Fix] Tambah UserGroupScopeId ke JWT agar NavMenu bisa match scope secara eksplisit
            if (userGroupScopeId.HasValue)
                claims.Add(new Claim("UserGroupScopeId", userGroupScopeId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtConfig.ValidIssuer,
                audience: jwtConfig.ValidAudience,
                claims: claims,
                expires: expired,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
