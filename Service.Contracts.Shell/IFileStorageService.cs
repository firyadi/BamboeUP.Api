using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public partial interface IFileStorageService
    {
        Task<IEnumerable<FileStorageDto>> GetAllFileStoragesAsync(bool trackChanges);
        Task<FileStorageDto?> GetFileStorageByGuidAsync(Guid fileStorageGuid, bool trackChanges);
        Task<FileStorageDto?> GetFileStorageByIdAsync(long fileStorageId, bool trackChanges);
        Task<FileStorageDto> CreateFileStorageAsync(FileStorageForCreationDto input);
        Task UpdateFileStorageAsync(Guid fileStorageGuid, FileStorageForUpdateDto input, bool trackChanges);
        Task DeleteFileStorageAsync(Guid fileStorageGuid, FileStorageForDeleteDto input, bool trackChanges);
        Task DeleteFileStorageByAdminAsync(Guid fileStorageGuid, bool trackChanges);

        /// <summary>
        /// Validates, stores physical file under UploadRoot/yyyy/MM/dd, inserts FileStorage metadata.
        /// </summary>
        Task<FileOperationResult<FileStorageDto>> UploadAsync(
            FileUploadRequest request,
            long srFileCategory,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves FileCategory SR item id by item name/initial (e.g. Photo, Logo).
        /// </summary>
        Task<long?> ResolveFileCategoryIdAsync(string categoryNameOrInitial, CancellationToken cancellationToken = default);

        /// <summary>
        /// Opens file stream for download and increments DownloadCount / LastAccessTime.
        /// </summary>
        Task<FileOperationResult<FileDownloadPayload>> OpenDownloadAsync(
            Guid fileStorageGuid,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft-deletes metadata and removes physical file (and thumbnail if present).
        /// </summary>
        Task<FileOperationResult> DeletePhysicalAsync(
            Guid fileStorageGuid,
            long deletedById,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<FileStorageDto>> SearchFileStorageAsync(
            string? fileStorageName, string? fileStorageNameSearchType, string? storedFileName, string? storedFileNameSearchType, string? extension, string? extensionSearchType, string? mimeType, string? mimeTypeSearchType, string? fileSize, string? fileSizeSearchType, string? storageProvider, string? storageProviderSearchType, string? relativePath, string? relativePathSearchType, string? width, string? widthSearchType, string? height, string? heightSearchType, string? isImage, string? isImageSearchType, string? fileHash, string? fileHashSearchType, string? downloadCount, string? downloadCountSearchType, string? lastAccessTime, string? lastAccessTimeSearchType, string? isTemporary, string? isTemporarySearchType, string? description, string? descriptionSearchType
, string? fileCategoryName, string? fileCategoryNameSearchType

        );
    }
}
