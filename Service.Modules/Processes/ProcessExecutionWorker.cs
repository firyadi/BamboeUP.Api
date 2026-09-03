using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Process;

namespace Service.Modules.Processes;

public sealed class ProcessExecutionWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProcessExecutionWorker> _logger;

    public ProcessExecutionWorker(IServiceScopeFactory scopeFactory, ILogger<ProcessExecutionWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IProcessRepository>();
                var orchestrator = scope.ServiceProvider.GetRequiredService<IProcessOrchestrator>();

                var queued = (await repository.SearchExecutionsAsync(ProcessExecutionStatus.Queued, null, null, null, null, 0, 5)).ToList();
                foreach (var execution in queued)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                    try
                    {
                        await orchestrator.RunExecutionAsync(execution.ProcessExecutionGuid, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed running process execution {Guid}", execution.ProcessExecutionGuid);
                        execution.Status = ProcessExecutionStatus.Failed;
                        execution.CompletedAt = DateTime.UtcNow;
                        await repository.UpdateExecutionAsync(execution);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Process worker loop error");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
