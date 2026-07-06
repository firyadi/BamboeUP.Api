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
    public partial class HospitalService : IHospitalService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public HospitalService(
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

        public async Task<IEnumerable<HospitalDto>> GetAllHospitalsAsync(bool trackChanges)
        {
            var entities = await _repository.Hospital.GetAllHospitalsAsync(trackChanges);
            return entities.Adapt<IEnumerable<HospitalDto>>();
        }

        public async Task<HospitalDto> GetHospitalByGuidAsync(Guid hospitalGuid, bool trackChanges)
        {
            var entity = await _repository.Hospital.GetHospitalAsync(hospitalGuid, trackChanges);
            return entity.Adapt<HospitalDto>();
        }

        public async Task<HospitalDto> CreateHospitalAsync(HospitalForCreationDto input)
        {
            var model = input.Adapt<Hospital>();
            model.StatusId = 1;
            await _repository.Hospital.CreateHospitalAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Hospital",
                RootEntityKey = model.HospitalGuid.ToString(),
                RootDisplayName = model.HospitalName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Hospital",
                        EntityKey = model.HospitalGuid.ToString(),
                        EntityDisplayName = model.HospitalName,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<HospitalDto>();
        }

        public async Task UpdateHospitalAsync(Guid hospitalGuid, HospitalForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await _repository.Hospital.GetHospitalAsync(hospitalGuid, false);

            var model = input.Adapt<Hospital>();
            model.HospitalGuid = hospitalGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.Hospital.UpdateHospitalAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Hospital",
                RootEntityKey = model.HospitalGuid.ToString(),
                RootDisplayName = model.HospitalName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Hospital",
                        EntityKey = model.HospitalGuid.ToString(),
                        EntityDisplayName = model.HospitalName,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteHospitalAsync(Guid hospitalGuid, HospitalForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await _repository.Hospital.GetHospitalAsync(hospitalGuid, false);
            var model = new Hospital { HospitalGuid = hospitalGuid };
            await _repository.Hospital.SoftDeleteHospitalAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Hospital",
                RootEntityKey = hospitalGuid.ToString(),
                RootDisplayName = oldEntity?.HospitalName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Hospital",
                        EntityKey = hospitalGuid.ToString(),
                        EntityDisplayName = oldEntity?.HospitalName,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteHospitalByAdminAsync(Guid hospitalGuid, bool trackChanges)
        {
            await _repository.Hospital.DeleteHospitalAsync(hospitalGuid);
        }

        public async Task<IEnumerable<HospitalDto>> SearchHospitalAsync(
            string? hospitalName, string? hospitalNameSearchType, string? hospitalCode, string? hospitalCodeSearchType, string? shortName, string? shortNameSearchType, string? licenseNo, string? licenseNoSearchType, string? hospitalType, string? hospitalTypeSearchType, string? hospitalClass, string? hospitalClassSearchType, string? phoneNo, string? phoneNoSearchType, string? email, string? emailSearchType, string? website, string? websiteSearchType
            )
        {
            var data = await _repository.Hospital.SearchHospitalAsync(
                hospitalName, hospitalNameSearchType, hospitalCode, hospitalCodeSearchType, shortName, shortNameSearchType, licenseNo, licenseNoSearchType, hospitalType, hospitalTypeSearchType, hospitalClass, hospitalClassSearchType, phoneNo, phoneNoSearchType, email, emailSearchType, website, websiteSearchType
                );
            return data.Adapt<IEnumerable<HospitalDto>>();
        }
    }
}
