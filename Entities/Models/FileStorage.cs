using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("FileStorage", Schema = "app")]
    public partial class FileStorage
    {
        [Column("FileStorageId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FileStorageId { get; set; }

        [Key]
        [Column("FileStorageGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FileStorageGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string FileStorageName { get; set; } = string.Empty;

        [Required]
        public string StoredFileName { get; set; } = string.Empty;

        [Required]
        public string Extension { get; set; } = string.Empty;

        [Required]
        public string MimeType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public long SrFileCategory { get; set; }

        [Required]
        public string StorageProvider { get; set; } = string.Empty;

        [Required]
        public string RelativePath { get; set; } = string.Empty;

        public int? Width { get; set; }

        public int? Height { get; set; }

        public bool IsImage { get; set; }

        public string? FileHash { get; set; }

        public long DownloadCount { get; set; }

        public DateTime? LastAccessTime { get; set; }

        public bool IsTemporary { get; set; }

        public string? Description { get; set; }
public string? FileCategoryName { get; set; }


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
