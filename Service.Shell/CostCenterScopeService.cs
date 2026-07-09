using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Shell
{
    public partial class CostCenterScopeService : ICostCenterScopeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public CostCenterScopeService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public CostCenterScopeService(
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

        public async Task<IEnumerable<CostCenterScopeDto>> GetAllCostCenterScopesAsync(bool trackChanges)
        {
            var entities = await _repository.CostCenterScope.GetAllByCostCenterGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<CostCenterScopeDto>>();
        }

        public async Task<CostCenterScopeDto> GetCostCenterScopeByGuidAsync(Guid costCenterScopeGuid, bool trackChanges)
        {
            var entity = await _repository.CostCenterScope.GetCostCenterScopeAsync(costCenterScopeGuid, trackChanges);
            return entity.Adapt<CostCenterScopeDto>();
        }

        public async Task<CostCenterScopeDto> CreateCostCenterScopeAsync(Guid costCenterGuid, CostCenterScopeForCreationDto input)
        {
            var model = input.Adapt<CostCenterScope>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.CostCenterScope.CreateCostCenterScopeAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "CostCenterScope",
                RootEntityKey = model.CostCenterScopeGuid.ToString(),
                RootDisplayName = model.ScopeType,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CostCenterScope",
                        EntityKey = model.CostCenterScopeGuid.ToString(),
                        EntityDisplayName = model.ScopeType,
                        ParentTableName = "CostCenter",
                        ParentEntityKey = costCenterGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<CostCenterScopeDto>();
        }

        public async Task UpdateCostCenterScopeAsync(Guid costCenterGuid, Guid costCenterScopeGuid, CostCenterScopeForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.CostCenterScope.GetCostCenterScopeAsync(costCenterScopeGuid, false);

            var model = input.Adapt<CostCenterScope>();
            model.CostCenterScopeGuid = costCenterScopeGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.CostCenterScope.UpdateCostCenterScopeAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "CostCenterScope",
                RootEntityKey = model.CostCenterScopeGuid.ToString(),
                RootDisplayName = model.ScopeType,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CostCenterScope",
                        EntityKey = model.CostCenterScopeGuid.ToString(),
                        EntityDisplayName = model.ScopeType,
                        ParentTableName = "CostCenter",
                        ParentEntityKey = costCenterGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteCostCenterScopeAsync(Guid costCenterGuid, Guid costCenterScopeGuid, CostCenterScopeForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.CostCenterScope.GetCostCenterScopeAsync(costCenterScopeGuid, false);
            var model = new CostCenterScope
            {
                CostCenterScopeGuid = costCenterScopeGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.CostCenterScope.SoftDeleteCostCenterScopeAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "CostCenterScope",
                RootEntityKey = costCenterScopeGuid.ToString(),
                RootDisplayName = oldDetail?.ScopeType,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CostCenterScope",
                        EntityKey = costCenterScopeGuid.ToString(),
                        EntityDisplayName = oldDetail?.ScopeType,
                        ParentTableName = "CostCenter",
                        ParentEntityKey = costCenterGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteCostCenterScopeByAdminAsync(Guid costCenterScopeGuid, bool trackChanges)
        {
            await _repository.CostCenterScope.DeleteCostCenterScopeAsync(costCenterScopeGuid);
        }

        public async Task<IEnumerable<CostCenterScopeDto>> SearchCostCenterScopeAsync(
            string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? scopeType, string? scopeTypeSearchType,
            Guid costCenterGuid, Guid costCenterScopeGuid)
        {
            var data = await _repository.CostCenterScope.SearchCostCenterScopeAsync(
                companyId, companyIdSearchType, companyOfficeId, companyOfficeIdSearchType, scopeType, scopeTypeSearchType, 
                costCenterGuid, costCenterScopeGuid);
            return data.Adapt<IEnumerable<CostCenterScopeDto>>();
        }

        public async Task<IEnumerable<CostCenterScopeDto>> GetAllByCostCenterGuidAsync(Guid costCenterGuid)
        {
            var result = await _repository.CostCenterScope.GetAllByCostCenterGuidAsync(costCenterGuid);
            return result.Adapt<IEnumerable<CostCenterScopeDto>>();
        }

        public async Task<CostCenterScopeDto> GetByCostCenterGuidAndCostCenterScopeGuidAsync(Guid costCenterGuid, Guid costCenterScopeGuid)
        {
            var result = await _repository.CostCenterScope.GetByCostCenterGuidAndCostCenterScopeGuidAsync(costCenterGuid, costCenterScopeGuid);
            return result.Adapt<CostCenterScopeDto>();
        }
    }
}
