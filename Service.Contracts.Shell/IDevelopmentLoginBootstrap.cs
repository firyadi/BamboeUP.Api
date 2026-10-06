namespace Service.Contracts.Shell;

/// <summary>
/// Development-only seed so DEV BYPASS can log in on an empty database.
/// </summary>
public interface IDevelopmentLoginBootstrap
{
    const string UserName = "admin";
    const string Password = "admin123";
    const string FullName = "Administrator";

    Task EnsureReadyAsync(CancellationToken cancellationToken = default);
}
