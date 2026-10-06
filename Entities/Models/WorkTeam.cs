using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("WorkTeam", Schema = "app")]
    public partial class WorkTeam
    {
        [Column("WorkTeamId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long WorkTeamId { get; set; }

        [Key]
        [Column("WorkTeamGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid WorkTeamGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string WorkTeamCode { get; set; } = string.Empty;

        [Required]
        public string WorkTeamName { get; set; } = string.Empty;

        public bool IsEditAble { get; set; }

        public string? Note { get; set; }




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
