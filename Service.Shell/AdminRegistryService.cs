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

    private string ResolveAbsolutePath(string configuredPath)
    {
        if (Path.IsPathRooted(configuredPath))
            return configuredPath;

        return Path.GetFullPath(Path.Combine(_environment.ContentRootPath, configuredPath));
    }
}
