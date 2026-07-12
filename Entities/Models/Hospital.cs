using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Hospital", Schema = "app")]
    public class Hospital
    {
        [Column("HospitalId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HospitalId { get; set; }

        [Key]
        [Column("HospitalGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid HospitalGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string HospitalName { get; set; } = string.Empty;

        public string HospitalCode { get; set; } = string.Empty;

        public string ShortName { get; set; } = string.Empty;

        public string LicenseNo { get; set; } = string.Empty;

        public string HospitalType { get; set; } = string.Empty;

        public string HospitalClass { get; set; } = string.Empty;

        public string PhoneNo { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Website { get; set; } = string.Empty;

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
