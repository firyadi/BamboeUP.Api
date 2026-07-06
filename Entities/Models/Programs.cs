using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Programs", Schema = "core")]
    public class Programs
    {
        [Column("ProgramId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProgramId { get; set; }

        [Key]
        [Column("ProgramGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProgramGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(30)]
        public string ProgramCode { get; set; }

        public long? ParentId { get; set; }

        [MaxLength(50)]
        public string? IconCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProgramName { get; set; }

        public long? TopLevelProgramId { get; set; }

        [Required]
        public byte RootLevel { get; set; }

        [Required]
        public byte RowIndex { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        public bool? IsParentProgram { get; set; }

        public bool? IsProgram { get; set; }

        public bool? IsBeginGroup { get; set; }

        [MaxLength(5)]
        public string? ProgramType { get; set; }

        public bool? IsProgramAddAble { get; set; }

        public bool? IsProgramEditAble { get; set; }

        public bool? IsProgramDeleteAble { get; set; }

        public bool? IsProgramViewAble { get; set; }

        public bool? IsProgramApprovalAble { get; set; }

        public bool? IsProgramUnApprovalAble { get; set; }

        public bool? IsProgramVoidAble { get; set; }

        public bool? IsProgramUnVoidAble { get; set; }

        public bool? IsProgramDirectVoid { get; set; }

        public bool? IsProgramPrintAble { get; set; }

        public bool? IsMenuAddVisible { get; set; }

        public bool? IsMenuHomeVisible { get; set; }

        public bool? IsVisible { get; set; }

        [MaxLength(1000)]
        public string? NavigateUrl { get; set; }

        [MaxLength(255)]
        public string? HelpLinkId { get; set; }

        [MaxLength(50)]
        public string? AssemblyName { get; set; }

        [MaxLength(200)]
        public string? AssemblyClassName { get; set; }

        [MaxLength(200)]
        public string? StoreProcedureName { get; set; }

        [MaxLength(100)]
        public string? AccessKey { get; set; }

        public bool? IsActive { get; set; }

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
