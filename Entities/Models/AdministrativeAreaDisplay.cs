using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("vw_AdministrativeArea", Schema = "core")]
    public class AdministrativeAreaDisplay
    {
        [Column("CountryId")]
        public long CountryId { get; set; }

        [Column("CountryGuid")]
        public Guid CountryGuid { get; set; }

        [Column("CountryIso")]
        public string CountryIso { get; set; }

        [Column("CountryIso3")]
        public string CountryIso3 { get; set; }

        [Column("CountryName")]
        public string CountryName { get; set; }

        [Column("PhoneCode")]
        public string PhoneCode { get; set; }

        [Column("CurrencyCode")]
        public string CurrencyCode { get; set; }

        [Column("ProvinceId")]
        public long ProvinceId { get; set; }

        [Column("ProvinceGuid")]
        public Guid ProvinceGuid { get; set; }

        [Column("ProvinceName")]
        public string ProvinceName { get; set; }

        [Column("CityId")]
        public long CityId { get; set; }

        [Column("CityGuid")]
        public Guid CityGuid { get; set; }

        [Column("CityName")]
        public string CityName { get; set; }

        [Column("DistrictId")]
        public long DistrictId { get; set; }

        [Column("DistrictGuid")]
        public Guid DistrictGuid { get; set; }

        [Column("DistrictName")]
        public string DistrictName { get; set; }

        [Column("SubDistrictId")]
        public long SubDistrictId { get; set; }

        [Column("SubDistrictGuid")]
        public Guid SubDistrictGuid { get; set; }

        [Column("SubDistrictName")]
        public string SubDistrictName { get; set; }

        [Column("PostalCodeId")]
        public long PostalCodeId { get; set; }

        [Column("PostalCodeGuid")]
        public Guid PostalCodeGuid { get; set; }

        [Column("PostalCode")]
        public string PostalCode { get; set; }
    }
}
