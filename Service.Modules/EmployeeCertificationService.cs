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
    public partial class EmployeeCertificationService : IEmployeeCertificationService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;
        private readonly IFileStorageService _fileStorageService;

        public EmployeeCertificationService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
            _fileStorageService = null!;
        }

        public EmployeeCertificationService(
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

        public async Task<IEnumerable<EmployeeCertificationDto>> GetAllEmployeeCertificationsAsync(bool trackChanges)
        {
            var entities = await _repository.EmployeeCertification.GetAllByEmployeeGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<EmployeeCertificationDto>>();
        }

        public async Task<EmployeeCertificationDto?> GetEmployeeCertificationByGuidAsync(Guid employeeCertificationGuid, bool trackChanges)
        {
            var entity = await _repository.EmployeeCertification.GetEmployeeCertificationAsync(employeeCertificationGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<EmployeeCertificationDto>();
            if (entity.FileStorageId is > 0)
            {
                var file = await _fileStorageService.GetFileStorageByIdAsync(entity.FileStorageId.Value, false);
                dto.FileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<EmployeeCertificationDto> CreateEmployeeCertificationAsync(Guid employeeGuid, EmployeeCertificationForCreationDto input)
        {
            var model = input.Adapt<EmployeeCertification>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.EmployeeCertification.CreateEmployeeCertificationAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "EmployeeCertification",
                RootEntityKey = model.EmployeeCertificationGuid.ToString(),
                RootDisplayName = model.CertificateNumber,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeCertification",
                        EntityKey = model.EmployeeCertificationGuid.ToString(),
                        EntityDisplayName = model.CertificateNumber,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<EmployeeCertificationDto>();
        }

        public async Task UpdateEmployeeCertificationAsync(Guid employeeGuid, Guid employeeCertificationGuid, EmployeeCertificationForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeCertification.GetEmployeeCertificationAsync(employeeCertificationGuid, false);

            var model = input.Adapt<EmployeeCertification>();
            model.EmployeeCertificationGuid = employeeCertificationGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.EmployeeCertification.UpdateEmployeeCertificationAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "EmployeeCertification",
                RootEntityKey = model.EmployeeCertificationGuid.ToString(),
                RootDisplayName = model.CertificateNumber,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeCertification",
                        EntityKey = model.EmployeeCertificationGuid.ToString(),
                        EntityDisplayName = model.CertificateNumber,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteEmployeeCertificationAsync(Guid employeeGuid, Guid employeeCertificationGuid, EmployeeCertificationForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeCertification.GetEmployeeCertificationAsync(employeeCertificationGuid, false);
            var model = new EmployeeCertification
            {
                EmployeeCertificationGuid = employeeCertificationGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.EmployeeCertification.SoftDeleteEmployeeCertificationAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "EmployeeCertification",
                RootEntityKey = employeeCertificationGuid.ToString(),
                RootDisplayName = oldDetail?.CertificateNumber,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeCertification",
                        EntityKey = employeeCertificationGuid.ToString(),
                        EntityDisplayName = oldDetail?.CertificateNumber,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteEmployeeCertificationByAdminAsync(Guid employeeCertificationGuid, bool trackChanges)
        {
            await _repository.EmployeeCertification.DeleteEmployeeCertificationAsync(employeeCertificationGuid);
        }

        public async Task<IEnumerable<EmployeeCertificationDto>> SearchEmployeeCertificationAsync(
            string? certificateNumber, string? certificateNumberSearchType, string? issuedDate, string? issuedDateSearchType, string? expiredDate, string? expiredDateSearchType, string? issuedby, string? issuedbySearchType, string? score, string? scoreSearchType, string? isLifetime, string? isLifetimeSearchType, string? isCurrent, string? isCurrentSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeCertificationGuid)
        {
            var data = await _repository.EmployeeCertification.SearchEmployeeCertificationAsync(
                certificateNumber, certificateNumberSearchType, issuedDate, issuedDateSearchType, expiredDate, expiredDateSearchType, issuedby, issuedbySearchType, score, scoreSearchType, isLifetime, isLifetimeSearchType, isCurrent, isCurrentSearchType, remarks, remarksSearchType, 
                employeeGuid, employeeCertificationGuid);
            return data.Adapt<IEnumerable<EmployeeCertificationDto>>();
        }

        public async Task<IEnumerable<EmployeeCertificationDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            var result = await _repository.EmployeeCertification.GetAllByEmployeeGuidAsync(employeeGuid);
            return result.Adapt<IEnumerable<EmployeeCertificationDto>>();
        }

        public async Task<EmployeeCertificationDto?> GetByEmployeeGuidAndEmployeeCertificationGuidAsync(Guid employeeGuid, Guid employeeCertificationGuid)
        {
            var result = await _repository.EmployeeCertification.GetByEmployeeGuidAndEmployeeCertificationGuidAsync(employeeGuid, employeeCertificationGuid);
            if (result == null) return null;
            return result.Adapt<EmployeeCertificationDto>();
        }
    }
}
