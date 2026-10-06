using System.Data;
using System.Text.Json;
using BamboeUp.AppShell.Services.Pivot;
using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Configuration;
using BamboeUp.Report.Handlers;
using BamboeUp.Report.Models;
using Xunit;

namespace BamboeUp.Api.Tests;

/// <summary>
/// Simulates PVT.01.004 (SalesTransaction Telerik pivot) for date 01-01-2025.
/// </summary>
public class TelerikPivotSimulationTests
{
    [Fact]
    public async Task RunAsync_TelerikPivot_WithDate20250101_ReturnsRowsAndVendorConfig()
    {
        var templateRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "BamboeUp.Report"));

        Assert.True(
            Directory.Exists(templateRoot),
            $"Pivot template root not found: {templateRoot}");

        var handler = new PivotReportHandler(
            new DateFilteredReportDataProvider(new DateTime(2025, 1, 1)),
            new ReportTemplateOptions { TemplateRoot = templateRoot });

        var result = await handler.RunAsync(new ReportRunContext
        {
            ProgramName = "SalesTransaction Pivot (Telerik)",
            DefinitionKey = "SalesTransaction",
            StoreProcedureName = "app.pvt_SalesTransaction",
            FilePath = "Pvt/Standard/SalesTransaction.TelerikPivot.json",
            RendererType = PivotRendererTypes.TelerikPivot,
            Parameters = new Dictionary<string, object?>
            {
                ["DateFrom"] = new DateTime(2025, 1, 1),
                ["DateTo"] = new DateTime(2025, 1, 1)
            }
        });

        Assert.True(result.Success, result.Message);
        Assert.False(string.IsNullOrWhiteSpace(result.PivotPayloadJson));

        var payload = JsonSerializer.Deserialize<PivotRunPayload>(result.PivotPayloadJson!);
        Assert.NotNull(payload);
        Assert.Equal(PivotRendererTypes.TelerikPivot, payload!.RendererType);
        Assert.Equal(3, payload.RowCount);
        Assert.Equal(3, payload.Rows.Count);

        var document = PivotPayloadReader.Read(result.PivotPayloadJson);
        var config = TelerikPivotConfigParser.Parse(document.VendorConfig);
        Assert.True(config.ShowConfigurator);
        Assert.Equal("100%", config.Height);
        Assert.Equal(2, config.Rows.Count);
        Assert.Equal(2, config.Columns.Count);
    }

    private sealed class DateFilteredReportDataProvider : IReportDataProvider
    {
        private readonly DateTime _filterDate;

        public DateFilteredReportDataProvider(DateTime filterDate) => _filterDate = filterDate;

        public Task<DataTable> ExecuteStoredProcedureAsync(
            string storedProcedureName,
            IReadOnlyDictionary<string, object?> parameters,
            CancellationToken cancellationToken = default)
        {
            Assert.Equal("app.pvt_SalesTransaction", storedProcedureName);
            Assert.Equal(_filterDate, parameters["DateFrom"]);
            Assert.Equal(_filterDate, parameters["DateTo"]);

            var table = new DataTable();
            table.Columns.Add("PeriodYear", typeof(int));
            table.Columns.Add("PeriodMonth", typeof(int));
            table.Columns.Add("RegionCode", typeof(string));
            table.Columns.Add("ProductCategory", typeof(string));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("SalesChannel", typeof(string));
            table.Columns.Add("SalesTransactionCount", typeof(long));
            table.Columns.Add("NetAmount", typeof(decimal));
            table.Columns.Add("Quantity", typeof(decimal));

            table.Rows.Add(2025, 1, "R01", "Electronics", "Laptop A", "Online", 1L, 1500m, 2m);
            table.Rows.Add(2025, 1, "R02", "FMCG", "Snack B", "Retail", 1L, 250m, 10m);
            table.Rows.Add(2025, 1, "R01", "Electronics", "Phone C", "Online", 1L, 800m, 1m);

            return Task.FromResult(table);
        }
    }
}
