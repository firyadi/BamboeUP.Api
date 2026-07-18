using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IFileStorageRepository
    {
        Task<FileStorage?> GetFileStorageAsync(Guid fileStorageGuid, bool trackChanges);
        Task<IEnumerable<FileStorage>> GetAllFileStoragesAsync(bool trackChanges);

        Task CreateFileStorageAsync(FileStorage fileStorage, IDbTransaction? transaction = null);
        Task UpdateFileStorageAsync(FileStorage fileStorage, IDbTransaction? transaction = null);
        Task DeleteFileStorageAsync(Guid fileStorageGuid, IDbTransaction? transaction = null);
        Task SoftDeleteFileStorageAsync(FileStorage fileStorage, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<FileStorage>> SearchFileStorageAsync(
            string? originalFileName,
            string? originalFileNameSearchType,
            string? storedFileName,
            string? storedFileNameSearchType,
            string? extension,
            string? extensionSearchType,
            string? mimeType,
            string? mimeTypeSearchType,
            string? fileSize,
            string? fileSizeSearchType,
            string? storageProvider,
            string? storageProviderSearchType,
            string? relativePath,
            string? relativePathSearchType,
            string? width,
            string? widthSearchType,
            string? height,
            string? heightSearchType,
            string? isImage,
            string? isImageSearchType,
            string? fileHash,
            string? fileHashSearchType,
            string? downloadCount,
            string? downloadCountSearchType,
            string? lastAccessTime,
            string? lastAccessTimeSearchType,
            string? isTemporary,
            string? isTemporarySearchType,
            string? description,
            string? descriptionSearchType,

            IDbTransaction? transaction = null);
    }
}
