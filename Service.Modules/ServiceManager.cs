using BamboeUp.Audit.Contracts;
using Contracts;
using Service.Contracts.Modules;

namespace Service.Modules;

public sealed class ServiceManager : IServiceManager
{
    // + GENERATED_SERVICE_PROPERTY_DECLARATION
    private readonly Lazy<IBankService> _bankService;
// + GENERATED_SERVICE_PROPERTY_DECLARATION_END

    public ServiceManager(IRepositoryManager repositoryManager,
                          ILoggerManager logger,
                          ITransactionManager transactionManager,
                          IAuditService auditService,
                          IUserContext userContext)
    {
        // + GENERATED_SERVICE_PROPERTY_INITIALIZATION
        _bankService = new Lazy<IBankService>(() => new BankService(repositoryManager, logger, transactionManager, auditService, userContext));
// + GENERATED_SERVICE_PROPERTY_INITIALIZATION_END
    }

    // + GENERATED_SERVICE_PROPERTY_ACCESSOR
    public IBankService BankService => _bankService.Value;
// + GENERATED_SERVICE_PROPERTY_ACCESSOR_END
}
