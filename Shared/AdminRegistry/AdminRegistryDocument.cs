namespace Shared.AdminRegistry;

public sealed class AdminRegistryDocument
{
    public const int CurrentVersion = 1;

    public int Version { get; set; } = CurrentVersion;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public List<AdminRegistryEntry> Administrators { get; set; } = [];
}
