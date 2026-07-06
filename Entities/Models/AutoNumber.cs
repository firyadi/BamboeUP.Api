using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("AutoNumber", Schema = "app")]
    public class AutoNumber
    {
        [Column("AutoNumberId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AutoNumberId { get; set; }

        [Key]
        [Column("AutoNumberGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AutoNumberGuid { get; set; } = Guid.NewGuid();

        [MaxLength(7)]
        public string Prefik { get; set; }

        [MaxLength(1)]
        public string SeparatorAfterPrefik { get; set; }

        public bool? IsUsedDepartment { get; set; }

        [MaxLength(1)]
        public string SeparatorAfterDept { get; set; }

        public bool? IsUsedYear { get; set; }

        public byte? YearDigit { get; set; }

        [MaxLength(1)]
        public string SeparatorAfterYear { get; set; }

        public bool? IsUsedMonth { get; set; }

        public bool? IsMonthInRomawi { get; set; }

        [MaxLength(1)]
        public string SeparatorAfterMonth { get; set; }

        public bool? IsUsedDay { get; set; }

        [MaxLength(1)]
        public string SeparatorAfterDay { get; set; }

        public byte? NumberLength { get; set; }

        public byte? NumberGroupLength { get; set; }

        [MaxLength(1)]
        public string NumberGroupSeparator { get; set; }

        [MaxLength(20)]
        public string NumberFormat { get; set; }

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
