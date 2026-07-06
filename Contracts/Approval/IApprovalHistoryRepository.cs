using Entities.Models.Approval;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts.Approval
{
    public interface IApprovalHistoryRepository
    {
        Task<IEnumerable<ApprovalHistory>> GetHistoryByRequestIdAsync(long requestId, bool trackChanges);
        Task CreateAsync(ApprovalHistory history, IDbTransaction? transaction = null);
    }
}
