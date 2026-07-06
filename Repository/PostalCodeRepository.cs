using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using System.Data;
using System.Text;
using Repository.Extensions;

namespace Repository
{
    public class PostalCodeRepository : IPostalCodeRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public PostalCodeRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<PostalCode> GetPostalCodeAsync(Guid subDistrictGuid, Guid postalCodeGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT p.PostalCodeId, p.PostalCodeGuid, p.SubDistrictId, p.PostalCode AS PostalCodeValue, p.StatusId, p.RowVersion, p.CreatedById, p.CreatedTime, p.UpdatedById, p.UpdatedTime, p.DeletedById, p.DeletedTime 
                FROM [core].[PostalCode] p
                INNER JOIN [core].[SubDistrict] s ON p.SubDistrictId = s.SubDistrictId
                WHERE p.PostalCodeGuid = @postalCodeGuid AND s.SubDistrictGuid = @subDistrictGuid AND p.StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<PostalCode>(sql, new { postalCodeGuid, subDistrictGuid });
        }

        public async Task<IEnumerable<PostalCode>> GetAllPostalCodesAsync(Guid subDistrictGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT p.PostalCodeId, p.PostalCodeGuid, p.SubDistrictId, p.PostalCode AS PostalCodeValue, p.StatusId, p.RowVersion, p.CreatedById, p.CreatedTime, p.UpdatedById, p.UpdatedTime, p.DeletedById, p.DeletedTime
                FROM [core].[PostalCode] p
                INNER JOIN [core].[SubDistrict] s ON p.SubDistrictId = s.SubDistrictId
                WHERE s.SubDistrictGuid = @subDistrictGuid AND p.StatusId > 0 AND p.DeletedTime IS NULL
                ORDER BY p.PostalCodeId DESC";

            return await connection.QueryAsync<PostalCode>(sql, new { subDistrictGuid });
        }

        public async Task CreatePostalCodeAsync(PostalCode postalCode, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampCreate(postalCode);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"INSERT INTO [core].[PostalCode]
                                 (PostalCodeGuid, PostalCode, SubDistrictId, StatusId, CreatedById, CreatedTime)
                                 VALUES
                                 (@PostalCodeGuid, @PostalCodeValue, @SubDistrictId, @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, postalCode, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "PostalCode",
                primaryKey: postalCode.PostalCodeGuid.ToString(),
                userId: postalCode.CreatedById.ToString(),
                oldEntity: null,
                newEntity: postalCode);
        }

        public async Task UpdatePostalCodeAsync(PostalCode postalCode, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampUpdate(postalCode);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            
            var getSql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[PostalCode] WHERE PostalCodeGuid = @PostalCodeGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<PostalCode>(getSql, new { postalCode.PostalCodeGuid }, transaction);

            const string sql = @"UPDATE [core].[PostalCode]
                                 SET PostalCode = @PostalCodeValue,
                                     StatusId = @StatusId,
                                     UpdatedById = @UpdatedById,
                                     UpdatedTime = @UpdatedTime
                                 WHERE PostalCodeGuid = @PostalCodeGuid";
            await conn.ExecuteAsync(sql, postalCode, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "PostalCode",
                primaryKey: postalCode.PostalCodeGuid.ToString(),
                userId: postalCode.UpdatedById?.ToString(),
                oldEntity: oldData,
                newEntity: postalCode);
        }

        public async Task SoftDeletePostalCodeAsync(PostalCode postalCode, long deletedBy, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(postalCode, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[PostalCode] WHERE PostalCodeGuid = @PostalCodeGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<PostalCode>(getSql, new { postalCode.PostalCodeGuid }, transaction);

            const string sql = @"UPDATE [core].[PostalCode]
                         SET StatusId = 0,
                             DeletedById = @DeletedById,
                             DeletedTime = @DeletedTime
                         WHERE PostalCodeGuid = @PostalCodeGuid";

            await conn.ExecuteAsync(sql, postalCode, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "PostalCode",
                primaryKey: postalCode.PostalCodeGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: postalCode);
        }

        public async Task DeletePostalCodeAsync(Guid postalCodeGuid, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[PostalCode] WHERE PostalCodeGuid = @PostalCodeGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<PostalCode>(getSql, new { postalCodeGuid }, transaction);

            const string sql = @"DELETE FROM [core].[PostalCode]
                                 WHERE PostalCodeGuid = @postalCodeGuid";
            await conn.ExecuteAsync(sql, new { postalCodeGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "PostalCode",
                primaryKey: postalCodeGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<PostalCode>> SearchPostalCodeAsync(
            Guid subDistrictGuid,
            string? postalCodeValue, string? postalCodeValueSearchType,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "s.SubDistrictGuid = @subDistrictGuid",
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };

            var parameters = new DynamicParameters();
            parameters.Add("@subDistrictGuid", subDistrictGuid);

            if (!string.IsNullOrWhiteSpace(postalCodeValue))
            {
                var param = SqlFilterHelper.BuildFilter("a.PostalCode", "@postalCodeValue", postalCodeValueSearchType, parameters, "postalCodeValue", postalCodeValue);
                whereClauses.Add(param);
            }

            var sql = $@"
            SELECT a.PostalCodeId, a.PostalCodeGuid, a.SubDistrictId, a.PostalCode AS PostalCodeValue, a.StatusId, a.RowVersion, a.CreatedById, a.CreatedTime, a.UpdatedById, a.UpdatedTime, a.DeletedById, a.DeletedTime
            FROM [core].[PostalCode] a
            INNER JOIN [core].[SubDistrict] s ON a.SubDistrictId = s.SubDistrictId
            WHERE {string.Join(" AND ", whereClauses)}
            ORDER BY a.PostalCodeId DESC";

            return await connection.QueryAsync<PostalCode>(sql, parameters, transaction);
        }
    }
}
