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
    public partial class PersonPhysicalCharacteristicService : IPersonPhysicalCharacteristicService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public PersonPhysicalCharacteristicService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public PersonPhysicalCharacteristicService(
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

        public async Task<IEnumerable<PersonPhysicalCharacteristicDto>> GetAllPersonPhysicalCharacteristicsAsync(bool trackChanges)
        {
            var entities = await _repository.PersonPhysicalCharacteristic.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<PersonPhysicalCharacteristicDto>>();
        }

        public async Task<PersonPhysicalCharacteristicDto?> GetPersonPhysicalCharacteristicByGuidAsync(Guid personPhysicalCharacteristicGuid, bool trackChanges)
        {
            var entity = await _repository.PersonPhysicalCharacteristic.GetPersonPhysicalCharacteristicAsync(personPhysicalCharacteristicGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<PersonPhysicalCharacteristicDto>();
        }

        public async Task<PersonPhysicalCharacteristicDto> CreatePersonPhysicalCharacteristicAsync(Guid personGuid, PersonPhysicalCharacteristicForCreationDto input)
        {
            var model = input.Adapt<PersonPhysicalCharacteristic>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.PersonPhysicalCharacteristic.CreatePersonPhysicalCharacteristicAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PersonPhysicalCharacteristic",
                RootEntityKey = model.PersonPhysicalCharacteristicGuid.ToString(),
                RootDisplayName = model.PhysicalValue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonPhysicalCharacteristic",
                        EntityKey = model.PersonPhysicalCharacteristicGuid.ToString(),
                        EntityDisplayName = model.PhysicalValue,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<PersonPhysicalCharacteristicDto>();
        }

        public async Task UpdatePersonPhysicalCharacteristicAsync(Guid personGuid, Guid personPhysicalCharacteristicGuid, PersonPhysicalCharacteristicForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonPhysicalCharacteristic.GetPersonPhysicalCharacteristicAsync(personPhysicalCharacteristicGuid, false);

            var model = input.Adapt<PersonPhysicalCharacteristic>();
            model.PersonPhysicalCharacteristicGuid = personPhysicalCharacteristicGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.PersonPhysicalCharacteristic.UpdatePersonPhysicalCharacteristicAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PersonPhysicalCharacteristic",
                RootEntityKey = model.PersonPhysicalCharacteristicGuid.ToString(),
                RootDisplayName = model.PhysicalValue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonPhysicalCharacteristic",
                        EntityKey = model.PersonPhysicalCharacteristicGuid.ToString(),
                        EntityDisplayName = model.PhysicalValue,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeletePersonPhysicalCharacteristicAsync(Guid personGuid, Guid personPhysicalCharacteristicGuid, PersonPhysicalCharacteristicForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonPhysicalCharacteristic.GetPersonPhysicalCharacteristicAsync(personPhysicalCharacteristicGuid, false);
            var model = new PersonPhysicalCharacteristic
            {
                PersonPhysicalCharacteristicGuid = personPhysicalCharacteristicGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.PersonPhysicalCharacteristic.SoftDeletePersonPhysicalCharacteristicAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PersonPhysicalCharacteristic",
                RootEntityKey = personPhysicalCharacteristicGuid.ToString(),
                RootDisplayName = oldDetail?.PhysicalValue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonPhysicalCharacteristic",
                        EntityKey = personPhysicalCharacteristicGuid.ToString(),
                        EntityDisplayName = oldDetail?.PhysicalValue,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeletePersonPhysicalCharacteristicByAdminAsync(Guid personPhysicalCharacteristicGuid, bool trackChanges)
        {
            await _repository.PersonPhysicalCharacteristic.DeletePersonPhysicalCharacteristicAsync(personPhysicalCharacteristicGuid);
        }

        public async Task<IEnumerable<PersonPhysicalCharacteristicDto>> SearchPersonPhysicalCharacteristicAsync(
            string? srPhysicalCharacteristic, string? srPhysicalCharacteristicSearchType, string? physicalValue, string? physicalValueSearchType, string? srMeasurementUnit, string? srMeasurementUnitSearchType, string? recordedDate, string? recordedDateSearchType, string? remarks, string? remarksSearchType,
            Guid personGuid, Guid personPhysicalCharacteristicGuid)
        {
            var data = await _repository.PersonPhysicalCharacteristic.SearchPersonPhysicalCharacteristicAsync(
                srPhysicalCharacteristic, srPhysicalCharacteristicSearchType, physicalValue, physicalValueSearchType, srMeasurementUnit, srMeasurementUnitSearchType, recordedDate, recordedDateSearchType, remarks, remarksSearchType, 
                personGuid, personPhysicalCharacteristicGuid);
            return data.Adapt<IEnumerable<PersonPhysicalCharacteristicDto>>();
        }

        public async Task<IEnumerable<PersonPhysicalCharacteristicDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.PersonPhysicalCharacteristic.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<PersonPhysicalCharacteristicDto>>();
        }

        public async Task<PersonPhysicalCharacteristicDto?> GetByPersonGuidAndPersonPhysicalCharacteristicGuidAsync(Guid personGuid, Guid personPhysicalCharacteristicGuid)
        {
            var result = await _repository.PersonPhysicalCharacteristic.GetByPersonGuidAndPersonPhysicalCharacteristicGuidAsync(personGuid, personPhysicalCharacteristicGuid);
            if (result == null) return null;
            return result.Adapt<PersonPhysicalCharacteristicDto>();
        }
    }
}
