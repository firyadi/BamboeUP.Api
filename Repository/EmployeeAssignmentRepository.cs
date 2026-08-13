using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class EmployeeAssignmentRepository(RepositoryContext context, IAuditService audit) : IEmployeeAssignmentRepository
    {

        public async Task<EmployeeAssignment?> GetEmployeeAssignmentAsync(Guid employeeAssignmentGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
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
, ISNULL(v_SrAssignmentReason.StandardReferenceItemName, '') AS AssignmentReasonName
, ISNULL(v_SrAssignmentType.StandardReferenceItemName, '') AS AssignmentTypeName
, ISNULL(v_SrEmploymentStatus.StandardReferenceItemName, '') AS EmploymentStatusName
, ISNULL(v_SrEmploymentType.StandardReferenceItemName, '') AS EmploymentTypeName


                FROM [app].[EmployeeAssignment] a
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN [app].[JobGrade] j_JobGradeId ON a.JobGradeId = j_JobGradeId.JobGradeId
LEFT JOIN [app].[JobLevel] j_JobLevelId ON a.JobLevelId = j_JobLevelId.JobLevelId
LEFT JOIN [app].[JobTitle] j_JobTitleId ON a.JobTitleId = j_JobTitleId.JobTitleId
LEFT JOIN [app].[ManagerEmployee] j_ManagerEmployeeId ON a.ManagerEmployeeId = j_ManagerEmployeeId.ManagerEmployeeId
LEFT JOIN [app].[OrganizationUnit] j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId
LEFT JOIN [app].[PayrollGroup] j_PayrollGroupId ON a.PayrollGroupId = j_PayrollGroupId.PayrollGroupId
LEFT JOIN [app].[Position] j_PositionId ON a.PositionId = j_PositionId.PositionId
LEFT JOIN [app].[Shift] j_ShiftId ON a.ShiftId = j_ShiftId.ShiftId
LEFT JOIN [app].[SupervisorEmployee] j_SupervisorEmployeeId ON a.SupervisorEmployeeId = j_SupervisorEmployeeId.SupervisorEmployeeId
LEFT JOIN [app].[WorkLocation] j_WorkLocationId ON a.WorkLocationId = j_WorkLocationId.WorkLocationId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AssignmentReason')
    WHERE StandardReferenceItemId = a.SrAssignmentReason
) v_SrAssignmentReason
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AssignmentType')
    WHERE StandardReferenceItemId = a.SrAssignmentType
) v_SrAssignmentType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'EmploymentStatus')
    WHERE StandardReferenceItemId = a.SrEmploymentStatus
) v_SrEmploymentStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'EmploymentType')
    WHERE StandardReferenceItemId = a.SrEmploymentType
) v_SrEmploymentType

                WHERE a.EmployeeAssignmentGuid = @employeeAssignmentGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeAssignment>(sql, new { employeeAssignmentGuid });
        }

        public async Task<EmployeeAssignment?> GetByEmployeeGuidAndEmployeeAssignmentGuidAsync(Guid employeeGuid, Guid employeeAssignmentGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
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
, ISNULL(v_SrAssignmentReason.StandardReferenceItemName, '') AS AssignmentReasonName
, ISNULL(v_SrAssignmentType.StandardReferenceItemName, '') AS AssignmentTypeName
, ISNULL(v_SrEmploymentStatus.StandardReferenceItemName, '') AS EmploymentStatusName
, ISNULL(v_SrEmploymentType.StandardReferenceItemName, '') AS EmploymentTypeName


                FROM [app].[EmployeeAssignment] a
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN [app].[JobGrade] j_JobGradeId ON a.JobGradeId = j_JobGradeId.JobGradeId
LEFT JOIN [app].[JobLevel] j_JobLevelId ON a.JobLevelId = j_JobLevelId.JobLevelId
LEFT JOIN [app].[JobTitle] j_JobTitleId ON a.JobTitleId = j_JobTitleId.JobTitleId
LEFT JOIN [app].[ManagerEmployee] j_ManagerEmployeeId ON a.ManagerEmployeeId = j_ManagerEmployeeId.ManagerEmployeeId
LEFT JOIN [app].[OrganizationUnit] j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId
LEFT JOIN [app].[PayrollGroup] j_PayrollGroupId ON a.PayrollGroupId = j_PayrollGroupId.PayrollGroupId
LEFT JOIN [app].[Position] j_PositionId ON a.PositionId = j_PositionId.PositionId
LEFT JOIN [app].[Shift] j_ShiftId ON a.ShiftId = j_ShiftId.ShiftId
LEFT JOIN [app].[SupervisorEmployee] j_SupervisorEmployeeId ON a.SupervisorEmployeeId = j_SupervisorEmployeeId.SupervisorEmployeeId
LEFT JOIN [app].[WorkLocation] j_WorkLocationId ON a.WorkLocationId = j_WorkLocationId.WorkLocationId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AssignmentReason')
    WHERE StandardReferenceItemId = a.SrAssignmentReason
) v_SrAssignmentReason
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AssignmentType')
    WHERE StandardReferenceItemId = a.SrAssignmentType
) v_SrAssignmentType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'EmploymentStatus')
    WHERE StandardReferenceItemId = a.SrEmploymentStatus
) v_SrEmploymentStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'EmploymentType')
    WHERE StandardReferenceItemId = a.SrEmploymentType
) v_SrEmploymentType

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
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
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
, ISNULL(v_SrAssignmentReason.StandardReferenceItemName, '') AS AssignmentReasonName
, ISNULL(v_SrAssignmentType.StandardReferenceItemName, '') AS AssignmentTypeName
, ISNULL(v_SrEmploymentStatus.StandardReferenceItemName, '') AS EmploymentStatusName
, ISNULL(v_SrEmploymentType.StandardReferenceItemName, '') AS EmploymentTypeName


                FROM [app].[EmployeeAssignment] a
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN [app].[JobGrade] j_JobGradeId ON a.JobGradeId = j_JobGradeId.JobGradeId
LEFT JOIN [app].[JobLevel] j_JobLevelId ON a.JobLevelId = j_JobLevelId.JobLevelId
LEFT JOIN [app].[JobTitle] j_JobTitleId ON a.JobTitleId = j_JobTitleId.JobTitleId
LEFT JOIN [app].[ManagerEmployee] j_ManagerEmployeeId ON a.ManagerEmployeeId = j_ManagerEmployeeId.ManagerEmployeeId
LEFT JOIN [app].[OrganizationUnit] j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId
LEFT JOIN [app].[PayrollGroup] j_PayrollGroupId ON a.PayrollGroupId = j_PayrollGroupId.PayrollGroupId
LEFT JOIN [app].[Position] j_PositionId ON a.PositionId = j_PositionId.PositionId
LEFT JOIN [app].[Shift] j_ShiftId ON a.ShiftId = j_ShiftId.ShiftId
LEFT JOIN [app].[SupervisorEmployee] j_SupervisorEmployeeId ON a.SupervisorEmployeeId = j_SupervisorEmployeeId.SupervisorEmployeeId
LEFT JOIN [app].[WorkLocation] j_WorkLocationId ON a.WorkLocationId = j_WorkLocationId.WorkLocationId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AssignmentReason')
    WHERE StandardReferenceItemId = a.SrAssignmentReason
) v_SrAssignmentReason
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AssignmentType')
    WHERE StandardReferenceItemId = a.SrAssignmentType
) v_SrAssignmentType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'EmploymentStatus')
    WHERE StandardReferenceItemId = a.SrEmploymentStatus
) v_SrEmploymentStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'EmploymentType')
    WHERE StandardReferenceItemId = a.SrEmploymentType
) v_SrEmploymentType

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeAssignmentId DESC";
            return await connection.QueryAsync<EmployeeAssignment>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeAssignmentAsync(EmployeeAssignment employeeAssignment, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeAssignment);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[EmployeeAssignment]
                (EmployeeAssignmentGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [CompanyId], [CompanyOfficeId], [OrganizationUnitId], [PositionId], [JobTitleId], [JobGradeId], [JobLevelId], [ManagerEmployeeId], [SupervisorEmployeeId], [CostCenterId], [PayrollGroupId], [ShiftId], [WorkLocationId], [SrEmploymentStatus], [SrEmploymentType], [SrAssignmentType], [SrAssignmentReason], [EffectiveFrom], [Effectiveto], [IsPrimary], [IsCurrent], [Remarks])
                VALUES
                (@EmployeeAssignmentGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @CompanyId, @CompanyOfficeId, @OrganizationUnitId, @PositionId, @JobTitleId, @JobGradeId, @JobLevelId, @ManagerEmployeeId, @SupervisorEmployeeId, @CostCenterId, @PayrollGroupId, @ShiftId, @WorkLocationId, @SrEmploymentStatus, @SrEmploymentType, @SrAssignmentType, @SrAssignmentReason, @EffectiveFrom, @Effectiveto, @IsPrimary, @IsCurrent, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            employeeAssignment.EmployeeAssignmentId = await conn.QuerySingleAsync<long>(sql, employeeAssignment, transaction);
        }

        public async Task UpdateEmployeeAssignmentAsync(EmployeeAssignment employeeAssignment, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeAssignment);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeAssignment]
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
            const string sql = @"
                UPDATE [app].[EmployeeAssignment]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeAssignmentGuid = @EmployeeAssignmentGuid";

            await conn.ExecuteAsync(sql, employeeAssignment, transaction);
        }

        public async Task DeleteEmployeeAssignmentAsync(Guid employeeAssignmentGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeAssignment] WHERE EmployeeAssignmentGuid = @employeeAssignmentGuid";
            await conn.ExecuteAsync(sql, new { employeeAssignmentGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeAssignment] WHERE EmployeeId IN (SELECT EmployeeId FROM [app].[Employee] WHERE EmployeeGuid = @employeeGuid)";
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
                var param = SqlFilterHelper.BuildFilter("a.EffectiveFrom", "@effectiveFrom", effectiveFromSearchType, parameters, "effectiveFrom", effectiveFrom);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(effectiveto))
            {
                var param = SqlFilterHelper.BuildFilter("a.Effectiveto", "@effectiveto", effectivetoSearchType, parameters, "effectiveto", effectiveto);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPrimary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPrimary", "@isPrimary", isPrimarySearchType, parameters, "isPrimary", isPrimary);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isCurrent))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCurrent", "@isCurrent", isCurrentSearchType, parameters, "isCurrent", isCurrent);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
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
, ISNULL(v_SrAssignmentReason.StandardReferenceItemName, '') AS AssignmentReasonName
, ISNULL(v_SrAssignmentType.StandardReferenceItemName, '') AS AssignmentTypeName
, ISNULL(v_SrEmploymentStatus.StandardReferenceItemName, '') AS EmploymentStatusName
, ISNULL(v_SrEmploymentType.StandardReferenceItemName, '') AS EmploymentTypeName


                FROM [app].[EmployeeAssignment] a
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [app].[CompanyOffice] j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN [app].[CostCenter] j_CostCenterId ON a.CostCenterId = j_CostCenterId.CostCenterId
LEFT JOIN [app].[JobGrade] j_JobGradeId ON a.JobGradeId = j_JobGradeId.JobGradeId
LEFT JOIN [app].[JobLevel] j_JobLevelId ON a.JobLevelId = j_JobLevelId.JobLevelId
LEFT JOIN [app].[JobTitle] j_JobTitleId ON a.JobTitleId = j_JobTitleId.JobTitleId
LEFT JOIN [app].[ManagerEmployee] j_ManagerEmployeeId ON a.ManagerEmployeeId = j_ManagerEmployeeId.ManagerEmployeeId
LEFT JOIN [app].[OrganizationUnit] j_OrganizationUnitId ON a.OrganizationUnitId = j_OrganizationUnitId.OrganizationUnitId
LEFT JOIN [app].[PayrollGroup] j_PayrollGroupId ON a.PayrollGroupId = j_PayrollGroupId.PayrollGroupId
LEFT JOIN [app].[Position] j_PositionId ON a.PositionId = j_PositionId.PositionId
LEFT JOIN [app].[Shift] j_ShiftId ON a.ShiftId = j_ShiftId.ShiftId
LEFT JOIN [app].[SupervisorEmployee] j_SupervisorEmployeeId ON a.SupervisorEmployeeId = j_SupervisorEmployeeId.SupervisorEmployeeId
LEFT JOIN [app].[WorkLocation] j_WorkLocationId ON a.WorkLocationId = j_WorkLocationId.WorkLocationId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AssignmentReason')
    WHERE StandardReferenceItemId = a.SrAssignmentReason
) v_SrAssignmentReason
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AssignmentType')
    WHERE StandardReferenceItemId = a.SrAssignmentType
) v_SrAssignmentType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'EmploymentStatus')
    WHERE StandardReferenceItemId = a.SrEmploymentStatus
) v_SrEmploymentStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'EmploymentType')
    WHERE StandardReferenceItemId = a.SrEmploymentType
) v_SrEmploymentType

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeAssignmentId DESC";

            return await connection.QueryAsync<EmployeeAssignment>(sql, parameters, transaction);
        }
    }
}
