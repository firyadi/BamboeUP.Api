using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class FileStorageRepository(RepositoryContext context) : IFileStorageRepository
    {
        public async Task<FileStorage?> GetFileStorageAsync(Guid fileStorageGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, v_SrFileCategory.StandardReferenceItemName AS FileCategoryName


                FROM [app].[FileStorage] a
LEFT JOIN [app].[vw_StandardReference_Display] v_SrFileCategory ON a.SrFileCategory = v_SrFileCategory.StandardReferenceItemId


                WHERE a.FileStorageGuid = @fileStorageGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<FileStorage>(sql, new { fileStorageGuid });
        }

        public async Task<IEnumerable<FileStorage>> GetAllFileStoragesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, v_SrFileCategory.StandardReferenceItemName AS FileCategoryName


                FROM [app].[FileStorage] a
LEFT JOIN [app].[vw_StandardReference_Display] v_SrFileCategory ON a.SrFileCategory = v_SrFileCategory.StandardReferenceItemId


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.FileStorageId DESC";
            return await connection.QueryAsync<FileStorage>(sql);
        }

        public async Task CreateFileStorageAsync(FileStorage fileStorage, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[FileStorage]
                (FileStorageGuid, CreatedById, StatusId, CreatedTime, OriginalFileName, StoredFileName, Extension, MimeType, FileSize, SrFileCategory, StorageProvider, RelativePath, Width, Height, IsImage, FileHash, DownloadCount, LastAccessTime, IsTemporary, Description
                )
                VALUES
                (@FileStorageGuid, @CreatedById, @StatusId, @CreatedTime, @OriginalFileName, @StoredFileName, @Extension, @MimeType, @FileSize, @SrFileCategory, @StorageProvider, @RelativePath, @Width, @Height, @IsImage, @FileHash, @DownloadCount, @LastAccessTime, @IsTemporary, @Description
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            fileStorage.FileStorageId = await conn.QuerySingleAsync<long>(sql, fileStorage, transaction);
        }

        public async Task UpdateFileStorageAsync(FileStorage fileStorage, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[FileStorage]
                SET                     OriginalFileName = @OriginalFileName,
                    StoredFileName = @StoredFileName,
                    Extension = @Extension,
                    MimeType = @MimeType,
                    FileSize = @FileSize,
                    SrFileCategory = @SrFileCategory,
                    StorageProvider = @StorageProvider,
                    RelativePath = @RelativePath,
                    Width = @Width,
                    Height = @Height,
                    IsImage = @IsImage,
                    FileHash = @FileHash,
                    DownloadCount = @DownloadCount,
                    LastAccessTime = @LastAccessTime,
                    IsTemporary = @IsTemporary,
                    Description = @Description,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE FileStorageGuid = @FileStorageGuid";
            await conn.ExecuteAsync(sql, fileStorage, transaction);
        }

        public async Task SoftDeleteFileStorageAsync(FileStorage fileStorage, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[FileStorage]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE FileStorageGuid = @FileStorageGuid";

            await conn.ExecuteAsync(sql, fileStorage, transaction);
        }

        public async Task DeleteFileStorageAsync(Guid fileStorageGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[FileStorage] WHERE FileStorageGuid = @fileStorageGuid";
            await conn.ExecuteAsync(sql, new { fileStorageGuid }, transaction);
        }

        public async Task<IEnumerable<FileStorage>> SearchFileStorageAsync(
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

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(originalFileName))
            {
                var param = SqlFilterHelper.BuildFilter("a.OriginalFileName", "@originalFileName", originalFileNameSearchType, parameters, "originalFileName", originalFileName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(storedFileName))
            {
                var param = SqlFilterHelper.BuildFilter("a.StoredFileName", "@storedFileName", storedFileNameSearchType, parameters, "storedFileName", storedFileName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(extension))
            {
                var param = SqlFilterHelper.BuildFilter("a.Extension", "@extension", extensionSearchType, parameters, "extension", extension);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                var param = SqlFilterHelper.BuildFilter("a.MimeType", "@mimeType", mimeTypeSearchType, parameters, "mimeType", mimeType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(fileSize))
            {
                var param = SqlFilterHelper.BuildFilter("a.FileSize", "@fileSize", fileSizeSearchType, parameters, "fileSize", fileSize);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(storageProvider))
            {
                var param = SqlFilterHelper.BuildFilter("a.StorageProvider", "@storageProvider", storageProviderSearchType, parameters, "storageProvider", storageProvider);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(relativePath))
            {
                var param = SqlFilterHelper.BuildFilter("a.RelativePath", "@relativePath", relativePathSearchType, parameters, "relativePath", relativePath);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(width))
            {
                var param = SqlFilterHelper.BuildFilter("a.Width", "@width", widthSearchType, parameters, "width", width);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(height))
            {
                var param = SqlFilterHelper.BuildFilter("a.Height", "@height", heightSearchType, parameters, "height", height);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isImage))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsImage", "@isImage", isImageSearchType, parameters, "isImage", isImage);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(fileHash))
            {
                var param = SqlFilterHelper.BuildFilter("a.FileHash", "@fileHash", fileHashSearchType, parameters, "fileHash", fileHash);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(downloadCount))
            {
                var param = SqlFilterHelper.BuildFilter("a.DownloadCount", "@downloadCount", downloadCountSearchType, parameters, "downloadCount", downloadCount);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(lastAccessTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.LastAccessTime", "@lastAccessTime", lastAccessTimeSearchType, parameters, "lastAccessTime", lastAccessTime);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isTemporary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsTemporary", "@isTemporary", isTemporarySearchType, parameters, "isTemporary", isTemporary);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, v_SrFileCategory.StandardReferenceItemName AS FileCategoryName


                FROM [app].[FileStorage] a
LEFT JOIN [app].[vw_StandardReference_Display] v_SrFileCategory ON a.SrFileCategory = v_SrFileCategory.StandardReferenceItemId


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.FileStorageId DESC";

            return await connection.QueryAsync<FileStorage>(sql, parameters, transaction);
        }
    }
}
