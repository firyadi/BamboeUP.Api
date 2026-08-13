using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IAssetRepository
    {
        Task<Asset?> GetAssetAsync(Guid assetGuid, bool trackChanges);
        Task<IEnumerable<Asset>> GetAllAssetsAsync(bool trackChanges);

        Task CreateAssetAsync(Asset asset, IDbTransaction? transaction = null);
        Task UpdateAssetAsync(Asset asset, IDbTransaction? transaction = null);
        Task DeleteAssetAsync(Guid assetGuid, IDbTransaction? transaction = null);
        Task SoftDeleteAssetAsync(Asset asset, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Asset>> SearchAssetAsync(
            string? assetCode,
            string? assetCodeSearchType,
            string? assetName,
            string? assetNameSearchType,
            string? assetCategoryId,
            string? assetCategoryIdSearchType,
            string? assetBrand,
            string? assetBrandSearchType,
            string? assetModel,
            string? assetModelSearchType,
            string? serialNumber,
            string? serialNumberSearchType,
            string? barcode,
            string? barcodeSearchType,
            string? companyId,
            string? companyIdSearchType,
            string? companyOfficeId,
            string? companyOfficeIdSearchType,
            string? costCenterId,
            string? costCenterIdSearchType,
            string? purchaseDate,
            string? purchaseDateSearchType,
            string? purchasePrice,
            string? purchasePriceSearchType,
            string? currencyId,
            string? currencyIdSearchType,
            string? warrantyStartDate,
            string? warrantyStartDateSearchType,
            string? warrantyEndDate,
            string? warrantyEndDateSearchType,
            string? srAssetCondition,
            string? srAssetConditionSearchType,
            string? srAssetStatus,
            string? srAssetStatusSearchType,
            string? currentLocation,
            string? currentLocationSearchType,
            string? description,
            string? descriptionSearchType,

            IDbTransaction? transaction = null);
    }
}
