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
    public partial class PersonFamilyService : IPersonFamilyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public PersonFamilyService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public PersonFamilyService(
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

        public async Task<IEnumerable<PersonFamilyDto>> GetAllPersonFamiliesAsync(bool trackChanges)
        {
            var entities = await _repository.PersonFamily.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<PersonFamilyDto>>();
        }

        public async Task<PersonFamilyDto?> GetPersonFamilyByGuidAsync(Guid personFamilyGuid, bool trackChanges)
        {
            var entity = await _repository.PersonFamily.GetPersonFamilyAsync(personFamilyGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<PersonFamilyDto>();
        }

        public async Task<PersonFamilyDto> CreatePersonFamilyAsync(Guid personGuid, PersonFamilyForCreationDto input)
        {
            var model = input.Adapt<PersonFamily>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.PersonFamily.CreatePersonFamilyAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PersonFamily",
                RootEntityKey = model.PersonFamilyGuid.ToString(),
                RootDisplayName = model.FamilyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonFamily",
                        EntityKey = model.PersonFamilyGuid.ToString(),
                        EntityDisplayName = model.FamilyName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<PersonFamilyDto>();
        }

        public async Task UpdatePersonFamilyAsync(Guid personGuid, Guid personFamilyGuid, PersonFamilyForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonFamily.GetPersonFamilyAsync(personFamilyGuid, false);

            var model = input.Adapt<PersonFamily>();
            model.PersonFamilyGuid = personFamilyGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.PersonFamily.UpdatePersonFamilyAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PersonFamily",
                RootEntityKey = model.PersonFamilyGuid.ToString(),
                RootDisplayName = model.FamilyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonFamily",
                        EntityKey = model.PersonFamilyGuid.ToString(),
                        EntityDisplayName = model.FamilyName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeletePersonFamilyAsync(Guid personGuid, Guid personFamilyGuid, PersonFamilyForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonFamily.GetPersonFamilyAsync(personFamilyGuid, false);
            var model = new PersonFamily
            {
                PersonFamilyGuid = personFamilyGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.PersonFamily.SoftDeletePersonFamilyAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PersonFamily",
                RootEntityKey = personFamilyGuid.ToString(),
                RootDisplayName = oldDetail?.FamilyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonFamily",
                        EntityKey = personFamilyGuid.ToString(),
                        EntityDisplayName = oldDetail?.FamilyName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeletePersonFamilyByAdminAsync(Guid personFamilyGuid, bool trackChanges)
        {
            await _repository.PersonFamily.DeletePersonFamilyAsync(personFamilyGuid);
        }

        public async Task<IEnumerable<PersonFamilyDto>> SearchPersonFamilyAsync(
            string? srFamilyRelation, string? srFamilyRelationSearchType, string? familyName, string? familyNameSearchType, string? dateBirth, string? dateBirthSearchType, string? srEducationLevel, string? srEducationLevelSearchType, string? address, string? addressSearchType, string? stateId, string? stateIdSearchType, string? cityId, string? cityIdSearchType, string? zipCode, string? zipCodeSearchType, string? phone, string? phoneSearchType, string? srMaritalStatus, string? srMaritalStatusSearchType, string? srGender, string? srGenderSearchType,
            Guid personGuid, Guid personFamilyGuid)
        {
            var data = await _repository.PersonFamily.SearchPersonFamilyAsync(
                srFamilyRelation, srFamilyRelationSearchType, familyName, familyNameSearchType, dateBirth, dateBirthSearchType, srEducationLevel, srEducationLevelSearchType, address, addressSearchType, stateId, stateIdSearchType, cityId, cityIdSearchType, zipCode, zipCodeSearchType, phone, phoneSearchType, srMaritalStatus, srMaritalStatusSearchType, srGender, srGenderSearchType, 
                personGuid, personFamilyGuid);
            return data.Adapt<IEnumerable<PersonFamilyDto>>();
        }

        public async Task<IEnumerable<PersonFamilyDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.PersonFamily.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<PersonFamilyDto>>();
        }

        public async Task<PersonFamilyDto?> GetByPersonGuidAndPersonFamilyGuidAsync(Guid personGuid, Guid personFamilyGuid)
        {
            var result = await _repository.PersonFamily.GetByPersonGuidAndPersonFamilyGuidAsync(personGuid, personFamilyGuid);
            if (result == null) return null;
            return result.Adapt<PersonFamilyDto>();
        }
    }
}
