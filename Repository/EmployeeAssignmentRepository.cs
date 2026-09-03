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
    public partial class EmployeeAssignmentRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IEmployeeAssignmentRepository
    {
        private readonly string _table = dialect.Qualify("app", "EmployeeAssignment");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<EmployeeAssignment?> GetEmployeeAssignmentAsync(Guid employeeAssignmentGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterId.CostCenterName AS CostCenterName
, j_JobGradeId.JobGradeName AS JobGradeName
, j_JobLevelId.JobLevelName AS JobLevelName
, j_JobTitleId.JobTitleName AS JobTitleName
, j_ManagerEmployeeId.ManagerEmployeeName AS ManagerEmployeeName
, j_OrganizationUnitId.OrganizationUnitName AS OrganizationUnitName
, j_PayrollGroupId.PayrollGroupName AS PayrollGroupName
, j_PositionId.PositionName AS PositionName
, j_ShiftId.ShiftName AS ShiftName
, j_SupervisorEmployeeId.SupervisorEmployeeName AS SupervisorEmployeeName
, j_WorkLocationId.WorkLocationName AS WorkLocationName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAssignmentReason.StandardReferenceItemName")} AS AssignmentReasonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAssignmentType.StandardReferenceItemName")} AS AssignmentTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentStatus.StandardReferenceItemName")} AS EmploymentStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentType.StandardReferenceItemName")} AS EmploymentTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "CostCenter")} j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN {dialect.Qualify("app", "JobGrade")} j_JobGradeId ON a.JobGradeId = j_JobGradeId.JobGradeId
LEFT JOIN {dialect.Qualify("app", "JobLevel")} j_JobLevelId ON a.JobLevelId = j_JobLevelId.JobLevelId
LEFT JOIN {dialect.Qualify("app", "JobTitle")} j_JobTitleId ON a.JobTitleId = j_JobTitleId.JobTitleId
LEFT JOIN {dialect.Qualify("app", "ManagerEmployee")} j_ManagerEmployeeId ON a.ManagerEmployeeId = j_ManagerEmployeeId.ManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "OrganizationUnit")} j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "PayrollGroup")} j_PayrollGroupId ON a.PayrollGroupId = j_PayrollGroupId.PayrollGroupId
LEFT JOIN {dialect.Qualify("app", "Position")} j_PositionId ON a.PositionId = j_PositionId.PositionId
LEFT JOIN {dialect.Qualify("app", "Shift")} j_ShiftId ON a.ShiftId = j_ShiftId.ShiftId
LEFT JOIN {dialect.Qualify("app", "SupervisorEmployee")} j_SupervisorEmployeeId ON a.SupervisorEmployeeId = j_SupervisorEmployeeId.SupervisorEmployeeId
LEFT JOIN {dialect.Qualify("app", "WorkLocation")} j_WorkLocationId ON a.WorkLocationId = j_WorkLocationId.WorkLocationId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAssignmentReason", "SrAssignmentReason", "AssignmentReason")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAssignmentType", "SrAssignmentType", "AssignmentType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentStatus", "SrEmploymentStatus", "EmploymentStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentType", "SrEmploymentType", "EmploymentType")}

                WHERE a.EmployeeAssignmentGuid = @employeeAssignmentGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeAssignment>(sql, new { employeeAssignmentGuid });
        }

        public async Task<EmployeeAssignment?> GetByEmployeeGuidAndEmployeeAssignmentGuidAsync(Guid employeeGuid, Guid employeeAssignmentGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterId.CostCenterName AS CostCenterName
, j_JobGradeId.JobGradeName AS JobGradeName
, j_JobLevelId.JobLevelName AS JobLevelName
, j_JobTitleId.JobTitleName AS JobTitleName
, j_ManagerEmployeeId.ManagerEmployeeName AS ManagerEmployeeName
, j_OrganizationUnitId.OrganizationUnitName AS OrganizationUnitName
, j_PayrollGroupId.PayrollGroupName AS PayrollGroupName
, j_PositionId.PositionName AS PositionName
, j_ShiftId.ShiftName AS ShiftName
, j_SupervisorEmployeeId.SupervisorEmployeeName AS SupervisorEmployeeName
, j_WorkLocationId.WorkLocationName AS WorkLocationName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAssignmentReason.StandardReferenceItemName")} AS AssignmentReasonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAssignmentType.StandardReferenceItemName")} AS AssignmentTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentStatus.StandardReferenceItemName")} AS EmploymentStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentType.StandardReferenceItemName")} AS EmploymentTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "CostCenter")} j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN {dialect.Qualify("app", "JobGrade")} j_JobGradeId ON a.JobGradeId = j_JobGradeId.JobGradeId
LEFT JOIN {dialect.Qualify("app", "JobLevel")} j_JobLevelId ON a.JobLevelId = j_JobLevelId.JobLevelId
LEFT JOIN {dialect.Qualify("app", "JobTitle")} j_JobTitleId ON a.JobTitleId = j_JobTitleId.JobTitleId
LEFT JOIN {dialect.Qualify("app", "ManagerEmployee")} j_ManagerEmployeeId ON a.ManagerEmployeeId = j_ManagerEmployeeId.ManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "OrganizationUnit")} j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "PayrollGroup")} j_PayrollGroupId ON a.PayrollGroupId = j_PayrollGroupId.PayrollGroupId
LEFT JOIN {dialect.Qualify("app", "Position")} j_PositionId ON a.PositionId = j_PositionId.PositionId
LEFT JOIN {dialect.Qualify("app", "Shift")} j_ShiftId ON a.ShiftId = j_ShiftId.ShiftId
LEFT JOIN {dialect.Qualify("app", "SupervisorEmployee")} j_SupervisorEmployeeId ON a.SupervisorEmployeeId = j_SupervisorEmployeeId.SupervisorEmployeeId
LEFT JOIN {dialect.Qualify("app", "WorkLocation")} j_WorkLocationId ON a.WorkLocationId = j_WorkLocationId.WorkLocationId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAssignmentReason", "SrAssignmentReason", "AssignmentReason")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAssignmentType", "SrAssignmentType", "AssignmentType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentStatus", "SrEmploymentStatus", "EmploymentStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentType", "SrEmploymentType", "EmploymentType")}

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.EmployeeAssignmentGuid = @employeeAssignmentGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeAssignment>(sql, new { employeeGuid, employeeAssignmentGuid });
        }

        public async Task<IEnumerable<EmployeeAssignment>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterId.CostCenterName AS CostCenterName
, j_JobGradeId.JobGradeName AS JobGradeName
, j_JobLevelId.JobLevelName AS JobLevelName
, j_JobTitleId.JobTitleName AS JobTitleName
, j_ManagerEmployeeId.ManagerEmployeeName AS ManagerEmployeeName
, j_OrganizationUnitId.OrganizationUnitName AS OrganizationUnitName
, j_PayrollGroupId.PayrollGroupName AS PayrollGroupName
, j_PositionId.PositionName AS PositionName
, j_ShiftId.ShiftName AS ShiftName
, j_SupervisorEmployeeId.SupervisorEmployeeName AS SupervisorEmployeeName
, j_WorkLocationId.WorkLocationName AS WorkLocationName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAssignmentReason.StandardReferenceItemName")} AS AssignmentReasonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAssignmentType.StandardReferenceItemName")} AS AssignmentTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentStatus.StandardReferenceItemName")} AS EmploymentStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentType.StandardReferenceItemName")} AS EmploymentTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "CostCenter")} j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN {dialect.Qualify("app", "JobGrade")} j_JobGradeId ON a.JobGradeId = j_JobGradeId.JobGradeId
LEFT JOIN {dialect.Qualify("app", "JobLevel")} j_JobLevelId ON a.JobLevelId = j_JobLevelId.JobLevelId
LEFT JOIN {dialect.Qualify("app", "JobTitle")} j_JobTitleId ON a.JobTitleId = j_JobTitleId.JobTitleId
LEFT JOIN {dialect.Qualify("app", "ManagerEmployee")} j_ManagerEmployeeId ON a.ManagerEmployeeId = j_ManagerEmployeeId.ManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "OrganizationUnit")} j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "PayrollGroup")} j_PayrollGroupId ON a.PayrollGroupId = j_PayrollGroupId.PayrollGroupId
LEFT JOIN {dialect.Qualify("app", "Position")} j_PositionId ON a.PositionId = j_PositionId.PositionId
LEFT JOIN {dialect.Qualify("app", "Shift")} j_ShiftId ON a.ShiftId = j_ShiftId.ShiftId
LEFT JOIN {dialect.Qualify("app", "SupervisorEmployee")} j_SupervisorEmployeeId ON a.SupervisorEmployeeId = j_SupervisorEmployeeId.SupervisorEmployeeId
LEFT JOIN {dialect.Qualify("app", "WorkLocation")} j_WorkLocationId ON a.WorkLocationId = j_WorkLocationId.WorkLocationId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAssignmentReason", "SrAssignmentReason", "AssignmentReason")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAssignmentType", "SrAssignmentType", "AssignmentType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentStatus", "SrEmploymentStatus", "EmploymentStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentType", "SrEmploymentType", "EmploymentType")}

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeAssignmentId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<EmployeeAssignment>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeAssignmentAsync(EmployeeAssignment employeeAssignment, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeAssignment);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (EmployeeAssignmentGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [CompanyId], [CompanyOfficeId], [OrganizationUnitId], [PositionId], [JobTitleId], [JobGradeId], [JobLevelId], [ManagerEmployeeId], [SupervisorEmployeeId], [CostCenterId], [PayrollGroupId], [ShiftId], [WorkLocationId], [SrEmploymentStatus], [SrEmploymentType], [SrAssignmentType], [SrAssignmentReason], [EffectiveFrom], [Effectiveto], [IsPrimary], [IsCurrent], [Remarks])
                VALUES
                (@EmployeeAssignmentGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @CompanyId, @CompanyOfficeId, @OrganizationUnitId, @PositionId, @JobTitleId, @JobGradeId, @JobLevelId, @ManagerEmployeeId, @SupervisorEmployeeId, @CostCenterId, @PayrollGroupId, @ShiftId, @WorkLocationId, @SrEmploymentStatus, @SrEmploymentType, @SrAssignmentType, @SrAssignmentReason, @EffectiveFrom, @Effectiveto, @IsPrimary, @IsCurrent, @Remarks){dialect.ReturnIdentitySuffix("EmployeeAssignmentId")}";
            employeeAssignment.EmployeeAssignmentId = await conn.QuerySingleAsync<long>(sql, employeeAssignment, transaction);
        }

        public async Task UpdateEmployeeAssignmentAsync(EmployeeAssignment employeeAssignment, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeAssignment);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [EmployeeId] = @EmployeeId,
                    [CompanyId] = @CompanyId,
                    [CompanyOfficeId] = @CompanyOfficeId,
                    [OrganizationUnitId] = @OrganizationUnitId,
                    [PositionId] = @PositionId,
                    [JobTitleId] = @JobTitleId,
                    [JobGradeId] = @JobGradeId,
                    [JobLevelId] = @JobLevelId,
                    [ManagerEmployeeId] = @ManagerEmployeeId,
                    [SupervisorEmployeeId] = @SupervisorEmployeeId,
                    [CostCenterId] = @CostCenterId,
                    [PayrollGroupId] = @PayrollGroupId,
                    [ShiftId] = @ShiftId,
                    [WorkLocationId] = @WorkLocationId,
                    [SrEmploymentStatus] = @SrEmploymentStatus,
                    [SrEmploymentType] = @SrEmploymentType,
                    [SrAssignmentType] = @SrAssignmentType,
                    [SrAssignmentReason] = @SrAssignmentReason,
                    [EffectiveFrom] = @EffectiveFrom,
                    [Effectiveto] = @Effectiveto,
                    [IsPrimary] = @IsPrimary,
                    [IsCurrent] = @IsCurrent,
                    [Remarks] = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE EmployeeAssignmentGuid = @EmployeeAssignmentGuid";
            await conn.ExecuteAsync(sql, employeeAssignment, transaction);
        }

        public async Task SoftDeleteEmployeeAssignmentAsync(EmployeeAssignment employeeAssignment, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(employeeAssignment, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeAssignmentGuid = @EmployeeAssignmentGuid";

            await conn.ExecuteAsync(sql, employeeAssignment, transaction);
        }

        public async Task DeleteEmployeeAssignmentAsync(Guid employeeAssignmentGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE EmployeeAssignmentGuid = @employeeAssignmentGuid";
            await conn.ExecuteAsync(sql, new { employeeAssignmentGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE EmployeeId IN (SELECT EmployeeId FROM {dialect.Qualify("app", "Employee")} WHERE EmployeeGuid = @employeeGuid)";
            await conn.ExecuteAsync(sql, new { employeeGuid }, transaction);
        }

        public async Task<IEnumerable<EmployeeAssignment>> SearchEmployeeAssignmentAsync(
            string? effectiveFrom,
            string? effectiveFromSearchType,
            string? effectiveto,
            string? effectivetoSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            string? isCurrent,
            string? isCurrentSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeAssignmentGuid,
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
            if (employeeAssignmentGuid != Guid.Empty)
            {
                whereClauses.Add("a.EmployeeAssignmentGuid = @employeeAssignmentGuid");
                parameters.Add("@employeeAssignmentGuid", employeeAssignmentGuid);
            }

            if (!string.IsNullOrWhiteSpace(effectiveFrom))
            {
                var param = SqlFilterHelper.BuildFilter("a.EffectiveFrom", "@effectiveFrom", effectiveFromSearchType, parameters, "effectiveFrom", effectiveFrom, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(effectiveto))
            {
                var param = SqlFilterHelper.BuildFilter("a.Effectiveto", "@effectiveto", effectivetoSearchType, parameters, "effectiveto", effectiveto, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPrimary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPrimary", "@isPrimary", isPrimarySearchType, parameters, "isPrimary", isPrimary, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isCurrent))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCurrent", "@isCurrent", isCurrentSearchType, parameters, "isCurrent", isCurrent, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_CostCenterId.CostCenterName AS CostCenterName
, j_JobGradeId.JobGradeName AS JobGradeName
, j_JobLevelId.JobLevelName AS JobLevelName
, j_JobTitleId.JobTitleName AS JobTitleName
, j_ManagerEmployeeId.ManagerEmployeeName AS ManagerEmployeeName
, j_OrganizationUnitId.OrganizationUnitName AS OrganizationUnitName
, j_PayrollGroupId.PayrollGroupName AS PayrollGroupName
, j_PositionId.PositionName AS PositionName
, j_ShiftId.ShiftName AS ShiftName
, j_SupervisorEmployeeId.SupervisorEmployeeName AS SupervisorEmployeeName
, j_WorkLocationId.WorkLocationName AS WorkLocationName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAssignmentReason.StandardReferenceItemName")} AS AssignmentReasonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAssignmentType.StandardReferenceItemName")} AS AssignmentTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentStatus.StandardReferenceItemName")} AS EmploymentStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentType.StandardReferenceItemName")} AS EmploymentTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "CostCenter")} j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN {dialect.Qualify("app", "JobGrade")} j_JobGradeId ON a.JobGradeId = j_JobGradeId.JobGradeId
LEFT JOIN {dialect.Qualify("app", "JobLevel")} j_JobLevelId ON a.JobLevelId = j_JobLevelId.JobLevelId
LEFT JOIN {dialect.Qualify("app", "JobTitle")} j_JobTitleId ON a.JobTitleId = j_JobTitleId.JobTitleId
LEFT JOIN {dialect.Qualify("app", "ManagerEmployee")} j_ManagerEmployeeId ON a.ManagerEmployeeId = j_ManagerEmployeeId.ManagerEmployeeId
LEFT JOIN {dialect.Qualify("app", "OrganizationUnit")} j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId
LEFT JOIN {dialect.Qualify("app", "PayrollGroup")} j_PayrollGroupId ON a.PayrollGroupId = j_PayrollGroupId.PayrollGroupId
LEFT JOIN {dialect.Qualify("app", "Position")} j_PositionId ON a.PositionId = j_PositionId.PositionId
LEFT JOIN {dialect.Qualify("app", "Shift")} j_ShiftId ON a.ShiftId = j_ShiftId.ShiftId
LEFT JOIN {dialect.Qualify("app", "SupervisorEmployee")} j_SupervisorEmployeeId ON a.SupervisorEmployeeId = j_SupervisorEmployeeId.SupervisorEmployeeId
LEFT JOIN {dialect.Qualify("app", "WorkLocation")} j_WorkLocationId ON a.WorkLocationId = j_WorkLocationId.WorkLocationId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAssignmentReason", "SrAssignmentReason", "AssignmentReason")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAssignmentType", "SrAssignmentType", "AssignmentType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentStatus", "SrEmploymentStatus", "EmploymentStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentType", "SrEmploymentType", "EmploymentType")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeAssignmentId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<EmployeeAssignment>(sql, parameters, transaction);
        }
    }
}
