using System.Text.Json;
using BamboeUp.AppShell.Services.Pivot;
using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Configuration;
using BamboeUp.Report.Handlers;
using BamboeUp.Report.Models;
using Xunit;

namespace BamboeUp.Api.Tests;

public class TelerikPivotPayloadReaderTests
{
    [Fact]
    public async Task PayloadReader_ReadsTelerikRows_ForDate20250101()
    {
        var templateRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "BamboeUp.Report"));

        Assert.True(
            Directory.Exists(templateRoot),
            $"Pivot template root not found: {templateRoot}");

        var handler = new PivotReportHandler(
            new TelerikPivotPayloadReaderTests.FakeProvider(),
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

        var document = PivotPayloadReader.Read(result.PivotPayloadJson);
        Assert.Equal(3, document.Rows.Count);
        Assert.Equal(3, document.DisplayedRowCount);
        Assert.False(document.WasRowLimited);

        var catalog = document.Catalog;
        Assert.All(document.Rows, row =>
        {
            Assert.Equal("2025", row.GetDimension(catalog.BindAs("PeriodYear")));
            Assert.Equal("Jan", row.GetDimension(catalog.BindAs("PeriodMonth")));
            Assert.False(string.IsNullOrWhiteSpace(row.GetDimension(catalog.BindAs("ProductCategory"))));
            Assert.NotNull(row.GetMeasure(catalog.BindAs("NetAmount")));
        });

        var config = TelerikPivotConfigParser.Parse(document.VendorConfig);
        Assert.True(config.ShowConfigurator);
        Assert.Equal("100%", config.Height);
        Assert.Equal(2, config.Rows.Count);
        Assert.Equal(2, config.Columns.Count);
        Assert.Equal(2, config.Measures.Count);
        Assert.Equal(20_000, config.MaxRows);
    }

    private sealed class FakeProvider : IReportDataProvider
    {
        public Task<System.Data.DataTable> ExecuteStoredProcedureAsync(
            string storedProcedureName,
            IReadOnlyDictionary<string, object?> parameters,
            CancellationToken cancellationToken = default)
        {
            var table = new System.Data.DataTable();
            table.Columns.Add("PeriodYear", typeof(int));
            table.Columns.Add("PeriodMonth", typeof(int));
            table.Columns.Add("ProductCategory", typeof(string));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("SalesTransactionCount", typeof(long));
            table.Columns.Add("NetAmount", typeof(decimal));
            table.Columns.Add("Quantity", typeof(decimal));

            table.Rows.Add(2025, 1, "Electronics", "Laptop", 1L, 1000m, 1m);
            table.Rows.Add(2025, 1, "FMCG", "Snack", 1L, 200m, 1m);
            table.Rows.Add(2025, 1, "Electronics", "Phone", 1L, 500m, 1m);

            return Task.FromResult(table);
        }
    }
}
