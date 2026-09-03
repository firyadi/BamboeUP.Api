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
    public partial class EmployeeContractRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IEmployeeContractRepository
    {
        private readonly string _table = dialect.Qualify("app", "EmployeeContract");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<EmployeeContract?> GetEmployeeContractAsync(Guid employeeContractGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrContractStatus.StandardReferenceItemName")} AS ContractStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrContractType.StandardReferenceItemName")} AS ContractTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "EmployeeAssignment")} j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrContractStatus", "SrContractStatus", "ContractStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrContractType", "SrContractType", "ContractType")}

                WHERE a.EmployeeContractGuid = @employeeContractGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeContract>(sql, new { employeeContractGuid });
        }

        public async Task<EmployeeContract?> GetByEmployeeGuidAndEmployeeContractGuidAsync(Guid employeeGuid, Guid employeeContractGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrContractStatus.StandardReferenceItemName")} AS ContractStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrContractType.StandardReferenceItemName")} AS ContractTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "EmployeeAssignment")} j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrContractStatus", "SrContractStatus", "ContractStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrContractType", "SrContractType", "ContractType")}

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
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrContractStatus.StandardReferenceItemName")} AS ContractStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrContractType.StandardReferenceItemName")} AS ContractTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "EmployeeAssignment")} j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrContractStatus", "SrContractStatus", "ContractStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrContractType", "SrContractType", "ContractType")}

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeContractId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<EmployeeContract>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeContractAsync(EmployeeContract employeeContract, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeContract);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (EmployeeContractGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [ContractNo], [SrContractType], [SrContractStatus], [ContractStartDate], [ContractEndDate], [EffectiveDate], [RenewalSequence], [IsCurrent], [FileStorageId], [Remarks])
                VALUES
                (@EmployeeContractGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @ContractNo, @SrContractType, @SrContractStatus, @ContractStartDate, @ContractEndDate, @EffectiveDate, @RenewalSequence, @IsCurrent, @FileStorageId, @Remarks){dialect.ReturnIdentitySuffix("EmployeeContractId")}";
            employeeContract.EmployeeContractId = await conn.QuerySingleAsync<long>(sql, employeeContract, transaction);
        }

        public async Task UpdateEmployeeContractAsync(EmployeeContract employeeContract, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeContract);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
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
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeContractGuid = @EmployeeContractGuid";

            await conn.ExecuteAsync(sql, employeeContract, transaction);
        }

        public async Task DeleteEmployeeContractAsync(Guid employeeContractGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE EmployeeContractGuid = @employeeContractGuid";
            await conn.ExecuteAsync(sql, new { employeeContractGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE EmployeeId IN (SELECT EmployeeId FROM {dialect.Qualify("app", "Employee")} WHERE EmployeeGuid = @employeeGuid)";
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
                var param = SqlFilterHelper.BuildFilter("a.ContractNo", "@contractNo", contractNoSearchType, parameters, "contractNo", contractNo, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(contractStartDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ContractStartDate", "@contractStartDate", contractStartDateSearchType, parameters, "contractStartDate", contractStartDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(contractEndDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ContractEndDate", "@contractEndDate", contractEndDateSearchType, parameters, "contractEndDate", contractEndDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(effectiveDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.EffectiveDate", "@effectiveDate", effectiveDateSearchType, parameters, "effectiveDate", effectiveDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(renewalSequence))
            {
                var param = SqlFilterHelper.BuildFilter("a.RenewalSequence", "@renewalSequence", renewalSequenceSearchType, parameters, "renewalSequence", renewalSequence, dialect);
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
, j_EmployeeAssignmentId.EmployeeAssignmentName AS EmployeeAssignmentName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrContractStatus.StandardReferenceItemName")} AS ContractStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrContractType.StandardReferenceItemName")} AS ContractTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "EmployeeAssignment")} j_EmployeeAssignmentId ON a.EmployeeAssignmentId = j_EmployeeAssignmentId.EmployeeAssignmentId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrContractStatus", "SrContractStatus", "ContractStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrContractType", "SrContractType", "ContractType")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeContractId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<EmployeeContract>(sql, parameters, transaction);
        }
    }
}
