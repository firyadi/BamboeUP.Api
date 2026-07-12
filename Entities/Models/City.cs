using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("City", Schema = "core")]
    public class City
    {
        [Column("CityId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CityId { get; set; }

        [Key]
        [Column("CityGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CityGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string CityName { get; set; } = string.Empty;

        [Required]
        public long ProvinceId { get; set; }

        [Required]
        public int StatusId { get; set; }  = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Required]
        public long CreatedById { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }

        public long? UpdatedById { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public long? DeletedById { get; set; }

        public DateTime? DeletedTime { get; set; }
    }
}
