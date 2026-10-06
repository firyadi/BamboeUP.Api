namespace Service.Contracts.Modules
{
    public interface IServiceModulesManager
    {
		// ##IServiceManager Start##
        IAdministrativeAreaDisplayService AdministrativeAreaDisplayService { get; }
        IAssetService AssetService { get; }
        IAttendanceService AttendanceService { get; }
        IAwardService AwardService { get; }
        IAwardCriteriaService AwardCriteriaService { get; }
        IAwardCriteriaRuleService AwardCriteriaRuleService { get; }
        IBankService BankService { get; }
        ICertificationService CertificationService { get; }
        ICityService CityService { get; }
        ICompanyService CompanyService { get; }
        ICostCenterAssignmentService CostCenterAssignmentService { get; }
        ICountryService CountryService { get; }
        IDistrictService DistrictService { get; }
        IEmployeeService EmployeeService { get; }
        IEmployeeAssetService EmployeeAssetService { get; }
        IEmployeeAssignmentService EmployeeAssignmentService { get; }
        IEmployeeCareerService EmployeeCareerService { get; }
        IEmployeeCertificationService EmployeeCertificationService { get; }
        IEmployeeContractService EmployeeContractService { get; }
        IEmployeeLanguageService EmployeeLanguageService { get; }
        IEmployeeSkillService EmployeeSkillService { get; }
        IEmployeeTrainingService EmployeeTrainingService { get; }
        IHolidayService HolidayService { get; }
        IHospitalService HospitalService { get; }
        IMeetingRoomService MeetingRoomService { get; }
        IOvertimeRequestService OvertimeRequestService { get; }
        IPersonService PersonService { get; }
        IPersonAddressService PersonAddressService { get; }
        IPersonBankAccountService PersonBankAccountService { get; }
        IPersonContactService PersonContactService { get; }
        IPersonEducationService PersonEducationService { get; }
        IPersonEmergencyContactService PersonEmergencyContactService { get; }
        IPersonFamilyService PersonFamilyService { get; }
        IPersonIdentificationService PersonIdentificationService { get; }
        IPersonPhysicalCharacteristicService PersonPhysicalCharacteristicService { get; }
        IPositionService PositionService { get; }
        IPositionGradeService PositionGradeService { get; }
        IPositionLevelService PositionLevelService { get; }
        IPostalCodeService PostalCodeService { get; }
        IProvinceService ProvinceService { get; }
        IRoomBookingService RoomBookingService { get; }
        ISalesAnalysisService SalesAnalysisService { get; }
        ISalesTransactionService SalesTransactionService { get; }
        ISalesTransactionPieService SalesTransactionPieService { get; }
        ISkillService SkillService { get; }
        IStandardReferenceItemDisplayService StandardReferenceItemDisplayService { get; }
        ISubDistrictService SubDistrictService { get; }
        ITempContoh1Service TempContoh1Service { get; }
        ITrainingService TrainingService { get; }
        IWorkItemService WorkItemService { get; }
        IWorkNoticeService WorkNoticeService { get; }
        IWorkTeamService WorkTeamService { get; }
        IWorkTeamCapabilityService WorkTeamCapabilityService { get; }
        IProcessExecutionService ProcessExecutionService { get; }
		
		
		
		// ##IServiceManager Start##
}
}
