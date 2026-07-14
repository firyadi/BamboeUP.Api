using Service.Shell;
using Shared.DataTransferObjects;

namespace BamboeUp.Api.Tests;

public class ReportParameterAdminValidatorTests
{
    [Fact]
    public void ValidateReplaceBatch_ValidScopeRows_Passes()
    {
        var rows = new[]
        {
            new ReportParameterForUpsertDto
            {
                ParameterName = "CompanyId",
                DisplayLabel = "Company",
                ControlType = "ReadonlyText",
                DataType = "string",
                IsRequired = true,
                SortOrder = 10,
                ColumnGroup = 1,
                ColumnSpan = 12
            },
            new ReportParameterForUpsertDto
            {
                ParameterName = "CompanyOfficeId",
                DisplayLabel = "Office",
                ControlType = "ReadonlyText",
                DataType = "string",
                IsRequired = true,
                SortOrder = 20,
                ColumnGroup = 1,
                ColumnSpan = 12
            }
        };

        var ex = Record.Exception(() => ReportParameterAdminValidator.ValidateReplaceBatch(rows));
        Assert.Null(ex);
    }

    [Fact]
    public void ValidateReplaceBatch_DuplicateName_Throws()
    {
        var rows = new[]
        {
            new ReportParameterForUpsertDto { ParameterName = "CompanyId", DisplayLabel = "A", ControlType = "TextBox", DataType = "string" },
            new ReportParameterForUpsertDto { ParameterName = "CompanyId", DisplayLabel = "B", ControlType = "TextBox", DataType = "string" }
        };

        Assert.Throws<ArgumentException>(() => ReportParameterAdminValidator.ValidateReplaceBatch(rows));
    }

    [Fact]
    public void ValidateReplaceBatch_InvalidControlType_Throws()
    {
        var rows = new[]
        {
            new ReportParameterForUpsertDto
            {
                ParameterName = "Foo",
                DisplayLabel = "Foo",
                ControlType = "Unknown",
                DataType = "string"
            }
        };

        Assert.Throws<ArgumentException>(() => ReportParameterAdminValidator.ValidateReplaceBatch(rows));
    }

    [Fact]
    public void ValidateReplaceBatch_ComboBoxWithoutLookup_Throws()
    {
        var rows = new[]
        {
            new ReportParameterForUpsertDto
            {
                ParameterName = "EmployeeId",
                DisplayLabel = "Employee",
                ControlType = "ComboBox",
                DataType = "string"
            }
        };

        Assert.Throws<ArgumentException>(() => ReportParameterAdminValidator.ValidateReplaceBatch(rows));
    }

    [Fact]
    public void ValidateReplaceBatch_EmptyList_Passes()
    {
        var ex = Record.Exception(() => ReportParameterAdminValidator.ValidateReplaceBatch(Array.Empty<ReportParameterForUpsertDto>()));
        Assert.Null(ex);
    }
}
