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
    public partial class PersonIdentificationService : IPersonIdentificationService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public PersonIdentificationService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public PersonIdentificationService(
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

        public async Task<IEnumerable<PersonIdentificationDto>> GetAllPersonIdentificationsAsync(bool trackChanges)
        {
            var entities = await _repository.PersonIdentification.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<PersonIdentificationDto>>();
        }

        public async Task<PersonIdentificationDto?> GetPersonIdentificationByGuidAsync(Guid personIdentificationGuid, bool trackChanges)
        {
            var entity = await _repository.PersonIdentification.GetPersonIdentificationAsync(personIdentificationGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<PersonIdentificationDto>();
        }

        public async Task<PersonIdentificationDto> CreatePersonIdentificationAsync(Guid personGuid, PersonIdentificationForCreationDto input)
        {
            var model = input.Adapt<PersonIdentification>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.PersonIdentification.CreatePersonIdentificationAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PersonIdentification",
                RootEntityKey = model.PersonIdentificationGuid.ToString(),
                RootDisplayName = model.IdentificationValue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonIdentification",
                        EntityKey = model.PersonIdentificationGuid.ToString(),
                        EntityDisplayName = model.IdentificationValue,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<PersonIdentificationDto>();
        }

        public async Task UpdatePersonIdentificationAsync(Guid personGuid, Guid personIdentificationGuid, PersonIdentificationForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonIdentification.GetPersonIdentificationAsync(personIdentificationGuid, false);

            var model = input.Adapt<PersonIdentification>();
            model.PersonIdentificationGuid = personIdentificationGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.PersonIdentification.UpdatePersonIdentificationAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PersonIdentification",
                RootEntityKey = model.PersonIdentificationGuid.ToString(),
                RootDisplayName = model.IdentificationValue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonIdentification",
                        EntityKey = model.PersonIdentificationGuid.ToString(),
                        EntityDisplayName = model.IdentificationValue,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeletePersonIdentificationAsync(Guid personGuid, Guid personIdentificationGuid, PersonIdentificationForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonIdentification.GetPersonIdentificationAsync(personIdentificationGuid, false);
            var model = new PersonIdentification
            {
                PersonIdentificationGuid = personIdentificationGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.PersonIdentification.SoftDeletePersonIdentificationAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PersonIdentification",
                RootEntityKey = personIdentificationGuid.ToString(),
                RootDisplayName = oldDetail?.IdentificationValue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonIdentification",
                        EntityKey = personIdentificationGuid.ToString(),
                        EntityDisplayName = oldDetail?.IdentificationValue,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeletePersonIdentificationByAdminAsync(Guid personIdentificationGuid, bool trackChanges)
        {
            await _repository.PersonIdentification.DeletePersonIdentificationAsync(personIdentificationGuid);
        }

        public async Task<IEnumerable<PersonIdentificationDto>> SearchPersonIdentificationAsync(
            string? srIdentificationTypeId, string? srIdentificationTypeIdSearchType, string? identificationValue, string? identificationValueSearchType,
            Guid personGuid, Guid personIdentificationGuid)
        {
            var data = await _repository.PersonIdentification.SearchPersonIdentificationAsync(
                srIdentificationTypeId, srIdentificationTypeIdSearchType, identificationValue, identificationValueSearchType, 
                personGuid, personIdentificationGuid);
            return data.Adapt<IEnumerable<PersonIdentificationDto>>();
        }

        public async Task<IEnumerable<PersonIdentificationDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.PersonIdentification.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<PersonIdentificationDto>>();
        }

        public async Task<PersonIdentificationDto?> GetByPersonGuidAndPersonIdentificationGuidAsync(Guid personGuid, Guid personIdentificationGuid)
        {
            var result = await _repository.PersonIdentification.GetByPersonGuidAndPersonIdentificationGuidAsync(personGuid, personIdentificationGuid);
            if (result == null) return null;
            return result.Adapt<PersonIdentificationDto>();
        }
    }
}
