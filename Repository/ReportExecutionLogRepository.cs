using Contracts;
using Dapper;
using Shared.DataTransferObjects;
using System.Data;
using System.Text;

namespace Repository
{
    public class ReportExecutionLogRepository : IReportExecutionLogRepository
    {
        private readonly AuditRepositoryContext _auditContext;

        public ReportExecutionLogRepository(AuditRepositoryContext auditContext)
        {
            _auditContext = auditContext;
        }

        public async Task<long> InsertAsync(ReportExecutionLogInsert row, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _auditContext.CreateConnection();
            const string sql = @"
                INSERT INTO [rpt].[ReportExecutionLog]
                (ReportExecutionGuid, ProgramId, ProgramCode, ProgramName, ReportDefinitionId,
                 UserId, UserDisplayName, CompanyId, CompanyOfficeId, ReportKind, ParametersJson,
                 ReportPrintId, ReportPrintIdMasked, IsReprint, ReprintOfPrintId, ReprintReason,
                 SubjectKey, Status, OutputFormat)
                VALUES
                (@ReportExecutionGuid, @ProgramId, @ProgramCode, @ProgramName, @ReportDefinitionId,
                 @UserId, @UserDisplayName, @CompanyId, @CompanyOfficeId, @ReportKind, @ParametersJson,
                 @ReportPrintId, @ReportPrintIdMasked, @IsReprint, @ReprintOfPrintId, @ReprintReason,
                 @SubjectKey, @Status, @OutputFormat);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
            return await conn.QuerySingleAsync<long>(sql, row, transaction);
        }

        public async Task UpdateAsync(ReportExecutionLogUpdate row, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _auditContext.CreateConnection();
            const string sql = @"
                UPDATE [rpt].[ReportExecutionLog]
                SET Status = @Status,
                    DurationMs = @DurationMs,
                    ErrorMessage = @ErrorMessage
                WHERE ReportExecutionGuid = @ReportExecutionGuid";
            await conn.ExecuteAsync(sql, row, transaction);
        }

        public async Task<ReportExecutionLogDto?> GetByPrintIdAsync(
            string reportPrintId,
            bool includeFullPrintId,
            IDbTransaction? transaction = null)
        {
            using var connection = _auditContext.CreateConnection();
            const string sql = @"
                SELECT ReportExecutionLogId, ReportExecutionGuid, ProgramId, ProgramCode, ProgramName,
                       ReportKind, ReportPrintId, ReportPrintIdMasked, Status, CreatedTime,
                       DurationMs, OutputFormat, UserDisplayName, CompanyId, CompanyOfficeId,
                       IsReprint, ReprintOfPrintId, ReprintReason, ParametersJson, ErrorMessage
                FROM [rpt].[ReportExecutionLog]
                WHERE ReportPrintId = @ReportPrintId OR ReportPrintIdMasked = @ReportPrintId";

            var row = await connection.QuerySingleOrDefaultAsync<ReportExecutionLogDto>(sql, new { reportPrintId }, transaction);
            if (row == null)
                return null;

            return includeFullPrintId ? row : row with { ReportPrintId = null };
        }

        public async Task<PagedResult<ReportExecutionLogDto>> GetPagedAsync(
            Guid? userGuid,
            string? companyId,
            string? officeId,
            bool scoped,
            ReportExecutionLogQueryDto query,
            IDbTransaction? transaction = null)
        {
            using var connection = _auditContext.CreateConnection();
            var parameters = new DynamicParameters();
            var where = new StringBuilder(" WHERE 1 = 1 ");

            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);
            var offset = (page - 1) * pageSize;

            if (scoped)
            {
                if (long.TryParse(companyId, out var companyIdLong))
                {
                    where.Append(" AND l.CompanyId = @ScopeCompanyId ");
                    parameters.Add("ScopeCompanyId", companyIdLong);
                }

                if (long.TryParse(officeId, out var officeIdLong))
                {
                    where.Append(" AND (l.CompanyOfficeId IS NULL OR l.CompanyOfficeId = @ScopeOfficeId) ");
                    parameters.Add("ScopeOfficeId", officeIdLong);
                }
            }

            if (!string.IsNullOrWhiteSpace(query.Q))
            {
                where.Append(@" AND (
                    l.ProgramCode LIKE @Q OR l.ProgramName LIKE @Q
                    OR l.ReportPrintIdMasked LIKE @Q
                    OR l.UserDisplayName LIKE @Q
                ) ");
                parameters.Add("Q", $"%{query.Q.Trim()}%");
            }

            if (query.ProgramId.HasValue)
            {
                where.Append(" AND l.ProgramId = @ProgramId ");
                parameters.Add("ProgramId", query.ProgramId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                where.Append(" AND l.Status = @Status ");
                parameters.Add("Status", query.Status.Trim());
            }

            if (query.DateFrom.HasValue)
            {
                where.Append(" AND l.CreatedTime >= @DateFrom ");
                parameters.Add("DateFrom", query.DateFrom.Value.Date);
            }

            if (query.DateTo.HasValue)
            {
                where.Append(" AND l.CreatedTime < @DateTo ");
                parameters.Add("DateTo", query.DateTo.Value.Date.AddDays(1));
            }

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var sql = $@"
                SELECT l.ReportExecutionLogId, l.ReportExecutionGuid, l.ProgramId, l.ProgramCode, l.ProgramName,
                       l.ReportKind, l.ReportPrintId, l.ReportPrintIdMasked, l.Status, l.CreatedTime,
                       l.DurationMs, l.OutputFormat, l.UserDisplayName, l.CompanyId, l.CompanyOfficeId,
                       l.IsReprint, l.ReprintOfPrintId, l.ReprintReason, l.ParametersJson, l.ErrorMessage,
                       COUNT(*) OVER() AS TotalCount
                FROM [rpt].[ReportExecutionLog] l
                {where}
                ORDER BY l.CreatedTime DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var rows = (await connection.QueryAsync<ExecutionLogListRow>(sql, parameters, transaction)).ToList();

            return new PagedResult<ReportExecutionLogDto>
            {
                Items = rows.Select(MapListRow).ToList(),
                TotalCount = rows.FirstOrDefault()?.TotalCount ?? 0,
                Page = page,
                PageSize = pageSize
            };
        }

        private static ReportExecutionLogDto MapListRow(ExecutionLogListRow row) => new()
        {
            ReportExecutionLogId = row.ReportExecutionLogId,
            ReportExecutionGuid = row.ReportExecutionGuid,
            ProgramId = row.ProgramId,
            ProgramCode = row.ProgramCode,
            ProgramName = row.ProgramName,
            ReportKind = row.ReportKind,
            ReportPrintId = row.ReportPrintId,
            ReportPrintIdMasked = row.ReportPrintIdMasked,
            Status = row.Status,
            CreatedTime = row.CreatedTime,
            DurationMs = row.DurationMs,
            OutputFormat = row.OutputFormat,
            UserDisplayName = row.UserDisplayName,
            CompanyId = row.CompanyId,
            CompanyOfficeId = row.CompanyOfficeId,
            IsReprint = row.IsReprint,
            ReprintOfPrintId = row.ReprintOfPrintId,
            ReprintReason = row.ReprintReason,
            ParametersJson = row.ParametersJson,
            ErrorMessage = row.ErrorMessage
        };

        private sealed class ExecutionLogListRow
        {
            public long ReportExecutionLogId { get; set; }
            public Guid ReportExecutionGuid { get; set; }
            public long ProgramId { get; set; }
            public string? ProgramCode { get; set; }
            public string? ProgramName { get; set; }
            public string ReportKind { get; set; } = string.Empty;
            public string? ReportPrintId { get; set; }
            public string? ReportPrintIdMasked { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime CreatedTime { get; set; }
            public int? DurationMs { get; set; }
            public string? OutputFormat { get; set; }
            public string? UserDisplayName { get; set; }
            public long? CompanyId { get; set; }
            public long? CompanyOfficeId { get; set; }
            public bool IsReprint { get; set; }
            public string? ReprintOfPrintId { get; set; }
            public string? ReprintReason { get; set; }
            public string? ParametersJson { get; set; }
            public string? ErrorMessage { get; set; }
            public int TotalCount { get; set; }
        }
    }
}
