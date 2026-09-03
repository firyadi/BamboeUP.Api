using Contracts.Approval;
using Dapper;
using Entities.Models.Approval;
using Repository.Database;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository.Approval
{
    public class ApprovalStepRepository : IApprovalStepRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public ApprovalStepRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        public async Task<ApprovalStep?> GetAsync(Guid guid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")} FROM {_dialect.Qualify("apv", "ApprovalStep")} WHERE ApprovalStepGuid = @guid";
            return await connection.QuerySingleOrDefaultAsync<ApprovalStep>(sql, new { guid });
        }

        public async Task<IEnumerable<ApprovalStep>> GetStepsByRequestIdAsync(long requestId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")} FROM {_dialect.Qualify("apv", "ApprovalStep")} WHERE ApprovalRequestId = @requestId ORDER BY LevelOrder";
            return await connection.QueryAsync<ApprovalStep>(sql, new { requestId });
        }

        public async Task<IEnumerable<ApprovalStep>> GetExpiredPendingStepsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")} FROM {_dialect.Qualify("apv", "ApprovalStep")} 
                WHERE StatusId = 1             -- Pending
                  AND IsEscalated = 0          -- Belum dieskalasi
                  AND SlaDeadline < {_dialect.UtcNowExpression}";
            return await connection.QueryAsync<ApprovalStep>(sql);
        }

        public async Task CreateAsync(ApprovalStep step, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(step);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_dialect.Qualify("apv", "ApprovalStep")}
                (ApprovalStepGuid, ApprovalRequestId, LevelOrder, LevelName, ApproverUserId, DelegatedFromUserId, 
                 StatusId, ActionTime, Comment, SlaDeadline, IsEscalated, CreatedById, CreatedTime)
                VALUES
                (@ApprovalStepGuid, @ApprovalRequestId, @LevelOrder, @LevelName, @ApproverUserId, @DelegatedFromUserId, 
                 @StatusId, @ActionTime, @Comment, @SlaDeadline, @IsEscalated, @CreatedById, @CreatedTime){_dialect.ReturnIdentitySuffix("ApprovalStepId")}";
                 
            step.ApprovalStepId = await conn.ExecuteScalarAsync<long>(sql, step, transaction);
        }

        public async Task UpdateAsync(ApprovalStep step, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(step);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("apv", "ApprovalStep")}
                SET StatusId = @StatusId, ActionTime = @ActionTime, Comment = @Comment, 
                    IsEscalated = @IsEscalated, ApproverUserId = @ApproverUserId, DelegatedFromUserId = @DelegatedFromUserId,
                    UpdatedById = @UpdatedById, UpdatedTime = @UpdatedTime
                WHERE ApprovalStepId = @ApprovalStepId";
                
            await conn.ExecuteAsync(sql, step, transaction);
        }
    }
}
