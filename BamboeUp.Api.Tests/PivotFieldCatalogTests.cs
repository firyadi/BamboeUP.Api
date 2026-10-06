using BamboeUp.AppShell.Services.Pivot;
using Xunit;

namespace BamboeUp.Api.Tests;

/// <summary>
/// The pivot runtime must render any pre-aggregated result set, not just SalesTransaction.
/// </summary>
public class PivotFieldCatalogTests
{
    private const string LeaveRequestPayload = """
        {
          "rendererType": "TelerikPivot",
          "title": "LeaveRequest Pivot",
          "rowCount": 2,
          "rows": [
            { "DepartmentName": "Finance", "LeaveType": "Annual", "RequestDateYear": 2026, "RequestDateMonth": 3, "CompanyId": 7, "TotalDays": 12.5, "LeaveRequestCount": 4 },
            { "DepartmentName": "IT", "LeaveType": "Sick", "RequestDateYear": 2026, "RequestDateMonth": 11, "CompanyId": 7, "TotalDays": 3, "LeaveRequestCount": 2 }
          ]
        }
        """;

    [Fact]
    public void Read_UnknownTable_BuildsCatalogFromPayload()
    {
        var document = PivotPayloadReader.Read(LeaveRequestPayload);
        var catalog = document.Catalog;

        Assert.Equal(2, document.Rows.Count);
        Assert.Equal(
            ["DepartmentName", "LeaveType", "RequestDateYear", "RequestDateMonth", "CompanyId"],
            catalog.Dimensions.Select(d => d.Name));
        Assert.Equal(["TotalDays", "LeaveRequestCount"], catalog.Measures.Select(m => m.Name));
    }

    [Fact]
    public void Read_UnknownTable_ProjectsValuesIntoSlots()
    {
        var document = PivotPayloadReader.Read(LeaveRequestPayload);
        var catalog = document.Catalog;
        var first = document.Rows[0];

        Assert.Equal("Finance", first.GetDimension(catalog.BindAs("DepartmentName")));
        Assert.Equal("2026", first.GetDimension(catalog.BindAs("RequestDateYear")));
        Assert.Equal(12.5m, first.GetMeasure(catalog.BindAs("TotalDays")));
        Assert.Equal(4m, first.GetMeasure(catalog.BindAs("LeaveRequestCount")));
    }

    [Fact]
    public void Read_MonthNumber_IsRenderedAsMonthName()
    {
        var document = PivotPayloadReader.Read(LeaveRequestPayload);
        var slot = document.Catalog.BindAs("RequestDateMonth");

        Assert.Equal("Mar", document.Rows[0].GetDimension(slot));
        Assert.Equal("Nov", document.Rows[1].GetDimension(slot));
    }

    [Fact]
    public void Read_ForeignKeyColumn_IsDimensionNotMeasure()
    {
        var catalog = PivotPayloadReader.Read(LeaveRequestPayload).Catalog;

        Assert.True(catalog.IsDimension("CompanyId"));
        Assert.False(catalog.IsMeasure("CompanyId"));
    }

    [Fact]
    public void Read_VendorConfig_OverridesCaptionAndAggregate()
    {
        const string payload = """
            {
              "rowCount": 1,
              "rows": [ { "BranchCode": "B01", "OpenTickets": 5 } ],
              "vendorConfig": {
                "slice": {
                  "rows": [ { "uniqueName": "BranchCode", "caption": "Branch" } ],
                  "measures": [ { "uniqueName": "OpenTickets", "caption": "Open", "aggregation": "max" } ]
                }
              }
            }
            """;

        var catalog = PivotPayloadReader.Read(payload).Catalog;

        Assert.Equal("Branch", catalog.TitleFor("BranchCode"));
        Assert.Equal("Open", catalog.TitleFor("OpenTickets"));
        Assert.Equal(Telerik.Blazor.PivotGridAggregateType.Max, catalog.AggregateFor("OpenTickets"));
    }

    [Fact]
    public void Read_EmptyPayload_ReturnsEmptyCatalog()
    {
        var document = PivotPayloadReader.Read(null);

        Assert.Empty(document.Rows);
        Assert.True(document.Catalog.IsEmpty);
    }
}
