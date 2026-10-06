using System;
using Service.Contracts.Shell.Approval;

namespace Service.Contracts.Shell
{
    public interface IServiceShellManager
    {
        // ##IServiceManager##
        IApprovalService ApprovalService { get; }
        IApprovalDelegationService ApprovalDelegationService { get; }
        IApprovalTemplateService ApprovalTemplateService { get; }
        IAutoNumberService AutoNumberService { get; }
        IAutoNumberComponentService AutoNumberComponentService { get; }
        IAutoNumberCounterService AutoNumberCounterService { get; }
        IAutoNumberGenerateService AutoNumberGenerateService { get; }
        IAutoNumberLogService AutoNumberLogService { get; }
        IAutoNumberTemplateService AutoNumberTemplateService { get; }
        ICompanyService CompanyService { get; }
        ICompanyOfficeService CompanyOfficeService { get; }
        ICostCenterService CostCenterService { get; }
        ICostCenterAssignmentService CostCenterAssignmentService { get; }
        ICostCenterScopeService CostCenterScopeService { get; }
        IDocumentNumberRequestService DocumentNumberRequestService { get; }
        IFileStorageService FileStorageService { get; }
        IOrganizationUnitService OrganizationUnitService { get; }
        IOrganizationUnitScopeService OrganizationUnitScopeService { get; }
        IParameterService ParameterService { get; }
        IParameterscopeService ParameterscopeService { get; }
        IParameterscopeService ParameterScopeService { get; }
        IProgramService ProgramService { get; }
        IReportService ReportService { get; }
        IReportDefinitionService ReportDefinitionService { get; }
        IStandardReferenceService StandardReferenceService { get; }
        IStandardReferenceDisplayService StandardReferenceDisplayService { get; }
        IStandardReferenceItemService StandardReferenceItemService { get; }
        IStandardReferenceScopeService StandardReferenceScopeService { get; }
        IStandardReferenceScopeItemService StandardReferenceScopeItemService { get; }
        IUserService UserService { get; }
        IUserCompanyScopeService UserCompanyScopeService { get; }
        IUserGroupService UserGroupService { get; }
        IUserGroupProgramService UserGroupProgramService { get; }
        IUserGroupScopeService UserGroupScopeService { get; }
        IUserScopeService UserScopeService { get; }
        IVwStandardReferenceItemService VwStandardReferenceItemService { get; }
        IAppParameterManager AppParameterManager { get; }
        // ##IServiceManager End##
    }
}
