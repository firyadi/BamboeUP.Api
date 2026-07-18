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
    public partial class PersonContactService : IPersonContactService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public PersonContactService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public PersonContactService(
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

        public async Task<IEnumerable<PersonContactDto>> GetAllPersonContactsAsync(bool trackChanges)
        {
            var entities = await _repository.PersonContact.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<PersonContactDto>>();
        }

        public async Task<PersonContactDto?> GetPersonContactByGuidAsync(Guid personContactGuid, bool trackChanges)
        {
            var entity = await _repository.PersonContact.GetPersonContactAsync(personContactGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<PersonContactDto>();
        }

        public async Task<PersonContactDto> CreatePersonContactAsync(Guid personGuid, PersonContactForCreationDto input)
        {
            var model = input.Adapt<PersonContact>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.PersonContact.CreatePersonContactAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PersonContact",
                RootEntityKey = model.PersonContactGuid.ToString(),
                RootDisplayName = model.ContactValue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonContact",
                        EntityKey = model.PersonContactGuid.ToString(),
                        EntityDisplayName = model.ContactValue,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<PersonContactDto>();
        }

        public async Task UpdatePersonContactAsync(Guid personGuid, Guid personContactGuid, PersonContactForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonContact.GetPersonContactAsync(personContactGuid, false);

            var model = input.Adapt<PersonContact>();
            model.PersonContactGuid = personContactGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.PersonContact.UpdatePersonContactAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PersonContact",
                RootEntityKey = model.PersonContactGuid.ToString(),
                RootDisplayName = model.ContactValue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonContact",
                        EntityKey = model.PersonContactGuid.ToString(),
                        EntityDisplayName = model.ContactValue,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeletePersonContactAsync(Guid personGuid, Guid personContactGuid, PersonContactForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonContact.GetPersonContactAsync(personContactGuid, false);
            var model = new PersonContact
            {
                PersonContactGuid = personContactGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.PersonContact.SoftDeletePersonContactAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PersonContact",
                RootEntityKey = personContactGuid.ToString(),
                RootDisplayName = oldDetail?.ContactValue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonContact",
                        EntityKey = personContactGuid.ToString(),
                        EntityDisplayName = oldDetail?.ContactValue,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeletePersonContactByAdminAsync(Guid personContactGuid, bool trackChanges)
        {
            await _repository.PersonContact.DeletePersonContactAsync(personContactGuid);
        }

        public async Task<IEnumerable<PersonContactDto>> SearchPersonContactAsync(
            string? srContactType, string? srContactTypeSearchType, string? contactValue, string? contactValueSearchType, string? isPrimary, string? isPrimarySearchType, string? remark, string? remarkSearchType,
            Guid personGuid, Guid personContactGuid)
        {
            var data = await _repository.PersonContact.SearchPersonContactAsync(
                srContactType, srContactTypeSearchType, contactValue, contactValueSearchType, isPrimary, isPrimarySearchType, remark, remarkSearchType, 
                personGuid, personContactGuid);
            return data.Adapt<IEnumerable<PersonContactDto>>();
        }

        public async Task<IEnumerable<PersonContactDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.PersonContact.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<PersonContactDto>>();
        }

        public async Task<PersonContactDto?> GetByPersonGuidAndPersonContactGuidAsync(Guid personGuid, Guid personContactGuid)
        {
            var result = await _repository.PersonContact.GetByPersonGuidAndPersonContactGuidAsync(personGuid, personContactGuid);
            if (result == null) return null;
            return result.Adapt<PersonContactDto>();
        }
    }
}
