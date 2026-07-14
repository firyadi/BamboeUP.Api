using Contracts;
using Entities.Models;
using Moq;
using Shared.DataTransferObjects;

namespace BamboeUp.Api.Tests;

public class ReportHeaderDataBuilderTests
{
    [Fact]
    public void TryCreate_StandardProfile_BuildsHeaderTableWithLogoFlag()
    {
        var context = new BamboeUp.Report.Abstractions.ReportRunContext
        {
            LayoutJson = "{\"header\":{\"profile\":\"Standard\",\"showCompanyLogo\":true,\"showCompanyName\":true}}",
            CompanyName = "PT Bamboe",
            CompanyLogo = new byte[] { 1, 2, 3 }
        };

        var table = BamboeUp.Report.Services.ReportHeaderDataBuilder.TryCreate(context);

        Assert.NotNull(table);
        Assert.Equal("HeaderData", table!.TableName);
        Assert.True((bool)table.Rows[0]["ShowCompanyLogo"]);
        Assert.Equal("PT Bamboe", table.Rows[0]["CompanyName"]);
    }

    [Fact]
    public void TryCreate_MinimalProfile_ReturnsNull()
    {
        var context = new BamboeUp.Report.Abstractions.ReportRunContext
        {
            LayoutJson = "{\"header\":{\"profile\":\"Minimal\"}}",
            CompanyName = "PT Bamboe",
            Print = new BamboeUp.Report.Abstractions.ReportPrintContext { RequiresPrintId = false }
        };

        Assert.Null(BamboeUp.Report.Services.ReportHeaderDataBuilder.TryCreate(context));
    }

    [Fact]
    public void TryCreate_PrintIdOnly_BuildsHeaderDataWithMaskedId()
    {
        var context = new BamboeUp.Report.Abstractions.ReportRunContext
        {
            LayoutJson = "{\"header\":{\"profile\":\"Minimal\"}}",
            Print = new BamboeUp.Report.Abstractions.ReportPrintContext
            {
                RequiresPrintId = true,
                ReportPrintId = "CO-202607-00001",
                ReportPrintIdMasked = "CO-202607-*****"
            }
        };

        var table = BamboeUp.Report.Services.ReportHeaderDataBuilder.TryCreate(context);

        Assert.NotNull(table);
        Assert.True((bool)table!.Rows[0]["ShowPrintId"]);
        Assert.Equal("CO-202607-*****", table.Rows[0]["ReportPrintIdMasked"]);
    }
}

public class ReportDataSetHelperTests
{
    [Theory]
    [InlineData("Award", "AwardData")]
    [InlineData("Bank", "BankData")]
    [InlineData(null, "ReportData")]
    public void ResolveDataSourceName_MatchesTemplateConvention(string? definitionKey, string expected)
    {
        Assert.Equal(expected, BamboeUp.Report.Services.ReportDataSetHelper.ResolveDataSourceName(definitionKey));
    }
}

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
    public void ValidateAndNormalize_CatalogReport_AllowsEmptySchemaWithoutDateRange()
    {
        var request = new ReportRunRequestDto
        {
            ProgramId = 1,
            ProgramCode = "96.01.06",
            ReportKind = "RPT",
            CompanyId = 40002,
            CompanyOfficeId = 1
        };

        var result = Service.Shell.ReportParameterValidator.ValidateAndNormalize(
            request,
            schemaFields: Array.Empty<ReportParameterDefinitionDto>(),
            isDocPrint: false);

        Assert.Empty(result);
        Assert.False(result.ContainsKey("CompanyId"));
        Assert.False(result.ContainsKey("CompanyOfficeId"));
        Assert.False(result.ContainsKey("DateFrom"));
        Assert.False(result.ContainsKey("DateTo"));
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

public class TelerikReportRendererTests
{
    [Fact]
    public async Task RenderAsync_WithValidTrdx_DoesNotThrowWindowsFormsException()
    {
        var renderer = new BamboeUp.Report.Telerik.TelerikReportRenderer();
        var templatePath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, 
            "..", "..", "..", "..", "..", 
            "BamboeUp.Report", "Doc", "Standard", "EngineDemo", "BankMasterSlip_Telerik.trdx"));

        if (!File.Exists(templatePath))
        {
            templatePath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(), 
                "..", "..", "..", "..", 
                "BamboeUp.Report", "Doc", "Standard", "EngineDemo", "BankMasterSlip_Telerik.trdx"));
        }

        Assert.True(File.Exists(templatePath), $"Template path not found: {templatePath}");

        var dataSet = new System.Data.DataSet();
        var table = new System.Data.DataTable("BankData");
        table.Columns.Add("BankId", typeof(int));
        table.Columns.Add("BankName", typeof(string));
        table.Columns.Add("BankInitial", typeof(string));
        table.Columns.Add("PrintedAt", typeof(string));
        table.Rows.Add(1, "Bank Central Asia", "BCA", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dataSet.Tables.Add(table);

        var request = new BamboeUp.Report.Abstractions.ReportRenderRequest
        {
            Context = new BamboeUp.Report.Abstractions.ReportRunContext
            {
                ProgramId = 1,
                ProgramCode = "02.09.04",
                Parameters = new Dictionary<string, object?>()
            },
            Data = dataSet,
            ResolvedTemplatePath = templatePath
        };

        var result = await renderer.RenderAsync(request);

        Assert.True(result.Success, result.Message);
        Assert.NotNull(result.OutputBytes);
        Assert.True(result.OutputBytes.Length > 0);
    }
}

public class DevExpressReportRendererTests
{
    [Fact]
    public async Task RenderAsync_WithValidRepx_DoesNotThrowException()
    {
        var renderer = new BamboeUp.Report.DevExpress.DevExpressReportRenderer();
        var templatePath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, 
            "..", "..", "..", "..", "..", 
            "BamboeUp.Report", "Doc", "Standard", "EngineDemo", "BankMasterSlip_DevExpress.repx"));

        if (!File.Exists(templatePath))
        {
            templatePath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(), 
                "..", "..", "..", "..", 
                "BamboeUp.Report", "Doc", "Standard", "EngineDemo", "BankMasterSlip_DevExpress.repx"));
        }

        Assert.True(File.Exists(templatePath), $"Template path not found: {templatePath}");

        var dataSet = new System.Data.DataSet();
        var table = new System.Data.DataTable("BankData");
        table.Columns.Add("BankId", typeof(int));
        table.Columns.Add("BankName", typeof(string));
        table.Columns.Add("BankInitial", typeof(string));
        table.Columns.Add("PrintedAt", typeof(string));
        table.Rows.Add(1, "Bank Central Asia", "BCA", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dataSet.Tables.Add(table);

        var request = new BamboeUp.Report.Abstractions.ReportRenderRequest
        {
            Context = new BamboeUp.Report.Abstractions.ReportRunContext
            {
                ProgramId = 1,
                ProgramCode = "02.09.04",
                Parameters = new Dictionary<string, object?>()
            },
            Data = dataSet,
            ResolvedTemplatePath = templatePath
        };

        var result = await renderer.RenderAsync(request);

        Assert.True(result.Success, result.Message);
        Assert.NotNull(result.OutputBytes);
        Assert.True(result.OutputBytes.Length > 0);
        File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(templatePath)!, "test_output.pdf"), result.OutputBytes);
    }
}

