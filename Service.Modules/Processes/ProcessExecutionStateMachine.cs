using Shared.Process;

namespace Service.Modules.Processes;

public static class ProcessExecutionStateMachine
{
    private static readonly Dictionary<string, HashSet<string>> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        [ProcessExecutionStatus.Draft] = [ProcessExecutionStatus.Ready],
        [ProcessExecutionStatus.Ready] = [ProcessExecutionStatus.Queued, ProcessExecutionStatus.Running],
        [ProcessExecutionStatus.Queued] = [ProcessExecutionStatus.Running, ProcessExecutionStatus.Cancelled],
        [ProcessExecutionStatus.Running] =
        [
            ProcessExecutionStatus.WaitingApproval,
            ProcessExecutionStatus.Completed,
            ProcessExecutionStatus.CompletedWithWarning,
            ProcessExecutionStatus.Failed,
            ProcessExecutionStatus.Stopping
        ],
        [ProcessExecutionStatus.Stopping] = [ProcessExecutionStatus.Cancelled],
        [ProcessExecutionStatus.WaitingApproval] = [ProcessExecutionStatus.Completed, ProcessExecutionStatus.Rejected],
        [ProcessExecutionStatus.Failed] = [ProcessExecutionStatus.Queued],
        [ProcessExecutionStatus.CompletedWithWarning] = [ProcessExecutionStatus.Queued],
        [ProcessExecutionStatus.Completed] = [],
        [ProcessExecutionStatus.Cancelled] = [ProcessExecutionStatus.Queued],
        [ProcessExecutionStatus.Rejected] = []
    };

    public static bool CanTransition(string currentStatus, string nextStatus)
    {
        if (string.Equals(currentStatus, nextStatus, StringComparison.OrdinalIgnoreCase))
            return true;

        return Allowed.TryGetValue(currentStatus, out var targets)
               && targets.Contains(nextStatus);
    }

    public static void EnsureTransition(string currentStatus, string nextStatus)
    {
        if (!CanTransition(currentStatus, nextStatus))
            throw new InvalidOperationException($"Invalid process status transition: {currentStatus} -> {nextStatus}");
    }
}
