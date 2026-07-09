using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;
using System.Text;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public UserRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<User?> GetUserByUserNameAsync(string userName, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[Users] 
                                 WHERE UserName = @userName AND StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { userName });
        }

        public async Task<User> GetUserAsync(Guid userGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[Users] 
                                 WHERE UserGuid = @userGuid AND StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { userGuid });
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [core].[Users] a
                
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.UserId DESC";

            return await connection.QueryAsync<User>(sql);
        }

        public async Task CreateUserAsync(User user, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampCreate(user);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"INSERT INTO [core].[Users]
                                 (UserGuid, CreatedById, StatusId, CreatedTime, UserName, PasswordHash, PasswordSalt, FullName, Email, IsAdmin)
                                 VALUES
                                 (@UserGuid, @CreatedById, @StatusId, @CreatedTime, @UserName, @PasswordHash, @PasswordSalt, @FullName, @Email, @IsAdmin)";
            await conn.ExecuteAsync(sql, user, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "Users",
                primaryKey: user.UserGuid.ToString(),
                userId: user.CreatedById.ToString(),
                oldEntity: null,
                newEntity: user);
        }

        public async Task UpdateUserAsync(User user, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampUpdate(user);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserAsync(user.UserGuid, false);

            const string sql = @"UPDATE [core].[Users]
                     SET
                         UserName = @UserName,
                                      PasswordHash = @PasswordHash,
                                      PasswordSalt = @PasswordSalt,
                                      FullName = @FullName,
                                      Email = @Email,
                                      StatusId = @StatusId,
                                      UpdatedById = @UpdatedById,
                                      UpdatedTime = @UpdatedTime
                     WHERE UserGuid = @UserGuid";

            await conn.ExecuteAsync(sql, user, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "Users",
                primaryKey: user.UserGuid.ToString(),
                userId: user.UpdatedById?.ToString(),
                oldEntity: oldData,
                newEntity: user);
        }

        public async Task SoftDeleteUserAsync(User user, long deletedBy, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(user, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserAsync(user.UserGuid, false);

            const string sql = @"UPDATE [core].[Users]
                                 SET StatusId = 0,
                                     DeletedById = @DeletedById,
                                     DeletedTime = @DeletedTime
                                 WHERE UserGuid = @UserGuid";
            await conn.ExecuteAsync(sql, user, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "Users",
                primaryKey: user.UserGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: user);
        }

        public async Task UpgradePasswordHashAsync(long userId, string newBcryptHash, string newSalt, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"UPDATE [core].[Users]
                                 SET PasswordHash = @newBcryptHash,
                                     PasswordSalt = @newSalt,
                                     UpdatedTime  = @UpdatedTime
                                 WHERE UserId = @userId";
            await conn.ExecuteAsync(sql, new { userId, newBcryptHash, newSalt, UpdatedTime = DateTime.UtcNow }, transaction);
        }

        public async Task DeleteUserAsync(Guid userGuid, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetUserAsync(userGuid, false);

            const string sql = @"DELETE FROM [core].[Users]
                                 WHERE UserGuid = @userGuid";
            await conn.ExecuteAsync(sql, new { userGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "Users",
                primaryKey: userGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<User>> SearchUserAsync(
            string? userName, string? userNameSearchType,
            string? fullName, string? fullNameSearchType,
            string? email, string? emailSearchType,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [core].[Users] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                  AND (@userName IS NULL OR ISNULL(UserName, '') LIKE '%' + @userName + '%')
                  AND (@fullName IS NULL OR ISNULL(FullName, '') LIKE '%' + @fullName + '%')
                  AND (@email   IS NULL OR a.Email LIKE '%' + @email + '%')
                ORDER BY a.UserId DESC";

            return await connection.QueryAsync<User>(sql, new { userName, fullName, email }, transaction);
        }

    }
}
