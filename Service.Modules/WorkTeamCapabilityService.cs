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
    public partial class WorkTeamCapabilityService : IWorkTeamCapabilityService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public WorkTeamCapabilityService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public WorkTeamCapabilityService(
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

        public async Task<IEnumerable<WorkTeamCapabilityDto>> GetAllWorkTeamCapabilitiesAsync(bool trackChanges)
        {
            var entities = await _repository.WorkTeamCapability.GetAllByWorkTeamGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<WorkTeamCapabilityDto>>();
        }

        public async Task<WorkTeamCapabilityDto?> GetWorkTeamCapabilityByGuidAsync(Guid workTeamCapabilityGuid, bool trackChanges)
        {
            var entity = await _repository.WorkTeamCapability.GetWorkTeamCapabilityAsync(workTeamCapabilityGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<WorkTeamCapabilityDto>();
            return dto;
        }

        public async Task<WorkTeamCapabilityDto> CreateWorkTeamCapabilityAsync(Guid workTeamGuid, WorkTeamCapabilityForCreationDto input)
        {
            var model = input.Adapt<WorkTeamCapability>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.WorkTeamCapability.CreateWorkTeamCapabilityAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "WorkTeamCapability",
                RootEntityKey = model.WorkTeamCapabilityGuid.ToString(),
                RootDisplayName = model.WorkTeamCapabilityGuid.ToString(),
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "WorkTeamCapability",
                        EntityKey = model.WorkTeamCapabilityGuid.ToString(),
                        EntityDisplayName = model.WorkTeamCapabilityGuid.ToString(),
                        ParentTableName = "WorkTeam",
                        ParentEntityKey = workTeamGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<WorkTeamCapabilityDto>();
        }

        public async Task UpdateWorkTeamCapabilityAsync(Guid workTeamGuid, Guid workTeamCapabilityGuid, WorkTeamCapabilityForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.WorkTeamCapability.GetWorkTeamCapabilityAsync(workTeamCapabilityGuid, false);

            var model = input.Adapt<WorkTeamCapability>();
            model.WorkTeamCapabilityGuid = workTeamCapabilityGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.WorkTeamCapability.UpdateWorkTeamCapabilityAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "WorkTeamCapability",
                RootEntityKey = model.WorkTeamCapabilityGuid.ToString(),
                RootDisplayName = model.WorkTeamCapabilityGuid.ToString(),
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "WorkTeamCapability",
                        EntityKey = model.WorkTeamCapabilityGuid.ToString(),
                        EntityDisplayName = model.WorkTeamCapabilityGuid.ToString(),
                        ParentTableName = "WorkTeam",
                        ParentEntityKey = workTeamGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteWorkTeamCapabilityAsync(Guid workTeamGuid, Guid workTeamCapabilityGuid, WorkTeamCapabilityForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.WorkTeamCapability.GetWorkTeamCapabilityAsync(workTeamCapabilityGuid, false);
            var model = new WorkTeamCapability
            {
                WorkTeamCapabilityGuid = workTeamCapabilityGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.WorkTeamCapability.SoftDeleteWorkTeamCapabilityAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "WorkTeamCapability",
                RootEntityKey = workTeamCapabilityGuid.ToString(),
                RootDisplayName = oldDetail?.WorkTeamCapabilityGuid.ToString(),
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "WorkTeamCapability",
                        EntityKey = workTeamCapabilityGuid.ToString(),
                        EntityDisplayName = oldDetail?.WorkTeamCapabilityGuid.ToString(),
                        ParentTableName = "WorkTeam",
                        ParentEntityKey = workTeamGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteWorkTeamCapabilityByAdminAsync(Guid workTeamCapabilityGuid, bool trackChanges)
        {
            await _repository.WorkTeamCapability.DeleteWorkTeamCapabilityAsync(workTeamCapabilityGuid);
        }

        public async Task<IEnumerable<WorkTeamCapabilityDto>> SearchWorkTeamCapabilityAsync(
            string? isViewAble, string? isViewAbleSearchType, string? isAddAble, string? isAddAbleSearchType, string? isEditAble, string? isEditAbleSearchType, string? isApproveAble, string? isApproveAbleSearchType, string? isExportAble, string? isExportAbleSearchType,
            Guid workTeamGuid, Guid workTeamCapabilityGuid)
        {
            var data = await _repository.WorkTeamCapability.SearchWorkTeamCapabilityAsync(
                isViewAble, isViewAbleSearchType, isAddAble, isAddAbleSearchType, isEditAble, isEditAbleSearchType, isApproveAble, isApproveAbleSearchType, isExportAble, isExportAbleSearchType, 
                workTeamGuid, workTeamCapabilityGuid);
            return data.Adapt<IEnumerable<WorkTeamCapabilityDto>>();
        }

        public async Task<IEnumerable<WorkTeamCapabilityDto>> GetAllByWorkTeamGuidAsync(Guid workTeamGuid)
        {
            var result = await _repository.WorkTeamCapability.GetAllByWorkTeamGuidAsync(workTeamGuid);
            return result.Adapt<IEnumerable<WorkTeamCapabilityDto>>();
        }

        public async Task<WorkTeamCapabilityDto?> GetByWorkTeamGuidAndWorkTeamCapabilityGuidAsync(Guid workTeamGuid, Guid workTeamCapabilityGuid)
        {
            var result = await _repository.WorkTeamCapability.GetByWorkTeamGuidAndWorkTeamCapabilityGuidAsync(workTeamGuid, workTeamCapabilityGuid);
            if (result == null) return null;
            return result.Adapt<WorkTeamCapabilityDto>();
        }

        public Task<IEnumerable<WorkCapabilityDto>> GetWorkCapabilitiesAsync() =>
            _repository.WorkTeamCapability.GetWorkCapabilitiesAsync();
    }
}
