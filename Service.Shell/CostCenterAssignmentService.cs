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
            var entities = await _repository.CostCenterAssignment.GetAllByCostCenterGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<CostCenterAssignmentDto>>();
        }

        public async Task<CostCenterAssignmentDto> GetCostCenterAssignmentByGuidAsync(Guid costCenterAssignmentGuid, bool trackChanges)
        {
            var entity = await _repository.CostCenterAssignment.GetCostCenterAssignmentAsync(costCenterAssignmentGuid, trackChanges);
            return entity.Adapt<CostCenterAssignmentDto>();
        }

        public async Task<CostCenterAssignmentDto> CreateCostCenterAssignmentAsync(Guid costCenterGuid, CostCenterAssignmentForCreationDto input)
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
                        ParentTableName = "CostCenter",
                        ParentEntityKey = costCenterGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<CostCenterAssignmentDto>();
        }

        public async Task UpdateCostCenterAssignmentAsync(Guid costCenterGuid, Guid costCenterAssignmentGuid, CostCenterAssignmentForUpdateDto input, bool trackChanges)
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
                        ParentTableName = "CostCenter",
                        ParentEntityKey = costCenterGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteCostCenterAssignmentAsync(Guid costCenterGuid, Guid costCenterAssignmentGuid, CostCenterAssignmentForDeleteDto input, bool trackChanges)
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
                        ParentTableName = "CostCenter",
                        ParentEntityKey = costCenterGuid.ToString(),
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
            string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? profitCenterId, string? profitCenterIdSearchType, string? costCenterManagerEmployeeId, string? costCenterManagerEmployeeIdSearchType, string? budgetControlType, string? budgetControlTypeSearchType, string? effectiveDate, string? effectiveDateSearchType, string? expiredDate, string? expiredDateSearchType,
            Guid costCenterGuid, Guid costCenterAssignmentGuid)
        {
            var data = await _repository.CostCenterAssignment.SearchCostCenterAssignmentAsync(
                companyId, companyIdSearchType, companyOfficeId, companyOfficeIdSearchType, profitCenterId, profitCenterIdSearchType, costCenterManagerEmployeeId, costCenterManagerEmployeeIdSearchType, budgetControlType, budgetControlTypeSearchType, effectiveDate, effectiveDateSearchType, expiredDate, expiredDateSearchType, 
                costCenterGuid, costCenterAssignmentGuid);
            return data.Adapt<IEnumerable<CostCenterAssignmentDto>>();
        }

        public async Task<IEnumerable<CostCenterAssignmentDto>> GetAllByCostCenterGuidAsync(Guid costCenterGuid)
        {
            var result = await _repository.CostCenterAssignment.GetAllByCostCenterGuidAsync(costCenterGuid);
            return result.Adapt<IEnumerable<CostCenterAssignmentDto>>();
        }

        public async Task<CostCenterAssignmentDto> GetByCostCenterGuidAndCostCenterAssignmentGuidAsync(Guid costCenterGuid, Guid costCenterAssignmentGuid)
        {
            var result = await _repository.CostCenterAssignment.GetByCostCenterGuidAndCostCenterAssignmentGuidAsync(costCenterGuid, costCenterAssignmentGuid);
            return result.Adapt<CostCenterAssignmentDto>();
        }
    }
}
