using System.Security.Cryptography;
using System.Text;
using Entities.Models;
using Mapster;
using Shared.DataTransferObjects;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Service.Shell;

public partial class FileStorageService
{
    public async Task<FileOperationResult<FileStorageDto>> UploadAsync(
        FileUploadRequest request,
        long srFileCategory,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateUpload(request);
        if (validation is not null)
            return FileOperationResult<FileStorageDto>.Fail(
                validation.Message,
                validation.ErrorCode,
                validation.ValidationErrors);

        try
        {
            var extension = Path.GetExtension(request.OriginalFileName).ToLowerInvariant();
            var storedFileName = $"{Guid.NewGuid():N}{extension}";
            var now = DateTime.UtcNow;
            var relativeDir = Path.Combine(now.Year.ToString("D4"), now.Month.ToString("D2"), now.Day.ToString("D2"));
            var relativePath = Path.Combine(relativeDir, storedFileName).Replace('\\', '/');

            var root = ResolveUploadRoot();
            var absoluteDir = Path.Combine(root, relativeDir);
            Directory.CreateDirectory(absoluteDir);

            var absolutePath = Path.Combine(absoluteDir, storedFileName);

            await using (var fs = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                if (request.Content.CanSeek)
                    request.Content.Position = 0;
                await request.Content.CopyToAsync(fs, cancellationToken);
            }

            int? width = null;
            int? height = null;
            var isImage = IsImageExtension(extension);
            string? fileHash;

            await using (var readFs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true))
            {
                fileHash = await ComputeSha256Async(readFs, cancellationToken);
                if (isImage)
                {
                    readFs.Position = 0;
                    try
                    {
                        using var image = await Image.LoadAsync(readFs, cancellationToken);
                        width = image.Width;
                        height = image.Height;

                        if (_options.GenerateThumbnail)
                        {
                            var thumbName = Path.GetFileNameWithoutExtension(storedFileName) + ".thumb" + extension;
                            var thumbPath = Path.Combine(absoluteDir, thumbName);
                            using var clone = image.Clone(ctx => ctx.Resize(new ResizeOptions
                            {
                                Size = new Size(_options.ThumbnailWidth, _options.ThumbnailHeight),
                                Mode = ResizeMode.Max
                            }));
                            await clone.SaveAsync(thumbPath, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarn($"Image metadata/thumbnail failed for {storedFileName}: {ex.Message}");
                        isImage = false;
                    }
                }
            }

            var entity = new FileStorage
            {
                FileStorageGuid = Guid.NewGuid(),
                FileStorageName = Path.GetFileName(request.OriginalFileName),
                StoredFileName = storedFileName,
                Extension = extension.TrimStart('.'),
                MimeType = string.IsNullOrWhiteSpace(request.ContentType) ? "application/octet-stream" : request.ContentType,
                FileSize = new FileInfo(absolutePath).Length,
                SrFileCategory = srFileCategory,
                StorageProvider = _options.StorageProvider,
                RelativePath = relativePath,
                Width = width,
                Height = height,
                IsImage = isImage,
                FileHash = fileHash,
                DownloadCount = 0,
                IsTemporary = false,
                StatusId = 1,
                CreatedById = userContext.UserId,
                CreatedTime = DateTime.UtcNow
            };

            await repository.FileStorage.CreateFileStorageAsync(entity);
            logger.LogInfo($"Uploaded file '{entity.FileStorageName}' → {entity.RelativePath} (Id={entity.FileStorageId})");

            return FileOperationResult<FileStorageDto>.Ok(entity.Adapt<FileStorageDto>(), "File uploaded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Upload failed: {ex.Message}");
            return FileOperationResult<FileStorageDto>.Fail("Upload failed.", "UPLOAD_FAILED", [ex.Message]);
        }
    }

    public async Task<long?> ResolveFileCategoryIdAsync(string categoryNameOrInitial, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categoryNameOrInitial))
            return null;

        var items = await repository.StandardReferenceItemDisplay.GetItemsAsync(0, 0, null, "FileCategory");
        var match = items.FirstOrDefault(i =>
            string.Equals(i.StandardReferenceItemInitial, categoryNameOrInitial, StringComparison.OrdinalIgnoreCase)
            || string.Equals(i.StandardReferenceItemName, categoryNameOrInitial, StringComparison.OrdinalIgnoreCase)
            || string.Equals(i.StandardReferenceItemValue, categoryNameOrInitial, StringComparison.OrdinalIgnoreCase));

        return match?.StandardReferenceItemId;
    }

    public async Task<FileOperationResult<FileDownloadPayload>> OpenDownloadAsync(
        Guid fileStorageGuid,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await repository.FileStorage.GetFileStorageAsync(fileStorageGuid, trackChanges: false);
            if (entity is null)
                return FileOperationResult<FileDownloadPayload>.Fail("File not found.", "NOT_FOUND");

            var absolutePath = Path.Combine(ResolveUploadRoot(), entity.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(absolutePath))
            {
                logger.LogWarn($"Physical file missing for {fileStorageGuid}: {absolutePath}");
                return FileOperationResult<FileDownloadPayload>.Fail("Physical file missing.", "FILE_MISSING");
            }

            await repository.FileStorage.RecordAccessAsync(fileStorageGuid, cancellationToken);

            var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
            logger.LogInfo($"Download started for FileStorage {fileStorageGuid}");

            return FileOperationResult<FileDownloadPayload>.Ok(new FileDownloadPayload
            {
                Content = stream,
                ContentType = entity.MimeType,
                DownloadFileName = entity.FileStorageName,
                Metadata = entity.Adapt<FileStorageDto>()
            });
        }
        catch (Exception ex)
        {
            logger.LogError($"Download failed for {fileStorageGuid}: {ex.Message}");
            return FileOperationResult<FileDownloadPayload>.Fail("Download failed.", "DOWNLOAD_FAILED", [ex.Message]);
        }
    }

    public async Task<FileOperationResult> DeletePhysicalAsync(
        Guid fileStorageGuid,
        long deletedById,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await repository.FileStorage.GetFileStorageAsync(fileStorageGuid, trackChanges: false);
            if (entity is null)
                return FileOperationResult.Fail("File not found.", "NOT_FOUND");

            var root = ResolveUploadRoot();
            var absolutePath = Path.Combine(root, entity.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            TryDeleteFile(absolutePath);

            var thumbPath = Path.Combine(
                Path.GetDirectoryName(absolutePath) ?? root,
                Path.GetFileNameWithoutExtension(entity.StoredFileName) + ".thumb." + entity.Extension.TrimStart('.'));
            // Stored as name.thumb.ext when extension already had no leading logic — also try name.thumb{ext}
            var thumbAlt = Path.Combine(
                Path.GetDirectoryName(absolutePath) ?? root,
                Path.GetFileNameWithoutExtension(entity.StoredFileName) + ".thumb" +
                (entity.Extension.StartsWith('.') ? entity.Extension : "." + entity.Extension));
            TryDeleteFile(thumbPath);
            TryDeleteFile(thumbAlt);

            await repository.FileStorage.SoftDeleteFileStorageAsync(entity, deletedById);
            logger.LogInfo($"Deleted FileStorage {fileStorageGuid} (physical + soft-delete)");

            return FileOperationResult.Ok("File deleted.");
        }
        catch (Exception ex)
        {
            logger.LogError($"DeletePhysical failed for {fileStorageGuid}: {ex.Message}");
            return FileOperationResult.Fail("Delete failed.", "DELETE_FAILED", [ex.Message]);
        }
    }

    private FileOperationResult? ValidateUpload(FileUploadRequest request)
    {
        var errors = new List<string>();

        if (request.Content is null || request.Length <= 0)
            errors.Add("File content is empty.");

        if (string.IsNullOrWhiteSpace(request.OriginalFileName))
            errors.Add("Original file name is required.");

        var extension = Path.GetExtension(request.OriginalFileName ?? string.Empty).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension)
            || !_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            errors.Add($"Extension '{extension}' is not allowed.");

        if (request.Length > _options.MaximumFileSize)
            errors.Add($"File size {request.Length} exceeds maximum {_options.MaximumFileSize} bytes.");

        if (!string.Equals(_options.StorageProvider, "Local", StringComparison.OrdinalIgnoreCase))
            errors.Add($"Storage provider '{_options.StorageProvider}' is not implemented.");

        if (errors.Count > 0)
            return FileOperationResult.Fail("Validation failed.", "VALIDATION", errors);

        return null;
    }

    private string ResolveUploadRoot()
    {
        var root = _options.UploadRoot;
        if (Path.IsPathRooted(root))
            return root;

        var baseDir = Directory.GetCurrentDirectory();
        return Path.GetFullPath(Path.Combine(baseDir, root));
    }

    private static bool IsImageExtension(string extension) =>
        extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp";

    private static async Task<string> ComputeSha256Async(Stream stream, CancellationToken cancellationToken)
    {
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // best-effort physical cleanup
        }
    }
}
