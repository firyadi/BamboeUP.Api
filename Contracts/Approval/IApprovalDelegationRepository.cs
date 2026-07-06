using Entities.Models.Approval;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts.Approval
{
    public interface IApprovalDelegationRepository
    {
        Task<ApprovalDelegation?> GetAsync(Guid guid, bool trackChanges);
        Task<IEnumerable<ApprovalDelegation>> GetActiveDelegationsAsync(long delegatorUserId, bool trackChanges);
        Task<ApprovalDelegation?> GetActiveDelegationForUserAsync(long delegatorUserId, DateTime dateToCheck, bool trackChanges);
        Task CreateAsync(ApprovalDelegation delegation, IDbTransaction? transaction = null);
        Task UpdateAsync(ApprovalDelegation delegation, IDbTransaction? transaction = null);
        Task DeleteAsync(Guid guid, IDbTransaction? transaction = null);
    }
}
