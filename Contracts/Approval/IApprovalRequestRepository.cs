using Entities.Models.Approval;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts.Approval
{
    public interface IApprovalRequestRepository
    {
        Task<ApprovalRequest?> GetAsync(Guid guid, bool trackChanges);
        Task<ApprovalRequest?> GetByDocumentAsync(string moduleCode, Guid referenceGuid, bool trackChanges);
        
        // For My Pending view
        Task<IEnumerable<ApprovalRequest>> GetPendingRequestsByUserIdAsync(long userId, bool trackChanges);
        
        Task CreateAsync(ApprovalRequest request, IDbTransaction? transaction = null);
        Task UpdateAsync(ApprovalRequest request, IDbTransaction? transaction = null);
    }
}
