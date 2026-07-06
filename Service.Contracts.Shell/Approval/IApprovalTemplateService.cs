using Shared.DataTransferObjects.Approval;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell.Approval
{
    public interface IApprovalTemplateService
    {
        Task<IEnumerable<ApprovalTemplateDto>> GetAllAsync(bool trackChanges);
        Task<ApprovalTemplateDto> GetAsync(Guid guid, bool trackChanges);
        Task<ApprovalTemplateDto> CreateAsync(ApprovalTemplateForCreationDto input);
        Task UpdateAsync(Guid guid, ApprovalTemplateForUpdateDto input, bool trackChanges);
        Task DeleteAsync(Guid guid, long deletedBy, bool trackChanges);
    }
}
