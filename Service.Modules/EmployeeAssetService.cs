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
    public partial class EmployeeAssetService : IEmployeeAssetService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public EmployeeAssetService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public EmployeeAssetService(
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

        public async Task<IEnumerable<EmployeeAssetDto>> GetAllEmployeeAssetsAsync(bool trackChanges)
        {
            var entities = await _repository.EmployeeAsset.GetAllByEmployeeGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<EmployeeAssetDto>>();
        }

        public async Task<EmployeeAssetDto?> GetEmployeeAssetByGuidAsync(Guid employeeAssetGuid, bool trackChanges)
        {
            var entity = await _repository.EmployeeAsset.GetEmployeeAssetAsync(employeeAssetGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<EmployeeAssetDto>();
            return dto;
        }

        public async Task<EmployeeAssetDto> CreateEmployeeAssetAsync(Guid employeeGuid, EmployeeAssetForCreationDto input)
        {
            var model = input.Adapt<EmployeeAsset>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.EmployeeAsset.CreateEmployeeAssetAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "EmployeeAsset",
                RootEntityKey = model.EmployeeAssetGuid.ToString(),
                RootDisplayName = model.ConditionWhenAssigned,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeAsset",
                        EntityKey = model.EmployeeAssetGuid.ToString(),
                        EntityDisplayName = model.ConditionWhenAssigned,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<EmployeeAssetDto>();
        }

        public async Task UpdateEmployeeAssetAsync(Guid employeeGuid, Guid employeeAssetGuid, EmployeeAssetForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeAsset.GetEmployeeAssetAsync(employeeAssetGuid, false);

            var model = input.Adapt<EmployeeAsset>();
            model.EmployeeAssetGuid = employeeAssetGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.EmployeeAsset.UpdateEmployeeAssetAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "EmployeeAsset",
                RootEntityKey = model.EmployeeAssetGuid.ToString(),
                RootDisplayName = model.ConditionWhenAssigned,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeAsset",
                        EntityKey = model.EmployeeAssetGuid.ToString(),
                        EntityDisplayName = model.ConditionWhenAssigned,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteEmployeeAssetAsync(Guid employeeGuid, Guid employeeAssetGuid, EmployeeAssetForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeAsset.GetEmployeeAssetAsync(employeeAssetGuid, false);
            var model = new EmployeeAsset
            {
                EmployeeAssetGuid = employeeAssetGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.EmployeeAsset.SoftDeleteEmployeeAssetAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "EmployeeAsset",
                RootEntityKey = employeeAssetGuid.ToString(),
                RootDisplayName = oldDetail?.ConditionWhenAssigned,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeAsset",
                        EntityKey = employeeAssetGuid.ToString(),
                        EntityDisplayName = oldDetail?.ConditionWhenAssigned,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteEmployeeAssetByAdminAsync(Guid employeeAssetGuid, bool trackChanges)
        {
            await _repository.EmployeeAsset.DeleteEmployeeAssetAsync(employeeAssetGuid);
        }

        public async Task<IEnumerable<EmployeeAssetDto>> SearchEmployeeAssetAsync(
            string? assignedDate, string? assignedDateSearchType, string? returnedDate, string? returnedDateSearchType, string? expectedReturnDate, string? expectedReturnDateSearchType, string? isCurrent, string? isCurrentSearchType, string? conditionWhenAssigned, string? conditionWhenAssignedSearchType, string? conditionWhenReturned, string? conditionWhenReturnedSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeAssetGuid)
        {
            var data = await _repository.EmployeeAsset.SearchEmployeeAssetAsync(
                assignedDate, assignedDateSearchType, returnedDate, returnedDateSearchType, expectedReturnDate, expectedReturnDateSearchType, isCurrent, isCurrentSearchType, conditionWhenAssigned, conditionWhenAssignedSearchType, conditionWhenReturned, conditionWhenReturnedSearchType, remarks, remarksSearchType, 
                employeeGuid, employeeAssetGuid);
            return data.Adapt<IEnumerable<EmployeeAssetDto>>();
        }

        public async Task<IEnumerable<EmployeeAssetDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            var result = await _repository.EmployeeAsset.GetAllByEmployeeGuidAsync(employeeGuid);
            return result.Adapt<IEnumerable<EmployeeAssetDto>>();
        }

        public async Task<EmployeeAssetDto?> GetByEmployeeGuidAndEmployeeAssetGuidAsync(Guid employeeGuid, Guid employeeAssetGuid)
        {
            var result = await _repository.EmployeeAsset.GetByEmployeeGuidAndEmployeeAssetGuidAsync(employeeGuid, employeeAssetGuid);
            if (result == null) return null;
            return result.Adapt<EmployeeAssetDto>();
        }
    }
}
