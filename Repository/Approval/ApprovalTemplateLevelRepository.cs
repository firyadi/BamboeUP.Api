using Contracts.Approval;
using Dapper;
using Entities.Models.Approval;
using Repository.Database;
using Repository.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository.Approval
{
    public class ApprovalTemplateLevelRepository : IApprovalTemplateLevelRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public ApprovalTemplateLevelRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        public async Task<IEnumerable<ApprovalTemplateLevel>> GetLevelsByTemplateIdAsync(long templateId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")} FROM {_dialect.Qualify("apv", "ApprovalTemplateLevel")} WHERE ApprovalTemplateId = @templateId AND StatusId > 0 ORDER BY LevelOrder";
            return await connection.QueryAsync<ApprovalTemplateLevel>(sql, new { templateId });
        }

        public async Task CreateAsync(ApprovalTemplateLevel level, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_dialect.Qualify("apv", "ApprovalTemplateLevel")}
                (ApprovalTemplateLevelGuid, ApprovalTemplateId, LevelOrder, LevelName, ApproverType, 
                 RequireAllApprovers, CanSkipIfPreviousNotApproved, SlaHours, EscalateToLevelOrder, 
                 StatusId, CreatedById, CreatedTime)
                VALUES
                (@ApprovalTemplateLevelGuid, @ApprovalTemplateId, @LevelOrder, @LevelName, @ApproverType, 
                 @RequireAllApprovers, @CanSkipIfPreviousNotApproved, @SlaHours, @EscalateToLevelOrder, 
                 @StatusId, @CreatedById, @CreatedTime){_dialect.ReturnIdentitySuffix("ApprovalTemplateLevelId")}";
                 
            level.ApprovalTemplateLevelId = await conn.ExecuteScalarAsync<long>(sql, level, transaction);
        }

        public async Task DeleteByTemplateIdAsync(long templateId, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $"DELETE FROM {_dialect.Qualify("apv", "ApprovalTemplateLevel")} WHERE ApprovalTemplateId = @templateId";
            await conn.ExecuteAsync(sql, new { templateId }, transaction);
        }
    }

    public class ApprovalTemplateLevelApproverRepository : IApprovalTemplateLevelApproverRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public ApprovalTemplateLevelApproverRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        public async Task<IEnumerable<ApprovalTemplateLevelApprover>> GetApproversByLevelIdAsync(long levelId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")} FROM {_dialect.Qualify("apv", "ApprovalTemplateLevelApprover")} WHERE ApprovalTemplateLevelId = @levelId AND StatusId > 0";
            return await connection.QueryAsync<ApprovalTemplateLevelApprover>(sql, new { levelId });
        }

        public async Task CreateAsync(ApprovalTemplateLevelApprover approver, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_dialect.Qualify("apv", "ApprovalTemplateLevelApprover")}
                (ApprovalTemplateLevelApproverGuid, ApprovalTemplateLevelId, UserId, UserGroupId, StatusId, CreatedById, CreatedTime)
                VALUES
                (@ApprovalTemplateLevelApproverGuid, @ApprovalTemplateLevelId, @UserId, @UserGroupId, @StatusId, @CreatedById, @CreatedTime){_dialect.ReturnIdentitySuffix("ApprovalTemplateLevelApproverId")}";
                
            approver.ApprovalTemplateLevelApproverId = await conn.ExecuteScalarAsync<long>(sql, approver, transaction);
        }

        public async Task DeleteByLevelIdAsync(long levelId, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $"DELETE FROM {_dialect.Qualify("apv", "ApprovalTemplateLevelApprover")} WHERE ApprovalTemplateLevelId = @levelId";
            await conn.ExecuteAsync(sql, new { levelId }, transaction);
        }

        public async Task DeleteByTemplateIdAsync(long templateId, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                DELETE FROM {_dialect.Qualify("apv", "ApprovalTemplateLevelApprover")}
                WHERE ApprovalTemplateLevelId IN (
                    SELECT ApprovalTemplateLevelId
                    FROM {_dialect.Qualify("apv", "ApprovalTemplateLevel")}
                    WHERE ApprovalTemplateId = @templateId
                )";
            await conn.ExecuteAsync(sql, new { templateId }, transaction);
        }
    }
}
