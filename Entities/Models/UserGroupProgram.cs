using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("UserGroupProgram", Schema = "core")]
    public class UserGroupProgram
    {
        [Column("UserGroupProgramId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserGroupProgramId { get; set; }

        [Key]
        [Column("UserGroupProgramGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserGroupProgramGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long UserGroupId { get; set; }

        [Required]
        public long ProgramsId { get; set; }

        public bool IsUserGroupViewAble { get; set; } = true;
        public bool? IsUserGroupAddAble { get; set; }
        public bool? IsUserGroupEditAble { get; set; }
        public bool? IsUserGroupDeleteAble { get; set; }
        public bool? IsUserGroupApprovalAble { get; set; }
        public bool? IsUserGroupUnApprovalAble { get; set; }
        public bool? IsUserGroupVoidAble { get; set; }
        public bool? IsUserGroupUnVoidAble { get; set; }
        public bool? IsUserGroupExportAble { get; set; }

        [Required]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Required]
        public long CreatedById { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public long? UpdatedById { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public long? DeletedById { get; set; }

        public DateTime? DeletedTime { get; set; }
    }
}
