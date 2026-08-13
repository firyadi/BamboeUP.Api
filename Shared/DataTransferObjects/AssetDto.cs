using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record AssetDto
    {
        public long AssetId { get; set; }
        public Guid AssetGuid { get; init; }
        
        public string AssetCode { get; set; } = string.Empty;
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
        public Guid? FileStorageGuid { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record AssetForCreationDto
    {
        public string AssetCode { get; set; } = string.Empty;
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
        public long CreatedById { get; set; } = 0;
    }

    public partial record AssetForUpdateDto
    {
        public string AssetCode { get; set; } = string.Empty;
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
        public long UpdatedById { get; set; }
    }

    public partial record AssetForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class AssetSearchDto
    {
        public string? AssetCode { get; set; }
        public SearchType AssetCodeSearchType { get; set; } = SearchType.Contains;

        public string? AssetName { get; set; }
        public SearchType AssetNameSearchType { get; set; } = SearchType.Contains;

        public string? AssetCategoryId { get; set; }
        public SearchType AssetCategoryIdSearchType { get; set; } = SearchType.Contains;

        public string? AssetBrand { get; set; }
        public SearchType AssetBrandSearchType { get; set; } = SearchType.Contains;

        public string? AssetModel { get; set; }
        public SearchType AssetModelSearchType { get; set; } = SearchType.Contains;

        public string? SerialNumber { get; set; }
        public SearchType SerialNumberSearchType { get; set; } = SearchType.Contains;

        public string? Barcode { get; set; }
        public SearchType BarcodeSearchType { get; set; } = SearchType.Contains;

        public string? CompanyId { get; set; }
        public SearchType CompanyIdSearchType { get; set; } = SearchType.Contains;

        public string? CompanyOfficeId { get; set; }
        public SearchType CompanyOfficeIdSearchType { get; set; } = SearchType.Contains;

        public string? CostCenterId { get; set; }
        public SearchType CostCenterIdSearchType { get; set; } = SearchType.Contains;

        public string? PurchaseDate { get; set; }
        public SearchType PurchaseDateSearchType { get; set; } = SearchType.Contains;

        public string? PurchasePrice { get; set; }
        public SearchType PurchasePriceSearchType { get; set; } = SearchType.Contains;

        public string? CurrencyId { get; set; }
        public SearchType CurrencyIdSearchType { get; set; } = SearchType.Contains;

        public string? WarrantyStartDate { get; set; }
        public SearchType WarrantyStartDateSearchType { get; set; } = SearchType.Contains;

        public string? WarrantyEndDate { get; set; }
        public SearchType WarrantyEndDateSearchType { get; set; } = SearchType.Contains;

        public string? SrAssetCondition { get; set; }
        public SearchType SrAssetConditionSearchType { get; set; } = SearchType.Contains;

        public string? SrAssetStatus { get; set; }
        public SearchType SrAssetStatusSearchType { get; set; } = SearchType.Contains;

        public string? CurrentLocation { get; set; }
        public SearchType CurrentLocationSearchType { get; set; } = SearchType.Contains;

        public string? Description { get; set; }
        public SearchType DescriptionSearchType { get; set; } = SearchType.Contains;

    }
}
