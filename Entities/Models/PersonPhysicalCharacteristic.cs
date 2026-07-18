using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PersonPhysicalCharacteristic", Schema = "app")]
    public partial class PersonPhysicalCharacteristic
    {
        [Column("PersonPhysicalCharacteristicId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PersonPhysicalCharacteristicId { get; set; }

        [Key]
        [Column("PersonPhysicalCharacteristicGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonPhysicalCharacteristicGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long PersonId { get; set; }

        public long SrPhysicalCharacteristic { get; set; }

        [Required]
        public string PhysicalValue { get; set; } = string.Empty;

        public long? SrMeasurementUnit { get; set; }

        public DateTime RecordedDate { get; set; }

        public string? Remarks { get; set; }



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
