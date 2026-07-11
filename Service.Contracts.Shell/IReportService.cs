using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IReportService
    {
        Task<IEnumerable<ReportProgramDto>> GetAllowedReportsAsync(string reportType, Guid userGuid, string? companyId, string? officeId, bool isAdmin);
        Task<ReportParameterSchemaDto> GetReportParameterSchemaAsync(long programId, long? companyId, long? officeId, string reportKind, string? companyName = null, string? officeName = null);
        Task<IEnumerable<ReportLookupItemDto>> LookupAsync(string lookupType, string? query, long? companyId, long? officeId, string? lookupConfig, int take);
        Task<ReportRunResultDto> RunReportAsync(ReportRunRequestDto request, Guid userGuid, long userId, string? companyId, string? officeId, bool isAdmin);
        Task<ReportExecutionLogDto?> GetExecutionByPrintIdAsync(string reportPrintId, bool includeFullPrintId);
    }
}
