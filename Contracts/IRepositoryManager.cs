using System;
using Contracts.Approval;

namespace Contracts
{
    public interface IRepositoryManager
    {
        // ##IRepositoryManager##
        IAdministrativeAreaDisplayRepository AdministrativeAreaDisplay { get; }
        IApprovalDelegationRepository ApprovalDelegation { get; }
        IApprovalHistoryRepository ApprovalHistory { get; }
        IApprovalRequestRepository ApprovalRequest { get; }
        IApprovalStepRepository ApprovalStep { get; }
        IApprovalTemplateRepository ApprovalTemplate { get; }
        IApprovalTemplateLevelRepository ApprovalTemplateLevel { get; }
        IApprovalTemplateLevelApproverRepository ApprovalTemplateLevelApprover { get; }
        IAssetRepository Asset { get; }
        IAttendanceRepository Attendance { get; }
        IAutoNumberRepository AutoNumber { get; }
        IAutoNumberComponentRepository AutoNumberComponent { get; }
        IAutoNumberCounterRepository AutoNumberCounter { get; }
        IAutoNumberGenerateRepository AutoNumberGenerate { get; }
        IAutoNumberLogRepository AutoNumberLog { get; }
        IAutoNumberTemplateRepository AutoNumberTemplate { get; }
        IAwardRepository Award { get; }
        IAwardCriteriaRepository AwardCriteria { get; }
        IAwardCriteriaRuleRepository AwardCriteriaRule { get; }
        IBankRepository Bank { get; }
        ICertificationRepository Certification { get; }
        ICityRepository City { get; }
        ICompanyRepository Company { get; }
        ICompanyOfficeRepository CompanyOffice { get; }
        ICostCenterRepository CostCenter { get; }
        ICostCenterAssignmentRepository CostCenterAssignment { get; }
        ICostCenterScopeRepository CostCenterScope { get; }
        ICountryRepository Country { get; }
        IDistrictRepository District { get; }
        IDocumentNumberRequestRepository DocumentNumberRequest { get; }
        IEmployeeRepository Employee { get; }
        IEmployeeAssetRepository EmployeeAsset { get; }
        IEmployeeAssignmentRepository EmployeeAssignment { get; }
        IEmployeeCareerRepository EmployeeCareer { get; }
        IEmployeeCertificationRepository EmployeeCertification { get; }
        IEmployeeContractRepository EmployeeContract { get; }
        IEmployeeLanguageRepository EmployeeLanguage { get; }
        IEmployeeSkillRepository EmployeeSkill { get; }
        IEmployeeTrainingRepository EmployeeTraining { get; }
        IFileStorageRepository FileStorage { get; }
        IHolidayRepository Holiday { get; }
        IHospitalRepository Hospital { get; }
        IMeetingRoomRepository MeetingRoom { get; }
        IOrganizationUnitRepository OrganizationUnit { get; }
        IOrganizationUnitScopeRepository OrganizationUnitScope { get; }
        IOvertimeRequestRepository OvertimeRequest { get; }
        IParameterRepository Parameter { get; }
        IParameterscopeRepository ParameterScope { get; }
        IPersonRepository Person { get; }
        IPersonAddressRepository PersonAddress { get; }
        IPersonBankAccountRepository PersonBankAccount { get; }
        IPersonContactRepository PersonContact { get; }
        IPersonEducationRepository PersonEducation { get; }
        IPersonEmergencyContactRepository PersonEmergencyContact { get; }
        IPersonFamilyRepository PersonFamily { get; }
        IPersonIdentificationRepository PersonIdentification { get; }
        IPersonPhysicalCharacteristicRepository PersonPhysicalCharacteristic { get; }
        IPositionRepository Position { get; }
        IPositionGradeRepository PositionGrade { get; }
        IPositionLevelRepository PositionLevel { get; }
        IPostalCodeRepository PostalCode { get; }
        IProgramRepository Program { get; }
        IProvinceRepository Province { get; }
        IReportRepository Report { get; }
        IReportDefinitionRepository ReportDefinition { get; }
        IReportExecutionLogRepository ReportExecutionLog { get; }
        IRoomBookingRepository RoomBooking { get; }
        ISalesAnalysisRepository SalesAnalysis { get; }
        ISalesTransactionRepository SalesTransaction { get; }
        ISalesTransactionPieRepository SalesTransactionPie { get; }
        ISkillRepository Skill { get; }
        IStandardReferenceRepository StandardReference { get; }
        IStandardReferenceDisplayRepository StandardReferenceDisplay { get; }
        IStandardReferenceItemRepository StandardReferenceItem { get; }
        IStandardReferenceItemDisplayRepository StandardReferenceItemDisplay { get; }
        IStandardReferenceScopeRepository StandardReferenceScope { get; }
        IStandardReferenceScopeItemRepository StandardReferenceScopeItem { get; }
        ISubDistrictRepository SubDistrict { get; }
        ITempContoh1Repository TempContoh1 { get; }
        ITrainingRepository Training { get; }
        IUserRepository User { get; }
        IUserCompanyScopeRepository UserCompanyScope { get; }
        IUserGroupRepository UserGroup { get; }
        IUserGroupProgramRepository UserGroupProgram { get; }
        IUserGroupScopeRepository UserGroupScope { get; }
        IUserScopeRepository UserScope { get; }
        IVwStandardReferenceItemRepository VwStandardReferenceItem { get; }
        IWorkItemRepository WorkItem { get; }
        IWorkNoticeRepository WorkNotice { get; }
        IWorkTeamRepository WorkTeam { get; }
        IWorkTeamCapabilityRepository WorkTeamCapability { get; }
    }
}