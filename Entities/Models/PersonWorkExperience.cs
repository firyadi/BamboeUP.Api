using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PersonWorkExperience", Schema = "app")]
    public partial class PersonWorkExperience
    {
        [Column("PersonWorkExperienceId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PersonWorkExperienceId { get; set; }

        [Key]
        [Column("PersonWorkExperienceGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonWorkExperienceGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long PersonId { get; set; }

        public long? SrIndustry { get; set; }

        public long? SrEmploymentType { get; set; }

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        public string JobTitle { get; set; } = string.Empty;

        public string? Department { get; set; }

        public string? Location { get; set; }

        public string? Supervisor { get; set; }

        public string? JobDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsCurrentEmployment { get; set; }

        public decimal? LastSalary { get; set; }

        public string? ReasonforLeaving { get; set; }

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
