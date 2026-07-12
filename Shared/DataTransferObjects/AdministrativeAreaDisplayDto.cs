namespace Shared.DataTransferObjects
{
    public class AdministrativeAreaDisplayDto
    {
        public long CountryId { get; set; }
        public Guid CountryGuid { get; set; }
        public string CountryIso { get; set; } = string.Empty;
        public string CountryIso3 { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        
        public long ProvinceId { get; set; }
        public Guid ProvinceGuid { get; set; }
        public string ProvinceName { get; set; } = string.Empty;
        
        public long CityId { get; set; }
        public Guid CityGuid { get; set; }
        public string CityName { get; set; } = string.Empty;
        
        public long DistrictId { get; set; }
        public Guid DistrictGuid { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        
        public long SubDistrictId { get; set; }
        public Guid SubDistrictGuid { get; set; }
        public string SubDistrictName { get; set; } = string.Empty;
        
        public long PostalCodeId { get; set; }
        public Guid PostalCodeGuid { get; set; }
        public string PostalCode { get; set; } = string.Empty;
    }
}
