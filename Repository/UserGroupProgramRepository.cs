using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;
using System.Text;

namespace Repository
{
    public class UserGroupProgramRepository : IUserGroupProgramRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public UserGroupProgramRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<UserGroupProgram?> GetUserGroupProgramAsync(Guid userGroupProgramGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[UserGroupProgram] 
                                 WHERE UserGroupProgramGuid = @userGroupProgramGuid AND StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<UserGroupProgram>(sql, new { userGroupProgramGuid });
        }

        public async Task<IEnumerable<UserGroupProgram>> GetAllUserGroupProgramsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT a.*
                FROM [core].[UserGroupProgram] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.UserGroupProgramId DESC";

            return await connection.QueryAsync<UserGroupProgram>(sql);
        }

        public async Task<IEnumerable<UserGroupProgram>> GetUserGroupProgramsByUserGroupGuidAsync(Guid userGroupGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT a.*
                FROM [core].[UserGroupProgram] a
                INNER JOIN [core].[UserGroup] ug ON ug.UserGroupId = a.UserGroupId
                WHERE ug.UserGroupGuid = @userGroupGuid
                  AND a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.UserGroupProgramId DESC";

            return await connection.QueryAsync<UserGroupProgram>(sql, new { userGroupGuid });
        }

        public async Task CreateUserGroupProgramAsync(UserGroupProgram userGroupProgram, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(userGroupProgram);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"INSERT INTO [core].[UserGroupProgram]
                                 (UserGroupProgramGuid, UserGroupId, ProgramsId, IsUserGroupViewAble, IsUserGroupAddAble, IsUserGroupEditAble, IsUserGroupDeleteAble,
                                  IsUserGroupApprovalAble, IsUserGroupUnApprovalAble, IsUserGroupVoidAble, IsUserGroupUnVoidAble, IsUserGroupExportAble,
                                  StatusId, CreatedById, CreatedTime)
                                 VALUES
                                 (@UserGroupProgramGuid, @UserGroupId, @ProgramsId, @IsUserGroupViewAble, @IsUserGroupAddAble, @IsUserGroupEditAble, @IsUserGroupDeleteAble,
                                  @IsUserGroupApprovalAble, @IsUserGroupUnApprovalAble, @IsUserGroupVoidAble, @IsUserGroupUnVoidAble, @IsUserGroupExportAble,
                                  @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, userGroupProgram, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "UserGroupProgram",
                primaryKey: userGroupProgram.UserGroupProgramGuid.ToString(),
                userId: userGroupProgram.CreatedById.ToString(),
                oldEntity: null,
                newEntity: userGroupProgram);
        }

        public async Task UpdateUserGroupProgramAsync(UserGroupProgram userGroupProgram, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(userGroupProgram);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserGroupProgramAsync(userGroupProgram.UserGroupProgramGuid, false);

            const string sql = @"UPDATE [core].[UserGroupProgram]
                                 SET
                                     UserGroupId = @UserGroupId,
                                     ProgramsId = @ProgramsId,
                                     IsUserGroupViewAble = @IsUserGroupViewAble,
                                     IsUserGroupAddAble = @IsUserGroupAddAble,
                                     IsUserGroupEditAble = @IsUserGroupEditAble,
                                     IsUserGroupDeleteAble = @IsUserGroupDeleteAble,
                                     IsUserGroupApprovalAble = @IsUserGroupApprovalAble,
                                     IsUserGroupUnApprovalAble = @IsUserGroupUnApprovalAble,
                                     IsUserGroupVoidAble = @IsUserGroupVoidAble,
                                     IsUserGroupUnVoidAble = @IsUserGroupUnVoidAble,
                                     IsUserGroupExportAble = @IsUserGroupExportAble,
                                     StatusId = @StatusId,
                                     UpdatedById = @UpdatedById,
                                     UpdatedTime = @UpdatedTime
                                 WHERE UserGroupProgramGuid = @UserGroupProgramGuid";

            await conn.ExecuteAsync(sql, userGroupProgram, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "UserGroupProgram",
                primaryKey: userGroupProgram.UserGroupProgramGuid.ToString(),
                userId: userGroupProgram.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: userGroupProgram);
        }

        public async Task SoftDeleteUserGroupProgramAsync(UserGroupProgram userGroupProgram, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(userGroupProgram, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserGroupProgramAsync(userGroupProgram.UserGroupProgramGuid, false);

            const string sql = @"UPDATE [core].[UserGroupProgram]
                                 SET StatusId = 0,
                                     DeletedById = @DeletedById,
                                     DeletedTime = @DeletedTime
                                 WHERE UserGroupProgramGuid = @UserGroupProgramGuid";
            await conn.ExecuteAsync(sql, userGroupProgram, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "UserGroupProgram",
                primaryKey: userGroupProgram.UserGroupProgramGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: userGroupProgram);
        }

        public async Task DeleteUserGroupProgramAsync(Guid userGroupProgramGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserGroupProgramAsync(userGroupProgramGuid, false);

            const string sql = @"DELETE FROM [core].[UserGroupProgram]
                                 WHERE UserGroupProgramGuid = @userGroupProgramGuid";
            await conn.ExecuteAsync(sql, new { userGroupProgramGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "UserGroupProgram",
                primaryKey: userGroupProgramGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }
    }
}
