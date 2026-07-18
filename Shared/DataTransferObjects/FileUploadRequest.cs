namespace Shared.DataTransferObjects;

/// <summary>
/// Framework-agnostic upload payload (controller maps IFormFile → this).
/// </summary>
public sealed class FileUploadRequest
{
    public required Stream Content { get; init; }
    public required string OriginalFileName { get; init; }
    public required string ContentType { get; init; }
    public long Length { get; init; }
}

/// <summary>
/// Open stream for download after access counters were updated.
/// Caller must dispose the stream.
/// </summary>
public sealed class FileDownloadPayload : IDisposable
{
    public required Stream Content { get; init; }
    public required string ContentType { get; init; }
    public required string DownloadFileName { get; init; }
    public required FileStorageDto Metadata { get; init; }

    public void Dispose() => Content.Dispose();
}
