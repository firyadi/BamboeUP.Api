using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("OvertimeRequest", Schema = "app")]
    public partial class OvertimeRequest
    {
        [Column("OvertimeRequestId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OvertimeRequestId { get; set; }

        [Key]
        [Column("OvertimeRequestGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OvertimeRequestGuid { get; set; } = Guid.NewGuid();

        public long CompanyId { get; set; }

        public long CompanyOfficeId { get; set; }

        public long PersonId { get; set; }

        [Required]
        public string RequestNo { get; set; } = string.Empty;

        public DateTime RequestDate { get; set; }

        public DateTime OvertimeDate { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        public string? WorkDescription { get; set; }

        [Required]
        public string PlannedStartTime { get; set; } = string.Empty;

        [Required]
        public string PlannedEndTime { get; set; } = string.Empty;

        public int? TotalPlannedMinutes { get; set; }

        public string? ActualStartTime { get; set; }

        public string? ActualEndTime { get; set; }

        public int? TotalActualMinutes { get; set; }

        public DateTime? ApprovedTime { get; set; }

        public string? RejectionReason { get; set; }

        public string? Note { get; set; }
public string? ApprovedbyName { get; set; }
public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? PersonName { get; set; }



        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long? UpdatedById { get; set; }

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long? DeletedById { get; set; }

        public DateTime? DeletedTime { get; set; }
    }
}
