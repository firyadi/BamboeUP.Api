using Contracts.Approval;
using Dapper;
using Entities.Models.Approval;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository.Approval
{
    public class ApprovalHistoryRepository : IApprovalHistoryRepository
    {
        private readonly RepositoryContext _context;

        public ApprovalHistoryRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApprovalHistory>> GetHistoryByRequestIdAsync(long requestId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalHistory] WHERE ApprovalRequestId = @requestId ORDER BY ActionTime DESC";
            return await connection.QueryAsync<ApprovalHistory>(sql, new { requestId });
        }

        public async Task CreateAsync(ApprovalHistory history, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [apv].[ApprovalHistory]
                (ApprovalHistoryGuid, ApprovalRequestId, ApprovalStepId, ActionType, ActionByUserId, ActionTime, Comment, FromStatus, ToStatus, LevelOrder)
                OUTPUT INSERTED.ApprovalHistoryId
                VALUES
                (@ApprovalHistoryGuid, @ApprovalRequestId, @ApprovalStepId, @ActionType, @ActionByUserId, @ActionTime, @Comment, @FromStatus, @ToStatus, @LevelOrder)";
                 
            history.ApprovalHistoryId = await conn.ExecuteScalarAsync<long>(sql, history, transaction);
        }
    }
}
