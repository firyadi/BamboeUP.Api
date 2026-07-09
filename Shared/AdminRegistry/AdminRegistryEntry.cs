namespace Shared.AdminRegistry;

public sealed class AdminRegistryEntry
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public long GrantedById { get; set; }
    public DateTime GrantedAt { get; set; }
}
