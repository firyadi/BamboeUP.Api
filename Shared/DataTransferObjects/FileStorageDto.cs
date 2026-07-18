using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record FileStorageDto
    {
        public long FileStorageId { get; set; }
        public Guid FileStorageGuid { get; init; }
        
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public long SrFileCategory { get; set; }
        public string StorageProvider { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool IsImage { get; set; }
        public string? FileHash { get; set; }
        public long DownloadCount { get; set; }
        public DateTime? LastAccessTime { get; set; }
        public bool IsTemporary { get; set; }
        public string? Description { get; set; }
public string? FileCategoryName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record FileStorageForCreationDto
    {
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public long SrFileCategory { get; set; }
        public string StorageProvider { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool IsImage { get; set; }
        public string? FileHash { get; set; }
        public long DownloadCount { get; set; }
        public DateTime? LastAccessTime { get; set; }
        public bool IsTemporary { get; set; }
        public string? Description { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record FileStorageForUpdateDto
    {
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public long SrFileCategory { get; set; }
        public string StorageProvider { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool IsImage { get; set; }
        public string? FileHash { get; set; }
        public long DownloadCount { get; set; }
        public DateTime? LastAccessTime { get; set; }
        public bool IsTemporary { get; set; }
        public string? Description { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record FileStorageForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class FileStorageSearchDto
    {
        public string? OriginalFileName { get; set; }
        public SearchType OriginalFileNameSearchType { get; set; } = SearchType.Contains;

        public string? StoredFileName { get; set; }
        public SearchType StoredFileNameSearchType { get; set; } = SearchType.Contains;

        public string? Extension { get; set; }
        public SearchType ExtensionSearchType { get; set; } = SearchType.Contains;

        public string? MimeType { get; set; }
        public SearchType MimeTypeSearchType { get; set; } = SearchType.Contains;

        public string? FileSize { get; set; }
        public SearchType FileSizeSearchType { get; set; } = SearchType.Contains;

        public string? StorageProvider { get; set; }
        public SearchType StorageProviderSearchType { get; set; } = SearchType.Contains;

        public string? RelativePath { get; set; }
        public SearchType RelativePathSearchType { get; set; } = SearchType.Contains;

        public string? Width { get; set; }
        public SearchType WidthSearchType { get; set; } = SearchType.Contains;

        public string? Height { get; set; }
        public SearchType HeightSearchType { get; set; } = SearchType.Contains;

        public string? IsImage { get; set; }
        public SearchType IsImageSearchType { get; set; } = SearchType.Contains;

        public string? FileHash { get; set; }
        public SearchType FileHashSearchType { get; set; } = SearchType.Contains;

        public string? DownloadCount { get; set; }
        public SearchType DownloadCountSearchType { get; set; } = SearchType.Contains;

        public string? LastAccessTime { get; set; }
        public SearchType LastAccessTimeSearchType { get; set; } = SearchType.Contains;

        public string? IsTemporary { get; set; }
        public SearchType IsTemporarySearchType { get; set; } = SearchType.Contains;

        public string? Description { get; set; }
        public SearchType DescriptionSearchType { get; set; } = SearchType.Contains;
public string? FileCategoryName { get; set; }
public SearchType FileCategoryNameSearchType { get; set; } = SearchType.Contains;

    }
}
