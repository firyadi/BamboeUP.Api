using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class AssetRepository(RepositoryContext context) : IAssetRepository
    {
        public async Task<Asset?> GetAssetAsync(Guid assetGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*


                FROM [app].[Asset] a


                WHERE a.AssetGuid = @assetGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Asset>(sql, new { assetGuid });
        }

        public async Task<IEnumerable<Asset>> GetAllAssetsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*



                FROM [app].[Asset] a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.AssetId DESC";
            return await connection.QueryAsync<Asset>(sql);
        }

        public async Task CreateAssetAsync(Asset asset, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Asset]
                (AssetGuid, CreatedById, StatusId, CreatedTime, AssetCode, AssetName, AssetCategoryId, AssetBrand, AssetModel, SerialNumber, Barcode, CompanyId, CompanyOfficeId, CostCenterId, PurchaseDate, PurchasePrice, CurrencyId, WarrantyStartDate, WarrantyEndDate, SrAssetCondition, SrAssetStatus, CurrentLocation, FileStorageId, Description
                )
                VALUES
                (@AssetGuid, @CreatedById, @StatusId, @CreatedTime, @AssetCode, @AssetName, @AssetCategoryId, @AssetBrand, @AssetModel, @SerialNumber, @Barcode, @CompanyId, @CompanyOfficeId, @CostCenterId, @PurchaseDate, @PurchasePrice, @CurrencyId, @WarrantyStartDate, @WarrantyEndDate, @SrAssetCondition, @SrAssetStatus, @CurrentLocation, @FileStorageId, @Description
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            asset.AssetId = await conn.QuerySingleAsync<long>(sql, asset, transaction);
        }

        public async Task UpdateAssetAsync(Asset asset, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Asset]
                SET                     AssetCode = @AssetCode,
                    AssetName = @AssetName,
                    AssetCategoryId = @AssetCategoryId,
                    AssetBrand = @AssetBrand,
                    AssetModel = @AssetModel,
                    SerialNumber = @SerialNumber,
                    Barcode = @Barcode,
                    CompanyId = @CompanyId,
                    CompanyOfficeId = @CompanyOfficeId,
                    CostCenterId = @CostCenterId,
                    PurchaseDate = @PurchaseDate,
                    PurchasePrice = @PurchasePrice,
                    CurrencyId = @CurrencyId,
                    WarrantyStartDate = @WarrantyStartDate,
                    WarrantyEndDate = @WarrantyEndDate,
                    SrAssetCondition = @SrAssetCondition,
                    SrAssetStatus = @SrAssetStatus,
                    CurrentLocation = @CurrentLocation,
                    FileStorageId = @FileStorageId,
                    Description = @Description,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE AssetGuid = @AssetGuid";
            await conn.ExecuteAsync(sql, asset, transaction);
        }

        public async Task SoftDeleteAssetAsync(Asset asset, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Asset]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AssetGuid = @AssetGuid";

            await conn.ExecuteAsync(sql, asset, transaction);
        }

        public async Task DeleteAssetAsync(Guid assetGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Asset] WHERE AssetGuid = @assetGuid";
            await conn.ExecuteAsync(sql, new { assetGuid }, transaction);
        }

        public async Task<IEnumerable<Asset>> SearchAssetAsync(
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

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(assetCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssetCode", "@assetCode", assetCodeSearchType, parameters, "assetCode", assetCode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(assetName))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssetName", "@assetName", assetNameSearchType, parameters, "assetName", assetName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(assetCategoryId))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssetCategoryId", "@assetCategoryId", assetCategoryIdSearchType, parameters, "assetCategoryId", assetCategoryId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(assetBrand))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssetBrand", "@assetBrand", assetBrandSearchType, parameters, "assetBrand", assetBrand);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(assetModel))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssetModel", "@assetModel", assetModelSearchType, parameters, "assetModel", assetModel);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(serialNumber))
            {
                var param = SqlFilterHelper.BuildFilter("a.SerialNumber", "@serialNumber", serialNumberSearchType, parameters, "serialNumber", serialNumber);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                var param = SqlFilterHelper.BuildFilter("a.Barcode", "@barcode", barcodeSearchType, parameters, "barcode", barcode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyId", "@companyId", companyIdSearchType, parameters, "companyId", companyId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(companyOfficeId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyOfficeId", "@companyOfficeId", companyOfficeIdSearchType, parameters, "companyOfficeId", companyOfficeId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(costCenterId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostCenterId", "@costCenterId", costCenterIdSearchType, parameters, "costCenterId", costCenterId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(purchaseDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.PurchaseDate", "@purchaseDate", purchaseDateSearchType, parameters, "purchaseDate", purchaseDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(purchasePrice))
            {
                var param = SqlFilterHelper.BuildFilter("a.PurchasePrice", "@purchasePrice", purchasePriceSearchType, parameters, "purchasePrice", purchasePrice);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(currencyId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CurrencyId", "@currencyId", currencyIdSearchType, parameters, "currencyId", currencyId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(warrantyStartDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.WarrantyStartDate", "@warrantyStartDate", warrantyStartDateSearchType, parameters, "warrantyStartDate", warrantyStartDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(warrantyEndDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.WarrantyEndDate", "@warrantyEndDate", warrantyEndDateSearchType, parameters, "warrantyEndDate", warrantyEndDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srAssetCondition))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrAssetCondition", "@srAssetCondition", srAssetConditionSearchType, parameters, "srAssetCondition", srAssetCondition);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srAssetStatus))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrAssetStatus", "@srAssetStatus", srAssetStatusSearchType, parameters, "srAssetStatus", srAssetStatus);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(currentLocation))
            {
                var param = SqlFilterHelper.BuildFilter("a.CurrentLocation", "@currentLocation", currentLocationSearchType, parameters, "currentLocation", currentLocation);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*


                FROM [app].[Asset] a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AssetId DESC";

            return await connection.QueryAsync<Asset>(sql, parameters, transaction);
        }
    }
}
