using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Shell
{
    public partial class FileStorageService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IFileStorageService
    {
        public async Task<IEnumerable<FileStorageDto>> GetAllFileStoragesAsync(bool trackChanges)
        {
            var entities = await repository.FileStorage.GetAllFileStoragesAsync(trackChanges);
            return entities.Adapt<IEnumerable<FileStorageDto>>();
        }

        public async Task<FileStorageDto?> GetFileStorageByGuidAsync(Guid fileStorageGuid, bool trackChanges)
        {
            var entity = await repository.FileStorage.GetFileStorageAsync(fileStorageGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<FileStorageDto>();
        }

        public async Task<FileStorageDto> CreateFileStorageAsync(FileStorageForCreationDto input)
        {
            var model = input.Adapt<FileStorage>();

            if (model.FileStorageGuid == Guid.Empty)
                model.FileStorageGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.FileStorage.CreateFileStorageAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "FileStorage",
                RootEntityKey = model.FileStorageGuid.ToString(),
                RootDisplayName = model.OriginalFileName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "FileStorage",
                        EntityKey = model.FileStorageGuid.ToString(),
                        EntityDisplayName = model.OriginalFileName,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<FileStorageDto>();
        }

        public async Task UpdateFileStorageAsync(Guid fileStorageGuid, FileStorageForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.FileStorage.GetFileStorageAsync(fileStorageGuid, false)
                ?? throw new KeyNotFoundException($"FileStorage '{{fileStorageGuid}}' was not found.");

            var model = input.Adapt<FileStorage>();
            model.FileStorageGuid = fileStorageGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.FileStorage.UpdateFileStorageAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "FileStorage",
                RootEntityKey = model.FileStorageGuid.ToString(),
                RootDisplayName = model.OriginalFileName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "FileStorage",
                        EntityKey = model.FileStorageGuid.ToString(),
                        EntityDisplayName = model.OriginalFileName,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteFileStorageAsync(Guid fileStorageGuid, FileStorageForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.FileStorage.GetFileStorageAsync(fileStorageGuid, false);
            var model = new FileStorage
            {
                FileStorageGuid = fileStorageGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.FileStorage.SoftDeleteFileStorageAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "FileStorage",
                RootEntityKey = fileStorageGuid.ToString(),
                RootDisplayName = oldEntity?.OriginalFileName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "FileStorage",
                        EntityKey = fileStorageGuid.ToString(),
                        EntityDisplayName = oldEntity?.OriginalFileName,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteFileStorageByAdminAsync(Guid fileStorageGuid, bool trackChanges)
        {
            await repository.FileStorage.DeleteFileStorageAsync(fileStorageGuid);
        }

        public async Task<IEnumerable<FileStorageDto>> SearchFileStorageAsync(
            string? originalFileName, string? originalFileNameSearchType, string? storedFileName, string? storedFileNameSearchType, string? extension, string? extensionSearchType, string? mimeType, string? mimeTypeSearchType, string? fileSize, string? fileSizeSearchType, string? storageProvider, string? storageProviderSearchType, string? relativePath, string? relativePathSearchType, string? width, string? widthSearchType, string? height, string? heightSearchType, string? isImage, string? isImageSearchType, string? fileHash, string? fileHashSearchType, string? downloadCount, string? downloadCountSearchType, string? lastAccessTime, string? lastAccessTimeSearchType, string? isTemporary, string? isTemporarySearchType, string? description, string? descriptionSearchType

            )
        {
            var data = await repository.FileStorage.SearchFileStorageAsync(
                originalFileName, originalFileNameSearchType, storedFileName, storedFileNameSearchType, extension, extensionSearchType, mimeType, mimeTypeSearchType, fileSize, fileSizeSearchType, storageProvider, storageProviderSearchType, relativePath, relativePathSearchType, width, widthSearchType, height, heightSearchType, isImage, isImageSearchType, fileHash, fileHashSearchType, downloadCount, downloadCountSearchType, lastAccessTime, lastAccessTimeSearchType, isTemporary, isTemporarySearchType, description, descriptionSearchType
, fileCategoryName, fileCategoryNameSearchType

                );
            return data.Adapt<IEnumerable<FileStorageDto>>();
        }
    }
}
