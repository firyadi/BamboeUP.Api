using Contracts.Approval;
using Dapper;
using Entities.Models.Approval;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository.Approval
{
    public class ApprovalTemplateLevelRepository : IApprovalTemplateLevelRepository
    {
        private readonly RepositoryContext _context;

        public ApprovalTemplateLevelRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApprovalTemplateLevel>> GetLevelsByTemplateIdAsync(long templateId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalTemplateLevel] WHERE ApprovalTemplateId = @templateId AND StatusId > 0 ORDER BY LevelOrder";
            return await connection.QueryAsync<ApprovalTemplateLevel>(sql, new { templateId });
        }

        public async Task CreateAsync(ApprovalTemplateLevel level, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [apv].[ApprovalTemplateLevel]
                (ApprovalTemplateLevelGuid, ApprovalTemplateId, LevelOrder, LevelName, ApproverType, 
                 RequireAllApprovers, CanSkipIfPreviousNotApproved, SlaHours, EscalateToLevelOrder, 
                 StatusId, CreatedById, CreatedTime)
                OUTPUT INSERTED.ApprovalTemplateLevelId
                VALUES
                (@ApprovalTemplateLevelGuid, @ApprovalTemplateId, @LevelOrder, @LevelName, @ApproverType, 
                 @RequireAllApprovers, @CanSkipIfPreviousNotApproved, @SlaHours, @EscalateToLevelOrder, 
                 @StatusId, @CreatedById, @CreatedTime)";
                 
            level.ApprovalTemplateLevelId = await conn.ExecuteScalarAsync<long>(sql, level, transaction);
        }

        public async Task DeleteByTemplateIdAsync(long templateId, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = "DELETE FROM [apv].[ApprovalTemplateLevel] WHERE ApprovalTemplateId = @templateId";
            await conn.ExecuteAsync(sql, new { templateId }, transaction);
        }
    }

    public class ApprovalTemplateLevelApproverRepository : IApprovalTemplateLevelApproverRepository
    {
        private readonly RepositoryContext _context;

        public ApprovalTemplateLevelApproverRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApprovalTemplateLevelApprover>> GetApproversByLevelIdAsync(long levelId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalTemplateLevelApprover] WHERE ApprovalTemplateLevelId = @levelId AND StatusId > 0";
            return await connection.QueryAsync<ApprovalTemplateLevelApprover>(sql, new { levelId });
        }

        public async Task CreateAsync(ApprovalTemplateLevelApprover approver, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [apv].[ApprovalTemplateLevelApprover]
                (ApprovalTemplateLevelApproverGuid, ApprovalTemplateLevelId, UserId, UserGroupId, StatusId, CreatedById, CreatedTime)
                OUTPUT INSERTED.ApprovalTemplateLevelApproverId
                VALUES
                (@ApprovalTemplateLevelApproverGuid, @ApprovalTemplateLevelId, @UserId, @UserGroupId, @StatusId, @CreatedById, @CreatedTime)";
                
            approver.ApprovalTemplateLevelApproverId = await conn.ExecuteScalarAsync<long>(sql, approver, transaction);
        }

        public async Task DeleteByLevelIdAsync(long levelId, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = "DELETE FROM [apv].[ApprovalTemplateLevelApprover] WHERE ApprovalTemplateLevelId = @levelId";
            await conn.ExecuteAsync(sql, new { levelId }, transaction);
        }

        public async Task DeleteByTemplateIdAsync(long templateId, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                DELETE a FROM [apv].[ApprovalTemplateLevelApprover] a
                INNER JOIN [apv].[ApprovalTemplateLevel] l ON a.ApprovalTemplateLevelId = l.ApprovalTemplateLevelId
                WHERE l.ApprovalTemplateId = @templateId";
            await conn.ExecuteAsync(sql, new { templateId }, transaction);
        }
    }
}
