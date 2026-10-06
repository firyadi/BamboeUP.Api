using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("CompanyOffice", Schema = "app")]
    public partial class CompanyOffice
    {
        [Column("CompanyOfficeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CompanyOfficeId { get; set; }

        [Key]
        [Column("CompanyOfficeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CompanyOfficeGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long CompanyId { get; set; }

        [Required]
        [Column("CompanyGuid")]
        public Guid CompanyGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string CompanyOfficeName { get; set; } = string.Empty;

        public long SrAddressType { get; set; }

        public long CountryId { get; set; }

        public long StateId { get; set; }

        public long CityId { get; set; }

        [Required]
        public string PostalCodeId { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public int? RadiusMeters { get; set; }

public string? CityName { get; set; }
public string? CountryName { get; set; }
public string? PostalCodeName { get; set; }
public string? AddressTypeName { get; set; }
public string? StateName { get; set; }


        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long UpdatedById { get; set; } = 0;

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long DeletedById { get; set; } = 0;

        public DateTime? DeletedTime { get; set; }
    }
}
