namespace Shared.Process;

/// <summary>Process execution lifecycle statuses (UI-07).</summary>
public static class ProcessExecutionStatus
{
    public const string Draft = "Draft";
    public const string Ready = "Ready";
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string WaitingApproval = "WaitingApproval";
    public const string Completed = "Completed";
    public const string CompletedWithWarning = "CompletedWithWarning";
    public const string Failed = "Failed";
    public const string Cancelled = "Cancelled";
    public const string Rejected = "Rejected";
    public const string Stopping = "Stopping";
}

public static class ProcessStepStatus
{
    public const string Pending = "Pending";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Skipped = "Skipped";
    public const string Cancelled = "Cancelled";
}

public static class ProcessResultStatus
{
    public const string Success = "Success";
    public const string CompletedWithWarning = "CompletedWithWarning";
    public const string Failed = "Failed";
}

public static class ProcessExecutionMode
{
    public const byte Sync = 1;
    public const byte Async = 2;
    public const byte Background = 3;
}

public static class ProcessMessageSeverity
{
    public const string Error = "Error";
    public const string Warning = "Warning";
    public const string Information = "Information";
}

public static class ProcessMessageSource
{
    public const string Ui = "UI";
    public const string Business = "Business";
    public const string Technical = "Technical";
}
