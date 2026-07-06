using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class UserGroupScopeRepository : IUserGroupScopeRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public UserGroupScopeRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<UserGroupScope?> GetAsync(Guid userGroupScopeGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) ugs.*,
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName
                FROM    [core].[UserGroupScope]   ugs
                JOIN    [core].[Users]             u   ON u.UserId           = ugs.UserId
                JOIN    [core].[UserGroup]         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    [app].[Company]            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN [app].[CompanyOffice]    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                WHERE   ugs.UserGroupScopeGuid = @userGroupScopeGuid
                  AND   ugs.StatusId > 0
                  AND   ugs.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<UserGroupScope>(sql, new { userGroupScopeGuid });
        }

        public async Task<IEnumerable<UserGroupScope>> GetAllByUserGroupGuidAsync(Guid userGroupGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) ugs.*,
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName
                FROM    [core].[UserGroupScope]   ugs
                JOIN    [core].[Users]             u   ON u.UserId           = ugs.UserId
                JOIN    [core].[UserGroup]         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    [app].[Company]            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN [app].[CompanyOffice]    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                WHERE   ug.UserGroupGuid = @userGroupGuid
                  AND   ugs.StatusId > 0
                  AND   ugs.DeletedTime IS NULL
                ORDER BY c.CompanyName, u.FullName";
            return await connection.QueryAsync<UserGroupScope>(sql, new { userGroupGuid });
        }

        public async Task<IEnumerable<UserGroupScope>> GetAllByUserIdAsync(long userId)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) ugs.*,
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName
                FROM    [core].[UserGroupScope]   ugs
                JOIN    [core].[Users]             u   ON u.UserId           = ugs.UserId
                JOIN    [core].[UserGroup]         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    [app].[Company]            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN [app].[CompanyOffice]    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                WHERE   ugs.UserId = @userId
                  AND   ugs.StatusId > 0
                  AND   ugs.DeletedTime IS NULL
                ORDER BY c.CompanyName, ug.UserGroupName";
            return await connection.QueryAsync<UserGroupScope>(sql, new { userId });
        }

        public async Task<IEnumerable<UserGroupScope>> GetAllByCompanyIdAsync(long companyId)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) ugs.*,
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName
                FROM    [core].[UserGroupScope]   ugs
                JOIN    [core].[Users]             u   ON u.UserId           = ugs.UserId
                JOIN    [core].[UserGroup]         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    [app].[Company]            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN [app].[CompanyOffice]    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                WHERE   ugs.CompanyId = @companyId
                  AND   ugs.StatusId > 0
                  AND   ugs.DeletedTime IS NULL
                ORDER BY ug.UserGroupName, u.FullName";
            return await connection.QueryAsync<UserGroupScope>(sql, new { companyId });
        }

        public async Task<UserGroupScope?> GetByUserGroupAndGuidAsync(Guid userGroupGuid, Guid userGroupScopeGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) ugs.*,
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName
                FROM    [core].[UserGroupScope]   ugs
                JOIN    [core].[Users]             u   ON u.UserId           = ugs.UserId
                JOIN    [core].[UserGroup]         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    [app].[Company]            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN [app].[CompanyOffice]    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                WHERE   ug.UserGroupGuid = @userGroupGuid
                  AND   ugs.UserGroupScopeGuid = @userGroupScopeGuid
                  AND   ugs.StatusId > 0
                  AND   ugs.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<UserGroupScope>(sql, new { userGroupGuid, userGroupScopeGuid });
        }

        public async Task CreateAsync(UserGroupScope entity, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(entity);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            entity.UserGroupScopeGuid = Guid.NewGuid();
            entity.StatusId = 1;

            // Jika scope ini di-set sebagai default, jalankan dalam satu transaksi atomik
            if (entity.IsDefault)
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using var localTran = conn.BeginTransaction();
                try
                {
                    var updatedTime = DateTime.UtcNow;
                    const string unsetSql = @"
                        UPDATE [core].[UserGroupScope]
                        SET    IsDefault   = 0,
                               UpdatedById = @CreatedById,
                               UpdatedTime = @UpdatedTime
                        WHERE  UserId      = @UserId
                          AND  StatusId    > 0
                          AND  DeletedTime IS NULL";
                    await conn.ExecuteAsync(unsetSql, new { entity.UserId, entity.CreatedById, UpdatedTime = updatedTime }, localTran);

                    const string insertSql = @"
                        INSERT INTO [core].[UserGroupScope]
                            (UserGroupScopeGuid, UserId, UserGuid, UserGroupId, UserGroupGuid, CompanyId, CompanyGuid, CompanyOfficeId, CompanyOfficeGuid,
                             IsDefault, StatusId, CreatedById, CreatedTime)
                        VALUES
                            (@UserGroupScopeGuid, @UserId, @UserGuid, @UserGroupId, @UserGroupGuid, @CompanyId, @CompanyGuid, @CompanyOfficeId, @CompanyOfficeGuid,
                             @IsDefault, @StatusId, @CreatedById, @CreatedTime);
                        SELECT SCOPE_IDENTITY();";
                    entity.UserGroupScopeId = await conn.ExecuteScalarAsync<long>(insertSql, entity, localTran);

                    localTran.Commit();
                }
                catch
                {
                    localTran.Rollback();
                    throw;
                }
            }
            else
            {
                const string sql = @"
                    INSERT INTO [core].[UserGroupScope]
                        (UserGroupScopeGuid, UserId, UserGuid, UserGroupId, UserGroupGuid, CompanyId, CompanyGuid, CompanyOfficeId, CompanyOfficeGuid,
                         IsDefault, StatusId, CreatedById, CreatedTime)
                    VALUES
                        (@UserGroupScopeGuid, @UserId, @UserGuid, @UserGroupId, @UserGroupGuid, @CompanyId, @CompanyGuid, @CompanyOfficeId, @CompanyOfficeGuid,
                         @IsDefault, @StatusId, @CreatedById, @CreatedTime);
                    SELECT SCOPE_IDENTITY();";
                entity.UserGroupScopeId = await conn.ExecuteScalarAsync<long>(sql, entity, transaction);
            }

            await _audit.LogAsync("CREATE", "UserGroupScope",
                entity.UserGroupScopeGuid.ToString(),
                entity.CreatedById.ToString(), null, entity);
        }

        public async Task SoftDeleteAsync(UserGroupScope entity, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(entity, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAsync(entity.UserGroupScopeGuid, false);

            const string sql = @"
                UPDATE [core].[UserGroupScope]
                SET    StatusId    = 0,
                       DeletedById = @DeletedById,
                       DeletedTime = @DeletedTime
                WHERE  UserGroupScopeGuid = @UserGroupScopeGuid";
            await conn.ExecuteAsync(sql, entity, transaction);

            await _audit.LogAsync("DELETE", "UserGroupScope",
                entity.UserGroupScopeGuid.ToString(),
                deletedBy.ToString(), oldData, null);
        }

        public async Task DeleteAsync(Guid userGroupScopeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAsync(userGroupScopeGuid, false);

            const string sql = @"
                DELETE FROM [core].[UserGroupScope]
                WHERE UserGroupScopeGuid = @userGroupScopeGuid";
            await conn.ExecuteAsync(sql, new { userGroupScopeGuid }, transaction);

            await _audit.LogAsync("DELETE_ADMIN", "UserGroupScope",
                userGroupScopeGuid.ToString(),
                oldData?.DeletedById?.ToString() ?? "system", oldData, null);
        }

        public async Task<UserGroupScope> SetDefaultAsync(Guid userGroupScopeGuid, long userId, long updatedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAsync(userGroupScopeGuid, false)
                ?? throw new KeyNotFoundException($"UserGroupScope '{userGroupScopeGuid}' tidak ditemukan.");

            if (conn.State != ConnectionState.Open) conn.Open();
            using var localTran = conn.BeginTransaction();
            var updatedTime = DateTime.UtcNow;
            try
            {
                // Langkah 1: Unset semua scope default milik userId ini
                const string unsetSql = @"
                    UPDATE [core].[UserGroupScope]
                    SET    IsDefault   = 0,
                           UpdatedById = @updatedBy,
                           UpdatedTime = @UpdatedTime
                    WHERE  UserId      = @userId
                      AND  StatusId    > 0
                      AND  DeletedTime IS NULL";
                await conn.ExecuteAsync(unsetSql, new { userId, updatedBy, UpdatedTime = updatedTime }, localTran);

                // Langkah 2: Set scope yang diminta sebagai default
                const string setSql = @"
                    UPDATE [core].[UserGroupScope]
                    SET    IsDefault   = 1,
                           UpdatedById = @updatedBy,
                           UpdatedTime = @UpdatedTime
                    WHERE  UserGroupScopeGuid = @userGroupScopeGuid";
                await conn.ExecuteAsync(setSql, new { userGroupScopeGuid, updatedBy, UpdatedTime = updatedTime }, localTran);

                localTran.Commit();
            }
            catch
            {
                localTran.Rollback();
                throw;
            }

            await _audit.LogAsync("SET_DEFAULT", "UserGroupScope",
                userGroupScopeGuid.ToString(),
                updatedBy.ToString(), oldData, null);

            // Return data terbaru
            return (await GetAsync(userGroupScopeGuid, false))!;
        }
    }
}
