using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using System.Data;

using Shared.RequestFeatures;
using System.Text;

namespace Repository
{
    public class AutoNumberLogRepository : IAutoNumberLogRepository
    {
        private readonly RepositoryContext _context;

        public AutoNumberLogRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task CreateAutoNumberLogAsync(AutoNumberLog log, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string sql = @"
                INSERT INTO [app].[AutoNumberLog]
                (AutoNumberLogGuid, AutoNumberTemplateId, GeneratedNumber, CounterValue, Status, ReferenceId,
                 CompanyId, CompanyOfficeId, OrganizationUnitId, YearNo, MonthNo, DayNo,
                 CreatedById, CreatedTime)
                OUTPUT INSERTED.AutoNumberLogId
                VALUES
                (@AutoNumberLogGuid, @AutoNumberTemplateId, @GeneratedNumber, @CounterValue, @Status, @ReferenceId,
                 @CompanyId, @CompanyOfficeId, @OrganizationUnitId, @YearNo, @MonthNo, @DayNo,
                 @CreatedById, @CreatedTime)";

            log.AutoNumberLogId = await conn.QuerySingleAsync<long>(sql, log, transaction);
        }

        public async Task<IEnumerable<AutoNumberLog>> GetLogsByTemplateIdAsync(long templateId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) l.*
                FROM [app].[AutoNumberLog] l
                WHERE l.AutoNumberTemplateId = @templateId
                ORDER BY l.AutoNumberLogId DESC";

            return await connection.QueryAsync<AutoNumberLog>(sql, new { templateId });
        }

        public async Task<AutoNumberLog> GetAutoNumberLogByGuidAsync(Guid logGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) l.*
                FROM [app].[AutoNumberLog] l
                WHERE l.AutoNumberLogGuid = @logGuid";

            return await connection.QuerySingleOrDefaultAsync<AutoNumberLog>(sql, new { logGuid });
        }

        public async Task<IEnumerable<AutoNumberLog>> GetAllLogsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) l.*
                FROM [app].[AutoNumberLog] l
                ORDER BY l.AutoNumberLogId DESC";

            return await connection.QueryAsync<AutoNumberLog>(sql);
        }

        public async Task<IEnumerable<AutoNumberLogView>> GetLogsAsync(AutoNumberLogParameters parameters, bool trackChanges)
        {
            using var connection = _context.CreateConnection();

            var sb = new StringBuilder(@"
                SELECT l.*,
                       t.TemplateName,
                       c.CompanyName,
                       co.CompanyOfficeName,
                       ou.OrganizationUnitName
                FROM [app].[AutoNumberLog] l
                LEFT JOIN [app].[AutoNumberTemplate] t ON l.AutoNumberTemplateId = t.AutoNumberTemplateId
                LEFT JOIN [app].[Company] c ON l.CompanyId = c.CompanyId
                LEFT JOIN [app].[CompanyOffice] co ON l.CompanyOfficeId = co.CompanyOfficeId
                LEFT JOIN [app].[OrganizationUnit] ou ON l.OrganizationUnitId = ou.OrganizationUnitId
                WHERE 1 = 1");

            var param = new DynamicParameters();

            if (parameters.TemplateId.HasValue)
            {
                sb.Append(" AND l.AutoNumberTemplateId = @TemplateId");
                param.Add("TemplateId", parameters.TemplateId.Value);
            }

            if (parameters.StartDate.HasValue)
            {
                sb.Append(" AND l.CreatedTime >= @StartDate");
                param.Add("StartDate", parameters.StartDate.Value);
            }

            if (parameters.EndDate.HasValue)
            {
                // End date is inclusive until 23:59:59
                sb.Append(" AND l.CreatedTime <= @EndDate");
                var endDate = parameters.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                param.Add("EndDate", endDate);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Status))
            {
                sb.Append(" AND l.Status = @Status");
                param.Add("Status", parameters.Status);
            }

            if (parameters.CompanyId.HasValue)
            {
                sb.Append(" AND l.CompanyId = @CompanyId");
                param.Add("CompanyId", parameters.CompanyId.Value);
            }

            if (parameters.CompanyOfficeId.HasValue)
            {
                sb.Append(" AND l.CompanyOfficeId = @CompanyOfficeId");
                param.Add("CompanyOfficeId", parameters.CompanyOfficeId.Value);
            }

            sb.Append(" ORDER BY l.AutoNumberLogId DESC");

            return await connection.QueryAsync<AutoNumberLogView>(sb.ToString(), param);
        }
    }
}
