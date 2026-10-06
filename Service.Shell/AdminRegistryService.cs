using Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.AdminRegistry;

namespace Service.Shell;

public sealed class AdminRegistryService : IAdminRegistryService
{
    private readonly AdminRegistrySettings _settings;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<AdminRegistryService> _logger;
    private readonly object _sync = new();
    private HashSet<long> _adminUserIds = [];
    private HashSet<string> _adminUserNames = new(StringComparer.OrdinalIgnoreCase);
    private List<AdminRegistryEntry> _entries = [];

    public AdminRegistryService(
        IOptions<AdminRegistrySettings> settings,
        IHostEnvironment environment,
        ILogger<AdminRegistryService> logger)
    {
        _settings = settings.Value;
        _environment = environment;
        _logger = logger;
        Reload();
    }

    public void Reload()
    {
        lock (_sync)
        {
            var absolutePath = ResolveAbsolutePath(_settings.FilePath);
            if (!File.Exists(absolutePath))
            {
                _logger.LogWarning("Admin registry file not found at {Path}. No administrators loaded.", absolutePath);
                _adminUserIds = [];
                _adminUserNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _entries = [];
                return;
            }

            try
            {
                var encrypted = File.ReadAllBytes(absolutePath);
                var document = AdminRegistryCrypto.Decrypt(encrypted, _settings.EncryptionKey);
                _entries = document.Administrators.ToList();
                _adminUserIds = _entries.Select(e => e.UserId).ToHashSet();
                _adminUserNames = _entries.Select(e => e.UserName).ToHashSet(StringComparer.OrdinalIgnoreCase);
                _logger.LogInformation(
                    "Admin registry loaded: {Count} administrator(s) from {Path}.",
                    _entries.Count,
                    absolutePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to decrypt admin registry at {Path}. Check AdminRegistrySettings:EncryptionKey matches the key used when generating the file.",
                    absolutePath);
                _adminUserIds = [];
                _adminUserNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _entries = [];
            }
        }
    }

    public bool IsAdministrator(long userId)
    {
        lock (_sync)
            return _adminUserIds.Contains(userId);
    }

    public bool IsAdministrator(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return false;

        lock (_sync)
            return _adminUserNames.Contains(userName);
    }

    public IReadOnlyList<AdminRegistryEntry> GetAdministrators()
    {
        lock (_sync)
            return _entries.ToList();
    }

    public void EnsureAdministrator(long userId, string userName)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName is required.", nameof(userName));

        lock (_sync)
        {
            if (_adminUserIds.Contains(userId))
                return;

            var entries = _entries
                .Where(e => e.UserId != userId)
                .Select(Clone)
                .ToList();

            entries.Insert(0, new AdminRegistryEntry
            {
                UserId = userId,
                UserName = userName.Trim(),
                GrantedById = userId,
                GrantedAt = DateTime.UtcNow
            });

            while (entries.Count > AdminRegistryLimits.MaximumAdministrators)
                entries.RemoveAt(entries.Count - 1);

            var placeholderId = NextPlaceholderUserId(entries, userId);
            while (entries.Count < AdminRegistryLimits.MinimumAdministrators)
            {
                entries.Add(new AdminRegistryEntry
                {
                    UserId = placeholderId,
                    UserName = "ops",
                    GrantedById = userId,
                    GrantedAt = DateTime.UtcNow
                });
                placeholderId++;
            }

            PersistUnlocked(new AdminRegistryDocument
            {
                Version = AdminRegistryDocument.CurrentVersion,
                GeneratedAt = DateTime.UtcNow,
                Administrators = entries
            });
        }
    }

    private void PersistUnlocked(AdminRegistryDocument document)
    {
        AdminRegistryLimits.Validate(document.Administrators);

        var absolutePath = ResolveAbsolutePath(_settings.FilePath);
        var directory = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var encrypted = AdminRegistryCrypto.Encrypt(document, _settings.EncryptionKey);
        File.WriteAllBytes(absolutePath, encrypted);

        _entries = document.Administrators.ToList();
        _adminUserIds = _entries.Select(e => e.UserId).ToHashSet();
        _adminUserNames = _entries.Select(e => e.UserName).ToHashSet(StringComparer.OrdinalIgnoreCase);

        _logger.LogInformation(
            "Admin registry updated: {Count} administrator(s) at {Path}.",
            _entries.Count,
            absolutePath);
    }

    private static AdminRegistryEntry Clone(AdminRegistryEntry entry) => new()
    {
        UserId = entry.UserId,
        UserName = entry.UserName,
        GrantedById = entry.GrantedById,
        GrantedAt = entry.GrantedAt
    };

    private static long NextPlaceholderUserId(IReadOnlyList<AdminRegistryEntry> entries, long adminUserId)
    {
        var used = entries.Select(e => e.UserId).ToHashSet();
        used.Add(adminUserId);
        var candidate = adminUserId == 1 ? 2 : 1;
        while (used.Contains(candidate))
            candidate++;
        return candidate;
    }

    private string ResolveAbsolutePath(string configuredPath)
    {
        if (Path.IsPathRooted(configuredPath))
            return configuredPath;

        return Path.GetFullPath(Path.Combine(_environment.ContentRootPath, configuredPath));
    }
}
