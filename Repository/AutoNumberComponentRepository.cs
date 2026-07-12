using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class AutoNumberComponentRepository : IAutoNumberComponentRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public AutoNumberComponentRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<AutoNumberComponent?> GetAutoNumberComponentAsync(Guid autoNumberComponentGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumberComponent] a
                WHERE a.AutoNumberComponentGuid = @autoNumberComponentGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<AutoNumberComponent>(sql, new { autoNumberComponentGuid });
        }

        public async Task<IEnumerable<AutoNumberComponent>> GetAllAutoNumberComponentsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumberComponent] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.AutoNumberTemplateId ASC, a.SeqNo ASC";
            return await connection.QueryAsync<AutoNumberComponent>(sql);
        }

        public async Task CreateAutoNumberComponentAsync(AutoNumberComponent autoNumberComponent, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(autoNumberComponent);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[AutoNumberComponent]
                (AutoNumberComponentGuid, AutoNumberTemplateId, SeqNo,
                 ComponentType, StaticValue, Format, CounterLength, IsResetKey,
                 StatusId, CreatedById, CreatedTime)
                VALUES
                (@AutoNumberComponentGuid, @AutoNumberTemplateId, @SeqNo,
                 @ComponentType, @StaticValue, @Format, @CounterLength, @IsResetKey,
                 @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, autoNumberComponent, transaction);

            await _audit.LogAsync(
                actionType: "CREATE",
                tableName: "AutoNumberComponent",
                primaryKey: autoNumberComponent.AutoNumberComponentGuid.ToString(),
                userId: autoNumberComponent.CreatedById.ToString(),
                oldEntity: null,
                newEntity: autoNumberComponent);
        }

        public async Task UpdateAutoNumberComponentAsync(AutoNumberComponent autoNumberComponent, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(autoNumberComponent);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberComponentAsync(autoNumberComponent.AutoNumberComponentGuid, false);

            const string sql = @"
                UPDATE [app].[AutoNumberComponent]
                SET AutoNumberTemplateId = @AutoNumberTemplateId,
                    SeqNo                = @SeqNo,
                    ComponentType        = @ComponentType,
                    StaticValue          = @StaticValue,
                    Format               = @Format,
                    CounterLength        = @CounterLength,
                    IsResetKey           = @IsResetKey,
                    StatusId             = @StatusId,
                    UpdatedById          = @UpdatedById,
                    UpdatedTime          = @UpdatedTime
                WHERE AutoNumberComponentGuid = @AutoNumberComponentGuid";
            await conn.ExecuteAsync(sql, autoNumberComponent, transaction);

            await _audit.LogAsync(
                actionType: "UPDATE",
                tableName: "AutoNumberComponent",
                primaryKey: autoNumberComponent.AutoNumberComponentGuid.ToString(),
                userId: autoNumberComponent.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: autoNumberComponent);
        }

        public async Task SoftDeleteAutoNumberComponentAsync(AutoNumberComponent autoNumberComponent, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(autoNumberComponent, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberComponentAsync(autoNumberComponent.AutoNumberComponentGuid, false);

            const string sql = @"
                UPDATE [app].[AutoNumberComponent]
                SET StatusId    = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AutoNumberComponentGuid = @AutoNumberComponentGuid";

            await conn.ExecuteAsync(sql, autoNumberComponent, transaction);

            await _audit.LogAsync(
                actionType: "DELETE",
                tableName: "AutoNumberComponent",
                primaryKey: autoNumberComponent.AutoNumberComponentGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task DeleteAutoNumberComponentAsync(Guid autoNumberComponentGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberComponentAsync(autoNumberComponentGuid, false);

            const string sql = @"DELETE FROM [app].[AutoNumberComponent] WHERE AutoNumberComponentGuid = @autoNumberComponentGuid";
            await conn.ExecuteAsync(sql, new { autoNumberComponentGuid }, transaction);

            await _audit.LogAsync(
                actionType: "DELETE_ADMIN",
                tableName: "AutoNumberComponent",
                primaryKey: autoNumberComponentGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<AutoNumberComponent>> SearchAutoNumberComponentAsync(
            string? componentType, string? componentTypeSearchType,
            string? staticValue, string? staticValueSearchType,
            string? format, string? formatSearchType,
            long? autoNumberTemplateId,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            // 🔍 ComponentType
            if (!string.IsNullOrWhiteSpace(componentType))
            {
                var param = SqlFilterHelper.BuildFilter("a.ComponentType", "@componentType", componentTypeSearchType, parameters, "componentType", componentType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            // 🔍 StaticValue
            if (!string.IsNullOrWhiteSpace(staticValue))
            {
                var param = SqlFilterHelper.BuildFilter("a.StaticValue", "@staticValue", staticValueSearchType, parameters, "staticValue", staticValue);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            // 🔍 Format
            if (!string.IsNullOrWhiteSpace(format))
            {
                var param = SqlFilterHelper.BuildFilter("a.Format", "@format", formatSearchType, parameters, "format", format);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            // 🔍 AutoNumberTemplateId
            if (autoNumberTemplateId.HasValue)
            {
                whereClauses.Add("a.AutoNumberTemplateId = @autoNumberTemplateId");
                parameters.Add("autoNumberTemplateId", autoNumberTemplateId.Value);
            }

            var sql = $@"
                SELECT a.*
                FROM [app].[AutoNumberComponent] a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AutoNumberTemplateId ASC, a.SeqNo ASC";

            return await connection.QueryAsync<AutoNumberComponent>(sql, parameters, transaction);
        }

        // Detail helpers — scoped by parent AutoNumberTemplateGuid
        public async Task<IEnumerable<AutoNumberComponent>> GetAllByAutoNumberTemplateGuidAsync(Guid autoNumberTemplateGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumberComponent] a
                INNER JOIN [app].[AutoNumberTemplate] t
                    ON a.AutoNumberTemplateId = t.AutoNumberTemplateId
                WHERE t.AutoNumberTemplateGuid = @autoNumberTemplateGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.SeqNo ASC, a.AutoNumberComponentId ASC";
            return await connection.QueryAsync<AutoNumberComponent>(sql, new { autoNumberTemplateGuid });
        }
    }
}
