namespace Shared.DataTransferObjects
{
    public class AdministrativeAreaDisplayDto
    {
        public long CountryId { get; set; }
        public Guid CountryGuid { get; set; }
        public string CountryIso { get; set; }
        public string CountryIso3 { get; set; }
        public string CountryName { get; set; }
        public string PhoneCode { get; set; }
        public string CurrencyCode { get; set; }
        
        public long ProvinceId { get; set; }
        public Guid ProvinceGuid { get; set; }
        public string ProvinceName { get; set; }
        
        public long CityId { get; set; }
        public Guid CityGuid { get; set; }
        public string CityName { get; set; }
        
        public long DistrictId { get; set; }
        public Guid DistrictGuid { get; set; }
        public string DistrictName { get; set; }
        
        public long SubDistrictId { get; set; }
        public Guid SubDistrictGuid { get; set; }
        public string SubDistrictName { get; set; }
        
        public long PostalCodeId { get; set; }
        public Guid PostalCodeGuid { get; set; }
        public string PostalCode { get; set; }
    }
}
