using Shared.DataTransferObjects;
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

        Task<long> InsertExecutionLogAsync(ReportExecutionLogInsert row, IDbTransaction? transaction = null);

        Task UpdateExecutionLogAsync(ReportExecutionLogUpdate row, IDbTransaction? transaction = null);

        Task<ReportExecutionLogDto?> GetExecutionByPrintIdAsync(string reportPrintId, bool includeFullPrintId, IDbTransaction? transaction = null);

        Task<IEnumerable<ReportLookupItemDto>> LookupAsync(
            string lookupType,
            string? query,
            long? companyId,
            long? officeId,
            string? lookupConfig,
            int take,
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
        public string? FilePath { get; set; }
        public string? StoreProcedureName { get; set; }
        public bool RequiresPrintId { get; set; }
        public string? PrintIdPolicy { get; set; }
        public string? PrintIdPrefix { get; set; }
    }

    public sealed class ReportExecutionLogInsert
    {
        public Guid ReportExecutionGuid { get; set; } = Guid.NewGuid();
        public long ProgramId { get; set; }
        public long? ReportDefinitionId { get; set; }
        public long UserId { get; set; }
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
