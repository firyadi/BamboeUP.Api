using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("WorkNotice", Schema = "app")]
    public partial class WorkNotice
    {
        [Column("WorkNoticeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long WorkNoticeId { get; set; }

        [Key]
        [Column("WorkNoticeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid WorkNoticeGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string WorkNoticeCode { get; set; } = string.Empty;

        [Required]
        public string WorkNoticeName { get; set; } = string.Empty;

        public DateTime WorkNoticeDate { get; set; }

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
