using Entities.Models.Approval;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts.Approval
{
    public interface IApprovalTemplateLevelRepository
    {
        Task<IEnumerable<ApprovalTemplateLevel>> GetLevelsByTemplateIdAsync(long templateId, bool trackChanges);
        Task CreateAsync(ApprovalTemplateLevel level, IDbTransaction? transaction = null);
        Task DeleteByTemplateIdAsync(long templateId, IDbTransaction? transaction = null);
    }

    public interface IApprovalTemplateLevelApproverRepository
    {
        Task<IEnumerable<ApprovalTemplateLevelApprover>> GetApproversByLevelIdAsync(long levelId, bool trackChanges);
        Task CreateAsync(ApprovalTemplateLevelApprover approver, IDbTransaction? transaction = null);
        Task DeleteByLevelIdAsync(long levelId, IDbTransaction? transaction = null);
        Task DeleteByTemplateIdAsync(long templateId, IDbTransaction? transaction = null);
    }
}
