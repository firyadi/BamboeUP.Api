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

        public string? HospitalCode { get; set; }

        public string? ShortName { get; set; }

        public string? LicenseNo { get; set; }

        public string? HospitalType { get; set; }

        public string? HospitalClass { get; set; }

        public string? PhoneNo { get; set; }

        public string? Email { get; set; }

        public string? Website { get; set; }


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
