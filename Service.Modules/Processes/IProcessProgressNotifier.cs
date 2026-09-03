namespace Service.Modules.Processes;

public interface IProcessProgressNotifier
{
    Task NotifyAsync(Guid processExecutionGuid, string eventName, string? stepCode, decimal progressPercent, string status);
}
