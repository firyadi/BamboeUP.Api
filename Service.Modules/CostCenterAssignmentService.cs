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
    public partial class CostCenterAssignmentService : ICostCenterAssignmentService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public CostCenterAssignmentService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public CostCenterAssignmentService(
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

        public async Task<IEnumerable<CostCenterAssignmentDto>> GetAllCostCenterAssignmentsAsync(bool trackChanges)
        {
            var entities = await _repository.CostCenterAssignment.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<CostCenterAssignmentDto>>();
        }

        public async Task<CostCenterAssignmentDto?> GetCostCenterAssignmentByGuidAsync(Guid costCenterAssignmentGuid, bool trackChanges)
        {
            var entity = await _repository.CostCenterAssignment.GetCostCenterAssignmentAsync(costCenterAssignmentGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<CostCenterAssignmentDto>();
            return dto;
        }

        public async Task<CostCenterAssignmentDto> CreateCostCenterAssignmentAsync(Guid personGuid, CostCenterAssignmentForCreationDto input)
        {
            var model = input.Adapt<CostCenterAssignment>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.CostCenterAssignment.CreateCostCenterAssignmentAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "CostCenterAssignment",
                RootEntityKey = model.CostCenterAssignmentGuid.ToString(),
                RootDisplayName = model.EffectiveDate.ToString(),
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CostCenterAssignment",
                        EntityKey = model.CostCenterAssignmentGuid.ToString(),
                        EntityDisplayName = model.EffectiveDate.ToString(),
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<CostCenterAssignmentDto>();
        }

        public async Task UpdateCostCenterAssignmentAsync(Guid personGuid, Guid costCenterAssignmentGuid, CostCenterAssignmentForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.CostCenterAssignment.GetCostCenterAssignmentAsync(costCenterAssignmentGuid, false);

            var model = input.Adapt<CostCenterAssignment>();
            model.CostCenterAssignmentGuid = costCenterAssignmentGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.CostCenterAssignment.UpdateCostCenterAssignmentAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "CostCenterAssignment",
                RootEntityKey = model.CostCenterAssignmentGuid.ToString(),
                RootDisplayName = model.EffectiveDate.ToString(),
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CostCenterAssignment",
                        EntityKey = model.CostCenterAssignmentGuid.ToString(),
                        EntityDisplayName = model.EffectiveDate.ToString(),
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteCostCenterAssignmentAsync(Guid personGuid, Guid costCenterAssignmentGuid, CostCenterAssignmentForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.CostCenterAssignment.GetCostCenterAssignmentAsync(costCenterAssignmentGuid, false);
            var model = new CostCenterAssignment
            {
                CostCenterAssignmentGuid = costCenterAssignmentGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.CostCenterAssignment.SoftDeleteCostCenterAssignmentAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "CostCenterAssignment",
                RootEntityKey = costCenterAssignmentGuid.ToString(),
                RootDisplayName = oldDetail?.EffectiveDate.ToString(),
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CostCenterAssignment",
                        EntityKey = costCenterAssignmentGuid.ToString(),
                        EntityDisplayName = oldDetail?.EffectiveDate.ToString(),
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteCostCenterAssignmentByAdminAsync(Guid costCenterAssignmentGuid, bool trackChanges)
        {
            await _repository.CostCenterAssignment.DeleteCostCenterAssignmentAsync(costCenterAssignmentGuid);
        }

        public async Task<IEnumerable<CostCenterAssignmentDto>> SearchCostCenterAssignmentAsync(
            string? budgetControlType, string? budgetControlTypeSearchType, string? effectiveDate, string? effectiveDateSearchType, string? expiredDate, string? expiredDateSearchType,
            Guid personGuid, Guid costCenterAssignmentGuid)
        {
            var data = await _repository.CostCenterAssignment.SearchCostCenterAssignmentAsync(
                budgetControlType, budgetControlTypeSearchType, effectiveDate, effectiveDateSearchType, expiredDate, expiredDateSearchType, 
                personGuid, costCenterAssignmentGuid);
            return data.Adapt<IEnumerable<CostCenterAssignmentDto>>();
        }

        public async Task<IEnumerable<CostCenterAssignmentDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.CostCenterAssignment.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<CostCenterAssignmentDto>>();
        }

        public async Task<CostCenterAssignmentDto?> GetByPersonGuidAndCostCenterAssignmentGuidAsync(Guid personGuid, Guid costCenterAssignmentGuid)
        {
            var result = await _repository.CostCenterAssignment.GetByPersonGuidAndCostCenterAssignmentGuidAsync(personGuid, costCenterAssignmentGuid);
            if (result == null) return null;
            return result.Adapt<CostCenterAssignmentDto>();
        }
    }
}
