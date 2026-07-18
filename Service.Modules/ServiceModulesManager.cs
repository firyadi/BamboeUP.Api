using BamboeUp.Audit.Contracts;
using Mapster;
using Contracts;
using Service.Contracts.Modules;
using Service.Contracts.Shell;

namespace Service.Modules;

public sealed class ServiceModulesManager : IServiceModulesManager
{
    private readonly Lazy<IBankService> _bankService;
    private readonly Lazy<ICityService> _cityService;
    private readonly Lazy<ICountryService> _countryService;
    private readonly Lazy<IDistrictService> _districtService;
    private readonly Lazy<IProvinceService> _provinceService;
    private readonly Lazy<ISubDistrictService> _subDistrictService;
    private readonly Lazy<IPostalCodeService> _postalCodeService;
    private readonly Lazy<IHolidayService> _holidayService;
    private readonly Lazy<IAdministrativeAreaDisplayService> _administrativeAreaDisplayService;
    private readonly Lazy<IStandardReferenceItemDisplayService> _standardReferenceItemDisplayService;

            private readonly Lazy<IHospitalService> _hospitalService;

        private readonly Lazy<IAwardService> _awardService;

        private readonly Lazy<IPersonService> _personService;

        private readonly Lazy<IPersonAddressService> _personAddressService;

        private readonly Lazy<IPersonIdentificationService> _personIdentificationService;

        private readonly Lazy<IPersonEducationService> _personEducationService;

        private readonly Lazy<IPersonEmergencyContactService> _personEmergencyContactService;

        private readonly Lazy<IPersonContactService> _personContactService;

        private readonly Lazy<IPersonFamilyService> _personFamilyService;

        private readonly Lazy<IPersonPhysicalCharacteristicService> _personPhysicalCharacteristicService;

        private readonly Lazy<IPersonWorkExperienceService> _personWorkExperienceService;

public ServiceModulesManager(
        IRepositoryManager repositoryManager,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService auditService,
        IUserContext userContext,
        IServiceShellManager shellManager)
    {
            _personWorkExperienceService = new Lazy<IPersonWorkExperienceService>(() => new PersonWorkExperienceService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personPhysicalCharacteristicService = new Lazy<IPersonPhysicalCharacteristicService>(() => new PersonPhysicalCharacteristicService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personFamilyService = new Lazy<IPersonFamilyService>(() => new PersonFamilyService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personContactService = new Lazy<IPersonContactService>(() => new PersonContactService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personEmergencyContactService = new Lazy<IPersonEmergencyContactService>(() => new PersonEmergencyContactService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personEducationService = new Lazy<IPersonEducationService>(() => new PersonEducationService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personIdentificationService = new Lazy<IPersonIdentificationService>(() => new PersonIdentificationService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personAddressService = new Lazy<IPersonAddressService>(() => new PersonAddressService(repositoryManager, logger, transactionManager, auditService, userContext));
            _personService = new Lazy<IPersonService>(() => new PersonService(repositoryManager, logger, transactionManager, auditService, userContext, shellManager.FileStorageService));
            _awardService = new Lazy<IAwardService>(() => new AwardService(repositoryManager, logger, transactionManager, auditService, userContext));
            _hospitalService = new Lazy<IHospitalService>(() => new HospitalService(repositoryManager, logger, transactionManager, auditService, userContext));
        _bankService = new Lazy<IBankService>(() =>
            new BankService(repositoryManager, logger, transactionManager, auditService, userContext));
        _cityService = new Lazy<ICityService>(() =>
            new CityService(repositoryManager, logger, transactionManager));
        _countryService = new Lazy<ICountryService>(() =>
            new CountryService(repositoryManager, logger, transactionManager));
        _districtService = new Lazy<IDistrictService>(() =>
            new DistrictService(repositoryManager, logger, transactionManager));
        _provinceService = new Lazy<IProvinceService>(() =>
            new ProvinceService(repositoryManager, logger, transactionManager));
        _subDistrictService = new Lazy<ISubDistrictService>(() =>
            new SubDistrictService(repositoryManager, logger, transactionManager));
        _postalCodeService = new Lazy<IPostalCodeService>(() =>
            new PostalCodeService(repositoryManager, logger, transactionManager));
        _holidayService = new Lazy<IHolidayService>(() =>
            new HolidayService(repositoryManager, logger, transactionManager));
        _administrativeAreaDisplayService = new Lazy<IAdministrativeAreaDisplayService>(() =>
            new AdministrativeAreaDisplayService(repositoryManager));
        _standardReferenceItemDisplayService = new Lazy<IStandardReferenceItemDisplayService>(() =>
            new StandardReferenceItemDisplayService(repositoryManager));
    }

    public IBankService BankService => _bankService.Value;
    public ICityService CityService => _cityService.Value;
    public ICountryService CountryService => _countryService.Value;
    public IDistrictService DistrictService => _districtService.Value;
    public IProvinceService ProvinceService => _provinceService.Value;
    public ISubDistrictService SubDistrictService => _subDistrictService.Value;
    public IPostalCodeService PostalCodeService => _postalCodeService.Value;
    public IHolidayService HolidayService => _holidayService.Value;
    public IAdministrativeAreaDisplayService AdministrativeAreaDisplayService => _administrativeAreaDisplayService.Value;
    public IStandardReferenceItemDisplayService StandardReferenceItemDisplayService => _standardReferenceItemDisplayService.Value;
        public IHospitalService HospitalService => _hospitalService.Value;
        public IAwardService AwardService => _awardService.Value;
        public IPersonService PersonService => _personService.Value;
        public IPersonAddressService PersonAddressService => _personAddressService.Value;
        public IPersonIdentificationService PersonIdentificationService => _personIdentificationService.Value;
        public IPersonEducationService PersonEducationService => _personEducationService.Value;
        public IPersonEmergencyContactService PersonEmergencyContactService => _personEmergencyContactService.Value;
        public IPersonContactService PersonContactService => _personContactService.Value;
        public IPersonFamilyService PersonFamilyService => _personFamilyService.Value;
        public IPersonPhysicalCharacteristicService PersonPhysicalCharacteristicService => _personPhysicalCharacteristicService.Value;
        public IPersonWorkExperienceService PersonWorkExperienceService => _personWorkExperienceService.Value;
}
