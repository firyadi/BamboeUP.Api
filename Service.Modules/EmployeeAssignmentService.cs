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
    public partial class EmployeeAssignmentService : IEmployeeAssignmentService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public EmployeeAssignmentService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public EmployeeAssignmentService(
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

        public async Task<IEnumerable<EmployeeAssignmentDto>> GetAllEmployeeAssignmentsAsync(bool trackChanges)
        {
            var entities = await _repository.EmployeeAssignment.GetAllByEmployeeGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<EmployeeAssignmentDto>>();
        }

        public async Task<EmployeeAssignmentDto?> GetEmployeeAssignmentByGuidAsync(Guid employeeAssignmentGuid, bool trackChanges)
        {
            var entity = await _repository.EmployeeAssignment.GetEmployeeAssignmentAsync(employeeAssignmentGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<EmployeeAssignmentDto>();
            return dto;
        }

        public async Task<EmployeeAssignmentDto> CreateEmployeeAssignmentAsync(Guid employeeGuid, EmployeeAssignmentForCreationDto input)
        {
            var model = input.Adapt<EmployeeAssignment>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.EmployeeAssignment.CreateEmployeeAssignmentAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "EmployeeAssignment",
                RootEntityKey = model.EmployeeAssignmentGuid.ToString(),
                RootDisplayName = model.Remarks,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeAssignment",
                        EntityKey = model.EmployeeAssignmentGuid.ToString(),
                        EntityDisplayName = model.Remarks,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<EmployeeAssignmentDto>();
        }

        public async Task UpdateEmployeeAssignmentAsync(Guid employeeGuid, Guid employeeAssignmentGuid, EmployeeAssignmentForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeAssignment.GetEmployeeAssignmentAsync(employeeAssignmentGuid, false);

            var model = input.Adapt<EmployeeAssignment>();
            model.EmployeeAssignmentGuid = employeeAssignmentGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.EmployeeAssignment.UpdateEmployeeAssignmentAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "EmployeeAssignment",
                RootEntityKey = model.EmployeeAssignmentGuid.ToString(),
                RootDisplayName = model.Remarks,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeAssignment",
                        EntityKey = model.EmployeeAssignmentGuid.ToString(),
                        EntityDisplayName = model.Remarks,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteEmployeeAssignmentAsync(Guid employeeGuid, Guid employeeAssignmentGuid, EmployeeAssignmentForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeAssignment.GetEmployeeAssignmentAsync(employeeAssignmentGuid, false);
            var model = new EmployeeAssignment
            {
                EmployeeAssignmentGuid = employeeAssignmentGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.EmployeeAssignment.SoftDeleteEmployeeAssignmentAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "EmployeeAssignment",
                RootEntityKey = employeeAssignmentGuid.ToString(),
                RootDisplayName = oldDetail?.Remarks,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeAssignment",
                        EntityKey = employeeAssignmentGuid.ToString(),
                        EntityDisplayName = oldDetail?.Remarks,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteEmployeeAssignmentByAdminAsync(Guid employeeAssignmentGuid, bool trackChanges)
        {
            await _repository.EmployeeAssignment.DeleteEmployeeAssignmentAsync(employeeAssignmentGuid);
        }

        public async Task<IEnumerable<EmployeeAssignmentDto>> SearchEmployeeAssignmentAsync(
            string? effectiveFrom, string? effectiveFromSearchType, string? effectiveto, string? effectivetoSearchType, string? isPrimary, string? isPrimarySearchType, string? isCurrent, string? isCurrentSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeAssignmentGuid)
        {
            var data = await _repository.EmployeeAssignment.SearchEmployeeAssignmentAsync(
                effectiveFrom, effectiveFromSearchType, effectiveto, effectivetoSearchType, isPrimary, isPrimarySearchType, isCurrent, isCurrentSearchType, remarks, remarksSearchType, 
                employeeGuid, employeeAssignmentGuid);
            return data.Adapt<IEnumerable<EmployeeAssignmentDto>>();
        }

        public async Task<IEnumerable<EmployeeAssignmentDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            var result = await _repository.EmployeeAssignment.GetAllByEmployeeGuidAsync(employeeGuid);
            return result.Adapt<IEnumerable<EmployeeAssignmentDto>>();
        }

        public async Task<EmployeeAssignmentDto?> GetByEmployeeGuidAndEmployeeAssignmentGuidAsync(Guid employeeGuid, Guid employeeAssignmentGuid)
        {
            var result = await _repository.EmployeeAssignment.GetByEmployeeGuidAndEmployeeAssignmentGuidAsync(employeeGuid, employeeAssignmentGuid);
            if (result == null) return null;
            return result.Adapt<EmployeeAssignmentDto>();
        }
    }
}
