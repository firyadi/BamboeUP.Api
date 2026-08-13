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
        IAwardService AwardService { get; }
        IAwardCriteriaService AwardCriteriaService { get; }
        IAwardCriteriaRuleService AwardCriteriaRuleService { get; }
        ICompanyService CompanyService { get; }
        ICostCenterAssignmentService CostCenterAssignmentService { get; }
        IPersonService PersonService { get; }
        IPersonAddressService PersonAddressService { get; }
        IPersonIdentificationService PersonIdentificationService { get; }
        IPersonEducationService PersonEducationService { get; }
        IPersonEmergencyContactService PersonEmergencyContactService { get; }
        IPersonContactService PersonContactService { get; }
        IPersonFamilyService PersonFamilyService { get; }
        IPersonPhysicalCharacteristicService PersonPhysicalCharacteristicService { get; }
        IPersonWorkExperienceService PersonWorkExperienceService { get; }
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
