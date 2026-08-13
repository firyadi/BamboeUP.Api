using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Asset", Schema = "app")]
    public partial class Asset
    {
        [Column("AssetId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AssetId { get; set; }

        [Key]
        [Column("AssetGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AssetGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string AssetCode { get; set; } = string.Empty;

        [Required]
        public string AssetName { get; set; } = string.Empty;

        public long AssetCategoryId { get; set; }

        public string? AssetBrand { get; set; }

        public string? AssetModel { get; set; }

        public string? SerialNumber { get; set; }

        public string? Barcode { get; set; }

        public long CompanyId { get; set; }

        public long? CompanyOfficeId { get; set; }

        public long? CostCenterId { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal? PurchasePrice { get; set; }

        public long? CurrencyId { get; set; }

        public DateTime? WarrantyStartDate { get; set; }

        public DateTime? WarrantyEndDate { get; set; }

        public long? SrAssetCondition { get; set; }

        public long SrAssetStatus { get; set; }

        public string? CurrentLocation { get; set; }

        public long? FileStorageId { get; set; }

        public string? Description { get; set; }



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
