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
    public partial class WorkTeamRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IWorkTeamRepository
    {
        private readonly string _table = dialect.Qualify("app", "WorkTeam");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<WorkTeam?> GetWorkTeamAsync(Guid workTeamGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a

                WHERE a.WorkTeamGuid = @workTeamGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<WorkTeam>(sql, new { workTeamGuid });
        }

        public async Task<WorkTeam?> GetWorkTeamByIdAsync(long workTeamId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a

                WHERE a.WorkTeamId = @workTeamId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<WorkTeam>(sql, new { workTeamId });
        }

        public async Task<IEnumerable<WorkTeam>> GetAllWorkTeamsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.WorkTeamId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<WorkTeam>(sql);
        }

        public async Task CreateWorkTeamAsync(WorkTeam workTeam, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(workTeam);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (WorkTeamGuid, CreatedById, StatusId, CreatedTime, WorkTeamCode, WorkTeamName, IsEditAble, Note)
                VALUES
                (@WorkTeamGuid, @CreatedById, @StatusId, @CreatedTime, @WorkTeamCode, @WorkTeamName, @IsEditAble, @Note){dialect.ReturnIdentitySuffix("WorkTeamId")}";
            workTeam.WorkTeamId = await conn.QuerySingleAsync<long>(sql, workTeam, transaction);
        }

        public async Task UpdateWorkTeamAsync(WorkTeam workTeam, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(workTeam);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     WorkTeamCode = @WorkTeamCode,
                    WorkTeamName = @WorkTeamName,
                    IsEditAble = @IsEditAble,
                    Note = @Note,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE WorkTeamGuid = @WorkTeamGuid";
            await conn.ExecuteAsync(sql, workTeam, transaction);
        }

        public async Task SoftDeleteWorkTeamAsync(WorkTeam workTeam, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(workTeam, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE WorkTeamGuid = @WorkTeamGuid";

            await conn.ExecuteAsync(sql, workTeam, transaction);
        }

        public async Task DeleteWorkTeamAsync(Guid workTeamGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE WorkTeamGuid = @workTeamGuid";
            await conn.ExecuteAsync(sql, new { workTeamGuid }, transaction);
        }

        public async Task<IEnumerable<WorkTeam>> SearchWorkTeamAsync(
            string? workTeamCode,
            string? workTeamCodeSearchType,
            string? workTeamName,
            string? workTeamNameSearchType,
            string? isEditAble,
            string? isEditAbleSearchType,
            string? note,
            string? noteSearchType,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(workTeamCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.WorkTeamCode", "@workTeamCode", workTeamCodeSearchType, parameters, "workTeamCode", workTeamCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(workTeamName))
            {
                var param = SqlFilterHelper.BuildFilter("a.WorkTeamName", "@workTeamName", workTeamNameSearchType, parameters, "workTeamName", workTeamName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isEditAble))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsEditAble", "@isEditAble", isEditAbleSearchType, parameters, "isEditAble", isEditAble, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.WorkTeamId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<WorkTeam>(sql, parameters, transaction);
        }
    }
}
