using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public partial interface IFileStorageService
    {
        Task<IEnumerable<FileStorageDto>> GetAllFileStoragesAsync(bool trackChanges);
        Task<FileStorageDto?> GetFileStorageByGuidAsync(Guid fileStorageGuid, bool trackChanges);
        Task<FileStorageDto> CreateFileStorageAsync(FileStorageForCreationDto input);
        Task UpdateFileStorageAsync(Guid fileStorageGuid, FileStorageForUpdateDto input, bool trackChanges);
        Task DeleteFileStorageAsync(Guid fileStorageGuid, FileStorageForDeleteDto input, bool trackChanges);
        Task DeleteFileStorageByAdminAsync(Guid fileStorageGuid, bool trackChanges);

        Task<IEnumerable<FileStorageDto>> SearchFileStorageAsync(
            string? originalFileName, string? originalFileNameSearchType, string? storedFileName, string? storedFileNameSearchType, string? extension, string? extensionSearchType, string? mimeType, string? mimeTypeSearchType, string? fileSize, string? fileSizeSearchType, string? storageProvider, string? storageProviderSearchType, string? relativePath, string? relativePathSearchType, string? width, string? widthSearchType, string? height, string? heightSearchType, string? isImage, string? isImageSearchType, string? fileHash, string? fileHashSearchType, string? downloadCount, string? downloadCountSearchType, string? lastAccessTime, string? lastAccessTimeSearchType, string? isTemporary, string? isTemporarySearchType, string? description, string? descriptionSearchType

        );
    }
}
