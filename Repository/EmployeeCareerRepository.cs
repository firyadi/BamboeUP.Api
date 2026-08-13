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
    public partial class EmployeeCareerRepository(RepositoryContext context, IAuditService audit) : IEmployeeCareerRepository
    {

        public async Task<EmployeeCareer?> GetEmployeeCareerAsync(Guid employeeCareerGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
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
, ISNULL(v_SrCareerEvent.StandardReferenceItemName, '') AS CareerEventName
, ISNULL(v_SrCareerReason.StandardReferenceItemName, '') AS CareerReasonName


                FROM [app].[EmployeeCareer] a
LEFT JOIN [app].[EmployeeAssignment] j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
LEFT JOIN [app].[EmployeeContract] j_EmployeeContractId ON a.EmployeeContractId = j_EmployeeContractId.EmployeeContractId
LEFT JOIN [app].[FromCompany] j_FromCompanyId ON a.FromCompanyId = j_FromCompanyId.FromCompanyId
LEFT JOIN [app].[FromJobGrade] j_FromJobGradeId ON a.FromJobGradeId = j_FromJobGradeId.FromJobGradeId
LEFT JOIN [app].[FromJobLevel] j_FromJobLevelId ON a.FromJobLevelId = j_FromJobLevelId.FromJobLevelId
LEFT JOIN [app].[FromJobTitle] j_FromJobTitleId ON a.FromJobTitleId = j_FromJobTitleId.FromJobTitleId
LEFT JOIN [app].[FromManagerEmployee] j_FromManagerEmployeeId ON a.FromManagerEmployeeId = j_FromManagerEmployeeId.FromManagerEmployeeId
LEFT JOIN [app].[FromOrganizationUnit] j_FromOrganizationUnitId ON a.FromOrganizationUnitId = j_FromOrganizationUnitId.FromOrganizationUnitId
LEFT JOIN [app].[FromPosition] j_FromPositionId ON a.FromPositionId = j_FromPositionId.FromPositionId
LEFT JOIN [app].[ToCompany] j_ToCompanyId ON a.ToCompanyId = j_ToCompanyId.ToCompanyId
LEFT JOIN [app].[ToJobGrade] j_ToJobGradeId ON a.ToJobGradeId = j_ToJobGradeId.ToJobGradeId
LEFT JOIN [app].[ToJobLevel] j_ToJobLevelId ON a.ToJobLevelId = j_ToJobLevelId.ToJobLevelId
LEFT JOIN [app].[ToJobTitle] j_ToJobTitleId ON a.ToJobTitleId = j_ToJobTitleId.ToJobTitleId
LEFT JOIN [app].[ToManagerEmployee] j_ToManagerEmployeeId ON a.ToManagerEmployeeId = j_ToManagerEmployeeId.ToManagerEmployeeId
LEFT JOIN [app].[ToOrganizationUnit] j_ToOrganizationUnitId ON a.ToOrganizationUnitId = j_ToOrganizationUnitId.ToOrganizationUnitId
LEFT JOIN [app].[ToPosition] j_ToPositionId ON a.ToPositionId = j_ToPositionId.ToPositionId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CareerEvent')
    WHERE StandardReferenceItemId = a.SrCareerEvent
) v_SrCareerEvent
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CareerReason')
    WHERE StandardReferenceItemId = a.SrCareerReason
) v_SrCareerReason

                WHERE a.EmployeeCareerGuid = @employeeCareerGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeCareer>(sql, new { employeeCareerGuid });
        }

        public async Task<EmployeeCareer?> GetByEmployeeGuidAndEmployeeCareerGuidAsync(Guid employeeGuid, Guid employeeCareerGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
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
, ISNULL(v_SrCareerEvent.StandardReferenceItemName, '') AS CareerEventName
, ISNULL(v_SrCareerReason.StandardReferenceItemName, '') AS CareerReasonName


                FROM [app].[EmployeeCareer] a
LEFT JOIN [app].[EmployeeAssignment] j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
LEFT JOIN [app].[EmployeeContract] j_EmployeeContractId ON a.EmployeeContractId = j_EmployeeContractId.EmployeeContractId
LEFT JOIN [app].[FromCompany] j_FromCompanyId ON a.FromCompanyId = j_FromCompanyId.FromCompanyId
LEFT JOIN [app].[FromJobGrade] j_FromJobGradeId ON a.FromJobGradeId = j_FromJobGradeId.FromJobGradeId
LEFT JOIN [app].[FromJobLevel] j_FromJobLevelId ON a.FromJobLevelId = j_FromJobLevelId.FromJobLevelId
LEFT JOIN [app].[FromJobTitle] j_FromJobTitleId ON a.FromJobTitleId = j_FromJobTitleId.FromJobTitleId
LEFT JOIN [app].[FromManagerEmployee] j_FromManagerEmployeeId ON a.FromManagerEmployeeId = j_FromManagerEmployeeId.FromManagerEmployeeId
LEFT JOIN [app].[FromOrganizationUnit] j_FromOrganizationUnitId ON a.FromOrganizationUnitId = j_FromOrganizationUnitId.FromOrganizationUnitId
LEFT JOIN [app].[FromPosition] j_FromPositionId ON a.FromPositionId = j_FromPositionId.FromPositionId
LEFT JOIN [app].[ToCompany] j_ToCompanyId ON a.ToCompanyId = j_ToCompanyId.ToCompanyId
LEFT JOIN [app].[ToJobGrade] j_ToJobGradeId ON a.ToJobGradeId = j_ToJobGradeId.ToJobGradeId
LEFT JOIN [app].[ToJobLevel] j_ToJobLevelId ON a.ToJobLevelId = j_ToJobLevelId.ToJobLevelId
LEFT JOIN [app].[ToJobTitle] j_ToJobTitleId ON a.ToJobTitleId = j_ToJobTitleId.ToJobTitleId
LEFT JOIN [app].[ToManagerEmployee] j_ToManagerEmployeeId ON a.ToManagerEmployeeId = j_ToManagerEmployeeId.ToManagerEmployeeId
LEFT JOIN [app].[ToOrganizationUnit] j_ToOrganizationUnitId ON a.ToOrganizationUnitId = j_ToOrganizationUnitId.ToOrganizationUnitId
LEFT JOIN [app].[ToPosition] j_ToPositionId ON a.ToPositionId = j_ToPositionId.ToPositionId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CareerEvent')
    WHERE StandardReferenceItemId = a.SrCareerEvent
) v_SrCareerEvent
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CareerReason')
    WHERE StandardReferenceItemId = a.SrCareerReason
) v_SrCareerReason

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
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
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
, ISNULL(v_SrCareerEvent.StandardReferenceItemName, '') AS CareerEventName
, ISNULL(v_SrCareerReason.StandardReferenceItemName, '') AS CareerReasonName


                FROM [app].[EmployeeCareer] a
LEFT JOIN [app].[EmployeeAssignment] j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
LEFT JOIN [app].[EmployeeContract] j_EmployeeContractId ON a.EmployeeContractId = j_EmployeeContractId.EmployeeContractId
LEFT JOIN [app].[FromCompany] j_FromCompanyId ON a.FromCompanyId = j_FromCompanyId.FromCompanyId
LEFT JOIN [app].[FromJobGrade] j_FromJobGradeId ON a.FromJobGradeId = j_FromJobGradeId.FromJobGradeId
LEFT JOIN [app].[FromJobLevel] j_FromJobLevelId ON a.FromJobLevelId = j_FromJobLevelId.FromJobLevelId
LEFT JOIN [app].[FromJobTitle] j_FromJobTitleId ON a.FromJobTitleId = j_FromJobTitleId.FromJobTitleId
LEFT JOIN [app].[FromManagerEmployee] j_FromManagerEmployeeId ON a.FromManagerEmployeeId = j_FromManagerEmployeeId.FromManagerEmployeeId
LEFT JOIN [app].[FromOrganizationUnit] j_FromOrganizationUnitId ON a.FromOrganizationUnitId = j_FromOrganizationUnitId.FromOrganizationUnitId
LEFT JOIN [app].[FromPosition] j_FromPositionId ON a.FromPositionId = j_FromPositionId.FromPositionId
LEFT JOIN [app].[ToCompany] j_ToCompanyId ON a.ToCompanyId = j_ToCompanyId.ToCompanyId
LEFT JOIN [app].[ToJobGrade] j_ToJobGradeId ON a.ToJobGradeId = j_ToJobGradeId.ToJobGradeId
LEFT JOIN [app].[ToJobLevel] j_ToJobLevelId ON a.ToJobLevelId = j_ToJobLevelId.ToJobLevelId
LEFT JOIN [app].[ToJobTitle] j_ToJobTitleId ON a.ToJobTitleId = j_ToJobTitleId.ToJobTitleId
LEFT JOIN [app].[ToManagerEmployee] j_ToManagerEmployeeId ON a.ToManagerEmployeeId = j_ToManagerEmployeeId.ToManagerEmployeeId
LEFT JOIN [app].[ToOrganizationUnit] j_ToOrganizationUnitId ON a.ToOrganizationUnitId = j_ToOrganizationUnitId.ToOrganizationUnitId
LEFT JOIN [app].[ToPosition] j_ToPositionId ON a.ToPositionId = j_ToPositionId.ToPositionId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CareerEvent')
    WHERE StandardReferenceItemId = a.SrCareerEvent
) v_SrCareerEvent
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CareerReason')
    WHERE StandardReferenceItemId = a.SrCareerReason
) v_SrCareerReason

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeCareerId DESC";
            return await connection.QueryAsync<EmployeeCareer>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeCareerAsync(EmployeeCareer employeeCareer, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeCareer);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[EmployeeCareer]
                (EmployeeCareerGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [EmployeeAssignmentId], [EmployeeContractId], [SrCareerEvent], [SrCareerReason], [EffectiveDate], [FromCompanyId], [FromOrganizationUnitId], [FromPositionId], [FromJobTitleId], [FromJobGradeId], [FromJobLevelId], [FromManagerEmployeeId], [ToCompanyId], [ToOrganizationUnitId], [ToPositionId], [ToJobTitleId], [ToJobGradeId], [ToJobLevelId], [ToManagerEmployeeId], [ReferenceNo], [ReferenceDate], [Remarks])
                VALUES
                (@EmployeeCareerGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @EmployeeAssignmentId, @EmployeeContractId, @SrCareerEvent, @SrCareerReason, @EffectiveDate, @FromCompanyId, @FromOrganizationUnitId, @FromPositionId, @FromJobTitleId, @FromJobGradeId, @FromJobLevelId, @FromManagerEmployeeId, @ToCompanyId, @ToOrganizationUnitId, @ToPositionId, @ToJobTitleId, @ToJobGradeId, @ToJobLevelId, @ToManagerEmployeeId, @ReferenceNo, @ReferenceDate, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            employeeCareer.EmployeeCareerId = await conn.QuerySingleAsync<long>(sql, employeeCareer, transaction);
        }

        public async Task UpdateEmployeeCareerAsync(EmployeeCareer employeeCareer, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeCareer);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeCareer]
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
            const string sql = @"
                UPDATE [app].[EmployeeCareer]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeCareerGuid = @EmployeeCareerGuid";

            await conn.ExecuteAsync(sql, employeeCareer, transaction);
        }

        public async Task DeleteEmployeeCareerAsync(Guid employeeCareerGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeCareer] WHERE EmployeeCareerGuid = @employeeCareerGuid";
            await conn.ExecuteAsync(sql, new { employeeCareerGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeCareer] WHERE EmployeeId IN (SELECT EmployeeId FROM [app].[Employee] WHERE EmployeeGuid = @employeeGuid)";
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
                var param = SqlFilterHelper.BuildFilter("a.EffectiveDate", "@effectiveDate", effectiveDateSearchType, parameters, "effectiveDate", effectiveDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(referenceNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.ReferenceNo", "@referenceNo", referenceNoSearchType, parameters, "referenceNo", referenceNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(referenceDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ReferenceDate", "@referenceDate", referenceDateSearchType, parameters, "referenceDate", referenceDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
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
, ISNULL(v_SrCareerEvent.StandardReferenceItemName, '') AS CareerEventName
, ISNULL(v_SrCareerReason.StandardReferenceItemName, '') AS CareerReasonName


                FROM [app].[EmployeeCareer] a
LEFT JOIN [app].[EmployeeAssignment] j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
LEFT JOIN [app].[EmployeeContract] j_EmployeeContractId ON a.EmployeeContractId = j_EmployeeContractId.EmployeeContractId
LEFT JOIN [app].[FromCompany] j_FromCompanyId ON a.FromCompanyId = j_FromCompanyId.FromCompanyId
LEFT JOIN [app].[FromJobGrade] j_FromJobGradeId ON a.FromJobGradeId = j_FromJobGradeId.FromJobGradeId
LEFT JOIN [app].[FromJobLevel] j_FromJobLevelId ON a.FromJobLevelId = j_FromJobLevelId.FromJobLevelId
LEFT JOIN [app].[FromJobTitle] j_FromJobTitleId ON a.FromJobTitleId = j_FromJobTitleId.FromJobTitleId
LEFT JOIN [app].[FromManagerEmployee] j_FromManagerEmployeeId ON a.FromManagerEmployeeId = j_FromManagerEmployeeId.FromManagerEmployeeId
LEFT JOIN [app].[FromOrganizationUnit] j_FromOrganizationUnitId ON a.FromOrganizationUnitId = j_FromOrganizationUnitId.FromOrganizationUnitId
LEFT JOIN [app].[FromPosition] j_FromPositionId ON a.FromPositionId = j_FromPositionId.FromPositionId
LEFT JOIN [app].[ToCompany] j_ToCompanyId ON a.ToCompanyId = j_ToCompanyId.ToCompanyId
LEFT JOIN [app].[ToJobGrade] j_ToJobGradeId ON a.ToJobGradeId = j_ToJobGradeId.ToJobGradeId
LEFT JOIN [app].[ToJobLevel] j_ToJobLevelId ON a.ToJobLevelId = j_ToJobLevelId.ToJobLevelId
LEFT JOIN [app].[ToJobTitle] j_ToJobTitleId ON a.ToJobTitleId = j_ToJobTitleId.ToJobTitleId
LEFT JOIN [app].[ToManagerEmployee] j_ToManagerEmployeeId ON a.ToManagerEmployeeId = j_ToManagerEmployeeId.ToManagerEmployeeId
LEFT JOIN [app].[ToOrganizationUnit] j_ToOrganizationUnitId ON a.ToOrganizationUnitId = j_ToOrganizationUnitId.ToOrganizationUnitId
LEFT JOIN [app].[ToPosition] j_ToPositionId ON a.ToPositionId = j_ToPositionId.ToPositionId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CareerEvent')
    WHERE StandardReferenceItemId = a.SrCareerEvent
) v_SrCareerEvent
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'CareerReason')
    WHERE StandardReferenceItemId = a.SrCareerReason
) v_SrCareerReason

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeCareerId DESC";

            return await connection.QueryAsync<EmployeeCareer>(sql, parameters, transaction);
        }
    }
}
