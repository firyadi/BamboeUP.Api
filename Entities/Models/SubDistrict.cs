using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("SubDistrict", Schema = "core")]
    public class SubDistrict
    {
        [Column("SubDistrictId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SubDistrictId { get; set; }

        [Key]
        [Column("SubDistrictGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SubDistrictGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string SubDistrictName { get; set; } = string.Empty;

        [Required]
        public long DistrictId { get; set; }

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
