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
    public class ApprovalTemplateService : IApprovalTemplateService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public ApprovalTemplateService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<ApprovalTemplateDto>> GetAllAsync(bool trackChanges)
        {
            var templates = await _repository.ApprovalTemplate.GetAllAsync(trackChanges);
            return templates.Adapt<IEnumerable<ApprovalTemplateDto>>();
        }

        public async Task<ApprovalTemplateDto> GetAsync(Guid guid, bool trackChanges)
        {
            var template = await _repository.ApprovalTemplate.GetAsync(guid, trackChanges);
            if (template == null) return null;

            var dto = template.Adapt<ApprovalTemplateDto>();

            // Load levels
            var levels = await _repository.ApprovalTemplateLevel.GetLevelsByTemplateIdAsync(template.ApprovalTemplateId, trackChanges);
            var levelDtos = new List<ApprovalTemplateLevelDto>();

            foreach (var level in levels)
            {
                var levelDto = level.Adapt<ApprovalTemplateLevelDto>();
                var approvers = await _repository.ApprovalTemplateLevelApprover.GetApproversByLevelIdAsync(level.ApprovalTemplateLevelId, trackChanges);
                levelDto.Approvers = approvers.Adapt<IEnumerable<ApprovalTemplateLevelApproverDto>>();
                levelDtos.Add(levelDto);
            }

            dto.Levels = levelDtos;
            return dto;
        }

        public async Task<ApprovalTemplateDto> CreateAsync(ApprovalTemplateForCreationDto input)
        {
            await _transactionManager.BeginTransactionAsync();
            var transaction = _transactionManager.GetTransaction();
            try
            {
                var template = input.Adapt<ApprovalTemplate>();
                template.StatusId = 1;
                await _repository.ApprovalTemplate.CreateAsync(template, transaction);

                foreach (var levelInput in input.Levels)
                {
                    var level = levelInput.Adapt<ApprovalTemplateLevel>();
                    level.ApprovalTemplateId = template.ApprovalTemplateId;
                    level.CreatedById = input.CreatedById;
                    level.StatusId = 1;
                    await _repository.ApprovalTemplateLevel.CreateAsync(level, transaction);

                    foreach (var approverInput in levelInput.Approvers)
                    {
                        var approver = approverInput.Adapt<ApprovalTemplateLevelApprover>();
                        approver.ApprovalTemplateLevelId = level.ApprovalTemplateLevelId;
                        approver.CreatedById = input.CreatedById;
                        approver.StatusId = 1;
                        await _repository.ApprovalTemplateLevelApprover.CreateAsync(approver, transaction);
                    }
                }

                await _transactionManager.CommitAsync();
                return await GetAsync(template.ApprovalTemplateGuid, false);
            }
            catch (Exception ex)
            {
                await _transactionManager.RollbackAsync();
                _logger.LogError($"Error in CreateAsync ApprovalTemplate: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(Guid guid, ApprovalTemplateForUpdateDto input, bool trackChanges)
        {
            var existing = await _repository.ApprovalTemplate.GetAsync(guid, trackChanges);
            if (existing == null) throw new Exception("Data not found");

            await _transactionManager.BeginTransactionAsync();
            var transaction = _transactionManager.GetTransaction();
            try
            {
                // Update master
                existing.TemplateName = input.TemplateName;
                existing.ModuleCode = input.ModuleCode;
                existing.Description = input.Description;
                existing.IsActive = input.IsActive;
                existing.UpdatedById = input.UpdatedById;
                await _repository.ApprovalTemplate.UpdateAsync(existing, transaction);

                // Hard replace levels & approvers (delete and insert) for simplicity in this example
                await _repository.ApprovalTemplateLevelApprover.DeleteByTemplateIdAsync(existing.ApprovalTemplateId, transaction);
                await _repository.ApprovalTemplateLevel.DeleteByTemplateIdAsync(existing.ApprovalTemplateId, transaction);

                foreach (var levelInput in input.Levels)
                {
                    var level = levelInput.Adapt<ApprovalTemplateLevel>();
                    level.ApprovalTemplateId = existing.ApprovalTemplateId;
                    level.CreatedById = input.UpdatedById ?? 0;
                    level.StatusId = 1;
                    await _repository.ApprovalTemplateLevel.CreateAsync(level, transaction);

                    foreach (var approverInput in levelInput.Approvers)
                    {
                        var approver = approverInput.Adapt<ApprovalTemplateLevelApprover>();
                        approver.ApprovalTemplateLevelId = level.ApprovalTemplateLevelId;
                        approver.CreatedById = input.UpdatedById ?? 0;
                        approver.StatusId = 1;
                        await _repository.ApprovalTemplateLevelApprover.CreateAsync(approver, transaction);
                    }
                }

                await _transactionManager.CommitAsync();
            }
            catch(Exception ex)
            {
                await _transactionManager.RollbackAsync();
                _logger.LogError($"Error in UpdateAsync ApprovalTemplate: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(Guid guid, long deletedBy, bool trackChanges)
        {
            var existing = await _repository.ApprovalTemplate.GetAsync(guid, trackChanges);
            if (existing == null) throw new Exception("Data not found");

            await _repository.ApprovalTemplate.SoftDeleteAsync(existing, deletedBy);
        }
    }
}
