using Contracts;
using Service.Contracts.Shell;

namespace Service.Shell;

/// <summary>
/// Legacy ServiceManager - delegates to ServiceShellManager.
/// Kept for API Generator tool compatibility (IServiceManager interface placeholder).
/// </summary>
public sealed class ServiceManager : IServiceManager
{
    // + GENERATED_SERVICE_PROPERTY_DECLARATION
    private readonly Lazy<IAutoNumberTemplateService> _autoNumberTemplateService;
    private readonly Lazy<IAutoNumberCounterService> _autoNumberCounterService;
    private readonly Lazy<IAutoNumberComponentService> _autoNumberComponentService;
    // + GENERATED_SERVICE_PROPERTY_DECLARATION_END

    public ServiceManager(IRepositoryManager repositoryManager,
                          ILoggerManager logger,
                          ITransactionManager transactionManager)
    {
        // + GENERATED_SERVICE_PROPERTY_INITIALIZATION
        _autoNumberTemplateService = new Lazy<IAutoNumberTemplateService>(() => new AutoNumberTemplateService(repositoryManager, logger, transactionManager));
        _autoNumberCounterService = new Lazy<IAutoNumberCounterService>(() => new AutoNumberCounterService(repositoryManager, logger, transactionManager));
        _autoNumberComponentService = new Lazy<IAutoNumberComponentService>(() => new AutoNumberComponentService(repositoryManager, logger, transactionManager));
        // + GENERATED_SERVICE_PROPERTY_INITIALIZATION_END
    }

    // + GENERATED_SERVICE_PROPERTY_ACCESSOR
    public IAutoNumberTemplateService AutoNumberTemplateService => _autoNumberTemplateService.Value;
    public IAutoNumberCounterService AutoNumberCounterService => _autoNumberCounterService.Value;
    public IAutoNumberComponentService AutoNumberComponentService => _autoNumberComponentService.Value;
    // + GENERATED_SERVICE_PROPERTY_ACCESSOR_END
}
