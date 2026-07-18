namespace Shared.Settings;

/// <summary>
/// Configuration for physical file storage under UploadRoot/yyyy/MM/dd.
/// </summary>
public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>Root folder for uploads (absolute or relative to content root).</summary>
    public string UploadRoot { get; set; } = "Uploads";

    /// <summary>Allowed extensions including the dot, e.g. .jpg.</summary>
    public string[] AllowedExtensions { get; set; } =
    [
        ".jpg", ".jpeg", ".png", ".gif", ".webp",
        ".pdf", ".doc", ".docx", ".xls", ".xlsx",
        ".mp4", ".mp3", ".zip"
    ];

    /// <summary>Maximum upload size in bytes (default 10 MB).</summary>
    public long MaximumFileSize { get; set; } = 10 * 1024 * 1024;

    public bool GenerateThumbnail { get; set; } = true;

    public int ThumbnailWidth { get; set; } = 150;

    public int ThumbnailHeight { get; set; } = 150;

    /// <summary>Local | AzureBlob | S3 — only Local is implemented.</summary>
    public string StorageProvider { get; set; } = "Local";
}
