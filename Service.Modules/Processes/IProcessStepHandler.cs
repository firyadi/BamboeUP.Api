namespace Service.Modules.Processes;

public interface IProcessStepHandler
{
    string HandlerKey { get; }
    Task<ProcessStepResult> ExecuteAsync(ProcessStepContext context, CancellationToken cancellationToken = default);
}

public sealed class ProcessStepContext
{
    public required Guid ProcessExecutionGuid { get; init; }
    public required long ProcessExecutionId { get; init; }
    public required string ProcessCode { get; init; }
    public required string StepCode { get; init; }
    public required string StepName { get; init; }
    public string? ConfigurationJson { get; init; }
    public required IReadOnlyDictionary<string, string?> Parameters { get; init; }
    public required Func<int, int, int, int, Task> ReportProgressAsync { get; init; }
    public required Func<string, string, Task> AddMessageAsync { get; init; }
    public required CancellationToken CancellationToken { get; init; }
}

public sealed class ProcessStepResult
{
    public bool Success { get; init; }
    public bool CompletedWithWarnings { get; init; }
    public string? Message { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
    public int ProcessedCount { get; init; }
    public int SuccessCount { get; init; }
    public int WarningCount { get; init; }
    public int FailedCount { get; init; }
    public IReadOnlyList<ProcessStepRecordResult> Records { get; init; } = [];

    public static ProcessStepResult Ok(string? message = null, int processed = 0, int success = 0, int warnings = 0, int failed = 0) =>
        new() { Success = true, Message = message, ProcessedCount = processed, SuccessCount = success, WarningCount = warnings, FailedCount = failed };

    public static ProcessStepResult Fail(string errorMessage, string? errorCode = null) =>
        new() { Success = false, ErrorMessage = errorMessage, ErrorCode = errorCode };
}

public sealed class ProcessStepRecordResult
{
    public string RecordKey { get; init; } = string.Empty;
    public string? RecordLabel { get; init; }
    public string Severity { get; init; } = "Information";
    public string Message { get; init; } = string.Empty;
    public string? ReferenceId { get; init; }
}
