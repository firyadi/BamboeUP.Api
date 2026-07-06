using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("UserCompanyScope", Schema = "core")]
    public class UserCompanyScope
    {
        [Column("UserCompanyScopeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserCompanyScopeId { get; set; }

        [Key]
        [Column("UserCompanyScopeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserCompanyScopeGuid { get; set; } = Guid.NewGuid();

        [Required]
        [Column("UserId")]
        public long UserId { get; set; }

        [Required]
        [Column("CompanyId")]
        public long CompanyId { get; set; }

        /// <summary>
        /// NULL = akses semua office di company tersebut.
        /// NOT NULL = akses hanya ke 1 office spesifik.
        /// </summary>
        [Column("CompanyOfficeId")]
        public long? CompanyOfficeId { get; set; }

        [Column("IsDefaultCompany")]
        public bool IsDefaultCompany { get; set; } = false;

        [Column("IsDefaultOffice")]
        public bool IsDefaultOffice { get; set; } = false;

        [Column("StatusId")]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; }

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

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
