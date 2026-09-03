namespace Service.Contracts.Modules
{
    public interface IServiceModulesManager
    {
		// ##IServiceManager Start##
        IBankService BankService { get; }
        ICityService CityService { get; }
        ICountryService CountryService { get; }
        IDistrictService DistrictService { get; }
        IProvinceService ProvinceService { get; }
        ISubDistrictService SubDistrictService { get; }
        IPostalCodeService PostalCodeService { get; }
        IAdministrativeAreaDisplayService AdministrativeAreaDisplayService { get; }
        IStandardReferenceItemDisplayService StandardReferenceItemDisplayService { get; }
		IHospitalService HospitalService { get; }
		
		
		
		// ##IServiceManager Start##
        IPersonBankAccountService PersonBankAccountService { get; }
        ISkillService SkillService { get; }
        ITrainingService TrainingService { get; }
        IAssetService AssetService { get; }
        ICertificationService CertificationService { get; }
        IHolidayService HolidayService { get; }
        IProcessExecutionService ProcessExecutionService { get; }
        IAwardService AwardService { get; }
        IAwardCriteriaService AwardCriteriaService { get; }
        IAwardCriteriaRuleService AwardCriteriaRuleService { get; }
        IMeetingRoomService MeetingRoomService { get; }
        IPersonService PersonService { get; }
        ICompanyService CompanyService { get; }
        ICostCenterAssignmentService CostCenterAssignmentService { get; }
        IPersonAddressService PersonAddressService { get; }
        IPersonContactService PersonContactService { get; }
        IPersonEducationService PersonEducationService { get; }
        IPersonEmergencyContactService PersonEmergencyContactService { get; }
        IPersonFamilyService PersonFamilyService { get; }
        IPersonIdentificationService PersonIdentificationService { get; }
        IPersonPhysicalCharacteristicService PersonPhysicalCharacteristicService { get; }
        IPersonWorkExperienceService PersonWorkExperienceService { get; }
        IRoomBookingService RoomBookingService { get; }
        IWorkItemService WorkItemService { get; }
        IEmployeeService EmployeeService { get; }
        IEmployeeAssetService EmployeeAssetService { get; }
        IEmployeeAssignmentService EmployeeAssignmentService { get; }
        IEmployeeCareerService EmployeeCareerService { get; }
        IEmployeeCertificationService EmployeeCertificationService { get; }
        IEmployeeContractService EmployeeContractService { get; }
        IEmployeeLanguageService EmployeeLanguageService { get; }
        IEmployeeSkillService EmployeeSkillService { get; }
        IEmployeeTrainingService EmployeeTrainingService { get; }
}
}
