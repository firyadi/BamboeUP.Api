using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Shared.DataTransferObjects;

namespace Shared.Helpers;

/// <summary>
/// Adapters + ControlType aliases for ParameterFormMaster (Report / Process / Pivot).
/// </summary>
public static class ParameterFormAdapter
{
    /// <summary>Map legacy/process control names to canonical ParameterFormMaster types.</summary>
    public static string NormalizeControlType(string? controlType, string? dataType = null, string? lookupType = null)
    {
        var raw = (controlType ?? dataType ?? "TextBox").Trim();
        if (string.IsNullOrWhiteSpace(raw))
            raw = "TextBox";

        // Lookup / Dropdown / ComboBox → searchable ComboBox (data via LookupType / LookupSource)
        if (raw.Equals("ComboBox", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("Dropdown", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("Lookup", StringComparison.OrdinalIgnoreCase)
            || !string.IsNullOrWhiteSpace(lookupType))
            return "ComboBox";

        if (raw.Equals("Boolean", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("bool", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("bit", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("CheckBox", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("Switch", StringComparison.OrdinalIgnoreCase))
            return "CheckBox";

        if (raw.Equals("Number", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("NumberBox", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("int", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("bigint", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("decimal", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("numeric", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("float", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("long", StringComparison.OrdinalIgnoreCase))
            return "NumberBox";

        if (raw.Equals("Date", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("DatePicker", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("datetime2", StringComparison.OrdinalIgnoreCase))
            return "DatePicker";

        if (raw.Equals("DateTime", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("DateTimePicker", StringComparison.OrdinalIgnoreCase))
            return "DateTimePicker";

        if (raw.Equals("ReadonlyText", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("Readonly", StringComparison.OrdinalIgnoreCase))
            return "ReadonlyText";

        if (raw.Equals("FileUpload", StringComparison.OrdinalIgnoreCase)
            || raw.Equals("File", StringComparison.OrdinalIgnoreCase))
            return "FileUpload";

        return "TextBox";
    }

    public static bool IsLoginScopeCompany(string? parameterCode) =>
        parameterCode is not null
        && (parameterCode.Equals("CompanyId", StringComparison.OrdinalIgnoreCase)
            || parameterCode.Equals("Company", StringComparison.OrdinalIgnoreCase));

    public static bool IsLoginScopeOffice(string? parameterCode) =>
        parameterCode is not null
        && (parameterCode.Equals("CompanyOfficeId", StringComparison.OrdinalIgnoreCase)
            || parameterCode.Equals("OfficeId", StringComparison.OrdinalIgnoreCase)
            || parameterCode.Equals("Office", StringComparison.OrdinalIgnoreCase));

    public static ParameterSchemaDto FromReport(ReportParameterSchemaDto report)
    {
        var fields = (report.Fields ?? Array.Empty<ReportParameterDefinitionDto>())
            .Select(f => new ParameterFieldDto
            {
                ParameterCode = f.ParameterName,
                DisplayLabel = string.IsNullOrWhiteSpace(f.DisplayLabel) ? f.ParameterName : f.DisplayLabel,
                ControlType = NormalizeControlType(f.ControlType, f.DataType, f.LookupType),
                DataType = f.DataType,
                IsRequired = f.IsRequired,
                IsVisible = string.IsNullOrWhiteSpace(f.VisibleWhen),
                DefaultValue = f.DefaultValue,
                SortOrder = f.SortOrder,
                ColumnGroup = f.ColumnGroup == 0 ? (byte)1 : f.ColumnGroup,
                GroupName = f.RowGroup,
                LookupType = f.LookupType,
                LookupConfig = f.LookupConfig,
                VisibleWhen = f.VisibleWhen,
                FieldKey = f.FieldKey
            })
            .ToList();

        return new ParameterSchemaDto
        {
            SchemaKey = report.DefinitionKey,
            LayoutColumns = report.LayoutColumns == 0 ? (byte)3 : report.LayoutColumns,
            SystemContext = new ParameterSystemContextDto
            {
                CompanyId = report.SystemContext?.CompanyId,
                CompanyName = report.SystemContext?.CompanyName,
                OfficeId = report.SystemContext?.OfficeId,
                OfficeName = report.SystemContext?.OfficeName
            },
            Fields = fields
        };
    }

    public static ParameterSchemaDto FromProcess(
        IEnumerable<ProcessParameterDefinitionDto> parameters,
        string schemaKey = "",
        byte layoutColumns = 1,
        ParameterSystemContextDto? systemContext = null)
    {
        var fields = parameters
            .Where(p => p.IsVisible)
            .OrderBy(p => p.DisplayOrder)
            .Select(MapProcessField)
            .ToList();

        return new ParameterSchemaDto
        {
            SchemaKey = schemaKey,
            LayoutColumns = layoutColumns == 0 ? (byte)1 : layoutColumns,
            SystemContext = systemContext ?? new ParameterSystemContextDto(),
            Fields = fields
        };
    }

    private static ParameterFieldDto MapProcessField(ProcessParameterDefinitionDto p)
    {
        var isCompany = IsLoginScopeCompany(p.ParameterCode);
        var isOffice = IsLoginScopeOffice(p.ParameterCode);
        var isLoginScope = isCompany || isOffice;

        var control = isLoginScope
            ? "ReadonlyText"
            : NormalizeControlType(p.ControlType, p.DataType, p.LookupSource);

        return new ParameterFieldDto
        {
            ParameterCode = p.ParameterCode,
            DisplayLabel = string.IsNullOrWhiteSpace(p.ParameterName) ? p.ParameterCode : p.ParameterName,
            ControlType = control,
            DataType = p.DataType,
            IsRequired = p.IsRequired,
            IsVisible = p.IsVisible,
            DefaultValue = p.DefaultValue,
            SortOrder = p.DisplayOrder,
            ColumnGroup = 1,
            GroupName = string.IsNullOrWhiteSpace(p.GroupName) && isLoginScope ? "Scope" : p.GroupName,
            LookupType = isLoginScope ? null : p.LookupSource,
            FieldKey = isCompany ? "Company" : isOffice ? "Office" : p.ParameterCode
        };
    }

    /// <summary>Pivot stubs can reuse the same field shape as process/report.</summary>
    public static ParameterSchemaDto FromFields(
        IEnumerable<ParameterFieldDto> fields,
        string schemaKey = "",
        byte layoutColumns = 1,
        ParameterSystemContextDto? systemContext = null)
    {
        var list = fields
            .Select(f =>
            {
                if (IsLoginScopeCompany(f.ParameterCode))
                {
                    f.ControlType = "ReadonlyText";
                    f.FieldKey = "Company";
                }
                else if (IsLoginScopeOffice(f.ParameterCode))
                {
                    f.ControlType = "ReadonlyText";
                    f.FieldKey = "Office";
                }
                else
                    f.ControlType = NormalizeControlType(f.ControlType, f.DataType, f.LookupType);
                return f;
            })
            .OrderBy(f => f.SortOrder)
            .ToList();

        return new ParameterSchemaDto
        {
            SchemaKey = schemaKey,
            LayoutColumns = layoutColumns == 0 ? (byte)1 : layoutColumns,
            SystemContext = systemContext ?? new ParameterSystemContextDto(),
            Fields = list
        };
    }

    /// <summary>
    /// Bind login Company/Office into SystemContext + Values (readonly scope fields).
    /// </summary>
    public static void ApplyLoginContext(
        ParameterSchemaDto schema,
        Dictionary<string, object?> values,
        LoginContext? login)
    {
        var companyId = login?.Company?.CompanyId;
        var officeId = login?.Office?.CompanyOfficeId;
        var companyName = login?.Company?.CompanyName;
        var officeName = login?.Office?.CompanyOfficeName;

        schema.SystemContext ??= new ParameterSystemContextDto();
        schema.SystemContext.CompanyId = companyId;
        schema.SystemContext.CompanyName = companyName;
        schema.SystemContext.OfficeId = officeId;
        schema.SystemContext.OfficeName = officeName;

        foreach (var field in schema.Fields)
        {
            if (IsLoginScopeCompany(field.ParameterCode))
            {
                field.ControlType = "ReadonlyText";
                field.FieldKey = "Company";
                values[field.ParameterCode] = companyId;
            }
            else if (IsLoginScopeOffice(field.ParameterCode))
            {
                field.ControlType = "ReadonlyText";
                field.FieldKey = "Office";
                values[field.ParameterCode] = officeId;
            }
        }
    }

    public static Dictionary<string, object?> BuildInitialValues(ParameterSchemaDto schema)
    {
        var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in schema.Fields)
        {
            if (string.IsNullOrWhiteSpace(field.DefaultValue))
            {
                values[field.ParameterCode] = null;
                continue;
            }

            values[field.ParameterCode] = field.ControlType switch
            {
                "CheckBox" => field.DefaultValue.Equals("true", StringComparison.OrdinalIgnoreCase)
                    || field.DefaultValue == "1",
                "NumberBox" or "ComboBox" or "ReadonlyText" when long.TryParse(field.DefaultValue, out var n) => n,
                "DatePicker" or "DateTimePicker" when DateTime.TryParse(field.DefaultValue, out var d) => d,
                _ => field.DefaultValue
            };
        }

        return values;
    }

    public static bool TryValidateRequired(ParameterSchemaDto schema, IReadOnlyDictionary<string, object?> values, out string message)
    {
        message = string.Empty;
        foreach (var field in schema.Fields.Where(f => f.IsRequired && f.IsVisible))
        {
            if (!values.TryGetValue(field.ParameterCode, out var raw) || raw is null || string.IsNullOrWhiteSpace(raw.ToString()))
            {
                message = $"{field.DisplayLabel} is required.";
                return false;
            }
        }

        return true;
    }

    public static List<ProcessParameterValueDto> ToProcessParameterValues(
        ParameterSchemaDto schema,
        IReadOnlyDictionary<string, object?> values)
    {
        var list = new List<ProcessParameterValueDto>();
        foreach (var field in schema.Fields)
        {
            values.TryGetValue(field.ParameterCode, out var raw);
            var dto = new ProcessParameterValueDto { ParameterCode = field.ParameterCode };

            switch (field.ControlType)
            {
                case "CheckBox":
                    var b = raw is bool bb && bb;
                    dto.ValueText = b ? "true" : "false";
                    break;
                case "NumberBox":
                case "ComboBox":
                case "ReadonlyText":
                    if (raw != null && decimal.TryParse(raw.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
                    {
                        dto.ValueNumber = num;
                        dto.ValueText = num.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                        dto.ValueText = raw?.ToString();
                    break;
                case "DatePicker":
                case "DateTimePicker":
                    if (raw is DateTime dt)
                    {
                        dto.ValueDate = dt;
                        dto.ValueText = dt.ToString("yyyy-MM-dd");
                    }
                    else
                        dto.ValueText = raw?.ToString();
                    break;
                default:
                    dto.ValueText = raw?.ToString();
                    break;
            }

            list.Add(dto);
        }

        return list;
    }
}
