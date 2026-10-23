using Contracts;
using Entities.Models;
using Moq;
using Service.Shell;
using Xunit;

namespace BamboeUp.Api.Tests.Services;

public sealed class UserMenuVisibilityTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Allowed_menus_exclude_inactive_and_deleted_programs_for_administrators_and_regular_users(bool administrator)
    {
        var userGuid = Guid.NewGuid();
        var repository = new Mock<IRepositoryManager>();
        var registry = new Mock<IAdminRegistryService>();
        repository.Setup(r => r.User.GetUserAsync(userGuid, false)).ReturnsAsync(new User { UserId = 7, UserGuid = userGuid });
        registry.Setup(r => r.IsAdministrator(7)).Returns(administrator);
        var programs = new[]
        {
            new Programs { ProgramId = 1, ProgramCode = "10.01", ProgramName = "Active group", ProgramType = "MNU", IsActive = true, StatusId = 1 },
            new Programs { ProgramId = 2, ProgramCode = "10.01.01", ProgramName = "Active program", ProgramType = "PRG", IsActive = true, StatusId = 1 },
            new Programs { ProgramId = 3, ProgramCode = "10.01.03", ProgramName = "Inactive one", ProgramType = "PRG", IsActive = false, StatusId = 1 },
            new Programs { ProgramId = 4, ProgramCode = "10.01.04", ProgramName = "Inactive two", ProgramType = "PRG", IsActive = false, StatusId = 1 },
            new Programs { ProgramId = 5, ProgramCode = "10.01.05", ProgramName = "Unspecified state", ProgramType = "PRG", IsActive = null, StatusId = 1 },
            new Programs { ProgramId = 6, ProgramCode = "10.01.06", ProgramName = "Deleted status", ProgramType = "PRG", IsActive = true, StatusId = 0 },
            new Programs { ProgramId = 7, ProgramCode = "10.01.07", ProgramName = "Deletion timestamp", ProgramType = "PRG", IsActive = true, StatusId = 1, DeletedTime = DateTime.UtcNow },
            new Programs { ProgramId = 8, ProgramCode = "10.02", ProgramName = "Inactive group", ProgramType = "MNU", IsActive = false, StatusId = 1 }
        };
        repository.Setup(r => r.Program.GetAllProgramsAsync(false)).ReturnsAsync(programs);
        repository.Setup(r => r.Program.GetAllowedProgramsAsync(userGuid, "company", "office", null)).ReturnsAsync(programs);
        var service = new UserService(repository.Object, Mock.Of<ILoggerManager>(), Mock.Of<ITransactionManager>(), registry.Object);

        var menus = (await service.GetAllowedMenusAsync(userGuid, "company", "office")).ToList();

        Assert.Equal(new[] { "10.01", "10.01.01" }, menus.Select(p => p.ProgramCode));
        Assert.All(menus, p => Assert.True(p.IsActive));
        repository.Verify(r => r.Program.GetAllProgramsAsync(false), administrator ? Times.Once() : Times.Never());
        repository.Verify(r => r.Program.GetAllowedProgramsAsync(userGuid, "company", "office", null), administrator ? Times.Never() : Times.Once());
    }
}
