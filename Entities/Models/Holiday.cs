using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Holiday", Schema = "app")]
    public class Holiday
    {
        [Column("HolidayId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HolidayId { get; set; }

        [Key]
        [Column("HolidayGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid HolidayGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(4)]
        public string YearPeriode { get; set; }

        [Required]
        public DateTime HolidayDates { get; set; }

        [Required]
        [MaxLength(500)]
        public string Note { get; set; }

        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

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
    }
}
