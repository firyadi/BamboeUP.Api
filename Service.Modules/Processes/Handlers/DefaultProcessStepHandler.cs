namespace Service.Modules.Processes.Handlers;

/// <summary>Fallback handler when no custom handler is registered.</summary>
public sealed class DefaultProcessStepHandler : IProcessStepHandler
{
    public string HandlerKey => "DefaultProcessStep";

    public Task<ProcessStepResult> ExecuteAsync(ProcessStepContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(ProcessStepResult.Ok($"Step {context.StepCode} completed (default handler)."));
}
