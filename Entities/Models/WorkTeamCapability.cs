using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("WorkTeamCapability", Schema = "app")]
    public partial class WorkTeamCapability
    {
        [Column("WorkTeamCapabilityId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long WorkTeamCapabilityId { get; set; }

        [Key]
        [Column("WorkTeamCapabilityGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid WorkTeamCapabilityGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long WorkTeamId { get; set; }

        public long WorkCapabilityId { get; set; }

        public bool IsViewAble { get; set; }

        public bool? IsAddAble { get; set; }

        public bool? IsEditAble { get; set; }

        public bool? IsApproveAble { get; set; }

        public bool? IsExportAble { get; set; }

public string? WorkCapabilityName { get; set; }


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
