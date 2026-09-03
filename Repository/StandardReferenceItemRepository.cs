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
    public class StandardReferenceItemRepository : IStandardReferenceItemRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public StandardReferenceItemRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
            _audit = auditService;
        }

        public async Task<StandardReferenceItem?> GetStandardReferenceItemAsync(Guid standardReferenceItemGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("app", "StandardReferenceItem")} a
                WHERE a.StandardReferenceItemGuid = @standardReferenceItemGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<StandardReferenceItem>(sql, new { standardReferenceItemGuid });
        }

        public async Task<IEnumerable<StandardReferenceItem>> GetAllStandardReferenceItemsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_dialect.Qualify("app", "StandardReferenceItem")} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.StandardReferenceItemId DESC{_dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<StandardReferenceItem>(sql);
        }

        public async Task CreateStandardReferenceItemAsync(StandardReferenceItem standardReferenceItem, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(standardReferenceItem);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_dialect.Qualify("app", "StandardReferenceItem")}
                (StandardReferenceItemGuid, CreatedById, StatusId, CreatedTime, StandardReferenceId, StandardReferenceGuid, StandardReferenceItemInitial, StandardReferenceItemName, StandardReferenceItemValue, DisplayOrder)
                VALUES
                (@StandardReferenceItemGuid, @CreatedById, @StatusId, @CreatedTime, @StandardReferenceId, @StandardReferenceGuid, @StandardReferenceItemInitial, @StandardReferenceItemName, @StandardReferenceItemValue, @DisplayOrder){_dialect.ReturnIdentitySuffix("StandardReferenceItemId")}";
            standardReferenceItem.StandardReferenceItemId = await conn.QuerySingleAsync<long>(sql, standardReferenceItem, transaction);
        }

        public async Task UpdateStandardReferenceItemAsync(StandardReferenceItem standardReferenceItem, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(standardReferenceItem);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "StandardReferenceItem")}
                SET StandardReferenceId = @StandardReferenceId,
                    StandardReferenceGuid = @StandardReferenceGuid,
                    StandardReferenceItemInitial = @StandardReferenceItemInitial,
                    StandardReferenceItemName = @StandardReferenceItemName,
                    StandardReferenceItemValue = @StandardReferenceItemValue,
                    DisplayOrder = @DisplayOrder,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE StandardReferenceItemGuid = @StandardReferenceItemGuid";
            await conn.ExecuteAsync(sql, standardReferenceItem, transaction);
        }

        public async Task SoftDeleteStandardReferenceItemAsync(StandardReferenceItem standardReferenceItem, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(standardReferenceItem, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "StandardReferenceItem")}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE StandardReferenceItemGuid = @StandardReferenceItemGuid";

            await conn.ExecuteAsync(sql, standardReferenceItem, transaction);
        }

        public async Task DeleteStandardReferenceItemAsync(Guid standardReferenceItemGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                DELETE FROM {_dialect.Qualify("app", "StandardReferenceItem")}
                WHERE StandardReferenceItemGuid = @standardReferenceItemGuid";
            await conn.ExecuteAsync(sql, new { standardReferenceItemGuid }, transaction);
        }

        public async Task<IEnumerable<StandardReferenceItem>> SearchStandardReferenceItemAsync(
            string? standardReferenceInitial, string? standardReferenceInitialSearchType,
            string? standardReferenceItemInitial, string? standardReferenceItemInitialSearchType,
            string? standardReferenceItemName, string? standardReferenceItemNameSearchType,
            string? note, string? noteSearchType,
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

            if (!string.IsNullOrWhiteSpace(standardReferenceItemInitial))
            {
                var param = SqlFilterHelper.BuildFilter("a.StandardReferenceItemInitial", "@standardReferenceItemInitial", standardReferenceItemInitialSearchType, parameters, "standardReferenceItemInitial", standardReferenceItemInitial, _dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(standardReferenceItemName))
            {
                var param = SqlFilterHelper.BuildFilter("a.StandardReferenceItemName", "@standardReferenceItemName", standardReferenceItemNameSearchType, parameters, "standardReferenceItemName", standardReferenceItemName, _dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            var sql = $@"
                SELECT a.*
                FROM {_dialect.Qualify("app", "StandardReferenceItem")} a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.DisplayOrder ASC, a.StandardReferenceItemId DESC";

            return await connection.QueryAsync<StandardReferenceItem>(sql, parameters, transaction);
        }

        public async Task<IEnumerable<StandardReferenceItem>> GetAllByStandardReferenceGuidAsync(Guid standardReferenceGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")} FROM {_dialect.Qualify("app", "StandardReferenceItem")}
                                  WHERE StandardReferenceGuid = @standardReferenceGuid AND StatusId > 0 AND DeletedTime IS NULL
                                  ORDER BY DisplayOrder ASC, StandardReferenceItemId DESC";
            return await connection.QueryAsync<StandardReferenceItem>(sql, new { standardReferenceGuid });
        }

        public async Task<StandardReferenceItem?> GetByStandardReferenceGuidAndStandardReferenceItemGuidAsync(Guid standardReferenceGuid, Guid standardReferenceItemGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")} FROM {_dialect.Qualify("app", "StandardReferenceItem")}
                                  WHERE StandardReferenceGuid = @standardReferenceGuid
                                    AND StandardReferenceItemGuid = @standardReferenceItemGuid
                                    AND StatusId > 0 AND DeletedTime IS NULL";

            return await connection.QuerySingleOrDefaultAsync<StandardReferenceItem>(sql, new {
                standardReferenceGuid,
                standardReferenceItemGuid
            });
        }
    }
}
