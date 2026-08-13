using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("EmployeeTraining", Schema = "app")]
    public partial class EmployeeTraining
    {
        [Column("EmployeeTrainingId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeTrainingId { get; set; }

        [Key]
        [Column("EmployeeTrainingGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeTrainingGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long EmployeeId { get; set; }

        public long TrainingId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? DurationHour { get; set; }

        public decimal? Score { get; set; }

        public string? Grade { get; set; }

        public bool IsPassed { get; set; }

        public string? CertificateNumber { get; set; }

        public long? FileStorageId { get; set; }

        public string? Remarks { get; set; }

public string? TrainingName { get; set; }


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
