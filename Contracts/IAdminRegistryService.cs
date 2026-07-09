using Shared.AdminRegistry;

namespace Contracts;

public interface IAdminRegistryService
{
    /// <summary>Loads or reloads the encrypted registry from disk.</summary>
    void Reload();

    bool IsAdministrator(long userId);
    bool IsAdministrator(string userName);
    IReadOnlyList<AdminRegistryEntry> GetAdministrators();
}
