using Contracts;
using Entities.Models;
using Moq;
using Shared.DataTransferObjects;

namespace BamboeUp.Api.Tests;

public class ReportParameterValidatorTests
{
    [Fact]
    public void ValidateAndNormalize_DocPrint_AllowsParametersWithoutDateRange()
    {
        var request = new ReportRunRequestDto
        {
            ProgramId = 1,
            ProgramCode = "02.09.04.01",
            ReportKind = "DOC",
            SourceProgramCode = "02.09.04",
            CompanyId = 40002,
            CompanyOfficeId = 1,
            Parameters = new Dictionary<string, string?>
            {
                ["EntityGuid"] = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
            }
        };

        var result = Service.Shell.ReportParameterValidator.ValidateAndNormalize(
            request,
            schemaFields: Array.Empty<ReportParameterDefinitionDto>(),
            isDocPrint: true);

        Assert.Equal("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee", result["EntityGuid"]);
        Assert.Equal("40002", result["CompanyId"]);
        Assert.Equal("1", result["CompanyOfficeId"]);
        Assert.False(result.ContainsKey("DateFrom"));
        Assert.False(result.ContainsKey("DateTo"));
    }

    [Fact]
    public void ValidateAndNormalize_CatalogReport_RequiresDateRange()
    {
        var request = new ReportRunRequestDto
        {
            ProgramId = 1,
            ProgramCode = "01.01.01",
            ReportKind = "RPT"
        };

        var ex = Assert.Throws<ArgumentException>(() =>
            Service.Shell.ReportParameterValidator.ValidateAndNormalize(
                request,
                schemaFields: Array.Empty<ReportParameterDefinitionDto>(),
                isDocPrint: false));

        Assert.Contains("Valid From and Valid To are required", ex.Message);
    }
}

public class PrintEntityValidatorTests
{
    [Fact]
    public async Task ValidateAsync_BankForm_RejectsMissingEntity()
    {
        var repository = new Mock<IRepositoryManager>(MockBehavior.Strict);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            Service.Shell.PrintEntityValidator.ValidateAsync(
                repository.Object,
                "02.09.04",
                entityId: null));

        Assert.Contains("saved record", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateAsync_BankForm_RejectsUnknownBank()
    {
        var bankGuid = Guid.NewGuid();
        var bankRepository = new Mock<IBankRepository>();
        bankRepository
            .Setup(r => r.GetBankAsync(bankGuid, false))
            .ReturnsAsync((Bank)null!);

        var repository = new Mock<IRepositoryManager>();
        repository.SetupGet(r => r.Bank).Returns(bankRepository.Object);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            Service.Shell.PrintEntityValidator.ValidateAsync(
                repository.Object,
                "02.09.04",
                entityId: bankGuid.ToString()));

        Assert.Contains("Bank record not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateAsync_BankForm_AcceptsExistingBank()
    {
        var bankGuid = Guid.NewGuid();
        var bankRepository = new Mock<IBankRepository>();
        bankRepository
            .Setup(r => r.GetBankAsync(bankGuid, false))
            .ReturnsAsync(new Bank { BankGuid = bankGuid, BankName = "Test Bank" });

        var repository = new Mock<IRepositoryManager>();
        repository.SetupGet(r => r.Bank).Returns(bankRepository.Object);

        await Service.Shell.PrintEntityValidator.ValidateAsync(
            repository.Object,
            "02.09.04",
            entityId: bankGuid.ToString());
    }
}
