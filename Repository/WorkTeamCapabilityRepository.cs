using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class WorkTeamCapabilityRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IWorkTeamCapabilityRepository
    {
        private readonly string _table = dialect.Qualify("app", "WorkTeamCapability");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<WorkTeamCapability?> GetWorkTeamCapabilityAsync(Guid workTeamCapabilityGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_WorkCapabilityId.CapabilityName AS WorkCapabilityName
, j_WorkTeamId.WorkTeamName AS WorkTeamName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "WorkCapability")} j_WorkCapabilityId ON a.WorkCapabilityId = j_WorkCapabilityId.WorkCapabilityId
LEFT JOIN {dialect.Qualify("app", "WorkTeam")} j_WorkTeamId ON a.WorkTeamId = j_WorkTeamId.WorkTeamId

                WHERE a.WorkTeamCapabilityGuid = @workTeamCapabilityGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<WorkTeamCapability>(sql, new { workTeamCapabilityGuid });
        }

        public async Task<WorkTeamCapability?> GetByWorkTeamGuidAndWorkTeamCapabilityGuidAsync(Guid workTeamGuid, Guid workTeamCapabilityGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_WorkCapabilityId.CapabilityName AS WorkCapabilityName
, j_WorkTeamId.WorkTeamName AS WorkTeamName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "WorkCapability")} j_WorkCapabilityId ON a.WorkCapabilityId = j_WorkCapabilityId.WorkCapabilityId
LEFT JOIN {dialect.Qualify("app", "WorkTeam")} j_WorkTeamId ON a.WorkTeamId = j_WorkTeamId.WorkTeamId

                WHERE j_WorkTeamId.WorkTeamGuid = @workTeamGuid
                  AND a.WorkTeamCapabilityGuid = @workTeamCapabilityGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<WorkTeamCapability>(sql, new { workTeamGuid, workTeamCapabilityGuid });
        }

        public async Task<IEnumerable<WorkTeamCapability>> GetAllByWorkTeamGuidAsync(Guid workTeamGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_WorkCapabilityId.CapabilityName AS WorkCapabilityName
, j_WorkTeamId.WorkTeamName AS WorkTeamName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "WorkCapability")} j_WorkCapabilityId ON a.WorkCapabilityId = j_WorkCapabilityId.WorkCapabilityId
LEFT JOIN {dialect.Qualify("app", "WorkTeam")} j_WorkTeamId ON a.WorkTeamId = j_WorkTeamId.WorkTeamId

                WHERE j_WorkTeamId.WorkTeamGuid = @workTeamGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.WorkTeamCapabilityId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<WorkTeamCapability>(sql, new { workTeamGuid });
        }

        public async Task CreateWorkTeamCapabilityAsync(WorkTeamCapability workTeamCapability, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(workTeamCapability);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (WorkTeamCapabilityGuid, WorkTeamId, CreatedById, StatusId, CreatedTime, [WorkCapabilityId], [IsViewAble], [IsAddAble], [IsEditAble], [IsApproveAble], [IsExportAble])
                VALUES
                (@WorkTeamCapabilityGuid, @WorkTeamId, @CreatedById, @StatusId, @CreatedTime, @WorkCapabilityId, @IsViewAble, @IsAddAble, @IsEditAble, @IsApproveAble, @IsExportAble){dialect.ReturnIdentitySuffix("WorkTeamCapabilityId")}";
            workTeamCapability.WorkTeamCapabilityId = await conn.QuerySingleAsync<long>(sql, workTeamCapability, transaction);
        }

        public async Task UpdateWorkTeamCapabilityAsync(WorkTeamCapability workTeamCapability, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(workTeamCapability);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [WorkTeamId] = @WorkTeamId,
                    [WorkCapabilityId] = @WorkCapabilityId,
                    [IsViewAble] = @IsViewAble,
                    [IsAddAble] = @IsAddAble,
                    [IsEditAble] = @IsEditAble,
                    [IsApproveAble] = @IsApproveAble,
                    [IsExportAble] = @IsExportAble,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE WorkTeamCapabilityGuid = @WorkTeamCapabilityGuid";
            await conn.ExecuteAsync(sql, workTeamCapability, transaction);
        }

        public async Task SoftDeleteWorkTeamCapabilityAsync(WorkTeamCapability workTeamCapability, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(workTeamCapability, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE WorkTeamCapabilityGuid = @WorkTeamCapabilityGuid";

            await conn.ExecuteAsync(sql, workTeamCapability, transaction);
        }

        public async Task DeleteWorkTeamCapabilityAsync(Guid workTeamCapabilityGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE WorkTeamCapabilityGuid = @workTeamCapabilityGuid";
            await conn.ExecuteAsync(sql, new { workTeamCapabilityGuid }, transaction);
        }

        public async Task DeleteByWorkTeamGuidAsync(Guid workTeamGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE WorkTeamId IN (SELECT WorkTeamId FROM {dialect.Qualify("app", "WorkTeam")} WHERE WorkTeamGuid = @workTeamGuid)";
            await conn.ExecuteAsync(sql, new { workTeamGuid }, transaction);
        }

        public async Task<IEnumerable<WorkTeamCapability>> SearchWorkTeamCapabilityAsync(
            string? isViewAble,
            string? isViewAbleSearchType,
            string? isAddAble,
            string? isAddAbleSearchType,
            string? isEditAble,
            string? isEditAbleSearchType,
            string? isApproveAble,
            string? isApproveAbleSearchType,
            string? isExportAble,
            string? isExportAbleSearchType,
            Guid workTeamGuid, Guid workTeamCapabilityGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (workTeamGuid != Guid.Empty)
            {
                whereClauses.Add("j_WorkTeamId.WorkTeamGuid = @workTeamGuid");
                parameters.Add("@workTeamGuid", workTeamGuid);
            }
            if (workTeamCapabilityGuid != Guid.Empty)
            {
                whereClauses.Add("a.WorkTeamCapabilityGuid = @workTeamCapabilityGuid");
                parameters.Add("@workTeamCapabilityGuid", workTeamCapabilityGuid);
            }

            if (!string.IsNullOrWhiteSpace(isViewAble))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsViewAble", "@isViewAble", isViewAbleSearchType, parameters, "isViewAble", isViewAble, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isAddAble))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsAddAble", "@isAddAble", isAddAbleSearchType, parameters, "isAddAble", isAddAble, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isEditAble))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsEditAble", "@isEditAble", isEditAbleSearchType, parameters, "isEditAble", isEditAble, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isApproveAble))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsApproveAble", "@isApproveAble", isApproveAbleSearchType, parameters, "isApproveAble", isApproveAble, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isExportAble))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsExportAble", "@isExportAble", isExportAbleSearchType, parameters, "isExportAble", isExportAble, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_WorkCapabilityId.CapabilityName AS WorkCapabilityName
, j_WorkTeamId.WorkTeamName AS WorkTeamName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "WorkCapability")} j_WorkCapabilityId ON a.WorkCapabilityId = j_WorkCapabilityId.WorkCapabilityId
LEFT JOIN {dialect.Qualify("app", "WorkTeam")} j_WorkTeamId ON a.WorkTeamId = j_WorkTeamId.WorkTeamId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.WorkTeamCapabilityId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<WorkTeamCapability>(sql, parameters, transaction);
        }

        public async Task<IEnumerable<WorkCapabilityDto>> GetWorkCapabilitiesAsync()
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT WorkCapabilityId, WorkCapabilityGuid, CapabilityCode, CapabilityName, CapabilityDescription, StatusId
                FROM {dialect.Qualify("app", "WorkCapability")}
                WHERE StatusId > 0 AND DeletedTime IS NULL
                ORDER BY CapabilityName";
            return await connection.QueryAsync<WorkCapabilityDto>(sql);
        }
    }
}
