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
            "..", "..", "..", "..",
            "BamboeUp.Report"));

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
        Assert.All(document.Rows, row =>
        {
            Assert.Equal(2025, row.PeriodYear);
            Assert.Equal(1, row.PeriodMonth);
            Assert.False(string.IsNullOrWhiteSpace(row.ProductCategory));
        });

        var config = TelerikPivotConfigParser.Parse(document.VendorConfig);
        Assert.True(config.ShowConfigurator);
        Assert.Equal("100%", config.Height);
        Assert.Equal(2, config.Rows.Count);
        Assert.Equal(2, config.Columns.Count);
        Assert.Single(config.Measures);
    }

    private sealed class FakeProvider : IReportDataProvider
    {
        public Task<System.Data.DataTable> ExecuteStoredProcedureAsync(
            string storedProcedureName,
            IReadOnlyDictionary<string, object?> parameters,
            CancellationToken cancellationToken = default)
        {
            var table = new System.Data.DataTable();
            table.Columns.Add("SalesTransactionId", typeof(long));
            table.Columns.Add("PeriodYear", typeof(int));
            table.Columns.Add("PeriodMonth", typeof(int));
            table.Columns.Add("TransactionDate", typeof(DateTime));
            table.Columns.Add("ProductCategory", typeof(string));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("NetAmount", typeof(decimal));

            var date = new DateTime(2025, 1, 1);
            table.Rows.Add(1L, 2025, 1, date, "Electronics", "Laptop", 1000m);
            table.Rows.Add(2L, 2025, 1, date, "FMCG", "Snack", 200m);
            table.Rows.Add(3L, 2025, 1, date, "Electronics", "Phone", 500m);

            return Task.FromResult(table);
        }
    }
}
