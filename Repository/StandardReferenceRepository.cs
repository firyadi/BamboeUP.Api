using BamboeUp.Audit.Contracts;
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
    public class StandardReferenceRepository : IStandardReferenceRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public StandardReferenceRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<StandardReference?> GetStandardReferenceAsync(Guid standardReferenceGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[StandardReference] a
                WHERE a.StandardReferenceGuid = @standardReferenceGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<StandardReference>(sql, new { standardReferenceGuid });
        }

        public async Task<IEnumerable<StandardReference>> GetAllStandardReferencesAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*,
                       (SELECT COUNT(*) FROM [app].[StandardReferenceItem] i WHERE i.StandardReferenceId = a.StandardReferenceId AND i.StatusId > 0 AND i.DeletedTime IS NULL) AS DefaultItemsCount,
                       (SELECT COUNT(*) FROM [app].[StandardReferenceScope] s WHERE s.StandardReferenceId = a.StandardReferenceId AND s.StatusId > 0 AND s.DeletedTime IS NULL) AS ScopesCount
                FROM [app].[StandardReference] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.StandardReferenceId DESC";
            return await connection.QueryAsync<StandardReference>(sql);
        }

        public async Task<IEnumerable<StandardReference>> GetStandardReferencesForParentSelectionAsync(Guid? currentRecordGuid, bool trackChanges)
        {
            // StandardReference no longer has self-referencing parent in the new schema,
            // but we keep the method signature with empty/basic implementation returning all active ones to avoid breaking compile.
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[StandardReference] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                  {(currentRecordGuid.HasValue && currentRecordGuid.Value != Guid.Empty ? "AND a.StandardReferenceGuid != @currentRecordGuid" : "")}
                ORDER BY a.StandardReferenceName ASC";
            return await connection.QueryAsync<StandardReference>(sql, new { currentRecordGuid });
        }

        public async Task CreateStandardReferenceAsync(StandardReference standardReference, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(standardReference);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[StandardReference]
                (StandardReferenceGuid, CreatedById, StatusId, CreatedTime, StandardReferenceInitial, StandardReferenceName, Description)
                VALUES
                (@StandardReferenceGuid, @CreatedById, @StatusId, @CreatedTime, @StandardReferenceInitial, @StandardReferenceName, @Description);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            standardReference.StandardReferenceId = await conn.QuerySingleAsync<long>(sql, standardReference, transaction);
        }

        public async Task UpdateStandardReferenceAsync(StandardReference standardReference, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(standardReference);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[StandardReference]
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
            const string sql = @"
                UPDATE [app].[StandardReference]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE StandardReferenceGuid = @StandardReferenceGuid";

            await conn.ExecuteAsync(sql, standardReference, transaction);
        }

        public async Task DeleteStandardReferenceAsync(Guid standardReferenceGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[StandardReference] WHERE StandardReferenceGuid = @standardReferenceGuid";
            await conn.ExecuteAsync(sql, new { standardReferenceGuid }, transaction);
        }

        public async Task<IEnumerable<StandardReference>> SearchStandardReferenceAsync(
            string? standardReferenceInitial, string? standardReferenceInitialSearchType,
            string? standardReferenceName, string? standardReferenceNameSearchType,
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

            if (!string.IsNullOrWhiteSpace(standardReferenceInitial))
            {
                var param = SqlFilterHelper.BuildFilter("a.StandardReferenceInitial", "@standardReferenceInitial", standardReferenceInitialSearchType, parameters, "standardReferenceInitial", standardReferenceInitial);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(standardReferenceName))
            {
                var param = SqlFilterHelper.BuildFilter("a.StandardReferenceName", "@standardReferenceName", standardReferenceNameSearchType, parameters, "standardReferenceName", standardReferenceName);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", noteSearchType, parameters, "description", note);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            var sql = $@"
                SELECT a.*,
                       (SELECT COUNT(*) FROM [app].[StandardReferenceItem] i WHERE i.StandardReferenceId = a.StandardReferenceId AND i.StatusId > 0 AND i.DeletedTime IS NULL) AS DefaultItemsCount,
                       (SELECT COUNT(*) FROM [app].[StandardReferenceScope] s WHERE s.StandardReferenceId = a.StandardReferenceId AND s.StatusId > 0 AND s.DeletedTime IS NULL) AS ScopesCount
                FROM [app].[StandardReference] a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.StandardReferenceId DESC";

            return await connection.QueryAsync<StandardReference>(sql, parameters, transaction);
        }
    }
}
