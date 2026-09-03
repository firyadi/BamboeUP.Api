using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class EmployeeCareerRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IEmployeeCareerRepository
    {
        private readonly string _table = dialect.Qualify("app", "EmployeeCareer");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<EmployeeCareer?> GetEmployeeCareerAsync(Guid employeeCareerGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, j_EmployeeContractId.EmployeeContractName AS EmployeeContractName
, j_FromCompanyId.FromCompanyName AS FromCompanyName
, j_FromJobGradeId.FromJobGradeName AS FromJobGradeName
, j_FromJobLevelId.FromJobLevelName AS FromJobLevelName
, j_FromJobTitleId.FromJobTitleName AS FromJobTitleName
, j_FromManagerEmployeeId.FromManagerEmployeeName AS FromManagerEmployeeName
, j_FromOrganizationUnitId.FromOrganizationUnitName AS FromOrganizationUnitName
, j_FromPositionId.FromPositionName AS FromPositionName
, j_ToCompanyId.ToCompanyName AS ToCompanyName
, j_ToJobGradeId.ToJobGradeName AS ToJobGradeName
, j_ToJobLevelId.ToJobLevelName AS ToJobLevelName
, j_ToJobTitleId.ToJobTitleName AS ToJobTitleName
, j_ToManagerEmployeeId.ToManagerEmployeeName AS ToManagerEmployeeName
, j_ToOrganizationUnitId.ToOrganizationUnitName AS ToOrganizationUnitName
, j_ToPositionId.ToPositionName AS ToPositionName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCareerEvent.StandardReferenceItemName")} AS CareerEventName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCareerReason.StandardReferenceItemName")} AS CareerReasonName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "EmployeeAssignment")} j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
LEFT JOIN {dialect.Qualify("app", "EmployeeContract")} j_EmployeeContractId ON a.EmployeeContractId = j_EmployeeContractId.EmployeeContractId
LEFT JOIN {dialect.Qualify("app", "FromCompany")} j_FromCompanyId ON a.FromCompanyId = j_FromCompanyId.FromCompanyId
LEFT JOIN {dialect.Qualify("app", "FromJobGrade")} j_FromJobGradeId ON a.FromJobGradeId = j_FromJobGradeId.FromJobGradeId
LEFT JOIN {dialect.Qualify("app", "FromJobLevel")} j_FromJobLevelId ON a.FromJobLevelId = j_FromJobLevelId.FromJobLevelId
LEFT JOIN {dialect.Qualify("app", "FromJobTitle")} j_FromJobTitleId ON a.FromJobTitleId = j_FromJobTitleId.FromJobTitleId
LEFT JOIN {dialect.Qualify("app", "FromManagerEmployee")} j_FromManagerEmployeeId ON a.FromManagerEmployeeId = j_FromManagerEmployeeId.FromManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "FromOrganizationUnit")} j_FromOrganizationUnitId ON a.FromOrganizationUnitId = j_FromOrganizationUnitId.FromOrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "FromPosition")} j_FromPositionId ON a.FromPositionId = j_FromPositionId.FromPositionId
LEFT JOIN {dialect.Qualify("app", "ToCompany")} j_ToCompanyId ON a.ToCompanyId = j_ToCompanyId.ToCompanyId
LEFT JOIN {dialect.Qualify("app", "ToJobGrade")} j_ToJobGradeId ON a.ToJobGradeId = j_ToJobGradeId.ToJobGradeId
LEFT JOIN {dialect.Qualify("app", "ToJobLevel")} j_ToJobLevelId ON a.ToJobLevelId = j_ToJobLevelId.ToJobLevelId
LEFT JOIN {dialect.Qualify("app", "ToJobTitle")} j_ToJobTitleId ON a.ToJobTitleId = j_ToJobTitleId.ToJobTitleId
LEFT JOIN {dialect.Qualify("app", "ToManagerEmployee")} j_ToManagerEmployeeId ON a.ToManagerEmployeeId = j_ToManagerEmployeeId.ToManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "ToOrganizationUnit")} j_ToOrganizationUnitId ON a.ToOrganizationUnitId = j_ToOrganizationUnitId.ToOrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "ToPosition")} j_ToPositionId ON a.ToPositionId = j_ToPositionId.ToPositionId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCareerEvent", "SrCareerEvent", "CareerEvent")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCareerReason", "SrCareerReason", "CareerReason")}

                WHERE a.EmployeeCareerGuid = @employeeCareerGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeCareer>(sql, new { employeeCareerGuid });
        }

        public async Task<EmployeeCareer?> GetByEmployeeGuidAndEmployeeCareerGuidAsync(Guid employeeGuid, Guid employeeCareerGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, j_EmployeeContractId.EmployeeContractName AS EmployeeContractName
, j_FromCompanyId.FromCompanyName AS FromCompanyName
, j_FromJobGradeId.FromJobGradeName AS FromJobGradeName
, j_FromJobLevelId.FromJobLevelName AS FromJobLevelName
, j_FromJobTitleId.FromJobTitleName AS FromJobTitleName
, j_FromManagerEmployeeId.FromManagerEmployeeName AS FromManagerEmployeeName
, j_FromOrganizationUnitId.FromOrganizationUnitName AS FromOrganizationUnitName
, j_FromPositionId.FromPositionName AS FromPositionName
, j_ToCompanyId.ToCompanyName AS ToCompanyName
, j_ToJobGradeId.ToJobGradeName AS ToJobGradeName
, j_ToJobLevelId.ToJobLevelName AS ToJobLevelName
, j_ToJobTitleId.ToJobTitleName AS ToJobTitleName
, j_ToManagerEmployeeId.ToManagerEmployeeName AS ToManagerEmployeeName
, j_ToOrganizationUnitId.ToOrganizationUnitName AS ToOrganizationUnitName
, j_ToPositionId.ToPositionName AS ToPositionName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCareerEvent.StandardReferenceItemName")} AS CareerEventName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCareerReason.StandardReferenceItemName")} AS CareerReasonName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "EmployeeAssignment")} j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
LEFT JOIN {dialect.Qualify("app", "EmployeeContract")} j_EmployeeContractId ON a.EmployeeContractId = j_EmployeeContractId.EmployeeContractId
LEFT JOIN {dialect.Qualify("app", "FromCompany")} j_FromCompanyId ON a.FromCompanyId = j_FromCompanyId.FromCompanyId
LEFT JOIN {dialect.Qualify("app", "FromJobGrade")} j_FromJobGradeId ON a.FromJobGradeId = j_FromJobGradeId.FromJobGradeId
LEFT JOIN {dialect.Qualify("app", "FromJobLevel")} j_FromJobLevelId ON a.FromJobLevelId = j_FromJobLevelId.FromJobLevelId
LEFT JOIN {dialect.Qualify("app", "FromJobTitle")} j_FromJobTitleId ON a.FromJobTitleId = j_FromJobTitleId.FromJobTitleId
LEFT JOIN {dialect.Qualify("app", "FromManagerEmployee")} j_FromManagerEmployeeId ON a.FromManagerEmployeeId = j_FromManagerEmployeeId.FromManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "FromOrganizationUnit")} j_FromOrganizationUnitId ON a.FromOrganizationUnitId = j_FromOrganizationUnitId.FromOrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "FromPosition")} j_FromPositionId ON a.FromPositionId = j_FromPositionId.FromPositionId
LEFT JOIN {dialect.Qualify("app", "ToCompany")} j_ToCompanyId ON a.ToCompanyId = j_ToCompanyId.ToCompanyId
LEFT JOIN {dialect.Qualify("app", "ToJobGrade")} j_ToJobGradeId ON a.ToJobGradeId = j_ToJobGradeId.ToJobGradeId
LEFT JOIN {dialect.Qualify("app", "ToJobLevel")} j_ToJobLevelId ON a.ToJobLevelId = j_ToJobLevelId.ToJobLevelId
LEFT JOIN {dialect.Qualify("app", "ToJobTitle")} j_ToJobTitleId ON a.ToJobTitleId = j_ToJobTitleId.ToJobTitleId
LEFT JOIN {dialect.Qualify("app", "ToManagerEmployee")} j_ToManagerEmployeeId ON a.ToManagerEmployeeId = j_ToManagerEmployeeId.ToManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "ToOrganizationUnit")} j_ToOrganizationUnitId ON a.ToOrganizationUnitId = j_ToOrganizationUnitId.ToOrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "ToPosition")} j_ToPositionId ON a.ToPositionId = j_ToPositionId.ToPositionId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCareerEvent", "SrCareerEvent", "CareerEvent")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCareerReason", "SrCareerReason", "CareerReason")}

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.EmployeeCareerGuid = @employeeCareerGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeCareer>(sql, new { employeeGuid, employeeCareerGuid });
        }

        public async Task<IEnumerable<EmployeeCareer>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, j_EmployeeContractId.EmployeeContractName AS EmployeeContractName
, j_FromCompanyId.FromCompanyName AS FromCompanyName
, j_FromJobGradeId.FromJobGradeName AS FromJobGradeName
, j_FromJobLevelId.FromJobLevelName AS FromJobLevelName
, j_FromJobTitleId.FromJobTitleName AS FromJobTitleName
, j_FromManagerEmployeeId.FromManagerEmployeeName AS FromManagerEmployeeName
, j_FromOrganizationUnitId.FromOrganizationUnitName AS FromOrganizationUnitName
, j_FromPositionId.FromPositionName AS FromPositionName
, j_ToCompanyId.ToCompanyName AS ToCompanyName
, j_ToJobGradeId.ToJobGradeName AS ToJobGradeName
, j_ToJobLevelId.ToJobLevelName AS ToJobLevelName
, j_ToJobTitleId.ToJobTitleName AS ToJobTitleName
, j_ToManagerEmployeeId.ToManagerEmployeeName AS ToManagerEmployeeName
, j_ToOrganizationUnitId.ToOrganizationUnitName AS ToOrganizationUnitName
, j_ToPositionId.ToPositionName AS ToPositionName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCareerEvent.StandardReferenceItemName")} AS CareerEventName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCareerReason.StandardReferenceItemName")} AS CareerReasonName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "EmployeeAssignment")} j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
LEFT JOIN {dialect.Qualify("app", "EmployeeContract")} j_EmployeeContractId ON a.EmployeeContractId = j_EmployeeContractId.EmployeeContractId
LEFT JOIN {dialect.Qualify("app", "FromCompany")} j_FromCompanyId ON a.FromCompanyId = j_FromCompanyId.FromCompanyId
LEFT JOIN {dialect.Qualify("app", "FromJobGrade")} j_FromJobGradeId ON a.FromJobGradeId = j_FromJobGradeId.FromJobGradeId
LEFT JOIN {dialect.Qualify("app", "FromJobLevel")} j_FromJobLevelId ON a.FromJobLevelId = j_FromJobLevelId.FromJobLevelId
LEFT JOIN {dialect.Qualify("app", "FromJobTitle")} j_FromJobTitleId ON a.FromJobTitleId = j_FromJobTitleId.FromJobTitleId
LEFT JOIN {dialect.Qualify("app", "FromManagerEmployee")} j_FromManagerEmployeeId ON a.FromManagerEmployeeId = j_FromManagerEmployeeId.FromManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "FromOrganizationUnit")} j_FromOrganizationUnitId ON a.FromOrganizationUnitId = j_FromOrganizationUnitId.FromOrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "FromPosition")} j_FromPositionId ON a.FromPositionId = j_FromPositionId.FromPositionId
LEFT JOIN {dialect.Qualify("app", "ToCompany")} j_ToCompanyId ON a.ToCompanyId = j_ToCompanyId.ToCompanyId
LEFT JOIN {dialect.Qualify("app", "ToJobGrade")} j_ToJobGradeId ON a.ToJobGradeId = j_ToJobGradeId.ToJobGradeId
LEFT JOIN {dialect.Qualify("app", "ToJobLevel")} j_ToJobLevelId ON a.ToJobLevelId = j_ToJobLevelId.ToJobLevelId
LEFT JOIN {dialect.Qualify("app", "ToJobTitle")} j_ToJobTitleId ON a.ToJobTitleId = j_ToJobTitleId.ToJobTitleId
LEFT JOIN {dialect.Qualify("app", "ToManagerEmployee")} j_ToManagerEmployeeId ON a.ToManagerEmployeeId = j_ToManagerEmployeeId.ToManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "ToOrganizationUnit")} j_ToOrganizationUnitId ON a.ToOrganizationUnitId = j_ToOrganizationUnitId.ToOrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "ToPosition")} j_ToPositionId ON a.ToPositionId = j_ToPositionId.ToPositionId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCareerEvent", "SrCareerEvent", "CareerEvent")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCareerReason", "SrCareerReason", "CareerReason")}

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeCareerId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<EmployeeCareer>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeCareerAsync(EmployeeCareer employeeCareer, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeCareer);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (EmployeeCareerGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [EmployeeAssignmentId], [EmployeeContractId], [SrCareerEvent], [SrCareerReason], [EffectiveDate], [FromCompanyId], [FromOrganizationUnitId], [FromPositionId], [FromJobTitleId], [FromJobGradeId], [FromJobLevelId], [FromManagerEmployeeId], [ToCompanyId], [ToOrganizationUnitId], [ToPositionId], [ToJobTitleId], [ToJobGradeId], [ToJobLevelId], [ToManagerEmployeeId], [ReferenceNo], [ReferenceDate], [Remarks])
                VALUES
                (@EmployeeCareerGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @EmployeeAssignmentId, @EmployeeContractId, @SrCareerEvent, @SrCareerReason, @EffectiveDate, @FromCompanyId, @FromOrganizationUnitId, @FromPositionId, @FromJobTitleId, @FromJobGradeId, @FromJobLevelId, @FromManagerEmployeeId, @ToCompanyId, @ToOrganizationUnitId, @ToPositionId, @ToJobTitleId, @ToJobGradeId, @ToJobLevelId, @ToManagerEmployeeId, @ReferenceNo, @ReferenceDate, @Remarks){dialect.ReturnIdentitySuffix("EmployeeCareerId")}";
            employeeCareer.EmployeeCareerId = await conn.QuerySingleAsync<long>(sql, employeeCareer, transaction);
        }

        public async Task UpdateEmployeeCareerAsync(EmployeeCareer employeeCareer, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeCareer);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [EmployeeId] = @EmployeeId,
                    [EmployeeAssignmentId] = @EmployeeAssignmentId,
                    [EmployeeContractId] = @EmployeeContractId,
                    [SrCareerEvent] = @SrCareerEvent,
                    [SrCareerReason] = @SrCareerReason,
                    [EffectiveDate] = @EffectiveDate,
                    [FromCompanyId] = @FromCompanyId,
                    [FromOrganizationUnitId] = @FromOrganizationUnitId,
                    [FromPositionId] = @FromPositionId,
                    [FromJobTitleId] = @FromJobTitleId,
                    [FromJobGradeId] = @FromJobGradeId,
                    [FromJobLevelId] = @FromJobLevelId,
                    [FromManagerEmployeeId] = @FromManagerEmployeeId,
                    [ToCompanyId] = @ToCompanyId,
                    [ToOrganizationUnitId] = @ToOrganizationUnitId,
                    [ToPositionId] = @ToPositionId,
                    [ToJobTitleId] = @ToJobTitleId,
                    [ToJobGradeId] = @ToJobGradeId,
                    [ToJobLevelId] = @ToJobLevelId,
                    [ToManagerEmployeeId] = @ToManagerEmployeeId,
                    [ReferenceNo] = @ReferenceNo,
                    [ReferenceDate] = @ReferenceDate,
                    [Remarks] = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE EmployeeCareerGuid = @EmployeeCareerGuid";
            await conn.ExecuteAsync(sql, employeeCareer, transaction);
        }

        public async Task SoftDeleteEmployeeCareerAsync(EmployeeCareer employeeCareer, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(employeeCareer, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeCareerGuid = @EmployeeCareerGuid";

            await conn.ExecuteAsync(sql, employeeCareer, transaction);
        }

        public async Task DeleteEmployeeCareerAsync(Guid employeeCareerGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE EmployeeCareerGuid = @employeeCareerGuid";
            await conn.ExecuteAsync(sql, new { employeeCareerGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE EmployeeId IN (SELECT EmployeeId FROM {dialect.Qualify("app", "Employee")} WHERE EmployeeGuid = @employeeGuid)";
            await conn.ExecuteAsync(sql, new { employeeGuid }, transaction);
        }

        public async Task<IEnumerable<EmployeeCareer>> SearchEmployeeCareerAsync(
            string? effectiveDate,
            string? effectiveDateSearchType,
            string? referenceNo,
            string? referenceNoSearchType,
            string? referenceDate,
            string? referenceDateSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeCareerGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (employeeGuid != Guid.Empty)
            {
                whereClauses.Add("j_EmployeeId.EmployeeGuid = @employeeGuid");
                parameters.Add("@employeeGuid", employeeGuid);
            }
            if (employeeCareerGuid != Guid.Empty)
            {
                whereClauses.Add("a.EmployeeCareerGuid = @employeeCareerGuid");
                parameters.Add("@employeeCareerGuid", employeeCareerGuid);
            }

            if (!string.IsNullOrWhiteSpace(effectiveDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.EffectiveDate", "@effectiveDate", effectiveDateSearchType, parameters, "effectiveDate", effectiveDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(referenceNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.ReferenceNo", "@referenceNo", referenceNoSearchType, parameters, "referenceNo", referenceNo, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(referenceDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ReferenceDate", "@referenceDate", referenceDateSearchType, parameters, "referenceDate", referenceDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, j_EmployeeContractId.EmployeeContractName AS EmployeeContractName
, j_FromCompanyId.FromCompanyName AS FromCompanyName
, j_FromJobGradeId.FromJobGradeName AS FromJobGradeName
, j_FromJobLevelId.FromJobLevelName AS FromJobLevelName
, j_FromJobTitleId.FromJobTitleName AS FromJobTitleName
, j_FromManagerEmployeeId.FromManagerEmployeeName AS FromManagerEmployeeName
, j_FromOrganizationUnitId.FromOrganizationUnitName AS FromOrganizationUnitName
, j_FromPositionId.FromPositionName AS FromPositionName
, j_ToCompanyId.ToCompanyName AS ToCompanyName
, j_ToJobGradeId.ToJobGradeName AS ToJobGradeName
, j_ToJobLevelId.ToJobLevelName AS ToJobLevelName
, j_ToJobTitleId.ToJobTitleName AS ToJobTitleName
, j_ToManagerEmployeeId.ToManagerEmployeeName AS ToManagerEmployeeName
, j_ToOrganizationUnitId.ToOrganizationUnitName AS ToOrganizationUnitName
, j_ToPositionId.ToPositionName AS ToPositionName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCareerEvent.StandardReferenceItemName")} AS CareerEventName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrCareerReason.StandardReferenceItemName")} AS CareerReasonName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "EmployeeAssignment")} j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
LEFT JOIN {dialect.Qualify("app", "EmployeeContract")} j_EmployeeContractId ON a.EmployeeContractId = j_EmployeeContractId.EmployeeContractId
LEFT JOIN {dialect.Qualify("app", "FromCompany")} j_FromCompanyId ON a.FromCompanyId = j_FromCompanyId.FromCompanyId
LEFT JOIN {dialect.Qualify("app", "FromJobGrade")} j_FromJobGradeId ON a.FromJobGradeId = j_FromJobGradeId.FromJobGradeId
LEFT JOIN {dialect.Qualify("app", "FromJobLevel")} j_FromJobLevelId ON a.FromJobLevelId = j_FromJobLevelId.FromJobLevelId
LEFT JOIN {dialect.Qualify("app", "FromJobTitle")} j_FromJobTitleId ON a.FromJobTitleId = j_FromJobTitleId.FromJobTitleId
LEFT JOIN {dialect.Qualify("app", "FromManagerEmployee")} j_FromManagerEmployeeId ON a.FromManagerEmployeeId = j_FromManagerEmployeeId.FromManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "FromOrganizationUnit")} j_FromOrganizationUnitId ON a.FromOrganizationUnitId = j_FromOrganizationUnitId.FromOrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "FromPosition")} j_FromPositionId ON a.FromPositionId = j_FromPositionId.FromPositionId
LEFT JOIN {dialect.Qualify("app", "ToCompany")} j_ToCompanyId ON a.ToCompanyId = j_ToCompanyId.ToCompanyId
LEFT JOIN {dialect.Qualify("app", "ToJobGrade")} j_ToJobGradeId ON a.ToJobGradeId = j_ToJobGradeId.ToJobGradeId
LEFT JOIN {dialect.Qualify("app", "ToJobLevel")} j_ToJobLevelId ON a.ToJobLevelId = j_ToJobLevelId.ToJobLevelId
LEFT JOIN {dialect.Qualify("app", "ToJobTitle")} j_ToJobTitleId ON a.ToJobTitleId = j_ToJobTitleId.ToJobTitleId
LEFT JOIN {dialect.Qualify("app", "ToManagerEmployee")} j_ToManagerEmployeeId ON a.ToManagerEmployeeId = j_ToManagerEmployeeId.ToManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "ToOrganizationUnit")} j_ToOrganizationUnitId ON a.ToOrganizationUnitId = j_ToOrganizationUnitId.ToOrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "ToPosition")} j_ToPositionId ON a.ToPositionId = j_ToPositionId.ToPositionId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCareerEvent", "SrCareerEvent", "CareerEvent")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrCareerReason", "SrCareerReason", "CareerReason")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeCareerId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<EmployeeCareer>(sql, parameters, transaction);
        }
    }
}
