using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IAssetService
    {
        Task<IEnumerable<AssetDto>> GetAllAssetsAsync(bool trackChanges);
        Task<AssetDto?> GetAssetByGuidAsync(Guid assetGuid, bool trackChanges);
        Task<AssetDto> CreateAssetAsync(AssetForCreationDto input);
        Task UpdateAssetAsync(Guid assetGuid, AssetForUpdateDto input, bool trackChanges);
        Task DeleteAssetAsync(Guid assetGuid, AssetForDeleteDto input, bool trackChanges);
        Task DeleteAssetByAdminAsync(Guid assetGuid, bool trackChanges);

        Task<IEnumerable<AssetDto>> SearchAssetAsync(
            string? assetCode, string? assetCodeSearchType, string? assetName, string? assetNameSearchType, string? assetCategoryId, string? assetCategoryIdSearchType, string? assetBrand, string? assetBrandSearchType, string? assetModel, string? assetModelSearchType, string? serialNumber, string? serialNumberSearchType, string? barcode, string? barcodeSearchType, string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? costCenterId, string? costCenterIdSearchType, string? purchaseDate, string? purchaseDateSearchType, string? purchasePrice, string? purchasePriceSearchType, string? currencyId, string? currencyIdSearchType, string? warrantyStartDate, string? warrantyStartDateSearchType, string? warrantyEndDate, string? warrantyEndDateSearchType, string? srAssetCondition, string? srAssetConditionSearchType, string? srAssetStatus, string? srAssetStatusSearchType, string? currentLocation, string? currentLocationSearchType, string? description, string? descriptionSearchType

        );
    }
}
