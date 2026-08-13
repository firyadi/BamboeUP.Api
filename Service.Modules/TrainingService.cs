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
    public partial class TrainingService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext,
        IFileStorageService fileStorageService) : ITrainingService
    {
        public async Task<IEnumerable<TrainingDto>> GetAllTrainingAsync(bool trackChanges)
        {
            var entities = await repository.Training.GetAllTrainingAsync(trackChanges);
            return entities.Adapt<IEnumerable<TrainingDto>>();
        }

        public async Task<TrainingDto?> GetTrainingByGuidAsync(Guid trainingGuid, bool trackChanges)
        {
            var entity = await repository.Training.GetTrainingAsync(trainingGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<TrainingDto>();
            if (entity.FileStorageId is > 0)
            {
                var file = await fileStorageService.GetFileStorageByIdAsync(entity.FileStorageId.Value, false);
                dto.FileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<TrainingDto> CreateTrainingAsync(TrainingForCreationDto input)
        {
            var model = input.Adapt<Training>();

            if (model.TrainingGuid == Guid.Empty)
                model.TrainingGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.Training.CreateTrainingAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Training",
                RootEntityKey = model.TrainingGuid.ToString(),
                RootDisplayName = model.TrainingCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Training",
                        EntityKey = model.TrainingGuid.ToString(),
                        EntityDisplayName = model.TrainingCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<TrainingDto>();
        }

        public async Task UpdateTrainingAsync(Guid trainingGuid, TrainingForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.Training.GetTrainingAsync(trainingGuid, false)
                ?? throw new KeyNotFoundException($"Training '{{trainingGuid}}' was not found.");

            var model = input.Adapt<Training>();
            model.TrainingGuid = trainingGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.Training.UpdateTrainingAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Training",
                RootEntityKey = model.TrainingGuid.ToString(),
                RootDisplayName = model.TrainingCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Training",
                        EntityKey = model.TrainingGuid.ToString(),
                        EntityDisplayName = model.TrainingCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteTrainingAsync(Guid trainingGuid, TrainingForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.Training.GetTrainingAsync(trainingGuid, false);
            var model = new Training
            {
                TrainingGuid = trainingGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Training.SoftDeleteTrainingAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Training",
                RootEntityKey = trainingGuid.ToString(),
                RootDisplayName = oldEntity?.TrainingCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Training",
                        EntityKey = trainingGuid.ToString(),
                        EntityDisplayName = oldEntity?.TrainingCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteTrainingByAdminAsync(Guid trainingGuid, bool trackChanges)
        {
            await repository.Training.DeleteTrainingAsync(trainingGuid);
        }

        public async Task<IEnumerable<TrainingDto>> SearchTrainingAsync(
            string? trainingCode, string? trainingCodeSearchType, string? trainingName, string? trainingNameSearchType, string? trainingShortName, string? trainingShortNameSearchType, string? srTrainingCategory, string? srTrainingCategorySearchType, string? srTrainingType, string? srTrainingTypeSearchType, string? srTrainingLevel, string? srTrainingLevelSearchType, string? durationHour, string? durationHourSearchType, string? validityMonth, string? validityMonthSearchType, string? srTrainingProvider, string? srTrainingProviderSearchType, string? isMandatory, string? isMandatorySearchType, string? isCertification, string? isCertificationSearchType, string? isExternal, string? isExternalSearchType, string? description, string? descriptionSearchType, string? isActive, string? isActiveSearchType

            )
        {
            var data = await repository.Training.SearchTrainingAsync(
                trainingCode, trainingCodeSearchType, trainingName, trainingNameSearchType, trainingShortName, trainingShortNameSearchType, srTrainingCategory, srTrainingCategorySearchType, srTrainingType, srTrainingTypeSearchType, srTrainingLevel, srTrainingLevelSearchType, durationHour, durationHourSearchType, validityMonth, validityMonthSearchType, srTrainingProvider, srTrainingProviderSearchType, isMandatory, isMandatorySearchType, isCertification, isCertificationSearchType, isExternal, isExternalSearchType, description, descriptionSearchType, isActive, isActiveSearchType

                );
            return data.Adapt<IEnumerable<TrainingDto>>();
        }
    }
}
