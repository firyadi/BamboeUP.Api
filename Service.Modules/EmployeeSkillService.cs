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
    public partial class EmployeeSkillService : IEmployeeSkillService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;
        private readonly IFileStorageService _fileStorageService;

        public EmployeeSkillService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
            _fileStorageService = null!;
        }

        public EmployeeSkillService(
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

        public async Task<IEnumerable<EmployeeSkillDto>> GetAllEmployeeSkillsAsync(bool trackChanges)
        {
            var entities = await _repository.EmployeeSkill.GetAllByEmployeeGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<EmployeeSkillDto>>();
        }

        public async Task<EmployeeSkillDto?> GetEmployeeSkillByGuidAsync(Guid employeeSkillGuid, bool trackChanges)
        {
            var entity = await _repository.EmployeeSkill.GetEmployeeSkillAsync(employeeSkillGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<EmployeeSkillDto>();
            if (entity.FileStorageId is > 0)
            {
                var file = await _fileStorageService.GetFileStorageByIdAsync(entity.FileStorageId.Value, false);
                dto.FileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<EmployeeSkillDto> CreateEmployeeSkillAsync(Guid employeeGuid, EmployeeSkillForCreationDto input)
        {
            var model = input.Adapt<EmployeeSkill>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.EmployeeSkill.CreateEmployeeSkillAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "EmployeeSkill",
                RootEntityKey = model.EmployeeSkillGuid.ToString(),
                RootDisplayName = model.CertificateNumber,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeSkill",
                        EntityKey = model.EmployeeSkillGuid.ToString(),
                        EntityDisplayName = model.CertificateNumber,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<EmployeeSkillDto>();
        }

        public async Task UpdateEmployeeSkillAsync(Guid employeeGuid, Guid employeeSkillGuid, EmployeeSkillForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeSkill.GetEmployeeSkillAsync(employeeSkillGuid, false);

            var model = input.Adapt<EmployeeSkill>();
            model.EmployeeSkillGuid = employeeSkillGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.EmployeeSkill.UpdateEmployeeSkillAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "EmployeeSkill",
                RootEntityKey = model.EmployeeSkillGuid.ToString(),
                RootDisplayName = model.CertificateNumber,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeSkill",
                        EntityKey = model.EmployeeSkillGuid.ToString(),
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

        public async Task DeleteEmployeeSkillAsync(Guid employeeGuid, Guid employeeSkillGuid, EmployeeSkillForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeSkill.GetEmployeeSkillAsync(employeeSkillGuid, false);
            var model = new EmployeeSkill
            {
                EmployeeSkillGuid = employeeSkillGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.EmployeeSkill.SoftDeleteEmployeeSkillAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "EmployeeSkill",
                RootEntityKey = employeeSkillGuid.ToString(),
                RootDisplayName = oldDetail?.CertificateNumber,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeSkill",
                        EntityKey = employeeSkillGuid.ToString(),
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

        public async Task DeleteEmployeeSkillByAdminAsync(Guid employeeSkillGuid, bool trackChanges)
        {
            await _repository.EmployeeSkill.DeleteEmployeeSkillAsync(employeeSkillGuid);
        }

        public async Task<IEnumerable<EmployeeSkillDto>> SearchEmployeeSkillAsync(
            string? yearsofExperience, string? yearsofExperienceSearchType, string? lastUsedDate, string? lastUsedDateSearchType, string? isPrimary, string? isPrimarySearchType, string? isCertified, string? isCertifiedSearchType, string? certificateNumber, string? certificateNumberSearchType, string? certificateIssueDate, string? certificateIssueDateSearchType, string? certificateExpiredDate, string? certificateExpiredDateSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeSkillGuid)
        {
            var data = await _repository.EmployeeSkill.SearchEmployeeSkillAsync(
                yearsofExperience, yearsofExperienceSearchType, lastUsedDate, lastUsedDateSearchType, isPrimary, isPrimarySearchType, isCertified, isCertifiedSearchType, certificateNumber, certificateNumberSearchType, certificateIssueDate, certificateIssueDateSearchType, certificateExpiredDate, certificateExpiredDateSearchType, remarks, remarksSearchType, 
                employeeGuid, employeeSkillGuid);
            return data.Adapt<IEnumerable<EmployeeSkillDto>>();
        }

        public async Task<IEnumerable<EmployeeSkillDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            var result = await _repository.EmployeeSkill.GetAllByEmployeeGuidAsync(employeeGuid);
            return result.Adapt<IEnumerable<EmployeeSkillDto>>();
        }

        public async Task<EmployeeSkillDto?> GetByEmployeeGuidAndEmployeeSkillGuidAsync(Guid employeeGuid, Guid employeeSkillGuid)
        {
            var result = await _repository.EmployeeSkill.GetByEmployeeGuidAndEmployeeSkillGuidAsync(employeeGuid, employeeSkillGuid);
            if (result == null) return null;
            return result.Adapt<EmployeeSkillDto>();
        }
    }
}
