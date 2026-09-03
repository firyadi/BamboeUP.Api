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
    public class StandardReferenceScopeItemRepository : IStandardReferenceScopeItemRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public StandardReferenceScopeItemRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        public async Task<StandardReferenceScopeItem?> GetStandardReferenceScopeItemAsync(Guid scopeItemGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("app", "StandardReferenceScopeItem")} a
                WHERE a.StandardReferenceScopeItemGuid = @scopeItemGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<StandardReferenceScopeItem>(sql, new { scopeItemGuid });
        }

        public async Task<IEnumerable<StandardReferenceScopeItem>> GetAllStandardReferenceScopeItemsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("app", "StandardReferenceScopeItem")} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.StandardReferenceScopeItemId DESC{_dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<StandardReferenceScopeItem>(sql);
        }

        public async Task CreateStandardReferenceScopeItemAsync(StandardReferenceScopeItem scopeItem, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(scopeItem);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_dialect.Qualify("app", "StandardReferenceScopeItem")}
                (StandardReferenceScopeItemGuid, StandardReferenceScopeId, StandardReferenceScopeGuid, CreatedById, StatusId, CreatedTime,
                 StandardReferenceScopeItemInitial, StandardReferenceScopeItemName, StandardReferenceScopeItemValue, DisplayOrder
                )
                VALUES
                (@StandardReferenceScopeItemGuid, @StandardReferenceScopeId, @StandardReferenceScopeGuid, @CreatedById, @StatusId, @CreatedTime,
                 @StandardReferenceScopeItemInitial, @StandardReferenceScopeItemName, @StandardReferenceScopeItemValue, @DisplayOrder
                ){_dialect.ReturnIdentitySuffix("StandardReferenceScopeItemId")}";
            scopeItem.StandardReferenceScopeItemId = await conn.QuerySingleAsync<long>(sql, scopeItem, transaction);
        }

        public async Task UpdateStandardReferenceScopeItemAsync(StandardReferenceScopeItem scopeItem, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(scopeItem);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "StandardReferenceScopeItem")}
                SET StandardReferenceScopeId = @StandardReferenceScopeId,
                    StandardReferenceScopeGuid = @StandardReferenceScopeGuid,
                    StandardReferenceScopeItemInitial = @StandardReferenceScopeItemInitial,
                    StandardReferenceScopeItemName = @StandardReferenceScopeItemName,
                    StandardReferenceScopeItemValue = @StandardReferenceScopeItemValue,
                    DisplayOrder = @DisplayOrder,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE StandardReferenceScopeItemGuid = @StandardReferenceScopeItemGuid";
            await conn.ExecuteAsync(sql, scopeItem, transaction);
        }

        public async Task SoftDeleteStandardReferenceScopeItemAsync(StandardReferenceScopeItem scopeItem, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(scopeItem, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "StandardReferenceScopeItem")}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE StandardReferenceScopeItemGuid = @StandardReferenceScopeItemGuid";

            await conn.ExecuteAsync(sql, scopeItem, transaction);
        }

        public async Task DeleteStandardReferenceScopeItemAsync(Guid scopeItemGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"DELETE FROM {_dialect.Qualify("app", "StandardReferenceScopeItem")} WHERE StandardReferenceScopeItemGuid = @scopeItemGuid";
            await conn.ExecuteAsync(sql, new { scopeItemGuid }, transaction);
        }

        public async Task DeleteByStandardReferenceScopeGuidAsync(Guid standardReferenceScopeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"DELETE FROM {_dialect.Qualify("app", "StandardReferenceScopeItem")} WHERE StandardReferenceScopeGuid = @standardReferenceScopeGuid";
            await conn.ExecuteAsync(sql, new { standardReferenceScopeGuid }, transaction);
        }

        public async Task<IEnumerable<StandardReferenceScopeItem>> GetAllByStandardReferenceScopeGuidAsync(Guid standardReferenceScopeGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("app", "StandardReferenceScopeItem")} a
                WHERE a.StandardReferenceScopeGuid = @standardReferenceScopeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.DisplayOrder ASC, a.StandardReferenceScopeItemId DESC";
            return await connection.QueryAsync<StandardReferenceScopeItem>(sql, new { standardReferenceScopeGuid });
        }

        public async Task<StandardReferenceScopeItem?> GetByScopeGuidAndItemGuidAsync(Guid standardReferenceScopeGuid, Guid standardReferenceScopeItemGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("app", "StandardReferenceScopeItem")} a
                WHERE a.StandardReferenceScopeGuid = @standardReferenceScopeGuid
                  AND a.StandardReferenceScopeItemGuid = @standardReferenceScopeItemGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<StandardReferenceScopeItem>(sql, new { standardReferenceScopeGuid, standardReferenceScopeItemGuid });
        }

        public async Task<IEnumerable<StandardReferenceScopeItem>> SearchStandardReferenceScopeItemAsync(
            string? scopeItemInitial, string? scopeItemInitialSearchType,
            Guid standardReferenceScopeGuid, Guid standardReferenceScopeItemGuid,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (standardReferenceScopeGuid != Guid.Empty)
            {
                whereClauses.Add("a.StandardReferenceScopeGuid = @standardReferenceScopeGuid");
                parameters.Add("@standardReferenceScopeGuid", standardReferenceScopeGuid);
            }
            if (standardReferenceScopeItemGuid != Guid.Empty)
            {
                whereClauses.Add("a.StandardReferenceScopeItemGuid = @standardReferenceScopeItemGuid");
                parameters.Add("@standardReferenceScopeItemGuid", standardReferenceScopeItemGuid);
            }
            if (!string.IsNullOrWhiteSpace(scopeItemInitial))
            {
                var param = SqlFilterHelper.BuildFilter("a.StandardReferenceScopeItemInitial", "@scopeItemInitial", scopeItemInitialSearchType, parameters, "scopeItemInitial", scopeItemInitial, _dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("app", "StandardReferenceScopeItem")} a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.DisplayOrder ASC, a.StandardReferenceScopeItemId DESC";

            return await connection.QueryAsync<StandardReferenceScopeItem>(sql, parameters, transaction);
        }
    }
}
