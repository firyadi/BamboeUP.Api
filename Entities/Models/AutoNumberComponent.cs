using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("AutoNumberComponent", Schema = "app")]
    public class AutoNumberComponent
    {
        [Column("AutoNumberComponentId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AutoNumberComponentId { get; set; }

        [Key]
        [Column("AutoNumberComponentGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AutoNumberComponentGuid { get; set; } = Guid.NewGuid();

        [Column("AutoNumberTemplateId")]
        public long AutoNumberTemplateId { get; set; }

        [Required]
        [Column("SeqNo")]
        public short SeqNo { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("ComponentType")]
        public string ComponentType { get; set; }
        // STATIC / COUNTER / YEAR / MONTH / DAY / COMPANY / OFFICE / DEPT

        [MaxLength(50)]
        [Column("StaticValue")]
        public string StaticValue { get; set; }

        /// <summary>Format for date/number components (e.g. yyyy, MM, dd)</summary>
        [MaxLength(20)]
        [Column("Format")]
        public string Format { get; set; }

        [Column("CounterLength")]
        public short? CounterLength { get; set; }

        [Required]
        [Column("IsResetKey")]
        public bool IsResetKey { get; set; } = false;

        // Standard BamboeUP Audit Fields
        [Column("StatusId")]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; }

        [Column("UpdatedById")]
        public long? UpdatedById { get; set; }

        [Column("UpdatedTime")]
        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long? DeletedById { get; set; }

        [Column("DeletedTime")]
        public DateTime? DeletedTime { get; set; }
    }
}
