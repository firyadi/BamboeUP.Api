using Shared.AdminRegistry;

namespace Contracts;

public interface IAdminRegistryService
{
    /// <summary>Loads or reloads the encrypted registry from disk.</summary>
    void Reload();

    bool IsAdministrator(long userId);
    bool IsAdministrator(string userName);
    IReadOnlyList<AdminRegistryEntry> GetAdministrators();

    /// <summary>
    /// Ensures <paramref name="userId"/> is listed in admin.registry.enc.
    /// Writes a bootstrap registry when the file is missing.
    /// </summary>
    void EnsureAdministrator(long userId, string userName);
}
