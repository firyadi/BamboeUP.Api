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
    public partial class EmployeeContractRepository(RepositoryContext context, IAuditService audit) : IEmployeeContractRepository
    {

        public async Task<EmployeeContract?> GetEmployeeContractAsync(Guid employeeContractGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, ISNULL(v_SrContractStatus.StandardReferenceItemName, '') AS ContractStatusName
, ISNULL(v_SrContractType.StandardReferenceItemName, '') AS ContractTypeName


                FROM [app].[EmployeeContract] a
LEFT JOIN [app].[EmployeeAssignment] j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'ContractStatus')
    WHERE StandardReferenceItemId = a.SrContractStatus
) v_SrContractStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'ContractType')
    WHERE StandardReferenceItemId = a.SrContractType
) v_SrContractType

                WHERE a.EmployeeContractGuid = @employeeContractGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeContract>(sql, new { employeeContractGuid });
        }

        public async Task<EmployeeContract?> GetByEmployeeGuidAndEmployeeContractGuidAsync(Guid employeeGuid, Guid employeeContractGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, ISNULL(v_SrContractStatus.StandardReferenceItemName, '') AS ContractStatusName
, ISNULL(v_SrContractType.StandardReferenceItemName, '') AS ContractTypeName


                FROM [app].[EmployeeContract] a
LEFT JOIN [app].[EmployeeAssignment] j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'ContractStatus')
    WHERE StandardReferenceItemId = a.SrContractStatus
) v_SrContractStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'ContractType')
    WHERE StandardReferenceItemId = a.SrContractType
) v_SrContractType

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.EmployeeContractGuid = @employeeContractGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeContract>(sql, new { employeeGuid, employeeContractGuid });
        }

        public async Task<IEnumerable<EmployeeContract>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, ISNULL(v_SrContractStatus.StandardReferenceItemName, '') AS ContractStatusName
, ISNULL(v_SrContractType.StandardReferenceItemName, '') AS ContractTypeName


                FROM [app].[EmployeeContract] a
LEFT JOIN [app].[EmployeeAssignment] j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'ContractStatus')
    WHERE StandardReferenceItemId = a.SrContractStatus
) v_SrContractStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'ContractType')
    WHERE StandardReferenceItemId = a.SrContractType
) v_SrContractType

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeContractId DESC";
            return await connection.QueryAsync<EmployeeContract>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeContractAsync(EmployeeContract employeeContract, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeContract);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[EmployeeContract]
                (EmployeeContractGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [ContractNo], [SrContractType], [SrContractStatus], [ContractStartDate], [ContractEndDate], [EffectiveDate], [RenewalSequence], [IsCurrent], [FileStorageId], [Remarks])
                VALUES
                (@EmployeeContractGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @ContractNo, @SrContractType, @SrContractStatus, @ContractStartDate, @ContractEndDate, @EffectiveDate, @RenewalSequence, @IsCurrent, @FileStorageId, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            employeeContract.EmployeeContractId = await conn.QuerySingleAsync<long>(sql, employeeContract, transaction);
        }

        public async Task UpdateEmployeeContractAsync(EmployeeContract employeeContract, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeContract);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeContract]
                SET                     [EmployeeId] = @EmployeeId,
                    [ContractNo] = @ContractNo,
                    [SrContractType] = @SrContractType,
                    [SrContractStatus] = @SrContractStatus,
                    [ContractStartDate] = @ContractStartDate,
                    [ContractEndDate] = @ContractEndDate,
                    [EffectiveDate] = @EffectiveDate,
                    [RenewalSequence] = @RenewalSequence,
                    [IsCurrent] = @IsCurrent,
                    [FileStorageId] = @FileStorageId,
                    [Remarks] = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE EmployeeContractGuid = @EmployeeContractGuid";
            await conn.ExecuteAsync(sql, employeeContract, transaction);
        }

        public async Task SoftDeleteEmployeeContractAsync(EmployeeContract employeeContract, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(employeeContract, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeContract]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeContractGuid = @EmployeeContractGuid";

            await conn.ExecuteAsync(sql, employeeContract, transaction);
        }

        public async Task DeleteEmployeeContractAsync(Guid employeeContractGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeContract] WHERE EmployeeContractGuid = @employeeContractGuid";
            await conn.ExecuteAsync(sql, new { employeeContractGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeContract] WHERE EmployeeId IN (SELECT EmployeeId FROM [app].[Employee] WHERE EmployeeGuid = @employeeGuid)";
            await conn.ExecuteAsync(sql, new { employeeGuid }, transaction);
        }

        public async Task<IEnumerable<EmployeeContract>> SearchEmployeeContractAsync(
            string? contractNo,
            string? contractNoSearchType,
            string? contractStartDate,
            string? contractStartDateSearchType,
            string? contractEndDate,
            string? contractEndDateSearchType,
            string? effectiveDate,
            string? effectiveDateSearchType,
            string? renewalSequence,
            string? renewalSequenceSearchType,
            string? isCurrent,
            string? isCurrentSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeContractGuid,
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
            if (employeeContractGuid != Guid.Empty)
            {
                whereClauses.Add("a.EmployeeContractGuid = @employeeContractGuid");
                parameters.Add("@employeeContractGuid", employeeContractGuid);
            }

            if (!string.IsNullOrWhiteSpace(contractNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.ContractNo", "@contractNo", contractNoSearchType, parameters, "contractNo", contractNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(contractStartDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ContractStartDate", "@contractStartDate", contractStartDateSearchType, parameters, "contractStartDate", contractStartDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(contractEndDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ContractEndDate", "@contractEndDate", contractEndDateSearchType, parameters, "contractEndDate", contractEndDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(effectiveDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.EffectiveDate", "@effectiveDate", effectiveDateSearchType, parameters, "effectiveDate", effectiveDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(renewalSequence))
            {
                var param = SqlFilterHelper.BuildFilter("a.RenewalSequence", "@renewalSequence", renewalSequenceSearchType, parameters, "renewalSequence", renewalSequence);
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
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, ISNULL(v_SrContractStatus.StandardReferenceItemName, '') AS ContractStatusName
, ISNULL(v_SrContractType.StandardReferenceItemName, '') AS ContractTypeName


                FROM [app].[EmployeeContract] a
LEFT JOIN [app].[EmployeeAssignment] j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'ContractStatus')
    WHERE StandardReferenceItemId = a.SrContractStatus
) v_SrContractStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'ContractType')
    WHERE StandardReferenceItemId = a.SrContractType
) v_SrContractType

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeContractId DESC";

            return await connection.QueryAsync<EmployeeContract>(sql, parameters, transaction);
        }
    }
}
