using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Training", Schema = "app")]
    public partial class Training
    {
        [Column("TrainingId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TrainingId { get; set; }

        [Key]
        [Column("TrainingGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TrainingGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string TrainingCode { get; set; } = string.Empty;

        [Required]
        public string TrainingName { get; set; } = string.Empty;

        public string? TrainingShortName { get; set; }

        public long SrTrainingCategory { get; set; }

        public long SrTrainingType { get; set; }

        public long? SrTrainingLevel { get; set; }

        public decimal? DurationHour { get; set; }

        public int? ValidityMonth { get; set; }

        public long? SrTrainingProvider { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsCertification { get; set; }

        public bool IsExternal { get; set; }

        public long? FileStorageId { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }



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
