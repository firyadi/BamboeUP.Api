using System;
using Contracts.Approval;

namespace Contracts
{
    public interface IRepositoryManager
    {
        // ##IRepositoryManager##
        IEmployeeTrainingRepository EmployeeTraining { get; }
        IEmployeeSkillRepository EmployeeSkill { get; }
        IEmployeeLanguageRepository EmployeeLanguage { get; }
        IEmployeeContractRepository EmployeeContract { get; }
        IEmployeeCertificationRepository EmployeeCertification { get; }
        IEmployeeCareerRepository EmployeeCareer { get; }
        IEmployeeAssignmentRepository EmployeeAssignment { get; }
        IEmployeeAssetRepository EmployeeAsset { get; }
        IEmployeeRepository Employee { get; }
        IWorkItemRepository WorkItem { get; }
        IRoomBookingRepository RoomBooking { get; }
        IPersonWorkExperienceRepository PersonWorkExperience { get; }
        IPersonPhysicalCharacteristicRepository PersonPhysicalCharacteristic { get; }
        IPersonIdentificationRepository PersonIdentification { get; }
        IPersonFamilyRepository PersonFamily { get; }
        IPersonEmergencyContactRepository PersonEmergencyContact { get; }
        IPersonEducationRepository PersonEducation { get; }
        IPersonContactRepository PersonContact { get; }
        IPersonAddressRepository PersonAddress { get; }
        ICostCenterAssignmentRepository CostCenterAssignment { get; }
        ICompanyRepository Company { get; }
        IPersonRepository Person { get; }
        IMeetingRoomRepository MeetingRoom { get; }
        IAwardCriteriaRuleRepository AwardCriteriaRule { get; }
        IAwardCriteriaRepository AwardCriteria { get; }
        IAwardRepository Award { get; }
        ICompanyOfficeRepository CompanyOffice { get; }
        IHolidayRepository Holiday { get; }
        ICertificationRepository Certification { get; }
        IAssetRepository Asset { get; }
        ITrainingRepository Training { get; }
        ISkillRepository Skill { get; }
        IPersonBankAccountRepository PersonBankAccount { get; }
        IFileStorageRepository FileStorage { get; }
        ICostCenterScopeRepository CostCenterScope { get; }
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
        ICountryRepository Country { get; }
        IDistrictRepository District { get; }
        IDocumentNumberRequestRepository DocumentNumberRequest { get; }
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