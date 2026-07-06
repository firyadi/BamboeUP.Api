using Service.Contracts.Shell;

namespace Service.Contracts.Shell;

/// <summary>
/// Legacy IServiceManager - kept for API Generator tool compatibility.
/// New code should use IServiceShellManager instead.
/// </summary>
public interface IServiceManager
{
    // + GENERATED_SERVICE_PROPERTY
    IAutoNumberTemplateService AutoNumberTemplateService { get; }
    IAutoNumberCounterService AutoNumberCounterService { get; }
    IAutoNumberComponentService AutoNumberComponentService { get; }
    // + GENERATED_SERVICE_PROPERTY_END
}
