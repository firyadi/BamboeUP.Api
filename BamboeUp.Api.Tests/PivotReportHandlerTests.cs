using System.Data;
using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Configuration;
using BamboeUp.Report.Handlers;
using BamboeUp.Report.Models;
using Xunit;

namespace BamboeUp.Api.Tests;

public class PivotRendererTypesTests
{
    [Theory]
    [InlineData("WebDataRocks", "WebDataRocks")]
    [InlineData("pivotbridge", "Native")]
    [InlineData("Native", "Native")]
    [InlineData("VoltCube", "VoltCube")]
    public void Normalize_MapsAliases(string input, string expected)
    {
        Assert.Equal(expected, PivotRendererTypes.Normalize(input));
    }

    [Theory]
    [InlineData("WebDataRocks", "WebDataRocks")]
    [InlineData("Native", "native")]
    [InlineData("VoltCube", "VoltCube")]
    public void ToBridgeKey_MapsCorrectly(string renderer, string bridgeKey)
    {
        Assert.Equal(bridgeKey, PivotRendererTypes.ToBridgeKey(renderer));
    }

    [Fact]
    public void TryResolve_JsonFilePath_DefaultsWebDataRocks()
    {
        Assert.True(PivotRendererTypes.TryResolve(null, "Pvt/Standard/Bank.json", out var resolved));
        Assert.Equal(PivotRendererTypes.WebDataRocks, resolved);
    }
}

public class PivotReportHandlerTests
{
    [Fact]
    public async Task RunAsync_ReturnsPayloadWithRowsAndBridgeOptions()
    {
        var table = BuildSampleTable();
        var handler = new PivotReportHandler(new FakeReportDataProvider(table), new ReportTemplateOptions
        {
            TemplateRoot = Path.GetTempPath()
        });

        var result = await handler.RunAsync(new ReportRunContext
        {
            ProgramName = "Sales Pivot",
            DefinitionKey = "SalesTransaction",
            StoreProcedureName = "app.pvt_SalesTransaction",
            RendererType = PivotRendererTypes.Native,
            Parameters = new Dictionary<string, object?>()
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(result.PivotPayloadJson);
        Assert.Equal(PivotRendererTypes.Native, result.RendererType);

        var payload = System.Text.Json.JsonSerializer.Deserialize<PivotRunPayload>(result.PivotPayloadJson!);
        Assert.NotNull(payload);
        Assert.Equal(2, payload!.RowCount);
        Assert.Equal("native", payload.BridgeKey);
        Assert.NotNull(payload.BridgeOptions);
        Assert.Contains("RegionCode", payload.BridgeOptions!.Rows);
    }

    [Fact]
    public async Task RunAsync_MissingSp_ReturnsError()
    {
        var handler = new PivotReportHandler(new FakeReportDataProvider(BuildSampleTable()), new ReportTemplateOptions
        {
            TemplateRoot = Path.GetTempPath()
        });

        var result = await handler.RunAsync(new ReportRunContext
        {
            RendererType = PivotRendererTypes.WebDataRocks
        });

        Assert.False(result.Success);
        Assert.Contains("stored procedure", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static DataTable BuildSampleTable()
    {
        var table = new DataTable();
        table.Columns.Add("RegionCode", typeof(string));
        table.Columns.Add("ProductCategory", typeof(string));
        table.Columns.Add("NetAmount", typeof(decimal));
        table.Rows.Add("R01", "Electronics", 1000m);
        table.Rows.Add("R02", "FMCG", 500m);
        return table;
    }

    private sealed class FakeReportDataProvider : IReportDataProvider
    {
        private readonly DataTable _table;

        public FakeReportDataProvider(DataTable table) => _table = table;

        public Task<DataTable> ExecuteStoredProcedureAsync(
            string storedProcedureName,
            IReadOnlyDictionary<string, object?> parameters,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_table.Copy());
    }
}
