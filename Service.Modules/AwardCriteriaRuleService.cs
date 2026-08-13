using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class AwardCriteriaRuleService : IAwardCriteriaRuleService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public AwardCriteriaRuleService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public AwardCriteriaRuleService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAuditService audit,
            IUserContext userContext)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
            _audit = audit;
            _userContext = userContext;
        }

        public async Task<IEnumerable<AwardCriteriaRuleDto>> GetAllAwardCriteriaRulesAsync(bool trackChanges)
        {
            var entities = await _repository.AwardCriteriaRule.GetAllByAwardCriteriaGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<AwardCriteriaRuleDto>>();
        }

        public async Task<AwardCriteriaRuleDto?> GetAwardCriteriaRuleByGuidAsync(Guid awardCriteriaRuleGuid, bool trackChanges)
        {
            var entity = await _repository.AwardCriteriaRule.GetAwardCriteriaRuleAsync(awardCriteriaRuleGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<AwardCriteriaRuleDto>();
            return dto;
        }

        public async Task<AwardCriteriaRuleDto> CreateAwardCriteriaRuleAsync(Guid awardCriteriaGuid, AwardCriteriaRuleForCreationDto input)
        {
            var parent = await _repository.AwardCriteria.GetAwardCriteriaAsync(awardCriteriaGuid, false)
                ?? throw new KeyNotFoundException($"AwardCriteria '{{awardCriteriaGuid}}' was not found.");
            var model = input.Adapt<AwardCriteriaRule>();
            model.AwardCriteriaId = parent.AwardCriteriaId;
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.AwardCriteriaRule.CreateAwardCriteriaRuleAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "AwardCriteriaRule",
                RootEntityKey = model.AwardCriteriaRuleGuid.ToString(),
                RootDisplayName = model.RuleCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "AwardCriteriaRule",
                        EntityKey = model.AwardCriteriaRuleGuid.ToString(),
                        EntityDisplayName = model.RuleCode,
                        ParentTableName = "AwardCriteria",
                        ParentEntityKey = awardCriteriaGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<AwardCriteriaRuleDto>();
        }

        public async Task UpdateAwardCriteriaRuleAsync(Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid, AwardCriteriaRuleForUpdateDto input, bool trackChanges)
        {
            var oldSubDetail = await _repository.AwardCriteriaRule.GetAwardCriteriaRuleAsync(awardCriteriaRuleGuid, false);
            var parent = await _repository.AwardCriteria.GetAwardCriteriaAsync(awardCriteriaGuid, false)
                ?? throw new KeyNotFoundException($"AwardCriteria '{{awardCriteriaGuid}}' was not found.");

            var model = input.Adapt<AwardCriteriaRule>();
            model.AwardCriteriaRuleGuid = awardCriteriaRuleGuid;
            model.AwardCriteriaId = parent.AwardCriteriaId;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.AwardCriteriaRule.UpdateAwardCriteriaRuleAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "AwardCriteriaRule",
                RootEntityKey = model.AwardCriteriaRuleGuid.ToString(),
                RootDisplayName = model.RuleCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "AwardCriteriaRule",
                        EntityKey = model.AwardCriteriaRuleGuid.ToString(),
                        EntityDisplayName = model.RuleCode,
                        ParentTableName = "AwardCriteria",
                        ParentEntityKey = awardCriteriaGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldSubDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteAwardCriteriaRuleAsync(Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid, AwardCriteriaRuleForDeleteDto input, bool trackChanges)
        {
            var oldSubDetail = await _repository.AwardCriteriaRule.GetAwardCriteriaRuleAsync(awardCriteriaRuleGuid, false);
            var model = new AwardCriteriaRule
            {
                AwardCriteriaRuleGuid = awardCriteriaRuleGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.AwardCriteriaRule.SoftDeleteAwardCriteriaRuleAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "AwardCriteriaRule",
                RootEntityKey = awardCriteriaRuleGuid.ToString(),
                RootDisplayName = oldSubDetail?.RuleCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "AwardCriteriaRule",
                        EntityKey = awardCriteriaRuleGuid.ToString(),
                        EntityDisplayName = oldSubDetail?.RuleCode,
                        ParentTableName = "AwardCriteria",
                        ParentEntityKey = awardCriteriaGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldSubDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteAwardCriteriaRuleByAdminAsync(Guid awardCriteriaRuleGuid, bool trackChanges)
        {
            await _repository.AwardCriteriaRule.DeleteAwardCriteriaRuleAsync(awardCriteriaRuleGuid);
        }

        public async Task<IEnumerable<AwardCriteriaRuleDto>> SearchAwardCriteriaRuleAsync(
            string? ruleCode, string? ruleCodeSearchType, string? ruleName, string? ruleNameSearchType, string? @operator, string? @operatorSearchType, string? valueFrom, string? valueFromSearchType, string? valueto, string? valuetoSearchType, string? score, string? scoreSearchType, string? note, string? noteSearchType,
            Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid)
        {
            var data = await _repository.AwardCriteriaRule.SearchAwardCriteriaRuleAsync(
                ruleCode, ruleCodeSearchType, ruleName, ruleNameSearchType, @operator, @operatorSearchType, valueFrom, valueFromSearchType, valueto, valuetoSearchType, score, scoreSearchType, note, noteSearchType, 
                awardCriteriaGuid, awardCriteriaRuleGuid);
            return data.Adapt<IEnumerable<AwardCriteriaRuleDto>>();
        }

        public async Task<IEnumerable<AwardCriteriaRuleDto>> GetAllByAwardCriteriaGuidAsync(Guid awardCriteriaGuid)
        {
            var result = await _repository.AwardCriteriaRule.GetAllByAwardCriteriaGuidAsync(awardCriteriaGuid);
            return result.Adapt<IEnumerable<AwardCriteriaRuleDto>>();
        }

        public async Task<AwardCriteriaRuleDto?> GetByAwardCriteriaGuidAndAwardCriteriaRuleGuidAsync(Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid)
        {
            var result = await _repository.AwardCriteriaRule.GetByAwardCriteriaGuidAndAwardCriteriaRuleGuidAsync(awardCriteriaGuid, awardCriteriaRuleGuid);
            if (result == null) return null;
            return result.Adapt<AwardCriteriaRuleDto>();
        }
    }
}
