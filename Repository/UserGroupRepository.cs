using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;
using System.Text;

namespace Repository
{
    public class UserGroupRepository : IUserGroupRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public UserGroupRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<UserGroup> GetUserGroupAsync(Guid userGroupGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[UserGroup] 
                                 WHERE UserGroupGuid = @userGroupGuid AND StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<UserGroup>(sql, new { userGroupGuid });
        }

        public async Task<IEnumerable<UserGroup>> GetAllUserGroupsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [core].[UserGroup] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.UserGroupId DESC";

            return await connection.QueryAsync<UserGroup>(sql);
        }

        public async Task CreateUserGroupAsync(UserGroup userGroup, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampCreate(userGroup);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"INSERT INTO [core].[UserGroup]
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

        public async Task UpdateUserGroupAsync(UserGroup userGroup, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampUpdate(userGroup);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserGroupAsync(userGroup.UserGroupGuid, false);

            const string sql = @"UPDATE [core].[UserGroup]
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
                userId: userGroup.UpdatedById?.ToString(),
                oldEntity: oldData,
                newEntity: userGroup);
        }

        public async Task SoftDeleteUserGroupAsync(UserGroup userGroup, long deletedBy, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(userGroup, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserGroupAsync(userGroup.UserGroupGuid, false);

            const string sql = @"UPDATE [core].[UserGroup]
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

        public async Task DeleteUserGroupAsync(Guid userGroupGuid, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserGroupAsync(userGroupGuid, false);

            const string sql = @"DELETE FROM [core].[UserGroup]
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
            var sql = @"
                SELECT a.*
                FROM [core].[UserGroup] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                AND (@userGroupName IS NULL OR ISNULL(UserGroupName, '') LIKE '%' + @userGroupName + '%')
                ORDER BY a.UserGroupId DESC";

            return await connection.QueryAsync<UserGroup>(sql, new {
                userGroupName = userGroupName
            }, transaction);
        }
    }
}
