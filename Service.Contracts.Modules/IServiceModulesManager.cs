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
        IHolidayService HolidayService { get; }
        IAdministrativeAreaDisplayService AdministrativeAreaDisplayService { get; }
        IStandardReferenceItemDisplayService StandardReferenceItemDisplayService { get; }
		IHospitalService HospitalService { get; }
		
		
		
		// ##IServiceManager Start##
    }        
}
