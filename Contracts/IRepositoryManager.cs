using System;
using Contracts.Approval;

namespace Contracts
{
    public interface IRepositoryManager
    {
        // ##IRepositoryManager##
        IFileStorageRepository FileStorage { get; }
        IPersonWorkExperienceRepository PersonWorkExperience { get; }
        IPersonPhysicalCharacteristicRepository PersonPhysicalCharacteristic { get; }
        IPersonFamilyRepository PersonFamily { get; }
        IPersonContactRepository PersonContact { get; }
        IPersonEmergencyContactRepository PersonEmergencyContact { get; }
        IPersonEducationRepository PersonEducation { get; }
        IPersonIdentificationRepository PersonIdentification { get; }
        IPersonAddressRepository PersonAddress { get; }
        IPersonRepository Person { get; }
        IAwardRepository Award { get; }
        ICostCenterScopeRepository CostCenterScope { get; }
        ICostCenterAssignmentRepository CostCenterAssignment { get; }
        ICostCenterRepository CostCenter { get; }
        IHospitalRepository Hospital { get; }
        IParameterscopeRepository Parameterscope { get; }
        IApprovalDelegationRepository ApprovalDelegation { get; }
        IApprovalHistoryRepository ApprovalHistory { get; }
        IApprovalRequestRepository ApprovalRequest { get; }
        IApprovalStepRepository ApprovalStep { get; }
        IApprovalTemplateLevelRepository ApprovalTemplateLevel { get; }
        IApprovalTemplateLevelApproverRepository ApprovalTemplateLevelApprover { get; }
        IApprovalTemplateRepository ApprovalTemplate { get; }
        IAdministrativeAreaDisplayRepository AdministrativeAreaDisplay { get; }
        IAutoNumberRepository AutoNumber { get; }
        IAutoNumberTemplateRepository AutoNumberTemplate { get; }
        IAutoNumberCounterRepository AutoNumberCounter { get; }
        IAutoNumberComponentRepository AutoNumberComponent { get; }
        IAutoNumberLogRepository AutoNumberLog { get; }
        IAutoNumberGenerateRepository AutoNumberGenerate { get; }
        IBankRepository Bank { get; }
        ICityRepository City { get; }
        ICompanyRepository Company { get; }
        ICompanyOfficeRepository CompanyOffice { get; }
        ICountryRepository Country { get; }
        IDistrictRepository District { get; }
        IDocumentNumberRequestRepository DocumentNumberRequest { get; }
        IHolidayRepository Holiday { get; }
        IOrganizationUnitRepository OrganizationUnit { get; }
        IOrganizationUnitScopeRepository OrganizationUnitScope { get; }
        IParameterRepository Parameter { get; }
        IParameterscopeRepository ParameterScope { get; }
        IPostalCodeRepository PostalCode { get; }
        IProvinceRepository Province { get; }
        IProgramRepository Program { get; }
        IReportRepository Report { get; }
        IReportDefinitionRepository ReportDefinition { get; }
        IReportExecutionLogRepository ReportExecutionLog { get; }
        IStandardReferenceRepository StandardReference { get; }
        IStandardReferenceItemRepository StandardReferenceItem { get; }
        IStandardReferenceScopeRepository StandardReferenceScope { get; }
        IStandardReferenceScopeItemRepository StandardReferenceScopeItem { get; }
        IStandardReferenceDisplayRepository StandardReferenceDisplay { get; }
        IStandardReferenceItemDisplayRepository StandardReferenceItemDisplay { get; }
        ISubDistrictRepository SubDistrict { get; }
        IUserRepository User { get; }
        IUserGroupRepository UserGroup { get; }
        IUserGroupScopeRepository UserGroupScope { get; }
        IUserGroupProgramRepository UserGroupProgram { get; }
        IUserCompanyScopeRepository UserCompanyScope { get; }
        IUserScopeRepository UserScope { get; }
        IVwStandardReferenceItemRepository VwStandardReferenceItem { get; }
    }
}