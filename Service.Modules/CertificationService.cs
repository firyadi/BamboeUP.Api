using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class CertificationService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext,
        IFileStorageService fileStorageService) : ICertificationService
    {
        public async Task<IEnumerable<CertificationDto>> GetAllCertificationsAsync(bool trackChanges)
        {
            var entities = await repository.Certification.GetAllCertificationsAsync(trackChanges);
            return entities.Adapt<IEnumerable<CertificationDto>>();
        }

        public async Task<CertificationDto?> GetCertificationByGuidAsync(Guid certificationGuid, bool trackChanges)
        {
            var entity = await repository.Certification.GetCertificationAsync(certificationGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<CertificationDto>();
            if (entity.LogoFileStorageId is > 0)
            {
                var file = await fileStorageService.GetFileStorageByIdAsync(entity.LogoFileStorageId.Value, false);
                dto.LogoFileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<CertificationDto> CreateCertificationAsync(CertificationForCreationDto input)
        {
            var model = input.Adapt<Certification>();

            if (model.CertificationGuid == Guid.Empty)
                model.CertificationGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.Certification.CreateCertificationAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Certification",
                RootEntityKey = model.CertificationGuid.ToString(),
                RootDisplayName = model.CertificationCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Certification",
                        EntityKey = model.CertificationGuid.ToString(),
                        EntityDisplayName = model.CertificationCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<CertificationDto>();
        }

        public async Task UpdateCertificationAsync(Guid certificationGuid, CertificationForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.Certification.GetCertificationAsync(certificationGuid, false)
                ?? throw new KeyNotFoundException($"Certification '{{certificationGuid}}' was not found.");

            var model = input.Adapt<Certification>();
            model.CertificationGuid = certificationGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.Certification.UpdateCertificationAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Certification",
                RootEntityKey = model.CertificationGuid.ToString(),
                RootDisplayName = model.CertificationCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Certification",
                        EntityKey = model.CertificationGuid.ToString(),
                        EntityDisplayName = model.CertificationCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteCertificationAsync(Guid certificationGuid, CertificationForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.Certification.GetCertificationAsync(certificationGuid, false);
            var model = new Certification
            {
                CertificationGuid = certificationGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Certification.SoftDeleteCertificationAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Certification",
                RootEntityKey = certificationGuid.ToString(),
                RootDisplayName = oldEntity?.CertificationCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Certification",
                        EntityKey = certificationGuid.ToString(),
                        EntityDisplayName = oldEntity?.CertificationCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteCertificationByAdminAsync(Guid certificationGuid, bool trackChanges)
        {
            await repository.Certification.DeleteCertificationAsync(certificationGuid);
        }

        public async Task<IEnumerable<CertificationDto>> SearchCertificationAsync(
            string? certificationCode, string? certificationCodeSearchType, string? certificationName, string? certificationNameSearchType, string? certificationShortName, string? certificationShortNameSearchType, string? defaultValidityMonth, string? defaultValidityMonthSearchType, string? isLifetime, string? isLifetimeSearchType, string? isRenewable, string? isRenewableSearchType, string? description, string? descriptionSearchType, string? isActive, string? isActiveSearchType
, string? certificationCategoryName, string? certificationCategoryNameSearchType
, string? certificationProviderName, string? certificationProviderNameSearchType

            )
        {
            var data = await repository.Certification.SearchCertificationAsync(
                certificationCode, certificationCodeSearchType, certificationName, certificationNameSearchType, certificationShortName, certificationShortNameSearchType, defaultValidityMonth, defaultValidityMonthSearchType, isLifetime, isLifetimeSearchType, isRenewable, isRenewableSearchType, description, descriptionSearchType, isActive, isActiveSearchType
, certificationCategoryName, certificationCategoryNameSearchType
, certificationProviderName, certificationProviderNameSearchType

                );
            return data.Adapt<IEnumerable<CertificationDto>>();
        }
    }
}
