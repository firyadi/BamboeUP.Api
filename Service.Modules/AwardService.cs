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
    public partial class AwardService : IAwardService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public AwardService(
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

        public async Task<IEnumerable<AwardDto>> GetAllAwardsAsync(bool trackChanges)
        {
            var entities = await _repository.Award.GetAllAwardsAsync(trackChanges);
            return entities.Adapt<IEnumerable<AwardDto>>();
        }

        public async Task<AwardDto?> GetAwardByGuidAsync(Guid awardGuid, bool trackChanges)
        {
            var entity = await _repository.Award.GetAwardAsync(awardGuid, trackChanges);
            return entity.Adapt<AwardDto>();
        }

        public async Task<AwardDto> CreateAwardAsync(AwardForCreationDto input)
        {
            var model = input.Adapt<Award>();

            if (model.AwardGuid == Guid.Empty)
                model.AwardGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.Award.CreateAwardAsync(model);


            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Award",
                RootEntityKey = model.AwardGuid.ToString(),
                RootDisplayName = model.AwardCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Award",
                        EntityKey = model.AwardGuid.ToString(),
                        EntityDisplayName = model.AwardCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<AwardDto>();
        }

        public async Task UpdateAwardAsync(Guid awardGuid, AwardForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await _repository.Award.GetAwardAsync(awardGuid, false);

            var model = input.Adapt<Award>();
            model.AwardGuid = awardGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.Award.UpdateAwardAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Award",
                RootEntityKey = model.AwardGuid.ToString(),
                RootDisplayName = model.AwardCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Award",
                        EntityKey = model.AwardGuid.ToString(),
                        EntityDisplayName = model.AwardCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteAwardAsync(Guid awardGuid, AwardForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await _repository.Award.GetAwardAsync(awardGuid, false);
            var model = new Award
            {
                AwardGuid = awardGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.Award.SoftDeleteAwardAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Award",
                RootEntityKey = awardGuid.ToString(),
                RootDisplayName = oldEntity?.AwardCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Award",
                        EntityKey = awardGuid.ToString(),
                        EntityDisplayName = oldEntity?.AwardCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteAwardByAdminAsync(Guid awardGuid, bool trackChanges)
        {
            await _repository.Award.DeleteAwardAsync(awardGuid);
        }

        public async Task<IEnumerable<AwardDto>> SearchAwardAsync(
            string? awardCode, string? awardCodeSearchType, string? awardName, string? awardNameSearchType, string? srAwardCriteria, string? srAwardCriteriaSearchType, string? srAwardType, string? srAwardTypeSearchType, string? validFrom, string? validFromSearchType, string? validto, string? validtoSearchType, string? awardPrize, string? awardPrizeSearchType, string? note, string? noteSearchType

            )
        {
            var data = await _repository.Award.SearchAwardAsync(
                awardCode, awardCodeSearchType, awardName, awardNameSearchType, srAwardCriteria, srAwardCriteriaSearchType, srAwardType, srAwardTypeSearchType, validFrom, validFromSearchType, validto, validtoSearchType, awardPrize, awardPrizeSearchType, note, noteSearchType

                );
            return data.Adapt<IEnumerable<AwardDto>>();
        }
    }
}
