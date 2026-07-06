using Entities.Models.Approval;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts.Approval
{
    public interface IApprovalStepRepository
    {
        Task<ApprovalStep?> GetAsync(Guid guid, bool trackChanges);
        Task<IEnumerable<ApprovalStep>> GetStepsByRequestIdAsync(long requestId, bool trackChanges);
        
        // Cek SLA yang terlewat untuk background job Hangfire
        Task<IEnumerable<ApprovalStep>> GetExpiredPendingStepsAsync(bool trackChanges);
        
        Task CreateAsync(ApprovalStep step, IDbTransaction? transaction = null);
        Task UpdateAsync(ApprovalStep step, IDbTransaction? transaction = null);
    }
}
