using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("CompanyOffice", Schema = "app")]
    public class CompanyOffice
    {
        [Column("CompanyOfficeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CompanyOfficeId { get; set; }

        [Key]
        [Column("CompanyOfficeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CompanyOfficeGuid { get; set; } = Guid.NewGuid();

        [Column("CompanyId")]
        public long CompanyId { get; set; } = 0;

        [Column("CompanyGuid")]
        public Guid CompanyGuid { get; set; } = Guid.Empty;

        [Required]
        [MaxLength(150)]
        public string CompanyOfficeName { get; set; } = string.Empty;

        [Required]
        public long SrAddressType { get; set; }

        [Required]
        public long CountryId { get; set; }

        [Required]
        public long StateId { get; set; }

        [Required]
        public long CityId { get; set; }

        [Required]
        [MaxLength(8)]
        public string PostalCodeId { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; }  = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long UpdatedById { get; set; } = 0;

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long DeletedById { get; set; } = 0;

        public DateTime? DeletedTime { get; set; }

        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [NotMapped]
        public string? AddressTypeName { get; set; }

        [NotMapped]
        public string? CityName { get; set; }

        [NotMapped]
        public string? StateName { get; set; }

        [NotMapped]
        public string? CountryName { get; set; }
    }
}

