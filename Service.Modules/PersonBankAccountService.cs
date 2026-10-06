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
    public partial class PersonBankAccountService : IPersonBankAccountService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public PersonBankAccountService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public PersonBankAccountService(
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

        public async Task<IEnumerable<PersonBankAccountDto>> GetAllPersonBankAccountsAsync(bool trackChanges)
        {
            var entities = await _repository.PersonBankAccount.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<PersonBankAccountDto>>();
        }

        public async Task<PersonBankAccountDto?> GetPersonBankAccountByGuidAsync(Guid personBankAccountGuid, bool trackChanges)
        {
            var entity = await _repository.PersonBankAccount.GetPersonBankAccountAsync(personBankAccountGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<PersonBankAccountDto>();
            return dto;
        }

        public async Task<PersonBankAccountDto> CreatePersonBankAccountAsync(Guid personGuid, PersonBankAccountForCreationDto input)
        {
            var model = input.Adapt<PersonBankAccount>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.PersonBankAccount.CreatePersonBankAccountAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PersonBankAccount",
                RootEntityKey = model.PersonBankAccountGuid.ToString(),
                RootDisplayName = model.BankBranch,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonBankAccount",
                        EntityKey = model.PersonBankAccountGuid.ToString(),
                        EntityDisplayName = model.BankBranch,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<PersonBankAccountDto>();
        }

        public async Task UpdatePersonBankAccountAsync(Guid personGuid, Guid personBankAccountGuid, PersonBankAccountForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonBankAccount.GetPersonBankAccountAsync(personBankAccountGuid, false);

            var model = input.Adapt<PersonBankAccount>();
            model.PersonBankAccountGuid = personBankAccountGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.PersonBankAccount.UpdatePersonBankAccountAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PersonBankAccount",
                RootEntityKey = model.PersonBankAccountGuid.ToString(),
                RootDisplayName = model.BankBranch,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonBankAccount",
                        EntityKey = model.PersonBankAccountGuid.ToString(),
                        EntityDisplayName = model.BankBranch,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeletePersonBankAccountAsync(Guid personGuid, Guid personBankAccountGuid, PersonBankAccountForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonBankAccount.GetPersonBankAccountAsync(personBankAccountGuid, false);
            var model = new PersonBankAccount
            {
                PersonBankAccountGuid = personBankAccountGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.PersonBankAccount.SoftDeletePersonBankAccountAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PersonBankAccount",
                RootEntityKey = personBankAccountGuid.ToString(),
                RootDisplayName = oldDetail?.BankBranch,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonBankAccount",
                        EntityKey = personBankAccountGuid.ToString(),
                        EntityDisplayName = oldDetail?.BankBranch,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeletePersonBankAccountByAdminAsync(Guid personBankAccountGuid, bool trackChanges)
        {
            await _repository.PersonBankAccount.DeletePersonBankAccountAsync(personBankAccountGuid);
        }

        public async Task<IEnumerable<PersonBankAccountDto>> SearchPersonBankAccountAsync(
            string? bankBranch, string? bankBranchSearchType, string? accountNumber, string? accountNumberSearchType, string? accountName, string? accountNameSearchType, string? swiftCode, string? swiftCodeSearchType, string? iBAN, string? iBANSearchType, string? isPrimary, string? isPrimarySearchType, string? isPayrollAccount, string? isPayrollAccountSearchType, string? isActive, string? isActiveSearchType, string? remarks, string? remarksSearchType,
            Guid personGuid, Guid personBankAccountGuid)
        {
            var data = await _repository.PersonBankAccount.SearchPersonBankAccountAsync(
                bankBranch, bankBranchSearchType, accountNumber, accountNumberSearchType, accountName, accountNameSearchType, swiftCode, swiftCodeSearchType, iBAN, iBANSearchType, isPrimary, isPrimarySearchType, isPayrollAccount, isPayrollAccountSearchType, isActive, isActiveSearchType, remarks, remarksSearchType, 
                personGuid, personBankAccountGuid);
            return data.Adapt<IEnumerable<PersonBankAccountDto>>();
        }

        public async Task<IEnumerable<PersonBankAccountDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.PersonBankAccount.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<PersonBankAccountDto>>();
        }

        public async Task<PersonBankAccountDto?> GetByPersonGuidAndPersonBankAccountGuidAsync(Guid personGuid, Guid personBankAccountGuid)
        {
            var result = await _repository.PersonBankAccount.GetByPersonGuidAndPersonBankAccountGuidAsync(personGuid, personBankAccountGuid);
            if (result == null) return null;
            return result.Adapt<PersonBankAccountDto>();
        }
    }
}
