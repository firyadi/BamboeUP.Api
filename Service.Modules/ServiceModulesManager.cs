using BamboeUp.Audit.Contracts;
using Mapster;
using Contracts;
using Service.Contracts.Modules;

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

public ServiceModulesManager(
        IRepositoryManager repositoryManager,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService auditService,
        IUserContext userContext)
    {
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
}
