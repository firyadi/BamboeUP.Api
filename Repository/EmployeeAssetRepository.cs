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
    public partial class EmployeeAssetRepository(RepositoryContext context, IAuditService audit) : IEmployeeAssetRepository
    {

        public async Task<EmployeeAsset?> GetEmployeeAssetAsync(Guid employeeAssetGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_AssetId.AssetName AS AssetName


                FROM [app].[EmployeeAsset] a
LEFT JOIN [app].[Asset] j_AssetId ON a.AssetId = j_AssetId.AssetId

                WHERE a.EmployeeAssetGuid = @employeeAssetGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeAsset>(sql, new { employeeAssetGuid });
        }

        public async Task<EmployeeAsset?> GetByEmployeeGuidAndEmployeeAssetGuidAsync(Guid employeeGuid, Guid employeeAssetGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_AssetId.AssetName AS AssetName


                FROM [app].[EmployeeAsset] a
LEFT JOIN [app].[Asset] j_AssetId ON a.AssetId = j_AssetId.AssetId

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.EmployeeAssetGuid = @employeeAssetGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeAsset>(sql, new { employeeGuid, employeeAssetGuid });
        }

        public async Task<IEnumerable<EmployeeAsset>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_AssetId.AssetName AS AssetName


                FROM [app].[EmployeeAsset] a
LEFT JOIN [app].[Asset] j_AssetId ON a.AssetId = j_AssetId.AssetId

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeAssetId DESC";
            return await connection.QueryAsync<EmployeeAsset>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeAssetAsync(EmployeeAsset employeeAsset, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeAsset);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[EmployeeAsset]
                (EmployeeAssetGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [AssetId], [AssignedDate], [ReturnedDate], [ExpectedReturnDate], [IsCurrent], [ConditionWhenAssigned], [ConditionWhenReturned], [Remarks])
                VALUES
                (@EmployeeAssetGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @AssetId, @AssignedDate, @ReturnedDate, @ExpectedReturnDate, @IsCurrent, @ConditionWhenAssigned, @ConditionWhenReturned, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            employeeAsset.EmployeeAssetId = await conn.QuerySingleAsync<long>(sql, employeeAsset, transaction);
        }

        public async Task UpdateEmployeeAssetAsync(EmployeeAsset employeeAsset, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeAsset);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeAsset]
                SET                     [EmployeeId] = @EmployeeId,
                    [AssetId] = @AssetId,
                    [AssignedDate] = @AssignedDate,
                    [ReturnedDate] = @ReturnedDate,
                    [ExpectedReturnDate] = @ExpectedReturnDate,
                    [IsCurrent] = @IsCurrent,
                    [ConditionWhenAssigned] = @ConditionWhenAssigned,
                    [ConditionWhenReturned] = @ConditionWhenReturned,
                    [Remarks] = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE EmployeeAssetGuid = @EmployeeAssetGuid";
            await conn.ExecuteAsync(sql, employeeAsset, transaction);
        }

        public async Task SoftDeleteEmployeeAssetAsync(EmployeeAsset employeeAsset, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(employeeAsset, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeAsset]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeAssetGuid = @EmployeeAssetGuid";

            await conn.ExecuteAsync(sql, employeeAsset, transaction);
        }

        public async Task DeleteEmployeeAssetAsync(Guid employeeAssetGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeAsset] WHERE EmployeeAssetGuid = @employeeAssetGuid";
            await conn.ExecuteAsync(sql, new { employeeAssetGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeAsset] WHERE EmployeeId IN (SELECT EmployeeId FROM [app].[Employee] WHERE EmployeeGuid = @employeeGuid)";
            await conn.ExecuteAsync(sql, new { employeeGuid }, transaction);
        }

        public async Task<IEnumerable<EmployeeAsset>> SearchEmployeeAssetAsync(
            string? assignedDate,
            string? assignedDateSearchType,
            string? returnedDate,
            string? returnedDateSearchType,
            string? expectedReturnDate,
            string? expectedReturnDateSearchType,
            string? isCurrent,
            string? isCurrentSearchType,
            string? conditionWhenAssigned,
            string? conditionWhenAssignedSearchType,
            string? conditionWhenReturned,
            string? conditionWhenReturnedSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeAssetGuid,
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
            if (employeeAssetGuid != Guid.Empty)
            {
                whereClauses.Add("a.EmployeeAssetGuid = @employeeAssetGuid");
                parameters.Add("@employeeAssetGuid", employeeAssetGuid);
            }

            if (!string.IsNullOrWhiteSpace(assignedDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssignedDate", "@assignedDate", assignedDateSearchType, parameters, "assignedDate", assignedDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(returnedDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ReturnedDate", "@returnedDate", returnedDateSearchType, parameters, "returnedDate", returnedDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(expectedReturnDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ExpectedReturnDate", "@expectedReturnDate", expectedReturnDateSearchType, parameters, "expectedReturnDate", expectedReturnDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isCurrent))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCurrent", "@isCurrent", isCurrentSearchType, parameters, "isCurrent", isCurrent);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(conditionWhenAssigned))
            {
                var param = SqlFilterHelper.BuildFilter("a.ConditionWhenAssigned", "@conditionWhenAssigned", conditionWhenAssignedSearchType, parameters, "conditionWhenAssigned", conditionWhenAssigned);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(conditionWhenReturned))
            {
                var param = SqlFilterHelper.BuildFilter("a.ConditionWhenReturned", "@conditionWhenReturned", conditionWhenReturnedSearchType, parameters, "conditionWhenReturned", conditionWhenReturned);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_AssetId.AssetName AS AssetName


                FROM [app].[EmployeeAsset] a
LEFT JOIN [app].[Asset] j_AssetId ON a.AssetId = j_AssetId.AssetId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeAssetId DESC";

            return await connection.QueryAsync<EmployeeAsset>(sql, parameters, transaction);
        }
    }
}
