using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Service.Contracts.Modules;
using Service.Modules.Processes;
using Service.Modules.Processes.Handlers;

namespace Service.Modules.Extensions;

public static class ProcessServiceConfiguration
{
    public static void ConfigureProcessServices(this IServiceCollection services)
    {
        services.AddScoped<IProcessExecutionService, ProcessExecutionService>();
        services.AddScoped<IProcessOrchestrator, ProcessOrchestrator>();

        services.AddScoped<IProcessStepHandler, DefaultProcessStepHandler>();
        services.AddScoped<IProcessStepHandler, ImportValidateFileStepHandler>();
        services.AddScoped<IProcessStepHandler, ImportParseRowsStepHandler>();
        services.AddScoped<IProcessStepHandler, ImportCommitDataStepHandler>();
        services.AddScoped<IProcessStepHandler, PayrollValidatePeriodStepHandler>();
        services.AddScoped<IProcessStepHandler, PayrollCalculatePayStepHandler>();
        services.AddScoped<IProcessStepHandler, PayrollCommitStepHandler>();

        services.AddHostedService<ProcessExecutionWorker>();
    }
}
