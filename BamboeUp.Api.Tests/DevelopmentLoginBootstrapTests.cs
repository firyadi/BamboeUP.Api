using Contracts;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Service.Contracts.Shell;
using Service.Shell;
using Shared.AdminRegistry;
using Shared.DataTransferObjects;

namespace BamboeUp.Api.Tests;

public class DevelopmentLoginBootstrapTests
{
    [Fact]
    public async Task EnsureReadyAsync_CreatesAdminUser_WhenUsersIsEmpty()
    {
        var userRepo = new Mock<IUserRepository>();
        var registry = new Mock<IAdminRegistryService>();
        var auth = new Mock<IAuthenticationService>();
        auth.Setup(a => a.HashPassword("admin123")).Returns(("$2hashed", "salt"));

        User? stored = null;
        userRepo.Setup(u => u.GetUserByUserNameAsync("admin", false))
            .Returns(() => Task.FromResult(stored));
        userRepo.Setup(u => u.CreateUserAsync(It.IsAny<User>(), It.IsAny<System.Data.IDbTransaction?>()))
            .Callback<User, System.Data.IDbTransaction?>((user, _) =>
            {
                user.UserId = 1;
                stored = user;
            })
            .Returns(Task.CompletedTask);

        var bootstrap = new DevelopmentLoginBootstrap(
            Repository(userRepo.Object),
            auth.Object,
            registry.Object,
            NullLogger<DevelopmentLoginBootstrap>.Instance);

        await bootstrap.EnsureReadyAsync();

        userRepo.Verify(u => u.CreateUserAsync(It.Is<User>(x => x.UserName == "admin" && x.StatusId == 1), It.IsAny<System.Data.IDbTransaction?>()), Times.Once);
        registry.Verify(r => r.EnsureAdministrator(1, "admin"), Times.Once);
    }

    [Fact]
    public async Task EnsureReadyAsync_DoesNotRecreateUser_WhenAdminExists()
    {
        var userRepo = new Mock<IUserRepository>();
        var registry = new Mock<IAdminRegistryService>();
        var auth = new Mock<IAuthenticationService>();

        userRepo.Setup(u => u.GetUserByUserNameAsync("admin", false))
            .ReturnsAsync(new User { UserId = 7, UserName = "admin" });

        var bootstrap = new DevelopmentLoginBootstrap(
            Repository(userRepo.Object),
            auth.Object,
            registry.Object,
            NullLogger<DevelopmentLoginBootstrap>.Instance);

        await bootstrap.EnsureReadyAsync();

        userRepo.Verify(u => u.CreateUserAsync(It.IsAny<User>(), It.IsAny<System.Data.IDbTransaction?>()), Times.Never);
        auth.Verify(a => a.HashPassword(It.IsAny<string>()), Times.Never);
        registry.Verify(r => r.EnsureAdministrator(7, "admin"), Times.Once);
    }

    private static IRepositoryManager Repository(IUserRepository users)
    {
        var repo = new Mock<IRepositoryManager>();
        repo.Setup(r => r.User).Returns(users);
        return repo.Object;
    }
}

public class AdminRegistryServiceTests
{
    private const string Key = "dev_admin_registry_key_change_in_production_32";

    [Fact]
    public void EnsureAdministrator_WritesBootstrapFile_WhenMissing()
    {
        var root = Path.Combine(Path.GetTempPath(), "admin-reg-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var service = CreateService(root);
            service.EnsureAdministrator(1, "admin");

            Assert.True(service.IsAdministrator(1));
            Assert.True(service.IsAdministrator("admin"));
            Assert.Equal(2, service.GetAdministrators().Count);
            Assert.True(File.Exists(Path.Combine(root, "App_Data", "admin.registry.enc")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void EnsureAdministrator_IsIdempotent_WhenUserAlreadyListed()
    {
        var root = Path.Combine(Path.GetTempPath(), "admin-reg-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var service = CreateService(root);
            service.EnsureAdministrator(1, "admin");
            var firstWrite = File.ReadAllBytes(Path.Combine(root, "App_Data", "admin.registry.enc"));

            service.EnsureAdministrator(1, "admin");
            var secondWrite = File.ReadAllBytes(Path.Combine(root, "App_Data", "admin.registry.enc"));

            Assert.Equal(firstWrite, secondWrite);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static AdminRegistryService CreateService(string contentRoot)
    {
        var env = new Mock<Microsoft.Extensions.Hosting.IHostEnvironment>();
        env.Setup(e => e.ContentRootPath).Returns(contentRoot);
        var options = Options.Create(new AdminRegistrySettings
        {
            FilePath = "App_Data/admin.registry.enc",
            EncryptionKey = Key
        });
        return new AdminRegistryService(options, env.Object, NullLogger<AdminRegistryService>.Instance);
    }
}

public class AuthenticationServiceAdminScopeTests
{
    [Fact]
    public async Task LoginAsync_AdminWithoutUserGroupScope_UsesFirstCompany()
    {
        var user = new User
        {
            UserId = 1,
            UserGuid = Guid.NewGuid(),
            UserName = "admin",
            PasswordHash = "admin123",
            PasswordSalt = "",
            FullName = "Administrator",
            StatusId = 1
        };
        var company = new Company
        {
            CompanyId = 42,
            CompanyGuid = Guid.NewGuid(),
            CompanyName = "Development Company",
            StatusId = 1
        };

        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(u => u.GetUserByUserNameAsync("admin", false)).ReturnsAsync(user);
        userRepo.Setup(u => u.UpgradePasswordHashAsync(1, It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(Task.CompletedTask);

        var companyRepo = new Mock<ICompanyRepository>();
        companyRepo.Setup(c => c.GetAllCompaniesAsync(false)).ReturnsAsync([company]);
        companyRepo.Setup(c => c.GetCompanyByIdAsync(42, false)).ReturnsAsync(company);

        var officeRepo = new Mock<ICompanyOfficeRepository>();
        officeRepo.Setup(o => o.GetAllCompanyOfficesAsync(false)).ReturnsAsync([]);

        var scopeRepo = new Mock<IUserGroupScopeRepository>();
        scopeRepo.Setup(s => s.GetAllByUserIdAsync(1)).ReturnsAsync([]);

        var repo = new Mock<IRepositoryManager>();
        repo.Setup(r => r.User).Returns(userRepo.Object);
        repo.Setup(r => r.Company).Returns(companyRepo.Object);
        repo.Setup(r => r.CompanyOffice).Returns(officeRepo.Object);
        repo.Setup(r => r.UserGroupScope).Returns(scopeRepo.Object);

        var registry = new Mock<IAdminRegistryService>();
        registry.Setup(r => r.IsAdministrator(1)).Returns(true);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:ValidIssuer"] = "BamboeUp.Api",
                ["JwtSettings:ValidAudience"] = "BamboeUp.App",
                ["JwtSettings:Expires"] = "60",
                ["JwtSettings:SecretKey"] = "this_is_a_long_secret_key_1234567890"
            })
            .Build();

        var service = new AuthenticationService(repo.Object, config, registry.Object);
        var result = await service.LoginAsync(new LoginRequestDto { Username = "admin", Password = "admin123" });

        Assert.True(result.IsAdmin);
        Assert.Equal("Admin", result.Role);
        Assert.Equal(42, result.ActiveCompanyId);
        Assert.Equal("Development Company", result.ActiveCompanyName);
        Assert.Contains(result.AvailableScopes, s => s.CompanyId == 42);
    }

    [Fact]
    public async Task LoginAsync_AdminWithoutCompany_SucceedsWithEmptyScope()
    {
        var user = new User
        {
            UserId = 1,
            UserGuid = Guid.NewGuid(),
            UserName = "admin",
            PasswordHash = "admin123",
            PasswordSalt = "",
            FullName = "Administrator",
            StatusId = 1
        };

        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(u => u.GetUserByUserNameAsync("admin", false)).ReturnsAsync(user);
        userRepo.Setup(u => u.UpgradePasswordHashAsync(1, It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(Task.CompletedTask);

        var companyRepo = new Mock<ICompanyRepository>();
        companyRepo.Setup(c => c.GetAllCompaniesAsync(false)).ReturnsAsync([]);

        var officeRepo = new Mock<ICompanyOfficeRepository>();
        officeRepo.Setup(o => o.GetAllCompanyOfficesAsync(false)).ReturnsAsync([]);

        var scopeRepo = new Mock<IUserGroupScopeRepository>();
        scopeRepo.Setup(s => s.GetAllByUserIdAsync(1)).ReturnsAsync([]);

        var repo = new Mock<IRepositoryManager>();
        repo.Setup(r => r.User).Returns(userRepo.Object);
        repo.Setup(r => r.Company).Returns(companyRepo.Object);
        repo.Setup(r => r.CompanyOffice).Returns(officeRepo.Object);
        repo.Setup(r => r.UserGroupScope).Returns(scopeRepo.Object);

        var registry = new Mock<IAdminRegistryService>();
        registry.Setup(r => r.IsAdministrator(1)).Returns(true);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:ValidIssuer"] = "BamboeUp.Api",
                ["JwtSettings:ValidAudience"] = "BamboeUp.App",
                ["JwtSettings:Expires"] = "60",
                ["JwtSettings:SecretKey"] = "this_is_a_long_secret_key_1234567890"
            })
            .Build();

        var service = new AuthenticationService(repo.Object, config, registry.Object);
        var result = await service.LoginAsync(new LoginRequestDto { Username = "admin", Password = "admin123" });

        Assert.True(result.IsAdmin);
        Assert.Null(result.ActiveCompanyId);
        Assert.Empty(result.AvailableScopes);
    }
}
