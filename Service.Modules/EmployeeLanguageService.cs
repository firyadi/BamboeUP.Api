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
    public partial class EmployeeLanguageService : IEmployeeLanguageService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;
        private readonly IFileStorageService _fileStorageService;

        public EmployeeLanguageService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
            _fileStorageService = null!;
        }

        public EmployeeLanguageService(
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

        public async Task<IEnumerable<EmployeeLanguageDto>> GetAllEmployeeLanguagesAsync(bool trackChanges)
        {
            var entities = await _repository.EmployeeLanguage.GetAllByEmployeeGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<EmployeeLanguageDto>>();
        }

        public async Task<EmployeeLanguageDto?> GetEmployeeLanguageByGuidAsync(Guid employeeLanguageGuid, bool trackChanges)
        {
            var entity = await _repository.EmployeeLanguage.GetEmployeeLanguageAsync(employeeLanguageGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<EmployeeLanguageDto>();
            if (entity.FileStorageId is > 0)
            {
                var file = await _fileStorageService.GetFileStorageByIdAsync(entity.FileStorageId.Value, false);
                dto.FileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<EmployeeLanguageDto> CreateEmployeeLanguageAsync(Guid employeeGuid, EmployeeLanguageForCreationDto input)
        {
            var model = input.Adapt<EmployeeLanguage>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.EmployeeLanguage.CreateEmployeeLanguageAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "EmployeeLanguage",
                RootEntityKey = model.EmployeeLanguageGuid.ToString(),
                RootDisplayName = model.CertificationName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeLanguage",
                        EntityKey = model.EmployeeLanguageGuid.ToString(),
                        EntityDisplayName = model.CertificationName,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<EmployeeLanguageDto>();
        }

        public async Task UpdateEmployeeLanguageAsync(Guid employeeGuid, Guid employeeLanguageGuid, EmployeeLanguageForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeLanguage.GetEmployeeLanguageAsync(employeeLanguageGuid, false);

            var model = input.Adapt<EmployeeLanguage>();
            model.EmployeeLanguageGuid = employeeLanguageGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.EmployeeLanguage.UpdateEmployeeLanguageAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "EmployeeLanguage",
                RootEntityKey = model.EmployeeLanguageGuid.ToString(),
                RootDisplayName = model.CertificationName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeLanguage",
                        EntityKey = model.EmployeeLanguageGuid.ToString(),
                        EntityDisplayName = model.CertificationName,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteEmployeeLanguageAsync(Guid employeeGuid, Guid employeeLanguageGuid, EmployeeLanguageForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeLanguage.GetEmployeeLanguageAsync(employeeLanguageGuid, false);
            var model = new EmployeeLanguage
            {
                EmployeeLanguageGuid = employeeLanguageGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.EmployeeLanguage.SoftDeleteEmployeeLanguageAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "EmployeeLanguage",
                RootEntityKey = employeeLanguageGuid.ToString(),
                RootDisplayName = oldDetail?.CertificationName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeLanguage",
                        EntityKey = employeeLanguageGuid.ToString(),
                        EntityDisplayName = oldDetail?.CertificationName,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteEmployeeLanguageByAdminAsync(Guid employeeLanguageGuid, bool trackChanges)
        {
            await _repository.EmployeeLanguage.DeleteEmployeeLanguageAsync(employeeLanguageGuid);
        }

        public async Task<IEnumerable<EmployeeLanguageDto>> SearchEmployeeLanguageAsync(
            string? certificationName, string? certificationNameSearchType, string? score, string? scoreSearchType, string? issuedDate, string? issuedDateSearchType, string? expiredDate, string? expiredDateSearchType, string? isPrimary, string? isPrimarySearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeLanguageGuid)
        {
            var data = await _repository.EmployeeLanguage.SearchEmployeeLanguageAsync(
                certificationName, certificationNameSearchType, score, scoreSearchType, issuedDate, issuedDateSearchType, expiredDate, expiredDateSearchType, isPrimary, isPrimarySearchType, remarks, remarksSearchType, 
                employeeGuid, employeeLanguageGuid);
            return data.Adapt<IEnumerable<EmployeeLanguageDto>>();
        }

        public async Task<IEnumerable<EmployeeLanguageDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            var result = await _repository.EmployeeLanguage.GetAllByEmployeeGuidAsync(employeeGuid);
            return result.Adapt<IEnumerable<EmployeeLanguageDto>>();
        }

        public async Task<EmployeeLanguageDto?> GetByEmployeeGuidAndEmployeeLanguageGuidAsync(Guid employeeGuid, Guid employeeLanguageGuid)
        {
            var result = await _repository.EmployeeLanguage.GetByEmployeeGuidAndEmployeeLanguageGuidAsync(employeeGuid, employeeLanguageGuid);
            if (result == null) return null;
            return result.Adapt<EmployeeLanguageDto>();
        }
    }
}
