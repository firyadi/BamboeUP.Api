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
    public partial class AwardCriteriaService : IAwardCriteriaService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public AwardCriteriaService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public AwardCriteriaService(
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

        public async Task<IEnumerable<AwardCriteriaDto>> GetAllAwardCriteriaAsync(bool trackChanges)
        {
            var entities = await _repository.AwardCriteria.GetAllByAwardGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<AwardCriteriaDto>>();
        }

        public async Task<AwardCriteriaDto?> GetAwardCriteriaByGuidAsync(Guid awardCriteriaGuid, bool trackChanges)
        {
            var entity = await _repository.AwardCriteria.GetAwardCriteriaAsync(awardCriteriaGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<AwardCriteriaDto>();
            return dto;
        }

        public async Task<AwardCriteriaDto> CreateAwardCriteriaAsync(Guid awardGuid, AwardCriteriaForCreationDto input)
        {
            var parent = await _repository.Award.GetAwardAsync(awardGuid, false)
                ?? throw new KeyNotFoundException($"Award '{{awardGuid}}' was not found.");
            var model = input.Adapt<AwardCriteria>();
            model.AwardId = parent.AwardId;
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.AwardCriteria.CreateAwardCriteriaAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "AwardCriteria",
                RootEntityKey = model.AwardCriteriaGuid.ToString(),
                RootDisplayName = model.CriteriaCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "AwardCriteria",
                        EntityKey = model.AwardCriteriaGuid.ToString(),
                        EntityDisplayName = model.CriteriaCode,
                        ParentTableName = "Award",
                        ParentEntityKey = awardGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<AwardCriteriaDto>();
        }

        public async Task UpdateAwardCriteriaAsync(Guid awardGuid, Guid awardCriteriaGuid, AwardCriteriaForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.AwardCriteria.GetAwardCriteriaAsync(awardCriteriaGuid, false);
            var parent = await _repository.Award.GetAwardAsync(awardGuid, false)
                ?? throw new KeyNotFoundException($"Award '{{awardGuid}}' was not found.");

            var model = input.Adapt<AwardCriteria>();
            model.AwardCriteriaGuid = awardCriteriaGuid;
            model.AwardId = parent.AwardId;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.AwardCriteria.UpdateAwardCriteriaAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "AwardCriteria",
                RootEntityKey = model.AwardCriteriaGuid.ToString(),
                RootDisplayName = model.CriteriaCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "AwardCriteria",
                        EntityKey = model.AwardCriteriaGuid.ToString(),
                        EntityDisplayName = model.CriteriaCode,
                        ParentTableName = "Award",
                        ParentEntityKey = awardGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteAwardCriteriaAsync(Guid awardGuid, Guid awardCriteriaGuid, AwardCriteriaForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.AwardCriteria.GetAwardCriteriaAsync(awardCriteriaGuid, false);
            var model = new AwardCriteria
            {
                AwardCriteriaGuid = awardCriteriaGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.AwardCriteria.SoftDeleteAwardCriteriaAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "AwardCriteria",
                RootEntityKey = awardCriteriaGuid.ToString(),
                RootDisplayName = oldDetail?.CriteriaCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "AwardCriteria",
                        EntityKey = awardCriteriaGuid.ToString(),
                        EntityDisplayName = oldDetail?.CriteriaCode,
                        ParentTableName = "Award",
                        ParentEntityKey = awardGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteAwardCriteriaByAdminAsync(Guid awardCriteriaGuid, bool trackChanges)
        {
            await _repository.AwardCriteria.DeleteAwardCriteriaAsync(awardCriteriaGuid);
        }

        public async Task<IEnumerable<AwardCriteriaDto>> SearchAwardCriteriaAsync(
            string? lineNo, string? lineNoSearchType, string? criteriaCode, string? criteriaCodeSearchType, string? criteriaName, string? criteriaNameSearchType, string? minimumScore, string? minimumScoreSearchType, string? maximumScore, string? maximumScoreSearchType, string? weight, string? weightSearchType, string? note, string? noteSearchType,
            Guid awardGuid, Guid awardCriteriaGuid)
        {
            var data = await _repository.AwardCriteria.SearchAwardCriteriaAsync(
                lineNo, lineNoSearchType, criteriaCode, criteriaCodeSearchType, criteriaName, criteriaNameSearchType, minimumScore, minimumScoreSearchType, maximumScore, maximumScoreSearchType, weight, weightSearchType, note, noteSearchType, 
                awardGuid, awardCriteriaGuid);
            return data.Adapt<IEnumerable<AwardCriteriaDto>>();
        }

        public async Task<IEnumerable<AwardCriteriaDto>> GetAllByAwardGuidAsync(Guid awardGuid)
        {
            var result = await _repository.AwardCriteria.GetAllByAwardGuidAsync(awardGuid);
            var dtos = result.Adapt<List<AwardCriteriaDto>>();
            foreach (var dto in dtos)
            {
                var subEntities = await _repository.AwardCriteriaRule.GetAllByAwardCriteriaIdAsync(dto.AwardCriteriaId);
                dto.AwardCriteriaRules = subEntities.Adapt<List<AwardCriteriaRuleDto>>();
            }
            return dtos;
        }

        public async Task<AwardCriteriaDto?> GetByAwardGuidAndAwardCriteriaGuidAsync(Guid awardGuid, Guid awardCriteriaGuid)
        {
            var result = await _repository.AwardCriteria.GetByAwardGuidAndAwardCriteriaGuidAsync(awardGuid, awardCriteriaGuid);
            if (result == null) return null;
            var dto = result.Adapt<AwardCriteriaDto>();
            var subEntities = await _repository.AwardCriteriaRule.GetAllByAwardCriteriaIdAsync(dto.AwardCriteriaId);
            dto.AwardCriteriaRules = subEntities.Adapt<List<AwardCriteriaRuleDto>>();
            return dto;
        }
    }
}
