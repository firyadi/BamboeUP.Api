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
    public partial class PersonEducationService : IPersonEducationService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public PersonEducationService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public PersonEducationService(
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

        public async Task<IEnumerable<PersonEducationDto>> GetAllPersonEducationsAsync(bool trackChanges)
        {
            var entities = await _repository.PersonEducation.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<PersonEducationDto>>();
        }

        public async Task<PersonEducationDto?> GetPersonEducationByGuidAsync(Guid personEducationGuid, bool trackChanges)
        {
            var entity = await _repository.PersonEducation.GetPersonEducationAsync(personEducationGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<PersonEducationDto>();
        }

        public async Task<PersonEducationDto> CreatePersonEducationAsync(Guid personGuid, PersonEducationForCreationDto input)
        {
            var model = input.Adapt<PersonEducation>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.PersonEducation.CreatePersonEducationAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PersonEducation",
                RootEntityKey = model.PersonEducationGuid.ToString(),
                RootDisplayName = model.InstitutionName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonEducation",
                        EntityKey = model.PersonEducationGuid.ToString(),
                        EntityDisplayName = model.InstitutionName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<PersonEducationDto>();
        }

        public async Task UpdatePersonEducationAsync(Guid personGuid, Guid personEducationGuid, PersonEducationForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonEducation.GetPersonEducationAsync(personEducationGuid, false);

            var model = input.Adapt<PersonEducation>();
            model.PersonEducationGuid = personEducationGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.PersonEducation.UpdatePersonEducationAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PersonEducation",
                RootEntityKey = model.PersonEducationGuid.ToString(),
                RootDisplayName = model.InstitutionName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonEducation",
                        EntityKey = model.PersonEducationGuid.ToString(),
                        EntityDisplayName = model.InstitutionName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeletePersonEducationAsync(Guid personGuid, Guid personEducationGuid, PersonEducationForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonEducation.GetPersonEducationAsync(personEducationGuid, false);
            var model = new PersonEducation
            {
                PersonEducationGuid = personEducationGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.PersonEducation.SoftDeletePersonEducationAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PersonEducation",
                RootEntityKey = personEducationGuid.ToString(),
                RootDisplayName = oldDetail?.InstitutionName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonEducation",
                        EntityKey = personEducationGuid.ToString(),
                        EntityDisplayName = oldDetail?.InstitutionName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeletePersonEducationByAdminAsync(Guid personEducationGuid, bool trackChanges)
        {
            await _repository.PersonEducation.DeletePersonEducationAsync(personEducationGuid);
        }

        public async Task<IEnumerable<PersonEducationDto>> SearchPersonEducationAsync(
            string? srEducationLevel, string? srEducationLevelSearchType, string? institutionName, string? institutionNameSearchType,
            Guid personGuid, Guid personEducationGuid)
        {
            var data = await _repository.PersonEducation.SearchPersonEducationAsync(
                srEducationLevel, srEducationLevelSearchType, institutionName, institutionNameSearchType, 
                personGuid, personEducationGuid);
            return data.Adapt<IEnumerable<PersonEducationDto>>();
        }

        public async Task<IEnumerable<PersonEducationDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.PersonEducation.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<PersonEducationDto>>();
        }

        public async Task<PersonEducationDto?> GetByPersonGuidAndPersonEducationGuidAsync(Guid personGuid, Guid personEducationGuid)
        {
            var result = await _repository.PersonEducation.GetByPersonGuidAndPersonEducationGuidAsync(personGuid, personEducationGuid);
            if (result == null) return null;
            return result.Adapt<PersonEducationDto>();
        }
    }
}
