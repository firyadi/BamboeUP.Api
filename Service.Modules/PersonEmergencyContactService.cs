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
    public partial class PersonEmergencyContactService : IPersonEmergencyContactService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public PersonEmergencyContactService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public PersonEmergencyContactService(
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

        public async Task<IEnumerable<PersonEmergencyContactDto>> GetAllPersonEmergencyContactsAsync(bool trackChanges)
        {
            var entities = await _repository.PersonEmergencyContact.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<PersonEmergencyContactDto>>();
        }

        public async Task<PersonEmergencyContactDto?> GetPersonEmergencyContactByGuidAsync(Guid personEmergencyContactGuid, bool trackChanges)
        {
            var entity = await _repository.PersonEmergencyContact.GetPersonEmergencyContactAsync(personEmergencyContactGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<PersonEmergencyContactDto>();
        }

        public async Task<PersonEmergencyContactDto> CreatePersonEmergencyContactAsync(Guid personGuid, PersonEmergencyContactForCreationDto input)
        {
            var model = input.Adapt<PersonEmergencyContact>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.PersonEmergencyContact.CreatePersonEmergencyContactAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PersonEmergencyContact",
                RootEntityKey = model.PersonEmergencyContactGuid.ToString(),
                RootDisplayName = model.ContactName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonEmergencyContact",
                        EntityKey = model.PersonEmergencyContactGuid.ToString(),
                        EntityDisplayName = model.ContactName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<PersonEmergencyContactDto>();
        }

        public async Task UpdatePersonEmergencyContactAsync(Guid personGuid, Guid personEmergencyContactGuid, PersonEmergencyContactForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonEmergencyContact.GetPersonEmergencyContactAsync(personEmergencyContactGuid, false);

            var model = input.Adapt<PersonEmergencyContact>();
            model.PersonEmergencyContactGuid = personEmergencyContactGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.PersonEmergencyContact.UpdatePersonEmergencyContactAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PersonEmergencyContact",
                RootEntityKey = model.PersonEmergencyContactGuid.ToString(),
                RootDisplayName = model.ContactName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonEmergencyContact",
                        EntityKey = model.PersonEmergencyContactGuid.ToString(),
                        EntityDisplayName = model.ContactName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeletePersonEmergencyContactAsync(Guid personGuid, Guid personEmergencyContactGuid, PersonEmergencyContactForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonEmergencyContact.GetPersonEmergencyContactAsync(personEmergencyContactGuid, false);
            var model = new PersonEmergencyContact
            {
                PersonEmergencyContactGuid = personEmergencyContactGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.PersonEmergencyContact.SoftDeletePersonEmergencyContactAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PersonEmergencyContact",
                RootEntityKey = personEmergencyContactGuid.ToString(),
                RootDisplayName = oldDetail?.ContactName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonEmergencyContact",
                        EntityKey = personEmergencyContactGuid.ToString(),
                        EntityDisplayName = oldDetail?.ContactName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeletePersonEmergencyContactByAdminAsync(Guid personEmergencyContactGuid, bool trackChanges)
        {
            await _repository.PersonEmergencyContact.DeletePersonEmergencyContactAsync(personEmergencyContactGuid);
        }

        public async Task<IEnumerable<PersonEmergencyContactDto>> SearchPersonEmergencyContactAsync(
            string? contactName, string? contactNameSearchType, string? srRelationship, string? srRelationshipSearchType, string? phone, string? phoneSearchType, string? isPrimary, string? isPrimarySearchType,
            Guid personGuid, Guid personEmergencyContactGuid)
        {
            var data = await _repository.PersonEmergencyContact.SearchPersonEmergencyContactAsync(
                contactName, contactNameSearchType, srRelationship, srRelationshipSearchType, phone, phoneSearchType, isPrimary, isPrimarySearchType, 
                personGuid, personEmergencyContactGuid);
            return data.Adapt<IEnumerable<PersonEmergencyContactDto>>();
        }

        public async Task<IEnumerable<PersonEmergencyContactDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.PersonEmergencyContact.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<PersonEmergencyContactDto>>();
        }

        public async Task<PersonEmergencyContactDto?> GetByPersonGuidAndPersonEmergencyContactGuidAsync(Guid personGuid, Guid personEmergencyContactGuid)
        {
            var result = await _repository.PersonEmergencyContact.GetByPersonGuidAndPersonEmergencyContactGuidAsync(personGuid, personEmergencyContactGuid);
            if (result == null) return null;
            return result.Adapt<PersonEmergencyContactDto>();
        }
    }
}
