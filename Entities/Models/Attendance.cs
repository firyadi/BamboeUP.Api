using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Attendance", Schema = "app")]
    public partial class Attendance
    {
        [Column("AttendanceId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AttendanceId { get; set; }

        [Key]
        [Column("AttendanceGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AttendanceGuid { get; set; } = Guid.NewGuid();

        public long CompanyId { get; set; }

        public long CompanyOfficeId { get; set; }

        public long PersonId { get; set; }

        public DateTime AttendanceDate { get; set; }

        public DateTime? CheckinTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public string? CheckinChannel { get; set; }

        public string? CheckOutChannel { get; set; }

        public long? CheckinFileStorageId { get; set; }

        public long? CheckOutFileStorageId { get; set; }

        public decimal? CheckinLatitude { get; set; }

        public decimal? CheckinLongitude { get; set; }

        public int? CheckinAccuracyMeters { get; set; }

        public decimal? CheckOutLatitude { get; set; }

        public decimal? CheckOutLongitude { get; set; }

        public int? CheckOutAccuracyMeters { get; set; }

        public string? Note { get; set; }
public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? PersonName { get; set; }



        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public long SyncVersion { get; set; } = 1;

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
