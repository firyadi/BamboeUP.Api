using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class FileStorageRepository(RepositoryContext context, ISqlDialect dialect) : IFileStorageRepository
    {
        private readonly string _table = dialect.Qualify("app", "FileStorage");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<FileStorage?> GetFileStorageAsync(Guid fileStorageGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrFileCategory.StandardReferenceItemName")} AS FileCategoryName


                FROM {_table} a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'FileCategory')
    WHERE StandardReferenceItemId = a.SrFileCategory
) v_SrFileCategory


                WHERE a.FileStorageGuid = @fileStorageGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<FileStorage>(sql, new { fileStorageGuid });
        }

        public async Task<FileStorage?> GetFileStorageByIdAsync(long fileStorageId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrFileCategory.StandardReferenceItemName")} AS FileCategoryName


                FROM {_table} a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'FileCategory')
    WHERE StandardReferenceItemId = a.SrFileCategory
) v_SrFileCategory


                WHERE a.FileStorageId = @fileStorageId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<FileStorage>(sql, new { fileStorageId });
        }

        public async Task RecordAccessAsync(Guid fileStorageGuid, CancellationToken cancellationToken = default)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET DownloadCount = DownloadCount + 1,
                    LastAccessTime = SYSUTCDATETIME()
                WHERE FileStorageGuid = @fileStorageGuid
                  AND StatusId > 0
                  AND DeletedTime IS NULL";
            await connection.ExecuteAsync(new CommandDefinition(sql, new { fileStorageGuid }, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<FileStorage>> GetAllFileStoragesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrFileCategory.StandardReferenceItemName")} AS FileCategoryName


                FROM {_table} a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'FileCategory')
    WHERE StandardReferenceItemId = a.SrFileCategory
) v_SrFileCategory


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.FileStorageId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<FileStorage>(sql);
        }

        public async Task CreateFileStorageAsync(FileStorage fileStorage, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (FileStorageGuid, CreatedById, StatusId, CreatedTime, FileStorageName, StoredFileName, Extension, MimeType, FileSize, SrFileCategory, StorageProvider, RelativePath, Width, Height, IsImage, FileHash, DownloadCount, LastAccessTime, IsTemporary, Description
                )
                VALUES
                (@FileStorageGuid, @CreatedById, @StatusId, @CreatedTime, @FileStorageName, @StoredFileName, @Extension, @MimeType, @FileSize, @SrFileCategory, @StorageProvider, @RelativePath, @Width, @Height, @IsImage, @FileHash, @DownloadCount, @LastAccessTime, @IsTemporary, @Description
                );
                {dialect.ReturnIdentitySuffix("FileStorageId")}";
            fileStorage.FileStorageId = await conn.QuerySingleAsync<long>(sql, fileStorage, transaction);
        }

        public async Task UpdateFileStorageAsync(FileStorage fileStorage, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     FileStorageName = @FileStorageName,
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
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE FileStorageGuid = @FileStorageGuid";

            await conn.ExecuteAsync(sql, fileStorage, transaction);
        }

        public async Task DeleteFileStorageAsync(Guid fileStorageGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE FileStorageGuid = @fileStorageGuid";
            await conn.ExecuteAsync(sql, new { fileStorageGuid }, transaction);
        }

        public async Task<IEnumerable<FileStorage>> SearchFileStorageAsync(
            string? fileStorageName,
            string? fileStorageNameSearchType,
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
string? fileCategoryName, string? fileCategoryNameSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(fileStorageName))
            {
                var param = SqlFilterHelper.BuildFilter("a.FileStorageName", "@fileStorageName", fileStorageNameSearchType, parameters, "fileStorageName", fileStorageName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(storedFileName))
            {
                var param = SqlFilterHelper.BuildFilter("a.StoredFileName", "@storedFileName", storedFileNameSearchType, parameters, "storedFileName", storedFileName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(extension))
            {
                var param = SqlFilterHelper.BuildFilter("a.Extension", "@extension", extensionSearchType, parameters, "extension", extension, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                var param = SqlFilterHelper.BuildFilter("a.MimeType", "@mimeType", mimeTypeSearchType, parameters, "mimeType", mimeType, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(fileSize))
            {
                var param = SqlFilterHelper.BuildFilter("a.FileSize", "@fileSize", fileSizeSearchType, parameters, "fileSize", fileSize, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(storageProvider))
            {
                var param = SqlFilterHelper.BuildFilter("a.StorageProvider", "@storageProvider", storageProviderSearchType, parameters, "storageProvider", storageProvider, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(relativePath))
            {
                var param = SqlFilterHelper.BuildFilter("a.RelativePath", "@relativePath", relativePathSearchType, parameters, "relativePath", relativePath, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(width))
            {
                var param = SqlFilterHelper.BuildFilter("a.Width", "@width", widthSearchType, parameters, "width", width, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(height))
            {
                var param = SqlFilterHelper.BuildFilter("a.Height", "@height", heightSearchType, parameters, "height", height, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isImage))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsImage", "@isImage", isImageSearchType, parameters, "isImage", isImage, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(fileHash))
            {
                var param = SqlFilterHelper.BuildFilter("a.FileHash", "@fileHash", fileHashSearchType, parameters, "fileHash", fileHash, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(downloadCount))
            {
                var param = SqlFilterHelper.BuildFilter("a.DownloadCount", "@downloadCount", downloadCountSearchType, parameters, "downloadCount", downloadCount, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(lastAccessTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.LastAccessTime", "@lastAccessTime", lastAccessTimeSearchType, parameters, "lastAccessTime", lastAccessTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isTemporary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsTemporary", "@isTemporary", isTemporarySearchType, parameters, "isTemporary", isTemporary, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }
if (!string.IsNullOrWhiteSpace(fileCategoryName)) { var param = SqlFilterHelper.BuildFilter("v_SrFileCategory.StandardReferenceItemName", "@fileCategoryName", fileCategoryNameSearchType, parameters, "fileCategoryName", fileCategoryName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrFileCategory.StandardReferenceItemName")} AS FileCategoryName


                FROM {_table} a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'FileCategory')
    WHERE StandardReferenceItemId = a.SrFileCategory
) v_SrFileCategory


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.FileStorageId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<FileStorage>(sql, parameters, transaction);
        }
    }
}
