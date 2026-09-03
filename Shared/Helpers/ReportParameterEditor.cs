using Shared.DataTransferObjects;

namespace Shared.Helpers;

/// <summary>Admin UI helpers for report parameter rows (Phase 4.8).</summary>
public static class ReportParameterEditor
{
    public static IReadOnlyList<string> ControlTypes { get; } =
    [
        "ReadonlyText", "TextBox", "DatePicker", "DateTimePicker", "NumberBox", "CheckBox", "ComboBox"
    ];

    public static IReadOnlyList<string> DataTypes { get; } =
    [
        "string", "DateTime", "int", "long", "decimal", "bool"
    ];

    public static IReadOnlyList<(string Token, string Label)> ProfileTemplates =>
        SharedParameterCatalog.Profiles;

    public static ReportParameterEditModel CreateEmptyRow(int sortOrder = 10) => new()
    {
        SortOrder = sortOrder,
        ControlType = "TextBox",
        DataType = "string",
        ColumnGroup = 3,
        ColumnSpan = 12,
        RowGroup = "Filter"
    };

    public static List<ReportParameterEditModel> FromDtos(IEnumerable<ReportParameterDefinitionDto> rows) =>
        rows.Select(r => new ReportParameterEditModel
        {
            ReportParameterId = r.ReportParameterId,
            ParameterName = r.ParameterName,
            DisplayLabel = r.DisplayLabel,
            ControlType = r.ControlType,
            DataType = r.DataType,
            IsRequired = r.IsRequired,
            DefaultValue = r.DefaultValue,
            SortOrder = r.SortOrder,
            ColumnGroup = r.ColumnGroup,
            ColumnSpan = r.ColumnSpan,
            RowGroup = r.RowGroup,
            FieldKey = r.FieldKey,
            LookupType = r.LookupType,
            VisibleWhen = r.VisibleWhen,
            IsSensitive = r.IsSensitive
        }).OrderBy(r => r.ColumnGroup).ThenBy(r => r.SortOrder).ToList();

    public static ReportParameterForUpsertDto ToUpsertDto(ReportParameterEditModel row) => new()
    {
        ParameterName = row.ParameterName?.Trim() ?? string.Empty,
        DisplayLabel = row.DisplayLabel?.Trim() ?? string.Empty,
        ControlType = row.ControlType?.Trim() ?? "TextBox",
        DataType = row.DataType?.Trim() ?? "string",
        IsRequired = row.IsRequired,
        DefaultValue = string.IsNullOrWhiteSpace(row.DefaultValue) ? null : row.DefaultValue.Trim(),
        SortOrder = row.SortOrder,
        ColumnGroup = row.ColumnGroup,
        ColumnSpan = row.ColumnSpan,
        RowGroup = string.IsNullOrWhiteSpace(row.RowGroup) ? null : row.RowGroup.Trim(),
        FieldKey = string.IsNullOrWhiteSpace(row.FieldKey) ? null : row.FieldKey.Trim(),
        LookupType = string.IsNullOrWhiteSpace(row.LookupType) ? null : row.LookupType.Trim(),
        VisibleWhen = string.IsNullOrWhiteSpace(row.VisibleWhen) ? null : row.VisibleWhen.Trim(),
        IsSensitive = row.IsSensitive
    };

    public static ReportParameterSchemaDto BuildPreviewSchema(
        long reportDefinitionId,
        string definitionKey,
        IReadOnlyList<ReportParameterEditModel> rows) => new()
    {
        ReportDefinitionId = reportDefinitionId,
        DefinitionKey = definitionKey,
        LayoutColumns = 3,
        IsTracked = true,
        RequiresPrintId = false,
        SystemContext = new ReportSystemContextDto
        {
            CompanyId = 1,
            CompanyName = "PT Bamboe (preview)",
            OfficeId = 1,
            OfficeName = "Head Office (preview)"
        },
        Fields = rows.Select(r => new ReportParameterDefinitionDto
        {
            ReportDefinitionId = reportDefinitionId,
            ParameterName = r.ParameterName,
            DisplayLabel = r.DisplayLabel,
            ControlType = r.ControlType,
            DataType = r.DataType,
            IsRequired = r.IsRequired,
            DefaultValue = r.DefaultValue,
            SortOrder = r.SortOrder,
            ColumnGroup = r.ColumnGroup,
            ColumnSpan = r.ColumnSpan,
            RowGroup = r.RowGroup,
            FieldKey = r.FieldKey,
            LookupType = r.LookupType,
            VisibleWhen = r.VisibleWhen,
            IsSensitive = r.IsSensitive
        }).ToList()
    };

    public static List<ReportParameterEditModel> ApplyProfileTemplate(string profileToken)
    {
        var audience = CatalogAudience.Report;
        var token = (profileToken ?? "none").Trim().ToLowerInvariant();
        // Report keeps DateFrom/To for period/standard; "process" profile maps to period for reports
        if (token == "process")
            token = "period";

        return SharedParameterCatalog.ApplyProfile(token, audience)
            .Select(ToReportEditModel)
            .ToList();
    }

    private static ReportParameterEditModel ToReportEditModel(CatalogField f) => new()
    {
        ParameterName = f.Code,
        DisplayLabel = f.Label,
        FieldKey = f.LookupSource ?? f.Code,
        ControlType = f.ReportControlType,
        DataType = f.DataType.Equals("bigint", StringComparison.OrdinalIgnoreCase) ? "long"
            : f.DataType.Equals("boolean", StringComparison.OrdinalIgnoreCase) ? "bool"
            : f.DataType.Equals("datetime", StringComparison.OrdinalIgnoreCase) ? "DateTime"
            : f.DataType,
        IsRequired = f.IsRequired,
        DefaultValue = f.DefaultValue,
        SortOrder = f.SortOrder,
        ColumnGroup = f.GroupName is "Scope" ? (byte)1 : (byte)2,
        ColumnSpan = (byte)(f.GroupName is "Period" ? 6 : 12),
        RowGroup = f.GroupName,
        LookupType = f.LookupSource
    };

    private static List<ReportParameterEditModel> ScopeTemplate() =>
        ApplyProfileTemplate("scope");

    private static List<ReportParameterEditModel> PeriodTemplate() =>
        ApplyProfileTemplate("period");
}

public sealed class ReportParameterEditModel
{
    public long ReportParameterId { get; set; }
    public string ParameterName { get; set; } = string.Empty;
    public string DisplayLabel { get; set; } = string.Empty;
    public string ControlType { get; set; } = "TextBox";
    public string DataType { get; set; } = "string";
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public int SortOrder { get; set; } = 10;
    public byte ColumnGroup { get; set; } = 1;
    public byte ColumnSpan { get; set; } = 12;
    public string? RowGroup { get; set; }
    public string? FieldKey { get; set; }
    public string? LookupType { get; set; }
    public string? VisibleWhen { get; set; }
    public bool IsSensitive { get; set; }
}
