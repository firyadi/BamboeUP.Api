using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("District", Schema = "core")]
    public class District
    {
        [Column("DistrictId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DistrictId { get; set; }

        [Key]
        [Column("DistrictGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DistrictGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string DistrictName { get; set; } = string.Empty;

        [Required]
        public long CityId { get; set; }

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
