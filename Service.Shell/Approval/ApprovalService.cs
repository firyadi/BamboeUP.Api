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
    public class ApprovalService : IApprovalService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IApprovalNotificationService _notificationService;

        public ApprovalService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IApprovalNotificationService notificationService)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
            _notificationService = notificationService;
        }

        public async Task<ApprovalRequestDto> SubmitRequestAsync(ApprovalRequestForCreationDto input)
        {
            await _transactionManager.BeginTransactionAsync();
            var transaction = _transactionManager.GetTransaction();
            try
            {
                // 1. Get Template & Levels
                var template = await _repository.ApprovalTemplate.GetAsync(input.ReferenceGuid, false); // Asumsi reference id ini akan dicari lewat query lain, untuk saat ini anggap input TemplateId dikirim (harus diperbaiki)
                // Kita perlu templateId, kita asumsikan Request Creation mengirimkan TemplateId atau ModuleCode, mari lookup via ModuleCode (kita perlu repository function utk get by module code, for now let's just cheat and fetch all templates then filter in mem for speed coding, later fix it).
                var templates = await _repository.ApprovalTemplate.GetAllAsync(false);
                template = templates.FirstOrDefault(t => t.ModuleCode == input.ModuleCode);
                
                if (template == null) throw new Exception($"Template for module {input.ModuleCode} not found.");

                var levels = (await _repository.ApprovalTemplateLevel.GetLevelsByTemplateIdAsync(template.ApprovalTemplateId, false)).ToList();
                if (!levels.Any()) throw new Exception("Template has no approval levels defined.");

                // 2. Create Request
                var request = new ApprovalRequest
                {
                    ApprovalTemplateId = template.ApprovalTemplateId,
                    RequestNumber = $"REQ-{input.ModuleCode}-{DateTime.UtcNow:yyMMddHHmmss}",
                    ModuleCode = input.ModuleCode,
                    ReferenceGuid = input.ReferenceGuid,
                    ReferenceNumber = input.ReferenceNumber,
                    RequestedByUserId = input.RequestedByUserId,
                    CurrentLevelOrder = levels.First().LevelOrder,
                    StatusId = ApprovalConstants.RequestStatus.Pending,
                    Notes = input.Notes,
                    CreatedById = input.RequestedByUserId,
                    RequestedTime = DateTime.UtcNow
                };

                await _repository.ApprovalRequest.CreateAsync(request, transaction);

                // 3. Create Steps for all levels
                ApprovalStep? firstPendingStep = null;

                foreach (var level in levels)
                {
                    // Get approvers
                    var approvers = await _repository.ApprovalTemplateLevelApprover.GetApproversByLevelIdAsync(level.ApprovalTemplateLevelId, false);
                    
                    // In real-world, if ApproverType is DIRECT_MANAGER, we find manager user ID here.
                    long approverId = approvers.FirstOrDefault()?.UserId ?? 0;
                    
                    var step = new ApprovalStep
                    {
                        ApprovalRequestId = request.ApprovalRequestId,
                        LevelOrder = level.LevelOrder,
                        LevelName = level.LevelName,
                        ApproverUserId = approverId,
                        StatusId = level.LevelOrder == request.CurrentLevelOrder ? ApprovalConstants.StepStatus.Pending : ApprovalConstants.StepStatus.Waiting,
                        CreatedById = input.RequestedByUserId,
                        SlaDeadline = level.SlaHours > 0 && level.LevelOrder == request.CurrentLevelOrder ? DateTime.UtcNow.AddHours(level.SlaHours) : null
                    };

                    // Check Delegation if this step is currently pending
                    if (step.StatusId == ApprovalConstants.StepStatus.Pending)
                    {
                        var delegation = await _repository.ApprovalDelegation.GetActiveDelegationForUserAsync(approverId, DateTime.UtcNow, false);
                        if (delegation != null)
                        {
                            step.DelegatedFromUserId = approverId;
                            step.ApproverUserId = delegation.DelegateUserId;
                        }
                        firstPendingStep = step;
                    }

                    await _repository.ApprovalStep.CreateAsync(step, transaction);
                }

                // 4. Create History Log
                var history = new ApprovalHistory
                {
                    ApprovalRequestId = request.ApprovalRequestId,
                    ActionType = ApprovalConstants.ActionType.Submitted,
                    ActionByUserId = input.RequestedByUserId,
                    FromStatus = "Draft",
                    ToStatus = "Pending",
                    LevelOrder = request.CurrentLevelOrder,
                    Comment = "Request submitted"
                };
                await _repository.ApprovalHistory.CreateAsync(history, transaction);

                await _transactionManager.CommitAsync();

                // 5. Send Notification
                if (firstPendingStep != null)
                {
                   await _notificationService.SendApprovalRequiredEmailAsync(request, firstPendingStep);
                }

                return await GetRequestAsync(request.ApprovalRequestGuid, false);
            }
            catch (Exception ex)
            {
                await _transactionManager.RollbackAsync();
                _logger.LogError($"SubmitRequest Error: {ex.Message}");
                throw;
            }
        }

        public async Task<ApprovalRequestDto> GetRequestAsync(Guid guid, bool trackChanges)
        {
            var req = await _repository.ApprovalRequest.GetAsync(guid, trackChanges);
            if (req == null) return null!;

            var dto = req.Adapt<ApprovalRequestDto>();
            var steps = await _repository.ApprovalStep.GetStepsByRequestIdAsync(req.ApprovalRequestId, trackChanges);
            dto.Steps = steps.Adapt<IEnumerable<ApprovalStepDto>>();
            
            return dto;
        }

        public async Task<IEnumerable<ApprovalRequestDto>> GetMyPendingRequestsAsync(long userId, bool trackChanges)
        {
            var reqs = await _repository.ApprovalRequest.GetPendingRequestsByUserIdAsync(userId, trackChanges);
            // In a real app, we'd also batch-load the steps so the UI can show which step is active.
            return reqs.Adapt<IEnumerable<ApprovalRequestDto>>();
        }

        public async Task ApproveStepAsync(Guid requestGuid, Guid stepGuid, ApprovalRequestForActionDto input)
        {
            await _transactionManager.BeginTransactionAsync();
            var transaction = _transactionManager.GetTransaction();
            try
            {
                var req = await _repository.ApprovalRequest.GetAsync(requestGuid, false);
                var step = await _repository.ApprovalStep.GetAsync(stepGuid, false);

                if (req == null || step == null) throw new Exception("Request or Step not found.");
                if (step.StatusId != ApprovalConstants.StepStatus.Pending) throw new Exception("Step is not pending.");
                if (step.ApproverUserId != input.ActionByUserId) throw new Exception("You are not the designated approver for this step.");

                // 1. Update Step
                step.StatusId = ApprovalConstants.StepStatus.Approved;
                step.ActionTime = DateTime.UtcNow;
                step.Comment = input.Comment;
                step.UpdatedById = input.ActionByUserId;
                await _repository.ApprovalStep.UpdateAsync(step, transaction);

                // 2. Log History
                var history = new ApprovalHistory
                {
                    ApprovalRequestId = req.ApprovalRequestId,
                    ApprovalStepId = step.ApprovalStepId,
                    ActionType = ApprovalConstants.ActionType.Approved,
                    ActionByUserId = input.ActionByUserId,
                    Comment = input.Comment,
                    FromStatus = "Pending",
                    ToStatus = "Approved",
                    LevelOrder = step.LevelOrder
                };
                await _repository.ApprovalHistory.CreateAsync(history, transaction);

                // 3. Check if there are more levels
                var allSteps = (await _repository.ApprovalStep.GetStepsByRequestIdAsync(req.ApprovalRequestId, false)).ToList();
                var nextStep = allSteps.FirstOrDefault(s => s.LevelOrder > step.LevelOrder);

                if (nextStep == null)
                {
                    // Full Approved
                    req.StatusId = ApprovalConstants.RequestStatus.Approved;
                    req.CompletedTime = DateTime.UtcNow;
                    req.UpdatedById = input.ActionByUserId;
                    await _repository.ApprovalRequest.UpdateAsync(req, transaction);
                    
                    await _transactionManager.CommitAsync();
                    await _notificationService.SendApprovalApprovedEmailAsync(req);
                }
                else
                {
                    // Move to Next Level
                    req.CurrentLevelOrder = nextStep.LevelOrder;
                    req.StatusId = ApprovalConstants.RequestStatus.InProgress;
                    req.UpdatedById = input.ActionByUserId;
                    await _repository.ApprovalRequest.UpdateAsync(req, transaction);

                    nextStep.StatusId = ApprovalConstants.StepStatus.Pending;
                    
                    // TODO: SLA Logic & Delegation Check for Next Step here.
                    
                    await _repository.ApprovalStep.UpdateAsync(nextStep, transaction);
                    
                    await _transactionManager.CommitAsync();
                    await _notificationService.SendApprovalRequiredEmailAsync(req, nextStep);
                }
            }
            catch(Exception ex)
            {
                await _transactionManager.RollbackAsync();
                _logger.LogError($"Approve Error: {ex.Message}");
                throw;
            }
        }

        public async Task RejectStepAsync(Guid requestGuid, Guid stepGuid, ApprovalRequestForActionDto input)
        {
            await _transactionManager.BeginTransactionAsync();
            var transaction = _transactionManager.GetTransaction();
            try
            {
                var req = await _repository.ApprovalRequest.GetAsync(requestGuid, false);
                var step = await _repository.ApprovalStep.GetAsync(stepGuid, false);

                if (req == null || step == null) throw new Exception("Request or Step not found.");
                if (step.StatusId != ApprovalConstants.StepStatus.Pending) throw new Exception("Step is not pending.");

                // Update Step
                step.StatusId = ApprovalConstants.StepStatus.Rejected;
                step.ActionTime = DateTime.UtcNow;
                step.Comment = input.Comment;
                step.UpdatedById = input.ActionByUserId;
                await _repository.ApprovalStep.UpdateAsync(step, transaction);

                // Update Request completely Rejected
                req.StatusId = ApprovalConstants.RequestStatus.Rejected;
                req.CompletedTime = DateTime.UtcNow;
                req.UpdatedById = input.ActionByUserId;
                await _repository.ApprovalRequest.UpdateAsync(req, transaction);

                // History
                var history = new ApprovalHistory
                {
                    ApprovalRequestId = req.ApprovalRequestId,
                    ApprovalStepId = step.ApprovalStepId,
                    ActionType = ApprovalConstants.ActionType.Rejected,
                    ActionByUserId = input.ActionByUserId,
                    Comment = input.Comment,
                    FromStatus = "Pending",
                    ToStatus = "Rejected",
                    LevelOrder = step.LevelOrder
                };
                await _repository.ApprovalHistory.CreateAsync(history, transaction);

                await _transactionManager.CommitAsync();
                await _notificationService.SendApprovalRejectedEmailAsync(req, step);
            }
            catch (Exception ex)
            {
                await _transactionManager.RollbackAsync();
                _logger.LogError($"RejectStep Error: {ex.Message}");
                throw;
            }
        }

        public async Task CancelRequestAsync(Guid requestGuid, long cancelledByUserId)
        {
            var req = await _repository.ApprovalRequest.GetAsync(requestGuid, false);
            if (req == null) throw new Exception("Not found");
            
            req.StatusId = ApprovalConstants.RequestStatus.Cancelled;
            req.UpdatedById = cancelledByUserId;
            await _repository.ApprovalRequest.UpdateAsync(req);
        }

        public async Task CheckAndProcessSlaAsync()
        {
            _logger.LogInfo("Running Approval SLA Checking Job...");
            var expiredSteps = await _repository.ApprovalStep.GetExpiredPendingStepsAsync(false);
            
            foreach (var step in expiredSteps)
            {
                // Simple version: Just mark as escalated. Real world: find EscalateToLevelOrder and re-route
                step.IsEscalated = true;
                step.UpdatedTime = DateTime.UtcNow;
                await _repository.ApprovalStep.UpdateAsync(step);
                
                var req = await _repository.ApprovalRequest.GetAsync(step.ApprovalStepGuid /* mock */, false); // Need proper fetch
                _logger.LogInfo($"Step {step.ApprovalStepId} Escalated.");
            }
        }
    }
}
