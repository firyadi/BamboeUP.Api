using Shared.DataTransferObjects;
using Shared.Helpers;

namespace BamboeUp.Api.Tests;

public class ParameterFormAdapterTests
{
    [Theory]
    [InlineData("Boolean", null, null, "CheckBox")]
    [InlineData("Number", null, null, "NumberBox")]
    [InlineData("Lookup", null, null, "ComboBox")]
    [InlineData("Lookup", null, "Employee", "ComboBox")]
    [InlineData("Dropdown", null, null, "ComboBox")]
    [InlineData("Date", null, null, "DatePicker")]
    [InlineData("DateTime", null, null, "DateTimePicker")]
    [InlineData("FileUpload", null, null, "FileUpload")]
    [InlineData("Text", null, null, "TextBox")]
    [InlineData("ReadonlyText", null, null, "ReadonlyText")]
    public void NormalizeControlType_MapsAliases(string? control, string? dataType, string? lookup, string expected)
    {
        Assert.Equal(expected, ParameterFormAdapter.NormalizeControlType(control, dataType, lookup));
    }

    [Fact]
    public void FromProcess_CompanyIsReadonly_LookupIsComboBox()
    {
        var parameters = new List<ProcessParameterDefinitionDto>
        {
            new()
            {
                ParameterCode = "CompanyId",
                ParameterName = "Company",
                ControlType = "Lookup",
                LookupSource = "Company",
                IsRequired = true,
                IsVisible = true,
                DisplayOrder = 1
            },
            new()
            {
                ParameterCode = "EmployeeId",
                ParameterName = "Employee",
                ControlType = "Lookup",
                LookupSource = "Employee",
                IsRequired = false,
                IsVisible = true,
                DisplayOrder = 2
            },
            new()
            {
                ParameterCode = "ValidateOnly",
                ParameterName = "Validate Only",
                ControlType = "Boolean",
                DefaultValue = "true",
                IsVisible = true,
                DisplayOrder = 3
            }
        };

        var schema = ParameterFormAdapter.FromProcess(parameters, schemaKey: "PAYROLL_RUN");
        Assert.Equal(3, schema.Fields.Count);

        var company = schema.Fields[0];
        Assert.Equal("ReadonlyText", company.ControlType);
        Assert.Equal("Company", company.FieldKey);

        var employee = schema.Fields[1];
        Assert.Equal("ComboBox", employee.ControlType);
        Assert.Equal("Employee", employee.LookupType);

        Assert.Equal("CheckBox", schema.Fields[2].ControlType);
    }

    [Fact]
    public void ApplyLoginContext_FillsCompanyOfficeValues()
    {
        var schema = ParameterFormAdapter.FromProcess(
        [
            new ProcessParameterDefinitionDto
            {
                ParameterCode = "CompanyId",
                ParameterName = "Company",
                ControlType = "ReadonlyText",
                IsRequired = true,
                IsVisible = true,
                DisplayOrder = 1
            },
            new ProcessParameterDefinitionDto
            {
                ParameterCode = "CompanyOfficeId",
                ParameterName = "Office",
                ControlType = "ReadonlyText",
                IsRequired = true,
                IsVisible = true,
                DisplayOrder = 2
            }
        ]);

        var values = ParameterFormAdapter.BuildInitialValues(schema);
        ParameterFormAdapter.ApplyLoginContext(schema, values, new LoginContext
        {
            Company = new CompanyDto { CompanyId = 10, CompanyName = "PT Demo" },
            Office = new CompanyOfficeDto { CompanyOfficeId = 20, CompanyOfficeName = "HO" }
        });

        Assert.Equal(10L, values["CompanyId"]);
        Assert.Equal(20L, values["CompanyOfficeId"]);
        Assert.Equal(10L, schema.SystemContext.CompanyId);
        Assert.Equal("PT Demo", schema.SystemContext.CompanyName);
        Assert.True(ParameterFormAdapter.TryValidateRequired(schema, values, out _));
    }

    [Fact]
    public void TryValidateRequired_And_ToProcessParameterValues()
    {
        var schema = ParameterFormAdapter.FromProcess(
        [
            new ProcessParameterDefinitionDto
            {
                ParameterCode = "PeriodYear",
                ParameterName = "Year",
                ControlType = "Number",
                IsRequired = true,
                IsVisible = true,
                DisplayOrder = 1
            }
        ]);

        var empty = ParameterFormAdapter.BuildInitialValues(schema);
        Assert.False(ParameterFormAdapter.TryValidateRequired(schema, empty, out var msg));
        Assert.Contains("Year", msg);

        var filled = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["PeriodYear"] = 2026
        };
        Assert.True(ParameterFormAdapter.TryValidateRequired(schema, filled, out _));

        var processValues = ParameterFormAdapter.ToProcessParameterValues(schema, filled);
        var year = Assert.Single(processValues);
        Assert.Equal("PeriodYear", year.ParameterCode);
        Assert.Equal(2026m, year.ValueNumber);
        Assert.Equal("2026", year.ValueText);
    }
}
