using System;
using BamboeUp.Audit.Contracts;
using Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Service.Contracts.Shell;
using Service.Contracts.Shell.Approval;
using Service.Shell.Approval;

namespace Service.Shell
{
    public class ServiceShellManager : IServiceShellManager
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly IAdminRegistryService _adminRegistry;

        // ##ServiceManagerFields##
        private readonly Lazy<ICostCenterScopeService> _costCenterScopeService;
        private readonly Lazy<ICostCenterAssignmentService> _costCenterAssignmentService;
        private readonly Lazy<ICostCenterService> _costCenterService;
        private readonly Lazy<IParameterscopeService> _parameterscopeService;
        private readonly Lazy<IAppParameterManager> _appParameterManager;
        private readonly Lazy<IApprovalNotificationService> _approvalNotificationService;
        private readonly Lazy<IApprovalTemplateService> _approvalTemplateService;
        private readonly Lazy<IApprovalDelegationService> _approvalDelegationService;
        private readonly Lazy<IApprovalService> _approvalService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IAutoNumberService> _autoNumberService;
        private readonly Lazy<IAutoNumberTemplateService> _autoNumberTemplateService;
        private readonly Lazy<IAutoNumberCounterService> _autoNumberCounterService;
        private readonly Lazy<IAutoNumberComponentService> _autoNumberComponentService;
        private readonly Lazy<IAutoNumberLogService> _autoNumberLogService;
        private readonly Lazy<IAutoNumberGenerateService> _autoNumberGenerateService;
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<ICompanyOfficeService> _companyOfficeService;
        private readonly Lazy<IDocumentNumberRequestService> _documentNumberRequestService;
        private readonly Lazy<IOrganizationUnitService> _organizationUnitService;
        private readonly Lazy<IOrganizationUnitScopeService> _organizationUnitScopeService;
        private readonly Lazy<IParameterService> _parameterService;
        private readonly Lazy<IProgramService> _programService;
        private readonly Lazy<IReportService> _reportService;
        private readonly Lazy<IReportDefinitionService> _reportDefinitionService;
        private readonly Lazy<IStandardReferenceService> _standardReferenceService;
        private readonly Lazy<IStandardReferenceItemService> _standardReferenceItemService;
        private readonly Lazy<IStandardReferenceScopeService> _standardReferenceScopeService;
        private readonly Lazy<IStandardReferenceScopeItemService> _standardReferenceScopeItemService;
        private readonly Lazy<IStandardReferenceDisplayService> _standardReferenceDisplayService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IUserGroupService> _userGroupService;
        private readonly Lazy<IUserGroupScopeService> _userGroupScopeService;
        private readonly Lazy<IUserGroupProgramService> _userGroupProgramService;
        private readonly Lazy<IUserCompanyScopeService> _userCompanyScopeService;
        private readonly Lazy<IUserScopeService> _userScopeService;
        private readonly Lazy<IVwStandardReferenceItemService> _vwStandardReferenceItemService;

        public ServiceShellManager(
            IRepositoryManager repositoryManager,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAuditService audit,
            IUserContext userContext,
            IConfiguration configuration,
            IMemoryCache memoryCache,
            IAdminRegistryService adminRegistry)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _transactionManager = transactionManager;
            _audit = audit;
            _userContext = userContext;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _adminRegistry = adminRegistry;

            // ##ServiceManagerConstructor##
            _costCenterScopeService = new Lazy<ICostCenterScopeService>(() => new CostCenterScopeService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _costCenterAssignmentService = new Lazy<ICostCenterAssignmentService>(() => new CostCenterAssignmentService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _costCenterService = new Lazy<ICostCenterService>(() => new CostCenterService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _parameterscopeService = new Lazy<IParameterscopeService>(() => new ParameterscopeService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _appParameterManager = new Lazy<IAppParameterManager>(() => new AppParameterManager(this, _userContext, _memoryCache));
            _approvalNotificationService = new Lazy<IApprovalNotificationService>(() => new ApprovalNotificationService(_logger));
            _approvalTemplateService = new Lazy<IApprovalTemplateService>(() => new ApprovalTemplateService(_repositoryManager, _logger, _transactionManager));
            _approvalDelegationService = new Lazy<IApprovalDelegationService>(() => new ApprovalDelegationService(_repositoryManager, _logger));
            _approvalService = new Lazy<IApprovalService>(() => new ApprovalService(_repositoryManager, _logger, _transactionManager, _approvalNotificationService.Value));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(_repositoryManager, _configuration, _adminRegistry));
            _autoNumberService = new Lazy<IAutoNumberService>(() => new AutoNumberService(_repositoryManager, _logger, _transactionManager));
            _autoNumberTemplateService = new Lazy<IAutoNumberTemplateService>(() => new AutoNumberTemplateService(_repositoryManager, _logger, _transactionManager));
            _autoNumberCounterService = new Lazy<IAutoNumberCounterService>(() => new AutoNumberCounterService(_repositoryManager, _logger, _transactionManager));
            _autoNumberComponentService = new Lazy<IAutoNumberComponentService>(() => new AutoNumberComponentService(_repositoryManager, _logger, _transactionManager));
            _autoNumberLogService = new Lazy<IAutoNumberLogService>(() => new AutoNumberLogService(_repositoryManager, _logger));
            _autoNumberGenerateService = new Lazy<IAutoNumberGenerateService>(() => new AutoNumberGenerateService(_repositoryManager, _logger));
            _userScopeService = new Lazy<IUserScopeService>(() => new UserScopeService(_repositoryManager, _userContext, _memoryCache, _configuration));
            _companyService = new Lazy<ICompanyService>(() => new CompanyService(_repositoryManager, _logger, _transactionManager, _audit, _userContext, _userScopeService.Value));
            _companyOfficeService = new Lazy<ICompanyOfficeService>(() => new CompanyOfficeService(_repositoryManager, _logger, _transactionManager, _audit, _userContext, _userScopeService.Value));
            _documentNumberRequestService = new Lazy<IDocumentNumberRequestService>(() => new DocumentNumberRequestService(_repositoryManager, _logger));
            _organizationUnitService = new Lazy<IOrganizationUnitService>(() => new OrganizationUnitService(_repositoryManager, _logger, _transactionManager, _audit, _userContext, _userScopeService.Value));
            _organizationUnitScopeService = new Lazy<IOrganizationUnitScopeService>(() => new OrganizationUnitScopeService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _parameterService = new Lazy<IParameterService>(() => new ParameterService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _programService = new Lazy<IProgramService>(() => new ProgramService(_repositoryManager, _logger, _transactionManager));
            _reportService = new Lazy<IReportService>(() => new ReportService(_repositoryManager, _logger));
            _reportDefinitionService = new Lazy<IReportDefinitionService>(() => new ReportDefinitionService(_repositoryManager));
            _standardReferenceService = new Lazy<IStandardReferenceService>(() => new StandardReferenceService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _standardReferenceItemService = new Lazy<IStandardReferenceItemService>(() => new StandardReferenceItemService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _standardReferenceScopeService = new Lazy<IStandardReferenceScopeService>(() => new StandardReferenceScopeService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _standardReferenceScopeItemService = new Lazy<IStandardReferenceScopeItemService>(() => new StandardReferenceScopeItemService(_repositoryManager, _logger, _transactionManager, _audit, _userContext));
            _standardReferenceDisplayService = new Lazy<IStandardReferenceDisplayService>(() => new StandardReferenceDisplayService(_repositoryManager));
            _userService = new Lazy<IUserService>(() => new UserService(_repositoryManager, _logger, _transactionManager, _adminRegistry));
            _userGroupService = new Lazy<IUserGroupService>(() => new UserGroupService(_repositoryManager, _logger, _transactionManager));
            _userGroupScopeService = new Lazy<IUserGroupScopeService>(() => new UserGroupScopeService(_repositoryManager, _logger, _transactionManager));
            _userGroupProgramService = new Lazy<IUserGroupProgramService>(() => new UserGroupProgramService(_repositoryManager, _logger, _transactionManager));
            _userCompanyScopeService = new Lazy<IUserCompanyScopeService>(() => new UserCompanyScopeService(_repositoryManager, _logger, _transactionManager));
            _vwStandardReferenceItemService = new Lazy<IVwStandardReferenceItemService>(() => new VwStandardReferenceItemService(_repositoryManager));
        }

        // ##ServiceManagerAccessors##
        public ICostCenterScopeService CostCenterScopeService => _costCenterScopeService.Value;
        public ICostCenterAssignmentService CostCenterAssignmentService => _costCenterAssignmentService.Value;
        public ICostCenterService CostCenterService => _costCenterService.Value;
        public IParameterscopeService ParameterscopeService => _parameterscopeService.Value;
        public IAppParameterManager AppParameterManager => _appParameterManager.Value;
        public IApprovalService ApprovalService => _approvalService.Value;
        public IApprovalDelegationService ApprovalDelegationService => _approvalDelegationService.Value;
        public IApprovalNotificationService ApprovalNotificationService => _approvalNotificationService.Value;
        public IApprovalTemplateService ApprovalTemplateService => _approvalTemplateService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IAutoNumberService AutoNumberService => _autoNumberService.Value;
        public IAutoNumberTemplateService AutoNumberTemplateService => _autoNumberTemplateService.Value;
        public IAutoNumberCounterService AutoNumberCounterService => _autoNumberCounterService.Value;
        public IAutoNumberComponentService AutoNumberComponentService => _autoNumberComponentService.Value;
        public IAutoNumberLogService AutoNumberLogService => _autoNumberLogService.Value;
        public IAutoNumberGenerateService AutoNumberGenerateService => _autoNumberGenerateService.Value;
        public ICompanyService CompanyService => _companyService.Value;
        public ICompanyOfficeService CompanyOfficeService => _companyOfficeService.Value;
        public IDocumentNumberRequestService DocumentNumberRequestService => _documentNumberRequestService.Value;
        public IOrganizationUnitService OrganizationUnitService => _organizationUnitService.Value;
        public IOrganizationUnitScopeService OrganizationUnitScopeService => _organizationUnitScopeService.Value;
        public IParameterService ParameterService => _parameterService.Value;
        public IParameterscopeService ParameterScopeService => _parameterscopeService.Value;
        public IProgramService ProgramService => _programService.Value;
        public IReportService ReportService => _reportService.Value;
        public IReportDefinitionService ReportDefinitionService => _reportDefinitionService.Value;
        public IStandardReferenceService StandardReferenceService => _standardReferenceService.Value;
        public IStandardReferenceItemService StandardReferenceItemService => _standardReferenceItemService.Value;
        public IStandardReferenceScopeService StandardReferenceScopeService => _standardReferenceScopeService.Value;
        public IStandardReferenceScopeItemService StandardReferenceScopeItemService => _standardReferenceScopeItemService.Value;
        public IStandardReferenceDisplayService StandardReferenceDisplayService => _standardReferenceDisplayService.Value;
        public IUserService UserService => _userService.Value;
        public IUserGroupService UserGroupService => _userGroupService.Value;
        public IUserGroupScopeService UserGroupScopeService => _userGroupScopeService.Value;
        public IUserGroupProgramService UserGroupProgramService => _userGroupProgramService.Value;
        public IUserCompanyScopeService UserCompanyScopeService => _userCompanyScopeService.Value;
        public IUserScopeService UserScopeService => _userScopeService.Value;
        public IVwStandardReferenceItemService VwStandardReferenceItemService => _vwStandardReferenceItemService.Value;
    }
}