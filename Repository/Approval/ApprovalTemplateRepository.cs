using BamboeUp.Audit.Contracts;
using Contracts.Approval;
using Dapper;
using Entities.Models.Approval;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository.Approval
{
    public class ApprovalTemplateRepository : IApprovalTemplateRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public ApprovalTemplateRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<ApprovalTemplate?> GetAsync(Guid guid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalTemplate] WHERE ApprovalTemplateGuid = @guid AND StatusId > 0 AND DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<ApprovalTemplate>(sql, new { guid });
        }

        public async Task<IEnumerable<ApprovalTemplate>> GetAllAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalTemplate] WHERE StatusId > 0 AND DeletedTime IS NULL ORDER BY TemplateName";
            return await connection.QueryAsync<ApprovalTemplate>(sql);
        }

        public async Task CreateAsync(ApprovalTemplate template, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(template);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [apv].[ApprovalTemplate]
                (ApprovalTemplateGuid, TemplateName, ModuleCode, Description, IsActive, StatusId, CreatedById, CreatedTime)
                OUTPUT INSERTED.ApprovalTemplateId
                VALUES
                (@ApprovalTemplateGuid, @TemplateName, @ModuleCode, @Description, @IsActive, @StatusId, @CreatedById, @CreatedTime)";
            
            template.ApprovalTemplateId = await conn.ExecuteScalarAsync<long>(sql, template, transaction);

            await _auditService.LogAsync("CREATE", "ApprovalTemplate", template.ApprovalTemplateGuid.ToString(), template.CreatedById.ToString(), null, template);
        }

        public async Task UpdateAsync(ApprovalTemplate template, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(template);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [apv].[ApprovalTemplate]
                SET TemplateName = @TemplateName, ModuleCode = @ModuleCode, Description = @Description, 
                    IsActive = @IsActive, StatusId = @StatusId, UpdatedById = @UpdatedById, UpdatedTime = @UpdatedTime
                WHERE ApprovalTemplateId = @ApprovalTemplateId";
            
            await conn.ExecuteAsync(sql, template, transaction);
            await _auditService.LogAsync("UPDATE", "ApprovalTemplate", template.ApprovalTemplateGuid.ToString(), template.UpdatedById?.ToString() ?? "system", null, template);
        }

        public async Task SoftDeleteAsync(ApprovalTemplate template, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(template, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [apv].[ApprovalTemplate]
                SET StatusId = 0, DeletedById = @DeletedById, DeletedTime = @DeletedTime
                WHERE ApprovalTemplateId = @ApprovalTemplateId";
            
            await conn.ExecuteAsync(sql, template, transaction);
            await _auditService.LogAsync("DELETE", "ApprovalTemplate", template.ApprovalTemplateGuid.ToString(), deletedBy.ToString(), null, null);
        }

        public async Task DeleteAsync(Guid guid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = "DELETE FROM [apv].[ApprovalTemplate] WHERE ApprovalTemplateGuid = @guid";
            await conn.ExecuteAsync(sql, new { guid }, transaction);
        }
    }
}
