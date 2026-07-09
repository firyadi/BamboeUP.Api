namespace Shared.AdminRegistry;

public sealed class AdminRegistrySettings
{
    public const string SectionName = "AdminRegistrySettings";

    /// <summary>Relative path from API content root, e.g. App_Data/admin.registry.enc</summary>
    public string FilePath { get; set; } = "App_Data/admin.registry.enc";

    /// <summary>AES-256 key material (min 32 characters recommended). Prefer environment variable.</summary>
    public string EncryptionKey { get; set; } = string.Empty;
}
