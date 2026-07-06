using Entities.Models.Approval;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts.Approval
{
    public interface IApprovalTemplateRepository
    {
        Task<ApprovalTemplate?> GetAsync(Guid guid, bool trackChanges);
        Task<IEnumerable<ApprovalTemplate>> GetAllAsync(bool trackChanges);
        Task CreateAsync(ApprovalTemplate template, IDbTransaction? transaction = null);
        Task UpdateAsync(ApprovalTemplate template, IDbTransaction? transaction = null);
        Task SoftDeleteAsync(ApprovalTemplate template, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteAsync(Guid guid, IDbTransaction? transaction = null);
    }
}
