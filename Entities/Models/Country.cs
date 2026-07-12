using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Country", Schema = "core")]
    public class Country
    {
        [Column("CountryId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CountryId { get; set; }

        [Key]
        [Column("CountryGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CountryGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(2)]
        [Column(TypeName = "char(2)")]
        public string CountryIso { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string CountryName { get; set; } = string.Empty;

        [MaxLength(3)]
        [Column(TypeName = "char(3)")]
        public string? CountryIso3 { get; set; }

        [Required]
        public int PhoneCode { get; set; }

        [Required]
        [MaxLength(3)]
        [Column(TypeName = "char(3)")]
        public string CurrencyCode { get; set; } = string.Empty;

        [Required]
        public int StatusId { get; set; }  = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Required]
        public long CreatedById { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }  = DateTime.UtcNow;

        public long? UpdatedById { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public long? DeletedById { get; set; }

        public DateTime? DeletedTime { get; set; }
    }
}
