using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class EmployeeTrainingService : IEmployeeTrainingService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;
        private readonly IFileStorageService _fileStorageService;

        public EmployeeTrainingService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
            _fileStorageService = null!;
        }

        public EmployeeTrainingService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAuditService audit,
            IUserContext userContext,
        IFileStorageService fileStorageService)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
            _audit = audit;
            _userContext = userContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<EmployeeTrainingDto>> GetAllEmployeeTrainingsAsync(bool trackChanges)
        {
            var entities = await _repository.EmployeeTraining.GetAllByEmployeeGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<EmployeeTrainingDto>>();
        }

        public async Task<EmployeeTrainingDto?> GetEmployeeTrainingByGuidAsync(Guid employeeTrainingGuid, bool trackChanges)
        {
            var entity = await _repository.EmployeeTraining.GetEmployeeTrainingAsync(employeeTrainingGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<EmployeeTrainingDto>();
            if (entity.FileStorageId is > 0)
            {
                var file = await _fileStorageService.GetFileStorageByIdAsync(entity.FileStorageId.Value, false);
                dto.FileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<EmployeeTrainingDto> CreateEmployeeTrainingAsync(Guid employeeGuid, EmployeeTrainingForCreationDto input)
        {
            var model = input.Adapt<EmployeeTraining>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.EmployeeTraining.CreateEmployeeTrainingAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "EmployeeTraining",
                RootEntityKey = model.EmployeeTrainingGuid.ToString(),
                RootDisplayName = model.Grade,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeTraining",
                        EntityKey = model.EmployeeTrainingGuid.ToString(),
                        EntityDisplayName = model.Grade,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<EmployeeTrainingDto>();
        }

        public async Task UpdateEmployeeTrainingAsync(Guid employeeGuid, Guid employeeTrainingGuid, EmployeeTrainingForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeTraining.GetEmployeeTrainingAsync(employeeTrainingGuid, false);

            var model = input.Adapt<EmployeeTraining>();
            model.EmployeeTrainingGuid = employeeTrainingGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.EmployeeTraining.UpdateEmployeeTrainingAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "EmployeeTraining",
                RootEntityKey = model.EmployeeTrainingGuid.ToString(),
                RootDisplayName = model.Grade,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeTraining",
                        EntityKey = model.EmployeeTrainingGuid.ToString(),
                        EntityDisplayName = model.Grade,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteEmployeeTrainingAsync(Guid employeeGuid, Guid employeeTrainingGuid, EmployeeTrainingForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeTraining.GetEmployeeTrainingAsync(employeeTrainingGuid, false);
            var model = new EmployeeTraining
            {
                EmployeeTrainingGuid = employeeTrainingGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.EmployeeTraining.SoftDeleteEmployeeTrainingAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "EmployeeTraining",
                RootEntityKey = employeeTrainingGuid.ToString(),
                RootDisplayName = oldDetail?.Grade,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeTraining",
                        EntityKey = employeeTrainingGuid.ToString(),
                        EntityDisplayName = oldDetail?.Grade,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteEmployeeTrainingByAdminAsync(Guid employeeTrainingGuid, bool trackChanges)
        {
            await _repository.EmployeeTraining.DeleteEmployeeTrainingAsync(employeeTrainingGuid);
        }

        public async Task<IEnumerable<EmployeeTrainingDto>> SearchEmployeeTrainingAsync(
            string? startDate, string? startDateSearchType, string? endDate, string? endDateSearchType, string? durationHour, string? durationHourSearchType, string? score, string? scoreSearchType, string? grade, string? gradeSearchType, string? isPassed, string? isPassedSearchType, string? certificateNumber, string? certificateNumberSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeTrainingGuid)
        {
            var data = await _repository.EmployeeTraining.SearchEmployeeTrainingAsync(
                startDate, startDateSearchType, endDate, endDateSearchType, durationHour, durationHourSearchType, score, scoreSearchType, grade, gradeSearchType, isPassed, isPassedSearchType, certificateNumber, certificateNumberSearchType, remarks, remarksSearchType, 
                employeeGuid, employeeTrainingGuid);
            return data.Adapt<IEnumerable<EmployeeTrainingDto>>();
        }

        public async Task<IEnumerable<EmployeeTrainingDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            var result = await _repository.EmployeeTraining.GetAllByEmployeeGuidAsync(employeeGuid);
            return result.Adapt<IEnumerable<EmployeeTrainingDto>>();
        }

        public async Task<EmployeeTrainingDto?> GetByEmployeeGuidAndEmployeeTrainingGuidAsync(Guid employeeGuid, Guid employeeTrainingGuid)
        {
            var result = await _repository.EmployeeTraining.GetByEmployeeGuidAndEmployeeTrainingGuidAsync(employeeGuid, employeeTrainingGuid);
            if (result == null) return null;
            return result.Adapt<EmployeeTrainingDto>();
        }
    }
}
