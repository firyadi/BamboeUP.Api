using Mapster;
using Contracts;
using Entities.Models.Approval;
using Service.Contracts.Shell;
using Service.Contracts.Shell.Approval;
using Shared.DataTransferObjects.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Shell.Approval
{
    public class ApprovalDelegationService : IApprovalDelegationService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ApprovalDelegationService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            }

        public async Task<IEnumerable<ApprovalDelegationDto>> GetMyDelegationsAsync(long userId, bool trackChanges)
        {
            var delegations = await _repository.ApprovalDelegation.GetActiveDelegationsAsync(userId, trackChanges);
            // Idealnya di sini kita me-load nama User melalui UserService / UserRepository untuk di view.
            return delegations.Adapt<IEnumerable<ApprovalDelegationDto>>();
        }

        public async Task<ApprovalDelegationDto> GetAsync(Guid guid, bool trackChanges)
        {
            var delegation = await _repository.ApprovalDelegation.GetAsync(guid, trackChanges);
            return delegation.Adapt<ApprovalDelegationDto>()!;
        }

        public async Task<ApprovalDelegationDto> CreateAsync(ApprovalDelegationForCreationDto input)
        {
            var delegation = input.Adapt<ApprovalDelegation>();
            delegation.IsActive = true;
            delegation.StatusId = 1;
            
            await _repository.ApprovalDelegation.CreateAsync(delegation);
            return delegation.Adapt<ApprovalDelegationDto>();
        }

        public async Task UpdateAsync(Guid guid, ApprovalDelegationForUpdateDto input, bool trackChanges)
        {
            var existing = await _repository.ApprovalDelegation.GetAsync(guid, trackChanges);
            if (existing == null) throw new Exception("Data not found");

            existing.DelegateUserId = input.DelegateUserId;
            existing.StartDate = input.StartDate;
            existing.EndDate = input.EndDate;
            existing.IsActive = input.IsActive;
            existing.Notes = input.Notes;
            existing.UpdatedById = input.UpdatedById;

            await _repository.ApprovalDelegation.UpdateAsync(existing);
        }

        public async Task DeleteAsync(Guid guid, bool trackChanges)
        {
            await _repository.ApprovalDelegation.DeleteAsync(guid);
        }
    }
}
