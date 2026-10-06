using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using Shared.Settings;
using System.Data;

namespace Repository
{
    public partial class OvertimeRequestRepository(RepositoryContext context, ISqlDialect dialect) : IOvertimeRequestRepository
    {
        private readonly string _table = dialect.Qualify("app", "OvertimeRequest");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        // SQL time cannot be read into string properties. Project those columns as text.
        private string SelectList => $"""
            a.OvertimeRequestId, a.OvertimeRequestGuid, a.CompanyId, a.CompanyOfficeId, a.PersonId, a.RequestNo, a.RequestDate, a.OvertimeDate, a.Reason, a.WorkDescription, {TimeAsText("a.PlannedStartTime")} AS PlannedStartTime, {TimeAsText("a.PlannedEndTime")} AS PlannedEndTime, a.TotalPlannedMinutes, {TimeAsText("a.ActualStartTime")} AS ActualStartTime, {TimeAsText("a.ActualEndTime")} AS ActualEndTime, a.TotalActualMinutes, a.StatusId, a.ApprovedById, a.ApprovedTime, a.RejectionReason, a.Note, a.CreatedById, a.CreatedTime, a.UpdatedById, a.UpdatedTime, a.DeletedById, a.DeletedTime, a.RowVersion
            """;

        private string TimeAsText(string column) =>
            dialect.Provider == DatabaseProvider.PostgreSql
                ? $"to_char({column}, 'HH24:MI:SS')"
                : $"CONVERT(varchar(8), {column}, 108)";

        public async Task<OvertimeRequest?> GetOvertimeRequestAsync(Guid overtimeRequestGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, SelectList)}
, j_ApprovedById.PersonName AS ApprovedbyName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_PersonId.PersonName AS PersonName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_ApprovedById ON a.ApprovedById = j_ApprovedById.PersonId
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId


                WHERE a.OvertimeRequestGuid = @overtimeRequestGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<OvertimeRequest>(sql, new { overtimeRequestGuid });
        }

        public async Task<IEnumerable<OvertimeRequest>> GetAllOvertimeRequestsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, SelectList)}
, j_ApprovedById.PersonName AS ApprovedbyName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_PersonId.PersonName AS PersonName



                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_ApprovedById ON a.ApprovedById = j_ApprovedById.PersonId
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.OvertimeRequestId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<OvertimeRequest>(sql);
        }

        public async Task CreateOvertimeRequestAsync(OvertimeRequest overtimeRequest, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (OvertimeRequestGuid, CreatedById, StatusId, CreatedTime, CompanyId, CompanyOfficeId, PersonId, RequestNo, RequestDate, OvertimeDate, Reason, WorkDescription, PlannedStartTime, PlannedEndTime, TotalPlannedMinutes, ActualStartTime, ActualEndTime, TotalActualMinutes, ApprovedTime, RejectionReason, Note
                )
                VALUES
                (@OvertimeRequestGuid, @CreatedById, @StatusId, @CreatedTime, @CompanyId, @CompanyOfficeId, @PersonId, @RequestNo, @RequestDate, @OvertimeDate, @Reason, @WorkDescription, @PlannedStartTime, @PlannedEndTime, @TotalPlannedMinutes, @ActualStartTime, @ActualEndTime, @TotalActualMinutes, @ApprovedTime, @RejectionReason, @Note
                ){dialect.ReturnIdentitySuffix("OvertimeRequestId")}";
            overtimeRequest.OvertimeRequestId = await conn.QuerySingleAsync<long>(sql, overtimeRequest, transaction);
        }

        public async Task UpdateOvertimeRequestAsync(OvertimeRequest overtimeRequest, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     CompanyId = @CompanyId,
                    CompanyOfficeId = @CompanyOfficeId,
                    PersonId = @PersonId,
                    RequestNo = @RequestNo,
                    RequestDate = @RequestDate,
                    OvertimeDate = @OvertimeDate,
                    Reason = @Reason,
                    WorkDescription = @WorkDescription,
                    PlannedStartTime = @PlannedStartTime,
                    PlannedEndTime = @PlannedEndTime,
                    TotalPlannedMinutes = @TotalPlannedMinutes,
                    ActualStartTime = @ActualStartTime,
                    ActualEndTime = @ActualEndTime,
                    TotalActualMinutes = @TotalActualMinutes,
                    ApprovedTime = @ApprovedTime,
                    RejectionReason = @RejectionReason,
                    Note = @Note,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE OvertimeRequestGuid = @OvertimeRequestGuid";
            await conn.ExecuteAsync(sql, overtimeRequest, transaction);
        }

        public async Task SoftDeleteOvertimeRequestAsync(OvertimeRequest overtimeRequest, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE OvertimeRequestGuid = @OvertimeRequestGuid";

            await conn.ExecuteAsync(sql, overtimeRequest, transaction);
        }

        public async Task DeleteOvertimeRequestAsync(Guid overtimeRequestGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE OvertimeRequestGuid = @overtimeRequestGuid";
            await conn.ExecuteAsync(sql, new { overtimeRequestGuid }, transaction);
        }

        public async Task<IEnumerable<OvertimeRequest>> SearchOvertimeRequestAsync(
            string? requestNo,
            string? requestNoSearchType,
            string? requestDate,
            string? requestDateSearchType,
            string? overtimeDate,
            string? overtimeDateSearchType,
            string? reason,
            string? reasonSearchType,
            string? workDescription,
            string? workDescriptionSearchType,
            string? plannedStartTime,
            string? plannedStartTimeSearchType,
            string? plannedEndTime,
            string? plannedEndTimeSearchType,
            string? totalPlannedMinutes,
            string? totalPlannedMinutesSearchType,
            string? actualStartTime,
            string? actualStartTimeSearchType,
            string? actualEndTime,
            string? actualEndTimeSearchType,
            string? totalActualMinutes,
            string? totalActualMinutesSearchType,
            string? approvedTime,
            string? approvedTimeSearchType,
            string? rejectionReason,
            string? rejectionReasonSearchType,
            string? note,
            string? noteSearchType,
string? approvedbyName, string? approvedbyNameSearchType,
string? companyName, string? companyNameSearchType,
string? companyOfficeName, string? companyOfficeNameSearchType,
string? personName, string? personNameSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(requestNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.RequestNo", "@requestNo", requestNoSearchType, parameters, "requestNo", requestNo, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(requestDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.RequestDate", "@requestDate", requestDateSearchType, parameters, "requestDate", requestDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(overtimeDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.OvertimeDate", "@overtimeDate", overtimeDateSearchType, parameters, "overtimeDate", overtimeDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(reason))
            {
                var param = SqlFilterHelper.BuildFilter("a.Reason", "@reason", reasonSearchType, parameters, "reason", reason, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(workDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.WorkDescription", "@workDescription", workDescriptionSearchType, parameters, "workDescription", workDescription, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(plannedStartTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.PlannedStartTime", "@plannedStartTime", plannedStartTimeSearchType, parameters, "plannedStartTime", plannedStartTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(plannedEndTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.PlannedEndTime", "@plannedEndTime", plannedEndTimeSearchType, parameters, "plannedEndTime", plannedEndTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(totalPlannedMinutes))
            {
                var param = SqlFilterHelper.BuildFilter("a.TotalPlannedMinutes", "@totalPlannedMinutes", totalPlannedMinutesSearchType, parameters, "totalPlannedMinutes", totalPlannedMinutes, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(actualStartTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.ActualStartTime", "@actualStartTime", actualStartTimeSearchType, parameters, "actualStartTime", actualStartTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(actualEndTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.ActualEndTime", "@actualEndTime", actualEndTimeSearchType, parameters, "actualEndTime", actualEndTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(totalActualMinutes))
            {
                var param = SqlFilterHelper.BuildFilter("a.TotalActualMinutes", "@totalActualMinutes", totalActualMinutesSearchType, parameters, "totalActualMinutes", totalActualMinutes, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(approvedTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.ApprovedTime", "@approvedTime", approvedTimeSearchType, parameters, "approvedTime", approvedTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(rejectionReason))
            {
                var param = SqlFilterHelper.BuildFilter("a.RejectionReason", "@rejectionReason", rejectionReasonSearchType, parameters, "rejectionReason", rejectionReason, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }
if (!string.IsNullOrWhiteSpace(approvedbyName)) { var param = SqlFilterHelper.BuildFilter("j_ApprovedById.PersonName", "@approvedbyName", approvedbyNameSearchType, parameters, "approvedbyName", approvedbyName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }
if (!string.IsNullOrWhiteSpace(companyName)) { var param = SqlFilterHelper.BuildFilter("j_CompanyId.CompanyName", "@companyName", companyNameSearchType, parameters, "companyName", companyName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }
if (!string.IsNullOrWhiteSpace(companyOfficeName)) { var param = SqlFilterHelper.BuildFilter("j_CompanyOfficeId.CompanyOfficeName", "@companyOfficeName", companyOfficeNameSearchType, parameters, "companyOfficeName", companyOfficeName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }
if (!string.IsNullOrWhiteSpace(personName)) { var param = SqlFilterHelper.BuildFilter("j_PersonId.PersonName", "@personName", personNameSearchType, parameters, "personName", personName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, SelectList)}
, j_ApprovedById.PersonName AS ApprovedbyName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_PersonId.PersonName AS PersonName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_ApprovedById ON a.ApprovedById = j_ApprovedById.PersonId
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.OvertimeRequestId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<OvertimeRequest>(sql, parameters, transaction);
        }
    }
}
