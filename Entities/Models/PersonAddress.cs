using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PersonAddress", Schema = "app")]
    public class PersonAddress
    {
        [Column("PersonAddressId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PersonAddressId { get; set; }

        [Key]
        [Column("PersonAddressGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonAddressGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long PersonId { get; set; }

        public long SrAddressType { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        public long CountryId { get; set; }

        public long ProvinceId { get; set; }

        public long CityId { get; set; }

public string? CityName { get; set; }
public string? CountryName { get; set; }
public string? ProvinceName { get; set; }


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
