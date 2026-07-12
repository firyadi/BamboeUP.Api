using System;
using Service.Contracts.Shell.Approval;

namespace Service.Contracts.Shell
{
    public interface IServiceShellManager
    {
        // ##IServiceManager##
        ICostCenterScopeService CostCenterScopeService { get; }
        ICostCenterAssignmentService CostCenterAssignmentService { get; }
        ICostCenterService CostCenterService { get; }
        IParameterscopeService ParameterscopeService { get; }
        IAppParameterManager AppParameterManager { get; }
        IApprovalService ApprovalService { get; }
        IApprovalDelegationService ApprovalDelegationService { get; }
        IApprovalNotificationService ApprovalNotificationService { get; }
        IApprovalTemplateService ApprovalTemplateService { get; }
        IAuthenticationService AuthenticationService { get; }
        IAutoNumberService AutoNumberService { get; }
        IAutoNumberTemplateService AutoNumberTemplateService { get; }
        IAutoNumberCounterService AutoNumberCounterService { get; }
        IAutoNumberComponentService AutoNumberComponentService { get; }
        IAutoNumberLogService AutoNumberLogService { get; }
        IAutoNumberGenerateService AutoNumberGenerateService { get; }
        ICompanyService CompanyService { get; }
        ICompanyOfficeService CompanyOfficeService { get; }
        IDocumentNumberRequestService DocumentNumberRequestService { get; }
        IOrganizationUnitService OrganizationUnitService { get; }
        IOrganizationUnitScopeService OrganizationUnitScopeService { get; }
        IParameterService ParameterService { get; }
        IParameterscopeService ParameterScopeService { get; }
        IProgramService ProgramService { get; }
        IReportService ReportService { get; }
        IReportDefinitionService ReportDefinitionService { get; }
        IStandardReferenceService StandardReferenceService { get; }
        IStandardReferenceItemService StandardReferenceItemService { get; }
        IStandardReferenceScopeService StandardReferenceScopeService { get; }
        IStandardReferenceScopeItemService StandardReferenceScopeItemService { get; }
        IStandardReferenceDisplayService StandardReferenceDisplayService { get; }
        IUserService UserService { get; }
        IUserGroupService UserGroupService { get; }
        IUserGroupScopeService UserGroupScopeService { get; }
        IUserGroupProgramService UserGroupProgramService { get; }
        IUserCompanyScopeService UserCompanyScopeService { get; }
        IUserScopeService UserScopeService { get; }
        IVwStandardReferenceItemService VwStandardReferenceItemService { get; }
		
		
		// ##IServiceManager End##
    }
}