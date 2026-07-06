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
    public class ApprovalStepRepository : IApprovalStepRepository
    {
        private readonly RepositoryContext _context;

        public ApprovalStepRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<ApprovalStep?> GetAsync(Guid guid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalStep] WHERE ApprovalStepGuid = @guid";
            return await connection.QuerySingleOrDefaultAsync<ApprovalStep>(sql, new { guid });
        }

        public async Task<IEnumerable<ApprovalStep>> GetStepsByRequestIdAsync(long requestId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalStep] WHERE ApprovalRequestId = @requestId ORDER BY LevelOrder";
            return await connection.QueryAsync<ApprovalStep>(sql, new { requestId });
        }

        public async Task<IEnumerable<ApprovalStep>> GetExpiredPendingStepsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalStep] 
                WHERE StatusId = 1             -- Pending
                  AND IsEscalated = 0          -- Belum dieskalasi
                  AND SlaDeadline < GETUTCDATE()";
            return await connection.QueryAsync<ApprovalStep>(sql);
        }

        public async Task CreateAsync(ApprovalStep step, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(step);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [apv].[ApprovalStep]
                (ApprovalStepGuid, ApprovalRequestId, LevelOrder, LevelName, ApproverUserId, DelegatedFromUserId, 
                 StatusId, ActionTime, Comment, SlaDeadline, IsEscalated, CreatedById, CreatedTime)
                OUTPUT INSERTED.ApprovalStepId
                VALUES
                (@ApprovalStepGuid, @ApprovalRequestId, @LevelOrder, @LevelName, @ApproverUserId, @DelegatedFromUserId, 
                 @StatusId, @ActionTime, @Comment, @SlaDeadline, @IsEscalated, @CreatedById, @CreatedTime)";
                 
            step.ApprovalStepId = await conn.ExecuteScalarAsync<long>(sql, step, transaction);
        }

        public async Task UpdateAsync(ApprovalStep step, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(step);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [apv].[ApprovalStep]
                SET StatusId = @StatusId, ActionTime = @ActionTime, Comment = @Comment, 
                    IsEscalated = @IsEscalated, ApproverUserId = @ApproverUserId, DelegatedFromUserId = @DelegatedFromUserId,
                    UpdatedById = @UpdatedById, UpdatedTime = @UpdatedTime
                WHERE ApprovalStepId = @ApprovalStepId";
                
            await conn.ExecuteAsync(sql, step, transaction);
        }
    }
}
