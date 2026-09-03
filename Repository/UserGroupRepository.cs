using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;
using System.Text;

namespace Repository
{
    public class UserGroupRepository : IUserGroupRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public UserGroupRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
            _auditService = auditService;
        }

        public async Task<UserGroup?> GetUserGroupAsync(Guid userGroupGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")} FROM {_dialect.Qualify("core", "UserGroup")} 
                                 WHERE UserGroupGuid = @userGroupGuid AND StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<UserGroup>(sql, new { userGroupGuid });
        }

        public async Task<IEnumerable<UserGroup>> GetAllUserGroupsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("core", "UserGroup")} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.UserGroupId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<UserGroup>(sql);
        }

        public async Task CreateUserGroupAsync(UserGroup userGroup, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(userGroup);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"INSERT INTO {_dialect.Qualify("core", "UserGroup")}
                                 (UserGroupGuid, UserGroupName, IsEditAble, StatusId, CreatedById, CreatedTime)
                                 VALUES
                                 (@UserGroupGuid, @UserGroupName, @IsEditAble, @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, userGroup, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "UserGroup",
                primaryKey: userGroup.UserGroupGuid.ToString(),
                userId: userGroup.CreatedById.ToString(),
                oldEntity: null,
                newEntity: userGroup);
        }

        public async Task UpdateUserGroupAsync(UserGroup userGroup, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(userGroup);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserGroupAsync(userGroup.UserGroupGuid, false);

            var sql = $@"UPDATE {_dialect.Qualify("core", "UserGroup")}
                                 SET
                                     UserGroupName = @UserGroupName,
                                     IsEditAble = @IsEditAble,
                                     StatusId = @StatusId,
                                     UpdatedById = @UpdatedById,
                                     UpdatedTime = @UpdatedTime
                                 WHERE UserGroupGuid = @UserGroupGuid";

            await conn.ExecuteAsync(sql, userGroup, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "UserGroup",
                primaryKey: userGroup.UserGroupGuid.ToString(),
                userId: userGroup.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: userGroup);
        }

        public async Task SoftDeleteUserGroupAsync(UserGroup userGroup, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(userGroup, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserGroupAsync(userGroup.UserGroupGuid, false);

            var sql = $@"UPDATE {_dialect.Qualify("core", "UserGroup")}
                                 SET StatusId = 0,
                                     DeletedById = @DeletedById,
                                     DeletedTime = @DeletedTime
                                 WHERE UserGroupGuid = @UserGroupGuid";
            await conn.ExecuteAsync(sql, userGroup, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "UserGroup",
                primaryKey: userGroup.UserGroupGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: userGroup);
        }

        public async Task DeleteUserGroupAsync(Guid userGroupGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserGroupAsync(userGroupGuid, false);

            var sql = $@"DELETE FROM {_dialect.Qualify("core", "UserGroup")}
                                 WHERE UserGroupGuid = @userGroupGuid";
            await conn.ExecuteAsync(sql, new { userGroupGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "UserGroup",
                primaryKey: userGroupGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<UserGroup>> SearchUserGroupAsync(
            string? userGroupName, string? userGroupNameSearchType, 
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT a.*
                FROM {_dialect.Qualify("core", "UserGroup")} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                AND (@userGroupName IS NULL OR {_dialect.LikeContains("UserGroupName", "@userGroupName")})
                ORDER BY a.UserGroupId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<UserGroup>(sql, new {
                userGroupName = string.IsNullOrWhiteSpace(userGroupName) ? null : $"%{userGroupName}%"
            }, transaction);
        }
    }
}
