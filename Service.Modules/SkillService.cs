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
    public partial class SkillService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext,
        IFileStorageService fileStorageService) : ISkillService
    {
        public async Task<IEnumerable<SkillDto>> GetAllSkillsAsync(bool trackChanges)
        {
            var entities = await repository.Skill.GetAllSkillsAsync(trackChanges);
            return entities.Adapt<IEnumerable<SkillDto>>();
        }

        public async Task<SkillDto?> GetSkillByGuidAsync(Guid skillGuid, bool trackChanges)
        {
            var entity = await repository.Skill.GetSkillAsync(skillGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<SkillDto>();
            if (entity.FileStorageId is > 0)
            {
                var file = await fileStorageService.GetFileStorageByIdAsync(entity.FileStorageId.Value, false);
                dto.FileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<SkillDto> CreateSkillAsync(SkillForCreationDto input)
        {
            var model = input.Adapt<Skill>();

            if (model.SkillGuid == Guid.Empty)
                model.SkillGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.Skill.CreateSkillAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Skill",
                RootEntityKey = model.SkillGuid.ToString(),
                RootDisplayName = model.SkillCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Skill",
                        EntityKey = model.SkillGuid.ToString(),
                        EntityDisplayName = model.SkillCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<SkillDto>();
        }

        public async Task UpdateSkillAsync(Guid skillGuid, SkillForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.Skill.GetSkillAsync(skillGuid, false)
                ?? throw new KeyNotFoundException($"Skill '{{skillGuid}}' was not found.");

            var model = input.Adapt<Skill>();
            model.SkillGuid = skillGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.Skill.UpdateSkillAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Skill",
                RootEntityKey = model.SkillGuid.ToString(),
                RootDisplayName = model.SkillCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Skill",
                        EntityKey = model.SkillGuid.ToString(),
                        EntityDisplayName = model.SkillCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteSkillAsync(Guid skillGuid, SkillForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.Skill.GetSkillAsync(skillGuid, false);
            var model = new Skill
            {
                SkillGuid = skillGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Skill.SoftDeleteSkillAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Skill",
                RootEntityKey = skillGuid.ToString(),
                RootDisplayName = oldEntity?.SkillCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Skill",
                        EntityKey = skillGuid.ToString(),
                        EntityDisplayName = oldEntity?.SkillCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteSkillByAdminAsync(Guid skillGuid, bool trackChanges)
        {
            await repository.Skill.DeleteSkillAsync(skillGuid);
        }

        public async Task<IEnumerable<SkillDto>> SearchSkillAsync(
            string? skillCode, string? skillCodeSearchType, string? skillName, string? skillNameSearchType, string? skillShortName, string? skillShortNameSearchType, string? srSkillCategory, string? srSkillCategorySearchType, string? srSkillType, string? srSkillTypeSearchType, string? srDefaultSkillLevel, string? srDefaultSkillLevelSearchType, string? isCertificationRequired, string? isCertificationRequiredSearchType, string? isMandatory, string? isMandatorySearchType, string? description, string? descriptionSearchType, string? isActive, string? isActiveSearchType

            )
        {
            var data = await repository.Skill.SearchSkillAsync(
                skillCode, skillCodeSearchType, skillName, skillNameSearchType, skillShortName, skillShortNameSearchType, srSkillCategory, srSkillCategorySearchType, srSkillType, srSkillTypeSearchType, srDefaultSkillLevel, srDefaultSkillLevelSearchType, isCertificationRequired, isCertificationRequiredSearchType, isMandatory, isMandatorySearchType, description, descriptionSearchType, isActive, isActiveSearchType

                );
            return data.Adapt<IEnumerable<SkillDto>>();
        }
    }
}
