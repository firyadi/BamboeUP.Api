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
    public class StandardReferenceScopeRepository : IStandardReferenceScopeRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public StandardReferenceScopeRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        public async Task<StandardReferenceScope?> GetStandardReferenceScopeAsync(Guid standardReferenceScopeGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_StandardReferenceId.StandardReferenceName AS StandardReferenceName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName

                FROM {_dialect.Qualify("app", "StandardReferenceScope")} a
LEFT JOIN {_dialect.Qualify("app", "StandardReference")} j_StandardReferenceId ON a.StandardReferenceId = j_StandardReferenceId.StandardReferenceId
LEFT JOIN {_dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId

                WHERE a.StandardReferenceScopeGuid = @standardReferenceScopeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<StandardReferenceScope>(sql, new { standardReferenceScopeGuid });
        }

        public async Task<StandardReferenceScope?> GetByStandardReferenceGuidAndScopeGuidAsync(Guid standardReferenceGuid, Guid standardReferenceScopeGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_StandardReferenceId.StandardReferenceName AS StandardReferenceName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName

                FROM {_dialect.Qualify("app", "StandardReferenceScope")} a
LEFT JOIN {_dialect.Qualify("app", "StandardReference")} j_StandardReferenceId ON a.StandardReferenceId = j_StandardReferenceId.StandardReferenceId
LEFT JOIN {_dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId

                WHERE a.StandardReferenceGuid = @standardReferenceGuid
                  AND a.StandardReferenceScopeGuid = @standardReferenceScopeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<StandardReferenceScope>(sql, new { standardReferenceGuid, standardReferenceScopeGuid });
        }

        public async Task<IEnumerable<StandardReferenceScope>> GetAllByStandardReferenceGuidAsync(Guid standardReferenceGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_StandardReferenceId.StandardReferenceName AS StandardReferenceName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName

                FROM {_dialect.Qualify("app", "StandardReferenceScope")} a
LEFT JOIN {_dialect.Qualify("app", "StandardReference")} j_StandardReferenceId ON a.StandardReferenceId = j_StandardReferenceId.StandardReferenceId
LEFT JOIN {_dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId

                WHERE a.StandardReferenceGuid = @standardReferenceGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.StandardReferenceScopeId DESC{_dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<StandardReferenceScope>(sql, new { standardReferenceGuid });
        }

        public async Task<IEnumerable<StandardReferenceScope>> GetAllStandardReferenceScopesAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_StandardReferenceId.StandardReferenceName AS StandardReferenceName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName

                FROM {_dialect.Qualify("app", "StandardReferenceScope")} a
LEFT JOIN {_dialect.Qualify("app", "StandardReference")} j_StandardReferenceId ON a.StandardReferenceId = j_StandardReferenceId.StandardReferenceId
LEFT JOIN {_dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId

                WHERE a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.StandardReferenceScopeId DESC{_dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<StandardReferenceScope>(sql);
        }

        public async Task CreateStandardReferenceScopeAsync(StandardReferenceScope scope, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(scope);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_dialect.Qualify("app", "StandardReferenceScope")}
                (StandardReferenceScopeGuid, StandardReferenceId, StandardReferenceGuid, CreatedById, StatusId, CreatedTime,
                 CompanyId, CompanyGuid, CompanyOfficeId, CompanyOfficeGuid
                )
                VALUES
                (@StandardReferenceScopeGuid, @StandardReferenceId, @StandardReferenceGuid, @CreatedById, @StatusId, @CreatedTime,
                 @CompanyId, @CompanyGuid, @CompanyOfficeId, @CompanyOfficeGuid
                ){_dialect.ReturnIdentitySuffix("StandardReferenceScopeId")}";
            scope.StandardReferenceScopeId = await conn.QuerySingleAsync<long>(sql, scope, transaction);
        }

        public async Task UpdateStandardReferenceScopeAsync(StandardReferenceScope scope, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(scope);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "StandardReferenceScope")}
                SET StandardReferenceId = @StandardReferenceId,
                    StandardReferenceGuid = @StandardReferenceGuid,
                    CompanyId = @CompanyId,
                    CompanyGuid = @CompanyGuid,
                    CompanyOfficeId = @CompanyOfficeId,
                    CompanyOfficeGuid = @CompanyOfficeGuid,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE StandardReferenceScopeGuid = @StandardReferenceScopeGuid";
            await conn.ExecuteAsync(sql, scope, transaction);
        }

        public async Task SoftDeleteStandardReferenceScopeAsync(StandardReferenceScope scope, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(scope, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "StandardReferenceScope")}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE StandardReferenceScopeGuid = @StandardReferenceScopeGuid";

            await conn.ExecuteAsync(sql, scope, transaction);
        }

        public async Task DeleteStandardReferenceScopeAsync(Guid standardReferenceScopeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"DELETE FROM {_dialect.Qualify("app", "StandardReferenceScope")} WHERE StandardReferenceScopeGuid = @standardReferenceScopeGuid";
            await conn.ExecuteAsync(sql, new { standardReferenceScopeGuid }, transaction);
        }

        public async Task DeleteByStandardReferenceGuidAsync(Guid standardReferenceGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"DELETE FROM {_dialect.Qualify("app", "StandardReferenceScope")} WHERE StandardReferenceGuid = @standardReferenceGuid";
            await conn.ExecuteAsync(sql, new { standardReferenceGuid }, transaction);
        }

        public async Task<IEnumerable<StandardReferenceScope>> SearchStandardReferenceScopeAsync(
            Guid companyGuid, Guid companyOfficeGuid,
            Guid standardReferenceGuid, Guid standardReferenceScopeGuid,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (standardReferenceGuid != Guid.Empty)
            {
                whereClauses.Add("a.StandardReferenceGuid = @standardReferenceGuid");
                parameters.Add("@standardReferenceGuid", standardReferenceGuid);
            }
            if (standardReferenceScopeGuid != Guid.Empty)
            {
                whereClauses.Add("a.StandardReferenceScopeGuid = @standardReferenceScopeGuid");
                parameters.Add("@standardReferenceScopeGuid", standardReferenceScopeGuid);
            }
            if (companyGuid != Guid.Empty)
            {
                whereClauses.Add("a.CompanyGuid = @companyGuid");
                parameters.Add("@companyGuid", companyGuid);
            }
            if (companyOfficeGuid != Guid.Empty)
            {
                whereClauses.Add("a.CompanyOfficeGuid = @companyOfficeGuid");
                parameters.Add("@companyOfficeGuid", companyOfficeGuid);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_StandardReferenceId.StandardReferenceName AS StandardReferenceName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName

                FROM {_dialect.Qualify("app", "StandardReferenceScope")} a
LEFT JOIN {_dialect.Qualify("app", "StandardReference")} j_StandardReferenceId ON a.StandardReferenceId = j_StandardReferenceId.StandardReferenceId
LEFT JOIN {_dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.StandardReferenceScopeId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<StandardReferenceScope>(sql, parameters, transaction);
        }
    }
}
