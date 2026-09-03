using System;
using System.Collections.Generic;

namespace Shared.DataTransferObjects;

/// <summary>System context for readonly/scoped parameter fields (company/office session).</summary>
public sealed class ParameterSystemContextDto
{
    public long? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public long? OfficeId { get; set; }
    public string? OfficeName { get; set; }
}

/// <summary>
/// Normalized parameter field for <c>ParameterFormMaster</c>.
/// Used by Report, Process, and Pivot via adapters.
/// </summary>
public sealed class ParameterFieldDto
{
    public string ParameterCode { get; set; } = string.Empty;
    public string DisplayLabel { get; set; } = string.Empty;
    /// <summary>Canonical: TextBox, NumberBox, CheckBox, DatePicker, DateTimePicker, ComboBox, ReadonlyText, FileUpload.</summary>
    public string ControlType { get; set; } = "TextBox";
    public string DataType { get; set; } = "string";
    public bool IsRequired { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? DefaultValue { get; set; }
    public int SortOrder { get; set; }
    public byte ColumnGroup { get; set; } = 1;
    public string? GroupName { get; set; }
    public string? LookupType { get; set; }
    public string? LookupConfig { get; set; }
    public string? VisibleWhen { get; set; }
    public string? FieldKey { get; set; }
}

/// <summary>Schema bag consumed by ParameterFormMaster.</summary>
public sealed class ParameterSchemaDto
{
    public string SchemaKey { get; set; } = string.Empty;
    public byte LayoutColumns { get; set; } = 1;
    public ParameterSystemContextDto SystemContext { get; set; } = new();
    public IReadOnlyList<ParameterFieldDto> Fields { get; set; } = Array.Empty<ParameterFieldDto>();
}
