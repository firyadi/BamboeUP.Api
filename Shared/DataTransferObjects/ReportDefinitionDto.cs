using Shared.Settings.Enums;
using System;

namespace Shared.DataTransferObjects
{
    public record ReportDefinitionDto
    {
        public long ReportDefinitionId { get; set; }
        public Guid ReportDefinitionGuid { get; set; }
        public long ProgramId { get; set; }
        public string? ProgramCode { get; set; }
        public string? ProgramName { get; set; }
        public string ReportScope { get; set; } = "Standard";
        public long? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string ReportKind { get; set; } = "RPT";
        public string DefinitionKey { get; set; } = string.Empty;
        public string? RendererType { get; set; }
        public string? FilePath { get; set; }
        public string? StoreProcedureName { get; set; }
        public bool IsTracked { get; set; }
        public bool RequiresPrintId { get; set; }
        public string? PrintIdPolicy { get; set; }
        public string? PrintIdPrefix { get; set; }
        public int Version { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int StatusId { get; set; } = 1;
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

    public record ReportDefinitionForCreationDto
    {
        public long ProgramId { get; set; }
        public string ReportScope { get; set; } = "Standard";
        public long? CompanyId { get; set; }
        public string ReportKind { get; set; } = "RPT";
        public string DefinitionKey { get; set; } = string.Empty;
        public string? RendererType { get; set; }
        public string? FilePath { get; set; }
        public string? StoreProcedureName { get; set; }
        public bool IsTracked { get; set; }
        public bool RequiresPrintId { get; set; }
        public string? PrintIdPolicy { get; set; }
        public string? PrintIdPrefix { get; set; }
        public int Version { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public long CreatedById { get; set; }
    }

    public record ReportDefinitionForUpdateDto
    {
        public long ProgramId { get; set; }
        public string ReportScope { get; set; } = "Standard";
        public long? CompanyId { get; set; }
        public string ReportKind { get; set; } = "RPT";
        public string DefinitionKey { get; set; } = string.Empty;
        public string? RendererType { get; set; }
        public string? FilePath { get; set; }
        public string? StoreProcedureName { get; set; }
        public bool IsTracked { get; set; }
        public bool RequiresPrintId { get; set; }
        public string? PrintIdPolicy { get; set; }
        public string? PrintIdPrefix { get; set; }
        public int Version { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public long UpdatedById { get; set; }
    }

    public record ReportDefinitionForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public record ReportDefinitionSearchDto
    {
        public string? DefinitionKey { get; set; }
        public SearchType DefinitionKeySearchType { get; set; } = SearchType.Contains;
        public string? ProgramCode { get; set; }
        public SearchType ProgramCodeSearchType { get; set; } = SearchType.Contains;
        public string? ProgramName { get; set; }
        public SearchType ProgramNameSearchType { get; set; } = SearchType.Contains;
        public string? ReportKind { get; set; }
        public string? ReportScope { get; set; }
        public bool? IsTracked { get; set; }
        public bool? RequiresPrintId { get; set; }
        public bool? IsActive { get; set; }
    }
}
