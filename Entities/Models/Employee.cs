using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Employee", Schema = "app")]
    public partial class Employee
    {
        [Column("EmployeeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeId { get; set; }

        [Key]
        [Column("EmployeeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeGuid { get; set; } = Guid.NewGuid();

        public long PersonId { get; set; }

        [Required]
        public string EmployeeNo { get; set; } = string.Empty;

        public DateTime HireDate { get; set; }

        public DateTime? ProbationEndDate { get; set; }

        public DateTime? ConfirmationDate { get; set; }

        public DateTime? TerminationDate { get; set; }

        public long SrEmploymentStatus { get; set; }

        public long SrEmploymentType { get; set; }

        public long? SrRemunerationType { get; set; }

public string? PersonName { get; set; }
public string? EmploymentStatusName { get; set; }
public string? EmploymentTypeName { get; set; }
public string? RemunerationTypeName { get; set; }

public string? PersonFileStorageId { get; set; }
public string? BirthDate { get; set; }


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
