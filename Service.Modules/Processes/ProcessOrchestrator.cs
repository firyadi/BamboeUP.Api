using Contracts;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Service.Modules.Processes.Handlers;
using Shared.Process;
using System.Text.Json;

namespace Service.Modules.Processes;

public interface IProcessOrchestrator
{
    Task RunExecutionAsync(Guid processExecutionGuid, CancellationToken cancellationToken = default);
}

public sealed class ProcessOrchestrator : IProcessOrchestrator
{
    private readonly IProcessRepository _processRepository;
    private readonly IEnumerable<IProcessStepHandler> _handlers;
    private readonly IProcessProgressNotifier? _notifier;
    private readonly ILogger<ProcessOrchestrator> _logger;

    public ProcessOrchestrator(
        IProcessRepository processRepository,
        IEnumerable<IProcessStepHandler> handlers,
        ILogger<ProcessOrchestrator> logger,
        IProcessProgressNotifier? notifier = null)
    {
        _processRepository = processRepository;
        _handlers = handlers;
        _logger = logger;
        _notifier = notifier;
    }

    public async Task RunExecutionAsync(Guid processExecutionGuid, CancellationToken cancellationToken = default)
    {
        var execution = await _processRepository.GetExecutionByGuidAsync(processExecutionGuid)
            ?? throw new InvalidOperationException($"Execution {processExecutionGuid} not found");

        var definition = await _processRepository.GetDefinitionByIdAsync(execution.ProcessDefinitionId)
            ?? throw new InvalidOperationException("Process definition not found");

        ProcessExecutionStateMachine.EnsureTransition(execution.Status, ProcessExecutionStatus.Running);
        execution.Status = ProcessExecutionStatus.Running;
        execution.StartedAt ??= DateTime.UtcNow;
        await _processRepository.UpdateExecutionAsync(execution);
        await NotifyAsync(execution, "ProcessStarted", null);

        var paramValues = await _processRepository.GetParameterValuesAsync(execution.ProcessExecutionId);
        var paramDict = paramValues.ToDictionary(p => p.ParameterCode, p => p.ValueText, StringComparer.OrdinalIgnoreCase);

        var steps = (await _processRepository.GetStepDefinitionsAsync(definition.ProcessDefinitionId)).ToList();
        var totalSteps = Math.Max(steps.Count, 1);
        var stepIndex = 0;
        var totalProcessed = 0;
        var totalSuccess = 0;
        var totalWarnings = 0;
        var totalFailed = 0;
        var fatalError = false;

        foreach (var stepDef in steps)
        {
            cancellationToken.ThrowIfCancellationRequested();

            execution = await _processRepository.GetExecutionByGuidAsync(processExecutionGuid)
                ?? throw new InvalidOperationException("Execution disappeared");

            if (execution.CancellationRequestedAt.HasValue)
            {
                ProcessExecutionStateMachine.EnsureTransition(execution.Status, ProcessExecutionStatus.Stopping);
                execution.Status = ProcessExecutionStatus.Stopping;
                await _processRepository.UpdateExecutionAsync(execution);
                ProcessExecutionStateMachine.EnsureTransition(execution.Status, ProcessExecutionStatus.Cancelled);
                execution.Status = ProcessExecutionStatus.Cancelled;
                execution.CompletedAt = DateTime.UtcNow;
                await _processRepository.UpdateExecutionAsync(execution);
                await NotifyAsync(execution, "ProcessCancelled", stepDef.StepCode);
                return;
            }

            stepIndex++;
            execution.CurrentStepCode = stepDef.StepCode;
            execution.ProgressPercent = Math.Round((decimal)(stepIndex - 1) * 100m / totalSteps, 2);
            await _processRepository.UpdateExecutionAsync(execution);

            var stepExec = new ProcessStepExecution
            {
                ProcessExecutionId = execution.ProcessExecutionId,
                StepCode = stepDef.StepCode,
                StepName = stepDef.StepName,
                Status = ProcessStepStatus.Running,
                StartedAt = DateTime.UtcNow,
                ProgressPercent = 0
            };
            await _processRepository.UpsertStepExecutionAsync(stepExec);
            await NotifyAsync(execution, "StepStarted", stepDef.StepCode);

            var handler = ResolveHandler(stepDef.ConfigurationJson);
            ProcessStepResult result;

            if (handler is null)
            {
                result = ProcessStepResult.Fail($"No handler registered for step {stepDef.StepCode}");
            }
            else
            {
                var context = new ProcessStepContext
                {
                    ProcessExecutionGuid = execution.ProcessExecutionGuid,
                    ProcessExecutionId = execution.ProcessExecutionId,
                    ProcessCode = definition.ProcessCode,
                    StepCode = stepDef.StepCode,
                    StepName = stepDef.StepName,
                    ConfigurationJson = stepDef.ConfigurationJson,
                    Parameters = paramDict,
                    CancellationToken = cancellationToken,
                    ReportProgressAsync = async (processed, success, warnings, failed) =>
                    {
                        stepExec.ProcessedCount = processed;
                        stepExec.SuccessCount = success;
                        stepExec.WarningCount = warnings;
                        stepExec.FailedCount = failed;
                        stepExec.ProgressPercent = processed == 0 ? 0 : 100;
                        await _processRepository.UpsertStepExecutionAsync(stepExec);
                        execution.ProgressPercent = Math.Round(((stepIndex - 1) + (stepExec.ProgressPercent / 100m)) * 100m / totalSteps, 2);
                        await _processRepository.UpdateExecutionAsync(execution);
                        await NotifyAsync(execution, "StepProgress", stepDef.StepCode);
                    },
                    AddMessageAsync = async (severity, message) =>
                    {
                        await _processRepository.InsertMessageAsync(new ProcessMessage
                        {
                            ProcessExecutionId = execution.ProcessExecutionId,
                            StepCode = stepDef.StepCode,
                            Severity = severity,
                            Source = ProcessMessageSource.Technical,
                            Message = message,
                            CreatedTime = DateTime.UtcNow
                        });
                        await NotifyAsync(execution, "Message", stepDef.StepCode);
                    }
                };

                try
                {
                    result = await handler.ExecuteAsync(context, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Step {StepCode} failed for execution {ExecutionGuid}", stepDef.StepCode, processExecutionGuid);
                    result = ProcessStepResult.Fail(ex.Message, "STEP_EXCEPTION");
                }
            }

            stepExec.CompletedAt = DateTime.UtcNow;
            stepExec.ProcessedCount = result.ProcessedCount;
            stepExec.SuccessCount = result.SuccessCount;
            stepExec.WarningCount = result.WarningCount;
            stepExec.FailedCount = result.FailedCount;
            stepExec.Message = result.Message;
            stepExec.ErrorCode = result.ErrorCode;
            stepExec.ErrorMessage = result.ErrorMessage;
            stepExec.ProgressPercent = 100;

            totalProcessed += result.ProcessedCount;
            totalSuccess += result.SuccessCount;
            totalWarnings += result.WarningCount;
            totalFailed += result.FailedCount;

            if (result.Records.Count > 0)
            {
                await _processRepository.InsertResultDetailsAsync(result.Records.Select(r => new ProcessResultDetail
                {
                    ProcessExecutionId = execution.ProcessExecutionId,
                    RecordKey = r.RecordKey,
                    RecordLabel = r.RecordLabel,
                    Severity = r.Severity,
                    Message = r.Message,
                    ReferenceId = r.ReferenceId,
                    CreatedTime = DateTime.UtcNow
                }));
            }

            if (!result.Success && stepDef.IsMandatory)
            {
                stepExec.Status = ProcessStepStatus.Failed;
                await _processRepository.UpsertStepExecutionAsync(stepExec);
                fatalError = true;
                execution.ErrorCount++;
                await _processRepository.InsertMessageAsync(new ProcessMessage
                {
                    ProcessExecutionId = execution.ProcessExecutionId,
                    StepCode = stepDef.StepCode,
                    Severity = ProcessMessageSeverity.Error,
                    Source = ProcessMessageSource.Technical,
                    Message = result.ErrorMessage ?? $"Step {stepDef.StepCode} failed",
                    CreatedTime = DateTime.UtcNow
                });
                await NotifyAsync(execution, "StepFailed", stepDef.StepCode);
                break;
            }

            stepExec.Status = result.CompletedWithWarnings || result.WarningCount > 0 || result.FailedCount > 0
                ? ProcessStepStatus.Completed
                : ProcessStepStatus.Completed;
            if (result.WarningCount > 0 || result.FailedCount > 0)
                execution.WarningCount += result.WarningCount + result.FailedCount;

            await _processRepository.UpsertStepExecutionAsync(stepExec);
            await NotifyAsync(execution, "StepCompleted", stepDef.StepCode);
        }

        execution = await _processRepository.GetExecutionByGuidAsync(processExecutionGuid)!;
        execution!.CompletedAt = DateTime.UtcNow;
        execution.ProgressPercent = 100;

        string resultStatus;
        string executionStatus;

        if (fatalError)
        {
            executionStatus = ProcessExecutionStatus.Failed;
            resultStatus = ProcessResultStatus.Failed;
        }
        else if (totalFailed > 0 || totalWarnings > 0)
        {
            executionStatus = ProcessExecutionStatus.CompletedWithWarning;
            resultStatus = ProcessResultStatus.CompletedWithWarning;
        }
        else
        {
            executionStatus = ProcessExecutionStatus.Completed;
            resultStatus = ProcessResultStatus.Success;
        }

        ProcessExecutionStateMachine.EnsureTransition(execution.Status, executionStatus);
        execution.Status = executionStatus;
        await _processRepository.UpdateExecutionAsync(execution);

        await _processRepository.UpsertResultAsync(new ProcessResult
        {
            ProcessExecutionId = execution.ProcessExecutionId,
            ResultStatus = resultStatus,
            ResultMessage = fatalError ? "Process failed" : "Process completed",
            TotalRecords = totalProcessed,
            SuccessRecords = totalSuccess,
            WarningRecords = totalWarnings,
            FailedRecords = totalFailed,
            CompletedAt = DateTime.UtcNow
        });

        await NotifyAsync(execution, "ProcessCompleted", null);
    }

    private IProcessStepHandler? ResolveHandler(string? configurationJson)
    {
        if (string.IsNullOrWhiteSpace(configurationJson))
            return _handlers.FirstOrDefault(h => h.HandlerKey == "DefaultProcessStep");

        try
        {
            using var doc = JsonDocument.Parse(configurationJson);
            if (doc.RootElement.TryGetProperty("handlerKey", out var key))
            {
                var handlerKey = key.GetString();
                return _handlers.FirstOrDefault(h => h.HandlerKey.Equals(handlerKey, StringComparison.OrdinalIgnoreCase));
            }
        }
        catch
        {
            // fall through
        }

        return _handlers.FirstOrDefault(h => h.HandlerKey == "DefaultProcessStep");
    }

    private Task NotifyAsync(ProcessExecution execution, string eventName, string? stepCode) =>
        _notifier?.NotifyAsync(execution.ProcessExecutionGuid, eventName, stepCode, execution.ProgressPercent, execution.Status)
        ?? Task.CompletedTask;
}
