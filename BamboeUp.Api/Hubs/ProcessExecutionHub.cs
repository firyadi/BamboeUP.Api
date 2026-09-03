using Microsoft.AspNetCore.SignalR;
using Service.Modules.Processes;

namespace BamboeUp.Api.Hubs;

public class ProcessExecutionHub : Hub
{
    public Task SubscribeExecution(Guid processExecutionGuid) =>
        Groups.AddToGroupAsync(Context.ConnectionId, GroupName(processExecutionGuid));

    public Task UnsubscribeExecution(Guid processExecutionGuid) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(processExecutionGuid));

    internal static string GroupName(Guid processExecutionGuid) => $"process:{processExecutionGuid:N}";
}

public sealed class SignalRProcessProgressNotifier : IProcessProgressNotifier
{
    private readonly IHubContext<ProcessExecutionHub> _hub;

    public SignalRProcessProgressNotifier(IHubContext<ProcessExecutionHub> hub) => _hub = hub;

    public Task NotifyAsync(Guid processExecutionGuid, string eventName, string? stepCode, decimal progressPercent, string status) =>
        _hub.Clients.Group(ProcessExecutionHub.GroupName(processExecutionGuid))
            .SendAsync("ProcessEvent", new
            {
                eventName,
                stepCode,
                progressPercent,
                status,
                processExecutionGuid
            });
}
