using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class UserGroupScopeRepository : IUserGroupScopeRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public UserGroupScopeRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
            _audit = auditService;
        }

        public async Task<UserGroupScope?> GetAsync(Guid userGroupScopeGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "ugs.*")},
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName,
                        p.PersonName
                FROM    {_dialect.Qualify("core", "UserGroupScope")}   ugs
                JOIN    {_dialect.Qualify("core", "Users")}             u   ON u.UserId           = ugs.UserId
                JOIN    {_dialect.Qualify("core", "UserGroup")}         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    {_dialect.Qualify("app", "Company")}            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")}    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                LEFT JOIN {_dialect.Qualify("app", "Person")}           p   ON p.PersonId         = ugs.PersonId
                WHERE   ugs.UserGroupScopeGuid = @userGroupScopeGuid
                  AND   ugs.StatusId > 0
                  AND   ugs.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<UserGroupScope>(sql, new { userGroupScopeGuid });
        }

        public async Task<IEnumerable<UserGroupScope>> GetAllByUserGroupGuidAsync(Guid userGroupGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "ugs.*")},
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName,
                        p.PersonName
                FROM    {_dialect.Qualify("core", "UserGroupScope")}   ugs
                JOIN    {_dialect.Qualify("core", "Users")}             u   ON u.UserId           = ugs.UserId
                JOIN    {_dialect.Qualify("core", "UserGroup")}         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    {_dialect.Qualify("app", "Company")}            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")}    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                LEFT JOIN {_dialect.Qualify("app", "Person")}           p   ON p.PersonId         = ugs.PersonId
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
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "ugs.*")},
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName,
                        p.PersonName
                FROM    {_dialect.Qualify("core", "UserGroupScope")}   ugs
                JOIN    {_dialect.Qualify("core", "Users")}             u   ON u.UserId           = ugs.UserId
                JOIN    {_dialect.Qualify("core", "UserGroup")}         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    {_dialect.Qualify("app", "Company")}            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")}    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                LEFT JOIN {_dialect.Qualify("app", "Person")}           p   ON p.PersonId         = ugs.PersonId
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
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "ugs.*")},
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName,
                        p.PersonName
                FROM    {_dialect.Qualify("core", "UserGroupScope")}   ugs
                JOIN    {_dialect.Qualify("core", "Users")}             u   ON u.UserId           = ugs.UserId
                JOIN    {_dialect.Qualify("core", "UserGroup")}         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    {_dialect.Qualify("app", "Company")}            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")}    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                LEFT JOIN {_dialect.Qualify("app", "Person")}           p   ON p.PersonId         = ugs.PersonId
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
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "ugs.*")},
                        u.FullName, u.UserName,
                        ug.UserGroupName,
                        c.CompanyName,
                        co.CompanyOfficeName,
                        p.PersonName
                FROM    {_dialect.Qualify("core", "UserGroupScope")}   ugs
                JOIN    {_dialect.Qualify("core", "Users")}             u   ON u.UserId           = ugs.UserId
                JOIN    {_dialect.Qualify("core", "UserGroup")}         ug  ON ug.UserGroupId     = ugs.UserGroupId
                JOIN    {_dialect.Qualify("app", "Company")}            c   ON c.CompanyId        = ugs.CompanyId
                LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")}    co  ON co.CompanyOfficeId = ugs.CompanyOfficeId
                LEFT JOIN {_dialect.Qualify("app", "Person")}           p   ON p.PersonId         = ugs.PersonId
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
                    var unsetSql = $@"
                        UPDATE {_dialect.Qualify("core", "UserGroupScope")}
                        SET    IsDefault   = 0,
                               UpdatedById = @CreatedById,
                               UpdatedTime = @UpdatedTime
                        WHERE  UserId      = @UserId
                          AND  StatusId    > 0
                          AND  DeletedTime IS NULL";
                    await conn.ExecuteAsync(unsetSql, new { entity.UserId, entity.CreatedById, UpdatedTime = updatedTime }, localTran);

                    var insertSql = $@"
                        INSERT INTO {_dialect.Qualify("core", "UserGroupScope")}
                            (UserGroupScopeGuid, UserId, UserGuid, UserGroupId, UserGroupGuid, CompanyId, CompanyGuid, CompanyOfficeId, CompanyOfficeGuid, PersonId,
                             IsDefault, StatusId, CreatedById, CreatedTime)
                        VALUES
                            (@UserGroupScopeGuid, @UserId, @UserGuid, @UserGroupId, @UserGroupGuid, @CompanyId, @CompanyGuid, @CompanyOfficeId, @CompanyOfficeGuid, @PersonId,
                             @IsDefault, @StatusId, @CreatedById, @CreatedTime){_dialect.ReturnIdentitySuffix("UserGroupScopeId")}";
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
                var sql = $@"
                    INSERT INTO {_dialect.Qualify("core", "UserGroupScope")}
                        (UserGroupScopeGuid, UserId, UserGuid, UserGroupId, UserGroupGuid, CompanyId, CompanyGuid, CompanyOfficeId, CompanyOfficeGuid, PersonId,
                         IsDefault, StatusId, CreatedById, CreatedTime)
                    VALUES
                        (@UserGroupScopeGuid, @UserId, @UserGuid, @UserGroupId, @UserGroupGuid, @CompanyId, @CompanyGuid, @CompanyOfficeId, @CompanyOfficeGuid, @PersonId,
                         @IsDefault, @StatusId, @CreatedById, @CreatedTime){_dialect.ReturnIdentitySuffix("UserGroupScopeId")}";
                entity.UserGroupScopeId = await conn.ExecuteScalarAsync<long>(sql, entity, transaction);
            }

            await _audit.LogAsync("CREATE", "UserGroupScope",
                entity.UserGroupScopeGuid.ToString(),
                entity.CreatedById.ToString(), null, entity);
        }

        public async Task UpdatePersonAsync(Guid userGroupScopeGuid, long? personId, long updatedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var ownsConnection = transaction == null;
            try
            {
                var sql = $@"
                    UPDATE {_dialect.Qualify("core", "UserGroupScope")}
                    SET    PersonId    = @personId,
                           UpdatedById = @updatedBy,
                           UpdatedTime = @updatedTime
                    WHERE  UserGroupScopeGuid = @userGroupScopeGuid";
                await conn.ExecuteAsync(sql, new { userGroupScopeGuid, personId, updatedBy, updatedTime = DateTime.UtcNow }, transaction);
            }
            finally
            {
                if (ownsConnection)
                    conn.Dispose();
            }
        }

        public async Task<UserGroupScope?> GetOtherUserByCompanyPersonAsync(long companyId, long personId, long userId)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"{SqlDialectQueryHelper.SelectLimited(_dialect, 1, "ugs.*")}, u.UserName
                FROM {_dialect.Qualify("core", "UserGroupScope")} ugs
                JOIN {_dialect.Qualify("core", "Users")} u ON u.UserId = ugs.UserId
                WHERE ugs.CompanyId = @companyId
                  AND ugs.PersonId = @personId
                  AND ugs.UserId <> @userId
                  AND ugs.StatusId > 0
                  AND ugs.DeletedTime IS NULL";
            return await connection.QueryFirstOrDefaultAsync<UserGroupScope>(sql, new { companyId, personId, userId });
        }

        public async Task SoftDeleteAsync(UserGroupScope entity, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(entity, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAsync(entity.UserGroupScopeGuid, false);

            var sql = $@"
                UPDATE {_dialect.Qualify("core", "UserGroupScope")}
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

            var sql = $@"
                DELETE FROM {_dialect.Qualify("core", "UserGroupScope")}
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
                var unsetSql = $@"
                    UPDATE {_dialect.Qualify("core", "UserGroupScope")}
                    SET    IsDefault   = 0,
                           UpdatedById = @updatedBy,
                           UpdatedTime = @UpdatedTime
                    WHERE  UserId      = @userId
                      AND  StatusId    > 0
                      AND  DeletedTime IS NULL";
                await conn.ExecuteAsync(unsetSql, new { userId, updatedBy, UpdatedTime = updatedTime }, localTran);

                // Langkah 2: Set scope yang diminta sebagai default
                var setSql = $@"
                    UPDATE {_dialect.Qualify("core", "UserGroupScope")}
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
