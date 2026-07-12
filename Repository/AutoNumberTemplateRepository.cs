using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class AutoNumberTemplateRepository : IAutoNumberTemplateRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public AutoNumberTemplateRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<AutoNumberTemplate?> GetAutoNumberTemplateAsync(Guid autoNumberTemplateGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumberTemplate] a
                WHERE a.AutoNumberTemplateGuid = @autoNumberTemplateGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<AutoNumberTemplate>(sql, new { autoNumberTemplateGuid });
        }

        public async Task<IEnumerable<AutoNumberTemplate>> GetAllAutoNumberTemplatesAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumberTemplate] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.AutoNumberTemplateId DESC";
            return await connection.QueryAsync<AutoNumberTemplate>(sql);
        }

        public async Task CreateAutoNumberTemplateAsync(AutoNumberTemplate autoNumberTemplate, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(autoNumberTemplate);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[AutoNumberTemplate]
                (AutoNumberTemplateGuid, TemplateName, Description, EffectiveDate,
                 TemplateScopeType, SrFormMappingNumbering, CompanyId, CompanyOfficeId,
                 StatusId, CreatedById, CreatedTime)
                VALUES
                (@AutoNumberTemplateGuid, @TemplateName, @Description, @EffectiveDate,
                 @TemplateScopeType, @SrFormMappingNumbering, @CompanyId, @CompanyOfficeId,
                 @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, autoNumberTemplate, transaction);

            await _audit.LogAsync(
                actionType: "CREATE",
                tableName: "AutoNumberTemplate",
                primaryKey: autoNumberTemplate.AutoNumberTemplateGuid.ToString(),
                userId: autoNumberTemplate.CreatedById.ToString(),
                oldEntity: null,
                newEntity: autoNumberTemplate);
        }

        public async Task UpdateAutoNumberTemplateAsync(AutoNumberTemplate autoNumberTemplate, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(autoNumberTemplate);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberTemplateAsync(autoNumberTemplate.AutoNumberTemplateGuid, false);

            const string sql = @"
                UPDATE [app].[AutoNumberTemplate]
                SET TemplateName           = @TemplateName,
                    Description            = @Description,
                    EffectiveDate          = @EffectiveDate,
                    TemplateScopeType      = @TemplateScopeType,
                    SrFormMappingNumbering = @SrFormMappingNumbering,
                    CompanyId              = @CompanyId,
                    CompanyOfficeId        = @CompanyOfficeId,
                    StatusId               = @StatusId,
                    UpdatedById            = @UpdatedById,
                    UpdatedTime            = @UpdatedTime
                WHERE AutoNumberTemplateGuid = @AutoNumberTemplateGuid";
            await conn.ExecuteAsync(sql, autoNumberTemplate, transaction);

            await _audit.LogAsync(
                actionType: "UPDATE",
                tableName: "AutoNumberTemplate",
                primaryKey: autoNumberTemplate.AutoNumberTemplateGuid.ToString(),
                userId: autoNumberTemplate.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: autoNumberTemplate);
        }

        public async Task SoftDeleteAutoNumberTemplateAsync(AutoNumberTemplate autoNumberTemplate, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(autoNumberTemplate, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberTemplateAsync(autoNumberTemplate.AutoNumberTemplateGuid, false);

            const string sql = @"
                UPDATE [app].[AutoNumberTemplate]
                SET StatusId    = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AutoNumberTemplateGuid = @AutoNumberTemplateGuid";

            await conn.ExecuteAsync(sql, autoNumberTemplate, transaction);

            await _audit.LogAsync(
                actionType: "DELETE",
                tableName: "AutoNumberTemplate",
                primaryKey: autoNumberTemplate.AutoNumberTemplateGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task DeleteAutoNumberTemplateAsync(Guid autoNumberTemplateGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberTemplateAsync(autoNumberTemplateGuid, false);

            const string sql = @"DELETE FROM [app].[AutoNumberTemplate] WHERE AutoNumberTemplateGuid = @autoNumberTemplateGuid";
            await conn.ExecuteAsync(sql, new { autoNumberTemplateGuid }, transaction);

            await _audit.LogAsync(
                actionType: "DELETE_ADMIN",
                tableName: "AutoNumberTemplate",
                primaryKey: autoNumberTemplateGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<AutoNumberTemplate>> SearchAutoNumberTemplateAsync(
            string? templateName, string? templateNameSearchType,
            string? description, string? descriptionSearchType,
            DateTime? effectiveDateFrom, DateTime? effectiveDateTo,
            string? templateScopeType,
            long? companyId,
            long? companyOfficeId,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            // 🔍 TemplateCode
            if (!string.IsNullOrWhiteSpace(templateName))
            {
                var param = SqlFilterHelper.BuildFilter("a.TemplateName", "@templateName", templateNameSearchType, parameters, "templateName", templateName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            // 🔍 Description
            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            // 🔍 EffectiveDate (range)
            if (effectiveDateFrom.HasValue)
            {
                whereClauses.Add("a.EffectiveDate >= @effectiveDateFrom");
                parameters.Add("effectiveDateFrom", effectiveDateFrom.Value.Date);
            }
            if (effectiveDateTo.HasValue)
            {
                whereClauses.Add("a.EffectiveDate < @effectiveDateToPlusOne");
                parameters.Add("effectiveDateToPlusOne", effectiveDateTo.Value.Date.AddDays(1));
            }

            // 🔍 TemplateScopeType
            if (!string.IsNullOrWhiteSpace(templateScopeType))
            {
                whereClauses.Add("a.TemplateScopeType = @templateScopeType");
                parameters.Add("templateScopeType", templateScopeType);
            }

            // 🔍 CompanyId
            if (companyId.HasValue)
            {
                whereClauses.Add("a.CompanyId = @companyId");
                parameters.Add("companyId", companyId.Value);
            }

            // 🔍 CompanyOfficeId
            if (companyOfficeId.HasValue)
            {
                whereClauses.Add("a.CompanyOfficeId = @companyOfficeId");
                parameters.Add("companyOfficeId", companyOfficeId.Value);
            }

            var sql = $@"
                SELECT a.*
                FROM [app].[AutoNumberTemplate] a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AutoNumberTemplateId DESC";

            return await connection.QueryAsync<AutoNumberTemplate>(sql, parameters, transaction);
        }
    }
}
