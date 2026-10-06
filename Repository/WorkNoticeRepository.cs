using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class WorkNoticeRepository(RepositoryContext context, ISqlDialect dialect) : IWorkNoticeRepository
    {
        private readonly string _table = dialect.Qualify("app", "WorkNotice");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<WorkNotice?> GetWorkNoticeAsync(Guid workNoticeGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE a.WorkNoticeGuid = @workNoticeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<WorkNotice>(sql, new { workNoticeGuid });
        }

        public async Task<IEnumerable<WorkNotice>> GetAllWorkNoticesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}



                FROM {_table} a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.WorkNoticeId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<WorkNotice>(sql);
        }

        public async Task CreateWorkNoticeAsync(WorkNotice workNotice, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (WorkNoticeGuid, CreatedById, StatusId, CreatedTime, WorkNoticeCode, WorkNoticeName, WorkNoticeDate, IsEditAble, Note
                )
                VALUES
                (@WorkNoticeGuid, @CreatedById, @StatusId, @CreatedTime, @WorkNoticeCode, @WorkNoticeName, @WorkNoticeDate, @IsEditAble, @Note
                ){dialect.ReturnIdentitySuffix("WorkNoticeId")}";
            workNotice.WorkNoticeId = await conn.QuerySingleAsync<long>(sql, workNotice, transaction);
        }

        public async Task UpdateWorkNoticeAsync(WorkNotice workNotice, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     WorkNoticeCode = @WorkNoticeCode,
                    WorkNoticeName = @WorkNoticeName,
                    WorkNoticeDate = @WorkNoticeDate,
                    IsEditAble = @IsEditAble,
                    Note = @Note,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE WorkNoticeGuid = @WorkNoticeGuid";
            await conn.ExecuteAsync(sql, workNotice, transaction);
        }

        public async Task SoftDeleteWorkNoticeAsync(WorkNotice workNotice, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE WorkNoticeGuid = @WorkNoticeGuid";

            await conn.ExecuteAsync(sql, workNotice, transaction);
        }

        public async Task DeleteWorkNoticeAsync(Guid workNoticeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE WorkNoticeGuid = @workNoticeGuid";
            await conn.ExecuteAsync(sql, new { workNoticeGuid }, transaction);
        }

        public async Task<IEnumerable<WorkNotice>> SearchWorkNoticeAsync(
            string? workNoticeCode,
            string? workNoticeCodeSearchType,
            string? workNoticeName,
            string? workNoticeNameSearchType,
            string? workNoticeDate,
            string? workNoticeDateSearchType,
            string? isEditAble,
            string? isEditAbleSearchType,
            string? note,
            string? noteSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(workNoticeCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.WorkNoticeCode", "@workNoticeCode", workNoticeCodeSearchType, parameters, "workNoticeCode", workNoticeCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(workNoticeName))
            {
                var param = SqlFilterHelper.BuildFilter("a.WorkNoticeName", "@workNoticeName", workNoticeNameSearchType, parameters, "workNoticeName", workNoticeName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(workNoticeDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.WorkNoticeDate", "@workNoticeDate", workNoticeDateSearchType, parameters, "workNoticeDate", workNoticeDate, dialect);
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
                ORDER BY a.WorkNoticeId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<WorkNotice>(sql, parameters, transaction);
        }
    }
}
