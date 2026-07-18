namespace Shared.DataTransferObjects;

/// <summary>
/// Result envelope for blob upload / delete / download operations.
/// </summary>
public sealed record FileOperationResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? ErrorCode { get; init; }
    public IReadOnlyList<string>? ValidationErrors { get; init; }

    public static FileOperationResult Ok(string message = "OK") =>
        new() { Success = true, Message = message };

    public static FileOperationResult Fail(string message, string? errorCode = null, IReadOnlyList<string>? validationErrors = null) =>
        new()
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            ValidationErrors = validationErrors
        };
}

/// <summary>
/// Typed result envelope for blob operations that return a payload.
/// </summary>
public sealed record FileOperationResult<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? ErrorCode { get; init; }
    public IReadOnlyList<string>? ValidationErrors { get; init; }
    public T? Data { get; init; }

    public static FileOperationResult<T> Ok(T data, string message = "OK") =>
        new() { Success = true, Message = message, Data = data };

    public static FileOperationResult<T> Fail(string message, string? errorCode = null, IReadOnlyList<string>? validationErrors = null) =>
        new()
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            ValidationErrors = validationErrors
        };
}
