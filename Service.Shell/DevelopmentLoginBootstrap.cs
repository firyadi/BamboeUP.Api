using Contracts;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Service.Contracts.Shell;

namespace Service.Shell;

public sealed class DevelopmentLoginBootstrap : IDevelopmentLoginBootstrap
{
    private readonly IRepositoryManager _repository;
    private readonly IAuthenticationService _authentication;
    private readonly IAdminRegistryService _adminRegistry;
    private readonly ILogger<DevelopmentLoginBootstrap> _logger;

    public DevelopmentLoginBootstrap(
        IRepositoryManager repository,
        IAuthenticationService authentication,
        IAdminRegistryService adminRegistry,
        ILogger<DevelopmentLoginBootstrap> logger)
    {
        _repository = repository;
        _authentication = authentication;
        _adminRegistry = adminRegistry;
        _logger = logger;
    }

    public async Task EnsureReadyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await EnsureAdminUserAsync();
        _adminRegistry.EnsureAdministrator(user.UserId, user.UserName);

        _logger.LogInformation(
            "Development login bootstrap ready for {UserName} (UserId {UserId}).",
            user.UserName,
            user.UserId);
    }

    private async Task<User> EnsureAdminUserAsync()
    {
        var existing = await _repository.User.GetUserByUserNameAsync(IDevelopmentLoginBootstrap.UserName, false);
        if (existing != null)
            return existing;

        var (hash, salt) = _authentication.HashPassword(IDevelopmentLoginBootstrap.Password);
        var user = new User
        {
            UserGuid = Guid.NewGuid(),
            UserName = IDevelopmentLoginBootstrap.UserName,
            PasswordHash = hash,
            PasswordSalt = salt,
            FullName = IDevelopmentLoginBootstrap.FullName,
            Email = "admin@localhost",
            IsAdmin = true,
            StatusId = 1,
            CreatedById = 0,
            CreatedTime = DateTime.UtcNow
        };

        await _repository.User.CreateUserAsync(user);

        var created = await _repository.User.GetUserByUserNameAsync(IDevelopmentLoginBootstrap.UserName, false);
        if (created == null)
            throw new InvalidOperationException("Failed to create the development admin user.");

        _logger.LogWarning(
            "Created development admin user {UserName} (UserId {UserId}).",
            created.UserName,
            created.UserId);

        return created;
    }
}
