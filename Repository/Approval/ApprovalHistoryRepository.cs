using Contracts.Approval;
using Dapper;
using Entities.Models.Approval;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Repository.Database;
using Repository.Extensions;

namespace Repository.Approval
{
    public class ApprovalHistoryRepository : IApprovalHistoryRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public ApprovalHistoryRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        public async Task<IEnumerable<ApprovalHistory>> GetHistoryByRequestIdAsync(long requestId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "*")} FROM {_dialect.Qualify("apv", "ApprovalHistory")} WHERE ApprovalRequestId = @requestId ORDER BY ActionTime DESC";
            return await connection.QueryAsync<ApprovalHistory>(sql, new { requestId });
        }

        public async Task CreateAsync(ApprovalHistory history, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_dialect.Qualify("apv", "ApprovalHistory")}
                (ApprovalHistoryGuid, ApprovalRequestId, ApprovalStepId, ActionType, ActionByUserId, ActionTime, Comment, FromStatus, ToStatus, LevelOrder)
                VALUES
                (@ApprovalHistoryGuid, @ApprovalRequestId, @ApprovalStepId, @ActionType, @ActionByUserId, @ActionTime, @Comment, @FromStatus, @ToStatus, @LevelOrder){_dialect.ReturnIdentitySuffix("ApprovalHistoryId")}";
                 
            history.ApprovalHistoryId = await conn.ExecuteScalarAsync<long>(sql, history, transaction);
        }
    }
}
