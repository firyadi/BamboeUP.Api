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
    public partial class HospitalService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IHospitalService
    {
        public async Task<IEnumerable<HospitalDto>> GetAllHospitalsAsync(bool trackChanges)
        {
            var entities = await repository.Hospital.GetAllHospitalsAsync(trackChanges);
            return entities.Adapt<IEnumerable<HospitalDto>>();
        }

        public async Task<HospitalDto?> GetHospitalByGuidAsync(Guid hospitalGuid, bool trackChanges)
        {
            var entity = await repository.Hospital.GetHospitalAsync(hospitalGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<HospitalDto>();
        }

        public async Task<HospitalDto> CreateHospitalAsync(HospitalForCreationDto input)
        {
            var model = input.Adapt<Hospital>();

            if (model.HospitalGuid == Guid.Empty)
                model.HospitalGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.Hospital.CreateHospitalAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Hospital",
                RootEntityKey = model.HospitalGuid.ToString(),
                RootDisplayName = model.HospitalName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Hospital",
                        EntityKey = model.HospitalGuid.ToString(),
                        EntityDisplayName = model.HospitalName,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<HospitalDto>();
        }

        public async Task UpdateHospitalAsync(Guid hospitalGuid, HospitalForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.Hospital.GetHospitalAsync(hospitalGuid, false)
                ?? throw new KeyNotFoundException($"Hospital '{{hospitalGuid}}' was not found.");

            var model = input.Adapt<Hospital>();
            model.HospitalGuid = hospitalGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.Hospital.UpdateHospitalAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Hospital",
                RootEntityKey = model.HospitalGuid.ToString(),
                RootDisplayName = model.HospitalName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Hospital",
                        EntityKey = model.HospitalGuid.ToString(),
                        EntityDisplayName = model.HospitalName,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteHospitalAsync(Guid hospitalGuid, HospitalForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.Hospital.GetHospitalAsync(hospitalGuid, false);
            var model = new Hospital
            {
                HospitalGuid = hospitalGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Hospital.SoftDeleteHospitalAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Hospital",
                RootEntityKey = hospitalGuid.ToString(),
                RootDisplayName = oldEntity?.HospitalName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Hospital",
                        EntityKey = hospitalGuid.ToString(),
                        EntityDisplayName = oldEntity?.HospitalName,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteHospitalByAdminAsync(Guid hospitalGuid, bool trackChanges)
        {
            await repository.Hospital.DeleteHospitalAsync(hospitalGuid);
        }

        public async Task<IEnumerable<HospitalDto>> SearchHospitalAsync(
            string? hospitalName, string? hospitalNameSearchType, string? hospitalCode, string? hospitalCodeSearchType, string? shortName, string? shortNameSearchType, string? licenseNo, string? licenseNoSearchType, string? hospitalType, string? hospitalTypeSearchType, string? hospitalClass, string? hospitalClassSearchType, string? phoneNo, string? phoneNoSearchType, string? email, string? emailSearchType, string? website, string? websiteSearchType

            )
        {
            var data = await repository.Hospital.SearchHospitalAsync(
                hospitalName, hospitalNameSearchType, hospitalCode, hospitalCodeSearchType, shortName, shortNameSearchType, licenseNo, licenseNoSearchType, hospitalType, hospitalTypeSearchType, hospitalClass, hospitalClassSearchType, phoneNo, phoneNoSearchType, email, emailSearchType, website, websiteSearchType

                );
            return data.Adapt<IEnumerable<HospitalDto>>();
        }
    }
}
