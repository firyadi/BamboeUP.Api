using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Data;

namespace Contracts
{
    public interface IReportRepository
    {
        Task<IEnumerable<ReportProgramDto>> GetAllowedReportsAsync(
            Guid userGuid,
            string? companyId,
            string? officeId,
            string reportType,
            IDbTransaction? transaction = null);

        Task<IEnumerable<ReportProgramDto>> GetAllReportsByTypeAsync(string reportType, IDbTransaction? transaction = null);

        Task<ResolvedReportDefinitionRow?> ResolveDefinitionAsync(long programId, long? companyId, string reportKind, IDbTransaction? transaction = null);

        Task<IEnumerable<ReportParameterDefinitionDto>> GetParametersAsync(long reportDefinitionId, IDbTransaction? transaction = null);

        Task<IEnumerable<ReportLookupItemDto>> LookupAsync(
            string lookupType,
            string? query,
            long? companyId,
            long? officeId,
            string? lookupConfig,
            int take,
            IDbTransaction? transaction = null);

        Task<IEnumerable<PrintSlipDto>> GetAllowedPrintsAsync(
            Guid userGuid,
            string? companyId,
            string? officeId,
            string sourceProgramCode,
            IDbTransaction? transaction = null);

        Task<IEnumerable<PrintSlipDto>> GetAllPrintSlipsBySourceAsync(
            string sourceProgramCode,
            IDbTransaction? transaction = null);

        Task<DataTable> ExecuteReportDataAsync(
            string storedProcedureName,
            IReadOnlyDictionary<string, object?> parameters,
            IDbTransaction? transaction = null);
    }

    public sealed class ResolvedReportDefinitionRow
    {
        public long ReportDefinitionId { get; set; }
        public long ProgramId { get; set; }
        public string ReportScope { get; set; } = "Standard";
        public long? CompanyId { get; set; }
        public string ReportKind { get; set; } = "RPT";
        public string DefinitionKey { get; set; } = string.Empty;
        public string? RendererType { get; set; }
        public string? FilePath { get; set; }
        public string? StoreProcedureName { get; set; }
        public bool RequiresPrintId { get; set; }
        public bool IsTracked { get; set; }
        public string? PrintIdPolicy { get; set; }
        public string? PrintIdPrefix { get; set; }
        public string? LayoutJson { get; set; }
    }

    public sealed class ReportExecutionLogInsert
    {
        public Guid ReportExecutionGuid { get; set; } = Guid.NewGuid();
        public long ProgramId { get; set; }
        public string ProgramCode { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public long? ReportDefinitionId { get; set; }
        public long UserId { get; set; }
        public string? UserDisplayName { get; set; }
        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public string ReportKind { get; set; } = "RPT";
        public string? ParametersJson { get; set; }
        public string? ReportPrintId { get; set; }
        public string? ReportPrintIdMasked { get; set; }
        public bool IsReprint { get; set; }
        public string? ReprintOfPrintId { get; set; }
        public string? ReprintReason { get; set; }
        public string? SubjectKey { get; set; }
        public string Status { get; set; } = "Running";
        public string? OutputFormat { get; set; }
    }

    public sealed class ReportExecutionLogUpdate
    {
        public Guid ReportExecutionGuid { get; set; }
        public string Status { get; set; } = "Completed";
        public int? DurationMs { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
