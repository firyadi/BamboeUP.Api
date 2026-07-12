using Contracts;
using Dapper;
using Entities.Models;
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

        public StandardReferenceScopeItemRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<StandardReferenceScopeItem?> GetStandardReferenceScopeItemAsync(Guid scopeItemGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[StandardReferenceScopeItem] a
                WHERE a.StandardReferenceScopeItemGuid = @scopeItemGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<StandardReferenceScopeItem>(sql, new { scopeItemGuid });
        }

        public async Task<IEnumerable<StandardReferenceScopeItem>> GetAllStandardReferenceScopeItemsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[StandardReferenceScopeItem] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.StandardReferenceScopeItemId DESC";
            return await connection.QueryAsync<StandardReferenceScopeItem>(sql);
        }

        public async Task CreateStandardReferenceScopeItemAsync(StandardReferenceScopeItem scopeItem, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(scopeItem);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[StandardReferenceScopeItem]
                (StandardReferenceScopeItemGuid, StandardReferenceScopeId, StandardReferenceScopeGuid, CreatedById, StatusId, CreatedTime,
                 StandardReferenceScopeItemInitial, StandardReferenceScopeItemName, StandardReferenceScopeItemValue, DisplayOrder
                )
                VALUES
                (@StandardReferenceScopeItemGuid, @StandardReferenceScopeId, @StandardReferenceScopeGuid, @CreatedById, @StatusId, @CreatedTime,
                 @StandardReferenceScopeItemInitial, @StandardReferenceScopeItemName, @StandardReferenceScopeItemValue, @DisplayOrder
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            scopeItem.StandardReferenceScopeItemId = await conn.QuerySingleAsync<long>(sql, scopeItem, transaction);
        }

        public async Task UpdateStandardReferenceScopeItemAsync(StandardReferenceScopeItem scopeItem, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(scopeItem);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[StandardReferenceScopeItem]
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
            const string sql = @"
                UPDATE [app].[StandardReferenceScopeItem]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE StandardReferenceScopeItemGuid = @StandardReferenceScopeItemGuid";

            await conn.ExecuteAsync(sql, scopeItem, transaction);
        }

        public async Task DeleteStandardReferenceScopeItemAsync(Guid scopeItemGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[StandardReferenceScopeItem] WHERE StandardReferenceScopeItemGuid = @scopeItemGuid";
            await conn.ExecuteAsync(sql, new { scopeItemGuid }, transaction);
        }

        public async Task DeleteByStandardReferenceScopeGuidAsync(Guid standardReferenceScopeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[StandardReferenceScopeItem] WHERE StandardReferenceScopeGuid = @standardReferenceScopeGuid";
            await conn.ExecuteAsync(sql, new { standardReferenceScopeGuid }, transaction);
        }

        public async Task<IEnumerable<StandardReferenceScopeItem>> GetAllByStandardReferenceScopeGuidAsync(Guid standardReferenceScopeGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[StandardReferenceScopeItem] a
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
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[StandardReferenceScopeItem] a
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
                var param = SqlFilterHelper.BuildFilter("a.StandardReferenceScopeItemInitial", "@scopeItemInitial", scopeItemInitialSearchType, parameters, "scopeItemInitial", scopeItemInitial);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[StandardReferenceScopeItem] a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.DisplayOrder ASC, a.StandardReferenceScopeItemId DESC";

            return await connection.QueryAsync<StandardReferenceScopeItem>(sql, parameters, transaction);
        }
    }
}
