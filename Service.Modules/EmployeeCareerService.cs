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
    public partial class EmployeeCareerService : IEmployeeCareerService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public EmployeeCareerService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public EmployeeCareerService(
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

        public async Task<IEnumerable<EmployeeCareerDto>> GetAllEmployeeCareersAsync(bool trackChanges)
        {
            var entities = await _repository.EmployeeCareer.GetAllByEmployeeGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<EmployeeCareerDto>>();
        }

        public async Task<EmployeeCareerDto?> GetEmployeeCareerByGuidAsync(Guid employeeCareerGuid, bool trackChanges)
        {
            var entity = await _repository.EmployeeCareer.GetEmployeeCareerAsync(employeeCareerGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<EmployeeCareerDto>();
            return dto;
        }

        public async Task<EmployeeCareerDto> CreateEmployeeCareerAsync(Guid employeeGuid, EmployeeCareerForCreationDto input)
        {
            var model = input.Adapt<EmployeeCareer>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.EmployeeCareer.CreateEmployeeCareerAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "EmployeeCareer",
                RootEntityKey = model.EmployeeCareerGuid.ToString(),
                RootDisplayName = model.ReferenceNo,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeCareer",
                        EntityKey = model.EmployeeCareerGuid.ToString(),
                        EntityDisplayName = model.ReferenceNo,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<EmployeeCareerDto>();
        }

        public async Task UpdateEmployeeCareerAsync(Guid employeeGuid, Guid employeeCareerGuid, EmployeeCareerForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeCareer.GetEmployeeCareerAsync(employeeCareerGuid, false);

            var model = input.Adapt<EmployeeCareer>();
            model.EmployeeCareerGuid = employeeCareerGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.EmployeeCareer.UpdateEmployeeCareerAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "EmployeeCareer",
                RootEntityKey = model.EmployeeCareerGuid.ToString(),
                RootDisplayName = model.ReferenceNo,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeCareer",
                        EntityKey = model.EmployeeCareerGuid.ToString(),
                        EntityDisplayName = model.ReferenceNo,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteEmployeeCareerAsync(Guid employeeGuid, Guid employeeCareerGuid, EmployeeCareerForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeCareer.GetEmployeeCareerAsync(employeeCareerGuid, false);
            var model = new EmployeeCareer
            {
                EmployeeCareerGuid = employeeCareerGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.EmployeeCareer.SoftDeleteEmployeeCareerAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "EmployeeCareer",
                RootEntityKey = employeeCareerGuid.ToString(),
                RootDisplayName = oldDetail?.ReferenceNo,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeCareer",
                        EntityKey = employeeCareerGuid.ToString(),
                        EntityDisplayName = oldDetail?.ReferenceNo,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteEmployeeCareerByAdminAsync(Guid employeeCareerGuid, bool trackChanges)
        {
            await _repository.EmployeeCareer.DeleteEmployeeCareerAsync(employeeCareerGuid);
        }

        public async Task<IEnumerable<EmployeeCareerDto>> SearchEmployeeCareerAsync(
            string? effectiveDate, string? effectiveDateSearchType, string? referenceNo, string? referenceNoSearchType, string? referenceDate, string? referenceDateSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeCareerGuid)
        {
            var data = await _repository.EmployeeCareer.SearchEmployeeCareerAsync(
                effectiveDate, effectiveDateSearchType, referenceNo, referenceNoSearchType, referenceDate, referenceDateSearchType, remarks, remarksSearchType, 
                employeeGuid, employeeCareerGuid);
            return data.Adapt<IEnumerable<EmployeeCareerDto>>();
        }

        public async Task<IEnumerable<EmployeeCareerDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            var result = await _repository.EmployeeCareer.GetAllByEmployeeGuidAsync(employeeGuid);
            return result.Adapt<IEnumerable<EmployeeCareerDto>>();
        }

        public async Task<EmployeeCareerDto?> GetByEmployeeGuidAndEmployeeCareerGuidAsync(Guid employeeGuid, Guid employeeCareerGuid)
        {
            var result = await _repository.EmployeeCareer.GetByEmployeeGuidAndEmployeeCareerGuidAsync(employeeGuid, employeeCareerGuid);
            if (result == null) return null;
            return result.Adapt<EmployeeCareerDto>();
        }
    }
}
