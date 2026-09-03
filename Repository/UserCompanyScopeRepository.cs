using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class UserCompanyScopeRepository : IUserCompanyScopeRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public UserCompanyScopeRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
            _audit = auditService;
        }

        public async Task<UserCompanyScope?> GetAsync(Guid userCompanyScopeGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "ucs.*")},
                        c.CompanyName,
                        co.CompanyOfficeName
                FROM    {_dialect.Qualify("core", "UserCompanyScope")} ucs
                JOIN    {_dialect.Qualify("app", "Company")}            c  ON c.CompanyId        = ucs.CompanyId
                LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")}    co ON co.CompanyOfficeId = ucs.CompanyOfficeId
                WHERE   ucs.UserCompanyScopeGuid = @userCompanyScopeGuid
                  AND   ucs.StatusId > 0
                  AND   ucs.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<UserCompanyScope>(sql, new { userCompanyScopeGuid });
        }

        public async Task<IEnumerable<UserCompanyScope>> GetAllAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "ucs.*")},
                        c.CompanyName,
                        co.CompanyOfficeName
                FROM    {_dialect.Qualify("core", "UserCompanyScope")} ucs
                JOIN    {_dialect.Qualify("app", "Company")}            c  ON c.CompanyId        = ucs.CompanyId
                LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")}    co ON co.CompanyOfficeId = ucs.CompanyOfficeId
                WHERE   ucs.StatusId > 0 AND ucs.DeletedTime IS NULL
                ORDER BY ucs.UserCompanyScopeId DESC";
            return await connection.QueryAsync<UserCompanyScope>(sql);
        }

        public async Task<IEnumerable<UserCompanyScope>> GetAllByUserIdAsync(long userId)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "ucs.*")},
                        c.CompanyName,
                        co.CompanyOfficeName
                FROM    {_dialect.Qualify("core", "UserCompanyScope")} ucs
                JOIN    {_dialect.Qualify("app", "Company")}            c  ON c.CompanyId        = ucs.CompanyId
                LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")}    co ON co.CompanyOfficeId = ucs.CompanyOfficeId
                WHERE   ucs.UserId = @userId
                  AND   ucs.StatusId > 0
                  AND   ucs.DeletedTime IS NULL
                ORDER BY ucs.IsDefaultCompany DESC, c.CompanyName ASC";
            return await connection.QueryAsync<UserCompanyScope>(sql, new { userId });
        }

        public async Task<UserCompanyScope?> GetByUserAndGuidAsync(long userId, Guid userCompanyScopeGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "ucs.*")},
                        c.CompanyName,
                        co.CompanyOfficeName
                FROM    {_dialect.Qualify("core", "UserCompanyScope")} ucs
                JOIN    {_dialect.Qualify("app", "Company")}            c  ON c.CompanyId        = ucs.CompanyId
                LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")}    co ON co.CompanyOfficeId = ucs.CompanyOfficeId
                WHERE   ucs.UserId = @userId
                  AND   ucs.UserCompanyScopeGuid = @userCompanyScopeGuid
                  AND   ucs.StatusId > 0
                  AND   ucs.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<UserCompanyScope>(sql, new { userId, userCompanyScopeGuid });
        }

        public async Task CreateAsync(UserCompanyScope entity, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(entity);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            entity.UserCompanyScopeGuid = Guid.NewGuid();
            entity.StatusId = 1;

            var sql = $@"
                INSERT INTO {_dialect.Qualify("core", "UserCompanyScope")}
                    (UserCompanyScopeGuid, UserId, CompanyId, CompanyOfficeId,
                     IsDefaultCompany, IsDefaultOffice, StatusId,
                     CreatedById, CreatedTime)
                VALUES
                    (@UserCompanyScopeGuid, @UserId, @CompanyId, @CompanyOfficeId,
                     @IsDefaultCompany, @IsDefaultOffice, @StatusId,
                     @CreatedById, @CreatedTime){_dialect.ReturnIdentitySuffix("UserCompanyScopeId")}";

            entity.UserCompanyScopeId = await conn.ExecuteScalarAsync<long>(sql, entity, transaction);

            await _audit.LogAsync("CREATE", "UserCompanyScope",
                entity.UserCompanyScopeGuid.ToString(),
                entity.CreatedById.ToString(), null, entity);
        }

        public async Task UpdateAsync(UserCompanyScope entity, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(entity);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAsync(entity.UserCompanyScopeGuid, false);

            var sql = $@"
                UPDATE {_dialect.Qualify("core", "UserCompanyScope")}
                SET    CompanyOfficeId   = @CompanyOfficeId,
                       IsDefaultCompany = @IsDefaultCompany,
                       IsDefaultOffice  = @IsDefaultOffice,
                       StatusId         = @StatusId,
                       UpdatedById      = @UpdatedById,
                       UpdatedTime      = @UpdatedTime
                WHERE  UserCompanyScopeGuid = @UserCompanyScopeGuid";
            await conn.ExecuteAsync(sql, entity, transaction);

            await _audit.LogAsync("UPDATE", "UserCompanyScope",
                entity.UserCompanyScopeGuid.ToString(),
                entity.UpdatedById?.ToString() ?? "system", oldData, entity);
        }

        public async Task SoftDeleteAsync(UserCompanyScope entity, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(entity, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAsync(entity.UserCompanyScopeGuid, false);

            var sql = $@"
                UPDATE {_dialect.Qualify("core", "UserCompanyScope")}
                SET    StatusId    = 0,
                       DeletedById = @DeletedById,
                       DeletedTime = @DeletedTime
                WHERE  UserCompanyScopeGuid = @UserCompanyScopeGuid";
            await conn.ExecuteAsync(sql, entity, transaction);

            await _audit.LogAsync("DELETE", "UserCompanyScope",
                entity.UserCompanyScopeGuid.ToString(),
                deletedBy.ToString(), oldData, null);
        }

        public async Task DeleteAsync(Guid userCompanyScopeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAsync(userCompanyScopeGuid, false);

            var sql = $@"
                DELETE FROM {_dialect.Qualify("core", "UserCompanyScope")}
                WHERE UserCompanyScopeGuid = @userCompanyScopeGuid";
            await conn.ExecuteAsync(sql, new { userCompanyScopeGuid }, transaction);

            await _audit.LogAsync("DELETE_ADMIN", "UserCompanyScope",
                userCompanyScopeGuid.ToString(),
                oldData?.DeletedById?.ToString() ?? "system", oldData, null);
        }
    }
}
