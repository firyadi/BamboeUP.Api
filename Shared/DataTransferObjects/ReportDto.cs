using System;
using System.Collections.Generic;

namespace Shared.DataTransferObjects
{
    public record ReportProgramDto
    {
        public long ProgramId { get; init; }
        public Guid ProgramGuid { get; init; }
        public string ProgramCode { get; init; } = string.Empty;
        public string ProgramName { get; init; } = string.Empty;
        public string? ProgramType { get; init; }
        public string? StoreProcedureName { get; init; }
        public bool IsProgramPrintAble { get; init; }
        public string? ReportScope { get; init; }
        public bool RequiresPrintId { get; init; }
        public long? ReportDefinitionId { get; init; }
    }

    public record ReportParameterDefinitionDto
    {
        public long ReportParameterId { get; init; }
        public long ReportDefinitionId { get; init; }
        public string? FieldKey { get; init; }
        public string ParameterName { get; init; } = string.Empty;
        public string DisplayLabel { get; init; } = string.Empty;
        public string ControlType { get; init; } = "TextBox";
        public string DataType { get; init; } = "string";
        public bool IsRequired { get; init; }
        public string? DefaultValue { get; init; }
        public int SortOrder { get; init; }
        public byte ColumnGroup { get; init; } = 1;
        public byte ColumnSpan { get; init; } = 12;
        public string? RowGroup { get; init; }
        public string? LookupType { get; init; }
        public string? LookupConfig { get; init; }
        public string? VisibleWhen { get; init; }
        public bool IsSensitive { get; init; }
    }

    public record ReportSystemContextDto
    {
        public long? CompanyId { get; init; }
        public string? CompanyName { get; init; }
        public long? OfficeId { get; init; }
        public string? OfficeName { get; init; }
    }

    public record ReportParameterSchemaDto
    {
        public long ReportDefinitionId { get; init; }
        public string DefinitionKey { get; init; } = string.Empty;
        public byte LayoutColumns { get; init; } = 3;
        public bool RequiresPrintId { get; init; }
        public ReportSystemContextDto SystemContext { get; init; } = new();
        public IReadOnlyList<ReportParameterDefinitionDto> Fields { get; init; } = Array.Empty<ReportParameterDefinitionDto>();
    }

    public record ReportLookupItemDto
    {
        public string Id { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
    }

    public record ReportRunRequestDto
    {
        public long ProgramId { get; init; }
        public string ProgramCode { get; init; } = string.Empty;
        public string ReportKind { get; init; } = "RPT";
        public long? CompanyId { get; init; }
        public long? CompanyOfficeId { get; init; }
        public DateTime? DateFrom { get; init; }
        public DateTime? DateTo { get; init; }
        public bool IsReprint { get; init; }
        public string? ReprintOfPrintId { get; init; }
        public string? ReprintReason { get; init; }
        public Dictionary<string, string?> Parameters { get; init; } = new();
    }

    public record ReportRunResultDto
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public Guid ReportExecutionGuid { get; init; }
        public string? ReportPrintId { get; init; }
        public string? ReportPrintIdMasked { get; init; }
    }

    public record ReportExecutionLogDto
    {
        public long ReportExecutionLogId { get; init; }
        public Guid ReportExecutionGuid { get; init; }
        public long ProgramId { get; init; }
        public string? ProgramCode { get; init; }
        public string? ProgramName { get; init; }
        public string ReportKind { get; init; } = string.Empty;
        public string? ReportPrintId { get; init; }
        public string? ReportPrintIdMasked { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedTime { get; init; }
    }
}
