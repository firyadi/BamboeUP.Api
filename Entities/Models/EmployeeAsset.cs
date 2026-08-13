using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("EmployeeAsset", Schema = "app")]
    public partial class EmployeeAsset
    {
        [Column("EmployeeAssetId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeAssetId { get; set; }

        [Key]
        [Column("EmployeeAssetGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeAssetGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long EmployeeId { get; set; }

        public long AssetId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public DateTime? ExpectedReturnDate { get; set; }

        public bool IsCurrent { get; set; }

        public string? ConditionWhenAssigned { get; set; }

        public string? ConditionWhenReturned { get; set; }

        public string? Remarks { get; set; }

public string? AssetName { get; set; }


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
