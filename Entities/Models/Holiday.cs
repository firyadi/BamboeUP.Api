using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Holiday", Schema = "app")]
    public partial class Holiday
    {
        [Column("HolidayId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HolidayId { get; set; }

        [Key]
        [Column("HolidayGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid HolidayGuid { get; set; } = Guid.NewGuid();

        public DateTime HolidayDate { get; set; }

        [Required]
        public string HolidayName { get; set; } = string.Empty;

        public string? HolidayDescription { get; set; }

        public long? SrHolidayType { get; set; }

        public bool IsNational { get; set; }

        public bool IsCompanyHoliday { get; set; }



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
