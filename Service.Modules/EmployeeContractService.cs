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
    public partial class EmployeeContractService : IEmployeeContractService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;
        private readonly IFileStorageService _fileStorageService;

        public EmployeeContractService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
            _fileStorageService = null!;
        }

        public EmployeeContractService(
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

        public async Task<IEnumerable<EmployeeContractDto>> GetAllEmployeeContractsAsync(bool trackChanges)
        {
            var entities = await _repository.EmployeeContract.GetAllByEmployeeGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<EmployeeContractDto>>();
        }

        public async Task<EmployeeContractDto?> GetEmployeeContractByGuidAsync(Guid employeeContractGuid, bool trackChanges)
        {
            var entity = await _repository.EmployeeContract.GetEmployeeContractAsync(employeeContractGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<EmployeeContractDto>();
            if (entity.FileStorageId is > 0)
            {
                var file = await _fileStorageService.GetFileStorageByIdAsync(entity.FileStorageId.Value, false);
                dto.FileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<EmployeeContractDto> CreateEmployeeContractAsync(Guid employeeGuid, EmployeeContractForCreationDto input)
        {
            var model = input.Adapt<EmployeeContract>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.EmployeeContract.CreateEmployeeContractAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "EmployeeContract",
                RootEntityKey = model.EmployeeContractGuid.ToString(),
                RootDisplayName = model.ContractNo,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeContract",
                        EntityKey = model.EmployeeContractGuid.ToString(),
                        EntityDisplayName = model.ContractNo,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<EmployeeContractDto>();
        }

        public async Task UpdateEmployeeContractAsync(Guid employeeGuid, Guid employeeContractGuid, EmployeeContractForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeContract.GetEmployeeContractAsync(employeeContractGuid, false);

            var model = input.Adapt<EmployeeContract>();
            model.EmployeeContractGuid = employeeContractGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.EmployeeContract.UpdateEmployeeContractAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "EmployeeContract",
                RootEntityKey = model.EmployeeContractGuid.ToString(),
                RootDisplayName = model.ContractNo,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeContract",
                        EntityKey = model.EmployeeContractGuid.ToString(),
                        EntityDisplayName = model.ContractNo,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteEmployeeContractAsync(Guid employeeGuid, Guid employeeContractGuid, EmployeeContractForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.EmployeeContract.GetEmployeeContractAsync(employeeContractGuid, false);
            var model = new EmployeeContract
            {
                EmployeeContractGuid = employeeContractGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.EmployeeContract.SoftDeleteEmployeeContractAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "EmployeeContract",
                RootEntityKey = employeeContractGuid.ToString(),
                RootDisplayName = oldDetail?.ContractNo,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "EmployeeContract",
                        EntityKey = employeeContractGuid.ToString(),
                        EntityDisplayName = oldDetail?.ContractNo,
                        ParentTableName = "Employee",
                        ParentEntityKey = employeeGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteEmployeeContractByAdminAsync(Guid employeeContractGuid, bool trackChanges)
        {
            await _repository.EmployeeContract.DeleteEmployeeContractAsync(employeeContractGuid);
        }

        public async Task<IEnumerable<EmployeeContractDto>> SearchEmployeeContractAsync(
            string? contractNo, string? contractNoSearchType, string? contractStartDate, string? contractStartDateSearchType, string? contractEndDate, string? contractEndDateSearchType, string? effectiveDate, string? effectiveDateSearchType, string? renewalSequence, string? renewalSequenceSearchType, string? isCurrent, string? isCurrentSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeContractGuid)
        {
            var data = await _repository.EmployeeContract.SearchEmployeeContractAsync(
                contractNo, contractNoSearchType, contractStartDate, contractStartDateSearchType, contractEndDate, contractEndDateSearchType, effectiveDate, effectiveDateSearchType, renewalSequence, renewalSequenceSearchType, isCurrent, isCurrentSearchType, remarks, remarksSearchType, 
                employeeGuid, employeeContractGuid);
            return data.Adapt<IEnumerable<EmployeeContractDto>>();
        }

        public async Task<IEnumerable<EmployeeContractDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            var result = await _repository.EmployeeContract.GetAllByEmployeeGuidAsync(employeeGuid);
            return result.Adapt<IEnumerable<EmployeeContractDto>>();
        }

        public async Task<EmployeeContractDto?> GetByEmployeeGuidAndEmployeeContractGuidAsync(Guid employeeGuid, Guid employeeContractGuid)
        {
            var result = await _repository.EmployeeContract.GetByEmployeeGuidAndEmployeeContractGuidAsync(employeeGuid, employeeContractGuid);
            if (result == null) return null;
            return result.Adapt<EmployeeContractDto>();
        }
    }
}
