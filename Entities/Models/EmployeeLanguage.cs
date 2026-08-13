using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("EmployeeLanguage", Schema = "app")]
    public partial class EmployeeLanguage
    {
        [Column("EmployeeLanguageId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeLanguageId { get; set; }

        [Key]
        [Column("EmployeeLanguageGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeLanguageGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long EmployeeId { get; set; }

        public long SrLanguage { get; set; }

        public long? SrReadingLevel { get; set; }

        public long? SrWritingLevel { get; set; }

        public long? SrListeningLevel { get; set; }

        public long? SrSpeakingLevel { get; set; }

        public string? CertificationName { get; set; }

        public decimal? Score { get; set; }

        public DateTime? IssuedDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public long? FileStorageId { get; set; }

        public bool IsPrimary { get; set; }

        public string? Remarks { get; set; }

public string? LanguageName { get; set; }
public string? ListeningLevelName { get; set; }
public string? ReadingLevelName { get; set; }
public string? SpeakingLevelName { get; set; }
public string? WritingLevelName { get; set; }


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
