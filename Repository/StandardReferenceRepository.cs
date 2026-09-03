using BamboeUp.Audit.Contracts;
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
    public class StandardReferenceRepository : IStandardReferenceRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public StandardReferenceRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
            _audit = auditService;
        }

        public async Task<StandardReference?> GetStandardReferenceAsync(Guid standardReferenceGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("app", "StandardReference")} a
                WHERE a.StandardReferenceGuid = @standardReferenceGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<StandardReference>(sql, new { standardReferenceGuid });
        }

        public async Task<IEnumerable<StandardReference>> GetAllStandardReferencesAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")},
                       (SELECT COUNT(*) FROM {_dialect.Qualify("app", "StandardReferenceItem")} i WHERE i.StandardReferenceId = a.StandardReferenceId AND i.StatusId > 0 AND i.DeletedTime IS NULL) AS DefaultItemsCount,
                       (SELECT COUNT(*) FROM {_dialect.Qualify("app", "StandardReferenceScope")} s WHERE s.StandardReferenceId = a.StandardReferenceId AND s.StatusId > 0 AND s.DeletedTime IS NULL) AS ScopesCount
                FROM {_dialect.Qualify("app", "StandardReference")} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.StandardReferenceId DESC{_dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<StandardReference>(sql);
        }

        public async Task<IEnumerable<StandardReference>> GetStandardReferencesForParentSelectionAsync(Guid? currentRecordGuid, bool trackChanges)
        {
            // StandardReference no longer has self-referencing parent in the new schema,
            // but we keep the method signature with empty/basic implementation returning all active ones to avoid breaking compile.
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("app", "StandardReference")} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                  {(currentRecordGuid.HasValue && currentRecordGuid.Value != Guid.Empty ? "AND a.StandardReferenceGuid != @currentRecordGuid" : "")}
                ORDER BY a.StandardReferenceName ASC";
            return await connection.QueryAsync<StandardReference>(sql, new { currentRecordGuid });
        }

        public async Task CreateStandardReferenceAsync(StandardReference standardReference, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(standardReference);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_dialect.Qualify("app", "StandardReference")}
                (StandardReferenceGuid, CreatedById, StatusId, CreatedTime, StandardReferenceInitial, StandardReferenceName, Description)
                VALUES
                (@StandardReferenceGuid, @CreatedById, @StatusId, @CreatedTime, @StandardReferenceInitial, @StandardReferenceName, @Description){_dialect.ReturnIdentitySuffix("StandardReferenceId")}";
            standardReference.StandardReferenceId = await conn.QuerySingleAsync<long>(sql, standardReference, transaction);
        }

        public async Task UpdateStandardReferenceAsync(StandardReference standardReference, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(standardReference);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "StandardReference")}
                SET StandardReferenceInitial = @StandardReferenceInitial,
                    StandardReferenceName = @StandardReferenceName,
                    Description = @Description,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE StandardReferenceGuid = @StandardReferenceGuid";
            await conn.ExecuteAsync(sql, standardReference, transaction);
        }

        public async Task SoftDeleteStandardReferenceAsync(StandardReference standardReference, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(standardReference, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "StandardReference")}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE StandardReferenceGuid = @StandardReferenceGuid";

            await conn.ExecuteAsync(sql, standardReference, transaction);
        }

        public async Task DeleteStandardReferenceAsync(Guid standardReferenceGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"DELETE FROM {_dialect.Qualify("app", "StandardReference")} WHERE StandardReferenceGuid = @standardReferenceGuid";
            await conn.ExecuteAsync(sql, new { standardReferenceGuid }, transaction);
        }

        public async Task<IEnumerable<StandardReference>> SearchStandardReferenceAsync(
            string? standardReferenceInitial, string? standardReferenceInitialSearchType,
            string? standardReferenceName, string? standardReferenceNameSearchType,
            string? note, string? noteSearchType,
            string? standardReferenceItemInitial, string? standardReferenceItemInitialSearchType,
            string? standardReferenceItemName, string? standardReferenceItemNameSearchType,
            Guid companyGuid, Guid companyOfficeGuid,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(standardReferenceInitial))
            {
                var param = SqlFilterHelper.BuildFilter("a.StandardReferenceInitial", "@standardReferenceInitial", standardReferenceInitialSearchType, parameters, "standardReferenceInitial", standardReferenceInitial, _dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(standardReferenceName))
            {
                var param = SqlFilterHelper.BuildFilter("a.StandardReferenceName", "@standardReferenceName", standardReferenceNameSearchType, parameters, "standardReferenceName", standardReferenceName, _dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", noteSearchType, parameters, "description", note, _dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            var itemFilters = new List<string>();
            if (!string.IsNullOrWhiteSpace(standardReferenceItemInitial))
            {
                var param = SqlFilterHelper.BuildFilter("i.StandardReferenceItemInitial", "@standardReferenceItemInitial", standardReferenceItemInitialSearchType, parameters, "standardReferenceItemInitial", standardReferenceItemInitial, _dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    itemFilters.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(standardReferenceItemName))
            {
                var param = SqlFilterHelper.BuildFilter("i.StandardReferenceItemName", "@standardReferenceItemName", standardReferenceItemNameSearchType, parameters, "standardReferenceItemName", standardReferenceItemName, _dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    itemFilters.Add(param);
            }

            if (itemFilters.Count > 0)
            {
                whereClauses.Add($@"
                    EXISTS (
                        SELECT 1
                        FROM {_dialect.Qualify("app", "StandardReferenceItem")} i
                        WHERE i.StandardReferenceId = a.StandardReferenceId
                          AND i.StatusId > 0
                          AND i.DeletedTime IS NULL
                          AND {string.Join(" AND ", itemFilters)}
                    )");
            }

            var sql = $@"
                SELECT a.*,
                       (SELECT COUNT(*) FROM {_dialect.Qualify("app", "StandardReferenceItem")} i WHERE i.StandardReferenceId = a.StandardReferenceId AND i.StatusId > 0 AND i.DeletedTime IS NULL) AS DefaultItemsCount,
                       (SELECT COUNT(*) FROM {_dialect.Qualify("app", "StandardReferenceScope")} s WHERE s.StandardReferenceId = a.StandardReferenceId AND s.StatusId > 0 AND s.DeletedTime IS NULL) AS ScopesCount
                FROM {_dialect.Qualify("app", "StandardReference")} a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.StandardReferenceId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<StandardReference>(sql, parameters, transaction);
        }
    }
}
