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
    public partial class PersonAddressService : IPersonAddressService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public PersonAddressService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public PersonAddressService(
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

        public async Task<IEnumerable<PersonAddressDto>> GetAllPersonAddressesAsync(bool trackChanges)
        {
            var entities = await _repository.PersonAddress.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<PersonAddressDto>>();
        }

        public async Task<PersonAddressDto?> GetPersonAddressByGuidAsync(Guid personAddressGuid, bool trackChanges)
        {
            var entity = await _repository.PersonAddress.GetPersonAddressAsync(personAddressGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<PersonAddressDto>();
        }

        public async Task<PersonAddressDto> CreatePersonAddressAsync(Guid personGuid, PersonAddressForCreationDto input)
        {
            var model = input.Adapt<PersonAddress>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.PersonAddress.CreatePersonAddressAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PersonAddress",
                RootEntityKey = model.PersonAddressGuid.ToString(),
                RootDisplayName = model.Address,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonAddress",
                        EntityKey = model.PersonAddressGuid.ToString(),
                        EntityDisplayName = model.Address,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<PersonAddressDto>();
        }

        public async Task UpdatePersonAddressAsync(Guid personGuid, Guid personAddressGuid, PersonAddressForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonAddress.GetPersonAddressAsync(personAddressGuid, false);

            var model = input.Adapt<PersonAddress>();
            model.PersonAddressGuid = personAddressGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.PersonAddress.UpdatePersonAddressAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PersonAddress",
                RootEntityKey = model.PersonAddressGuid.ToString(),
                RootDisplayName = model.Address,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonAddress",
                        EntityKey = model.PersonAddressGuid.ToString(),
                        EntityDisplayName = model.Address,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeletePersonAddressAsync(Guid personGuid, Guid personAddressGuid, PersonAddressForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonAddress.GetPersonAddressAsync(personAddressGuid, false);
            var model = new PersonAddress
            {
                PersonAddressGuid = personAddressGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.PersonAddress.SoftDeletePersonAddressAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PersonAddress",
                RootEntityKey = personAddressGuid.ToString(),
                RootDisplayName = oldDetail?.Address,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonAddress",
                        EntityKey = personAddressGuid.ToString(),
                        EntityDisplayName = oldDetail?.Address,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeletePersonAddressByAdminAsync(Guid personAddressGuid, bool trackChanges)
        {
            await _repository.PersonAddress.DeletePersonAddressAsync(personAddressGuid);
        }

        public async Task<IEnumerable<PersonAddressDto>> SearchPersonAddressAsync(
            string? srAddressType, string? srAddressTypeSearchType, string? address, string? addressSearchType, string? countryId, string? countryIdSearchType, string? provinceId, string? provinceIdSearchType, string? cityId, string? cityIdSearchType,
            Guid personGuid, Guid personAddressGuid)
        {
            var data = await _repository.PersonAddress.SearchPersonAddressAsync(
                srAddressType, srAddressTypeSearchType, address, addressSearchType, countryId, countryIdSearchType, provinceId, provinceIdSearchType, cityId, cityIdSearchType, 
                personGuid, personAddressGuid);
            return data.Adapt<IEnumerable<PersonAddressDto>>();
        }

        public async Task<IEnumerable<PersonAddressDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.PersonAddress.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<PersonAddressDto>>();
        }

        public async Task<PersonAddressDto?> GetByPersonGuidAndPersonAddressGuidAsync(Guid personGuid, Guid personAddressGuid)
        {
            var result = await _repository.PersonAddress.GetByPersonGuidAndPersonAddressGuidAsync(personGuid, personAddressGuid);
            if (result == null) return null;
            return result.Adapt<PersonAddressDto>();
        }
    }
}
