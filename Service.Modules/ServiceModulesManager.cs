using BamboeUp.Audit.Contracts;
using Mapster;
using Contracts;
using Service.Contracts.Modules;
using Service.Contracts.Shell;
using ICostCenterAssignmentService = Service.Contracts.Modules.ICostCenterAssignmentService;
using ICompanyService = Service.Contracts.Modules.ICompanyService;

namespace Service.Modules;

public sealed class ServiceModulesManager : IServiceModulesManager
{

        private readonly IProcessExecutionService _processExecutionService;

        private readonly Lazy<IAdministrativeAreaDisplayService> _administrativeAreaDisplayService;
        private readonly Lazy<IAssetService> _assetService;
        private readonly Lazy<IAttendanceService> _attendanceService;
        private readonly Lazy<IAwardService> _awardService;
        private readonly Lazy<IAwardCriteriaService> _awardCriteriaService;
        private readonly Lazy<IAwardCriteriaRuleService> _awardCriteriaRuleService;
        private readonly Lazy<IBankService> _bankService;
        private readonly Lazy<ICertificationService> _certificationService;
        private readonly Lazy<ICityService> _cityService;
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<ICostCenterAssignmentService> _costCenterAssignmentService;
        private readonly Lazy<ICountryService> _countryService;
        private readonly Lazy<IDistrictService> _districtService;
        private readonly Lazy<IEmployeeService> _employeeService;
        private readonly Lazy<IEmployeeAssetService> _employeeAssetService;
        private readonly Lazy<IEmployeeAssignmentService> _employeeAssignmentService;
        private readonly Lazy<IEmployeeCareerService> _employeeCareerService;
        private readonly Lazy<IEmployeeCertificationService> _employeeCertificationService;
        private readonly Lazy<IEmployeeContractService> _employeeContractService;
        private readonly Lazy<IEmployeeLanguageService> _employeeLanguageService;
        private readonly Lazy<IEmployeeSkillService> _employeeSkillService;
        private readonly Lazy<IEmployeeTrainingService> _employeeTrainingService;
        private readonly Lazy<IHolidayService> _holidayService;
        private readonly Lazy<IHospitalService> _hospitalService;
        private readonly Lazy<IMeetingRoomService> _meetingRoomService;
        private readonly Lazy<IOvertimeRequestService> _overtimeRequestService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<IPersonAddressService> _personAddressService;
        private readonly Lazy<IPersonBankAccountService> _personBankAccountService;
        private readonly Lazy<IPersonContactService> _personContactService;
        private readonly Lazy<IPersonEducationService> _personEducationService;
        private readonly Lazy<IPersonEmergencyContactService> _personEmergencyContactService;
        private readonly Lazy<IPersonFamilyService> _personFamilyService;
        private readonly Lazy<IPersonIdentificationService> _personIdentificationService;
        private readonly Lazy<IPersonPhysicalCharacteristicService> _personPhysicalCharacteristicService;
        private readonly Lazy<IPositionService> _positionService;
        private readonly Lazy<IPositionGradeService> _positionGradeService;
        private readonly Lazy<IPositionLevelService> _positionLevelService;
        private readonly Lazy<IPostalCodeService> _postalCodeService;
        private readonly Lazy<IProvinceService> _provinceService;
        private readonly Lazy<IRoomBookingService> _roomBookingService;
        private readonly Lazy<ISalesAnalysisService> _salesAnalysisService;
        private readonly Lazy<ISalesTransactionService> _salesTransactionService;
        private readonly Lazy<ISalesTransactionPieService> _salesTransactionPieService;
        private readonly Lazy<ISkillService> _skillService;
        private readonly Lazy<IStandardReferenceItemDisplayService> _standardReferenceItemDisplayService;
        private readonly Lazy<ISubDistrictService> _subDistrictService;
        private readonly Lazy<ITempContoh1Service> _tempContoh1Service;
        private readonly Lazy<ITrainingService> _trainingService;
        private readonly Lazy<IWorkItemService> _workItemService;
        private readonly Lazy<IWorkNoticeService> _workNoticeService;
        private readonly Lazy<IWorkTeamService> _workTeamService;
        private readonly Lazy<IWorkTeamCapabilityService> _workTeamCapabilityService;

public ServiceModulesManager(
        IRepositoryManager repositoryManager,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService auditService,
        IUserContext userContext,
        IServiceShellManager shellManager,
        IProcessExecutionService processExecutionService)
    {
            _administrativeAreaDisplayService = new Lazy<IAdministrativeAreaDisplayService>(() => new AdministrativeAreaDisplayService(repositoryManager));
            _assetService = new Lazy<IAssetService>(() => new AssetService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _attendanceService = new Lazy<IAttendanceService>(() => new AttendanceService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _awardService = new Lazy<IAwardService>(() => new AwardService(repositoryManager, logger, transactionManager, auditService, userContext));
            _awardCriteriaService = new Lazy<IAwardCriteriaService>(() => new AwardCriteriaService(repositoryManager, logger, transactionManager, auditService, userContext));
            _awardCriteriaRuleService = new Lazy<IAwardCriteriaRuleService>(() => new AwardCriteriaRuleService(repositoryManager, logger, transactionManager, auditService, userContext));
            _bankService = new Lazy<IBankService>(() => new BankService(repositoryManager, logger, transactionManager, auditService, userContext));
            _certificationService = new Lazy<ICertificationService>(() => new CertificationService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _cityService = new Lazy<ICityService>(() => new CityService(repositoryManager, logger, transactionManager));
            _companyService = new Lazy<ICompanyService>(() => new CompanyService(repositoryManager, logger, transactionManager, auditService, userContext));
            _costCenterAssignmentService = new Lazy<ICostCenterAssignmentService>(() => new CostCenterAssignmentService(repositoryManager, logger, transactionManager, auditService, userContext));
            _countryService = new Lazy<ICountryService>(() => new CountryService(repositoryManager, logger, transactionManager));
            _districtService = new Lazy<IDistrictService>(() => new DistrictService(repositoryManager, logger, transactionManager));
            _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repositoryManager, logger, transactionManager, auditService, userContext));
            _employeeAssetService = new Lazy<IEmployeeAssetService>(() => new EmployeeAssetService(repositoryManager, logger, transactionManager, auditService, userContext));
            _employeeAssignmentService = new Lazy<IEmployeeAssignmentService>(() => new EmployeeAssignmentService(repositoryManager, logger, transactionManager, auditService, userContext));
            _employeeCareerService = new Lazy<IEmployeeCareerService>(() => new EmployeeCareerService(repositoryManager, logger, transactionManager, auditService, userContext));
            _employeeCertificationService = new Lazy<IEmployeeCertificationService>(() => new EmployeeCertificationService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _employeeContractService = new Lazy<IEmployeeContractService>(() => new EmployeeContractService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _employeeLanguageService = new Lazy<IEmployeeLanguageService>(() => new EmployeeLanguageService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _employeeSkillService = new Lazy<IEmployeeSkillService>(() => new EmployeeSkillService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _employeeTrainingService = new Lazy<IEmployeeTrainingService>(() => new EmployeeTrainingService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _holidayService = new Lazy<IHolidayService>(() => new HolidayService(repositoryManager, logger, transactionManager, auditService, userContext));
            _hospitalService = new Lazy<IHospitalService>(() => new HospitalService(repositoryManager, logger, transactionManager, auditService, userContext));
            _meetingRoomService = new Lazy<IMeetingRoomService>(() => new MeetingRoomService(repositoryManager, logger, transactionManager, auditService, userContext));
            _overtimeRequestService = new Lazy<IOvertimeRequestService>(() => new OvertimeRequestService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personService = new Lazy<IPersonService>(() => new PersonService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _personAddressService = new Lazy<IPersonAddressService>(() => new PersonAddressService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personBankAccountService = new Lazy<IPersonBankAccountService>(() => new PersonBankAccountService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personContactService = new Lazy<IPersonContactService>(() => new PersonContactService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personEducationService = new Lazy<IPersonEducationService>(() => new PersonEducationService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personEmergencyContactService = new Lazy<IPersonEmergencyContactService>(() => new PersonEmergencyContactService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personFamilyService = new Lazy<IPersonFamilyService>(() => new PersonFamilyService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personIdentificationService = new Lazy<IPersonIdentificationService>(() => new PersonIdentificationService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personPhysicalCharacteristicService = new Lazy<IPersonPhysicalCharacteristicService>(() => new PersonPhysicalCharacteristicService(repositoryManager, logger, transactionManager, auditService, userContext));
            _positionService = new Lazy<IPositionService>(() => new PositionService(repositoryManager, logger, transactionManager, auditService, userContext));
            _positionGradeService = new Lazy<IPositionGradeService>(() => new PositionGradeService(repositoryManager, logger, transactionManager, auditService, userContext));
            _positionLevelService = new Lazy<IPositionLevelService>(() => new PositionLevelService(repositoryManager, logger, transactionManager, auditService, userContext));
            _postalCodeService = new Lazy<IPostalCodeService>(() => new PostalCodeService(repositoryManager, logger, transactionManager));
            _provinceService = new Lazy<IProvinceService>(() => new ProvinceService(repositoryManager, logger, transactionManager));
            _roomBookingService = new Lazy<IRoomBookingService>(() => new RoomBookingService(repositoryManager, logger, transactionManager, auditService, userContext));
            _salesAnalysisService = new Lazy<ISalesAnalysisService>(() => new SalesAnalysisService(repositoryManager, logger, transactionManager, auditService, userContext));
            _salesTransactionService = new Lazy<ISalesTransactionService>(() => new SalesTransactionService(repositoryManager, logger, transactionManager, auditService, userContext));
            _salesTransactionPieService = new Lazy<ISalesTransactionPieService>(() => new SalesTransactionPieService(repositoryManager, logger, transactionManager, auditService, userContext));
            _skillService = new Lazy<ISkillService>(() => new SkillService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _standardReferenceItemDisplayService = new Lazy<IStandardReferenceItemDisplayService>(() => new StandardReferenceItemDisplayService(repositoryManager));
            _subDistrictService = new Lazy<ISubDistrictService>(() => new SubDistrictService(repositoryManager, logger, transactionManager));
            _tempContoh1Service = new Lazy<ITempContoh1Service>(() => new TempContoh1Service(repositoryManager, logger, transactionManager, auditService, userContext));
            _trainingService = new Lazy<ITrainingService>(() => new TrainingService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _workItemService = new Lazy<IWorkItemService>(() => new WorkItemService(repositoryManager, logger, transactionManager, auditService, userContext));
            _workNoticeService = new Lazy<IWorkNoticeService>(() => new WorkNoticeService(repositoryManager, logger, transactionManager, auditService, userContext));
            _workTeamService = new Lazy<IWorkTeamService>(() => new WorkTeamService(repositoryManager, logger, transactionManager, auditService, userContext));
            _workTeamCapabilityService = new Lazy<IWorkTeamCapabilityService>(() => new WorkTeamCapabilityService(repositoryManager, logger, transactionManager, auditService, userContext));
            _processExecutionService = processExecutionService;
        }

        public IAdministrativeAreaDisplayService AdministrativeAreaDisplayService => _administrativeAreaDisplayService.Value;
        public IAssetService AssetService => _assetService.Value;
        public IAttendanceService AttendanceService => _attendanceService.Value;
        public IAwardService AwardService => _awardService.Value;
        public IAwardCriteriaService AwardCriteriaService => _awardCriteriaService.Value;
        public IAwardCriteriaRuleService AwardCriteriaRuleService => _awardCriteriaRuleService.Value;
        public IBankService BankService => _bankService.Value;
        public ICertificationService CertificationService => _certificationService.Value;
        public ICityService CityService => _cityService.Value;
        public ICompanyService CompanyService => _companyService.Value;
        public ICostCenterAssignmentService CostCenterAssignmentService => _costCenterAssignmentService.Value;
        public ICountryService CountryService => _countryService.Value;
        public IDistrictService DistrictService => _districtService.Value;
        public IEmployeeService EmployeeService => _employeeService.Value;
        public IEmployeeAssetService EmployeeAssetService => _employeeAssetService.Value;
        public IEmployeeAssignmentService EmployeeAssignmentService => _employeeAssignmentService.Value;
        public IEmployeeCareerService EmployeeCareerService => _employeeCareerService.Value;
        public IEmployeeCertificationService EmployeeCertificationService => _employeeCertificationService.Value;
        public IEmployeeContractService EmployeeContractService => _employeeContractService.Value;
        public IEmployeeLanguageService EmployeeLanguageService => _employeeLanguageService.Value;
        public IEmployeeSkillService EmployeeSkillService => _employeeSkillService.Value;
        public IEmployeeTrainingService EmployeeTrainingService => _employeeTrainingService.Value;
        public IHolidayService HolidayService => _holidayService.Value;
        public IHospitalService HospitalService => _hospitalService.Value;
        public IMeetingRoomService MeetingRoomService => _meetingRoomService.Value;
        public IOvertimeRequestService OvertimeRequestService => _overtimeRequestService.Value;
        public IPersonService PersonService => _personService.Value;
        public IPersonAddressService PersonAddressService => _personAddressService.Value;
        public IPersonBankAccountService PersonBankAccountService => _personBankAccountService.Value;
        public IPersonContactService PersonContactService => _personContactService.Value;
        public IPersonEducationService PersonEducationService => _personEducationService.Value;
        public IPersonEmergencyContactService PersonEmergencyContactService => _personEmergencyContactService.Value;
        public IPersonFamilyService PersonFamilyService => _personFamilyService.Value;
        public IPersonIdentificationService PersonIdentificationService => _personIdentificationService.Value;
        public IPersonPhysicalCharacteristicService PersonPhysicalCharacteristicService => _personPhysicalCharacteristicService.Value;
        public IPositionService PositionService => _positionService.Value;
        public IPositionGradeService PositionGradeService => _positionGradeService.Value;
        public IPositionLevelService PositionLevelService => _positionLevelService.Value;
        public IPostalCodeService PostalCodeService => _postalCodeService.Value;
        public IProvinceService ProvinceService => _provinceService.Value;
        public IRoomBookingService RoomBookingService => _roomBookingService.Value;
        public ISalesAnalysisService SalesAnalysisService => _salesAnalysisService.Value;
        public ISalesTransactionService SalesTransactionService => _salesTransactionService.Value;
        public ISalesTransactionPieService SalesTransactionPieService => _salesTransactionPieService.Value;
        public ISkillService SkillService => _skillService.Value;
        public IStandardReferenceItemDisplayService StandardReferenceItemDisplayService => _standardReferenceItemDisplayService.Value;
        public ISubDistrictService SubDistrictService => _subDistrictService.Value;
        public ITempContoh1Service TempContoh1Service => _tempContoh1Service.Value;
        public ITrainingService TrainingService => _trainingService.Value;
        public IWorkItemService WorkItemService => _workItemService.Value;
        public IWorkNoticeService WorkNoticeService => _workNoticeService.Value;
        public IWorkTeamService WorkTeamService => _workTeamService.Value;
        public IWorkTeamCapabilityService WorkTeamCapabilityService => _workTeamCapabilityService.Value;
        public IProcessExecutionService ProcessExecutionService => _processExecutionService;
}
