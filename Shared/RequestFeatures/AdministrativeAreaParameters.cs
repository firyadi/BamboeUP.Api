namespace Shared.RequestFeatures
{
    public class AdministrativeAreaParameters
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string ProvinceName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string SubDistrictName { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }
}
