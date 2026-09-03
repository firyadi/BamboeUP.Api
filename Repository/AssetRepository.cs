using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class AssetRepository(RepositoryContext context, ISqlDialect dialect) : IAssetRepository
    {
        private readonly string _table = dialect.Qualify("app", "Asset");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<Asset?> GetAssetAsync(Guid assetGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE a.AssetGuid = @assetGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Asset>(sql, new { assetGuid });
        }

        public async Task<IEnumerable<Asset>> GetAllAssetsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}



                FROM {_table} a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.AssetId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Asset>(sql);
        }

        public async Task CreateAssetAsync(Asset asset, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (AssetGuid, CreatedById, StatusId, CreatedTime, AssetCode, AssetName, AssetCategoryId, AssetBrand, AssetModel, SerialNumber, Barcode, CompanyId, CompanyOfficeId, CostCenterId, PurchaseDate, PurchasePrice, CurrencyId, WarrantyStartDate, WarrantyEndDate, SrAssetCondition, SrAssetStatus, CurrentLocation, FileStorageId, Description
                )
                VALUES
                (@AssetGuid, @CreatedById, @StatusId, @CreatedTime, @AssetCode, @AssetName, @AssetCategoryId, @AssetBrand, @AssetModel, @SerialNumber, @Barcode, @CompanyId, @CompanyOfficeId, @CostCenterId, @PurchaseDate, @PurchasePrice, @CurrencyId, @WarrantyStartDate, @WarrantyEndDate, @SrAssetCondition, @SrAssetStatus, @CurrentLocation, @FileStorageId, @Description
                );
                {dialect.ReturnIdentitySuffix("AssetId")}";
            asset.AssetId = await conn.QuerySingleAsync<long>(sql, asset, transaction);
        }

        public async Task UpdateAssetAsync(Asset asset, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
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
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AssetGuid = @AssetGuid";

            await conn.ExecuteAsync(sql, asset, transaction);
        }

        public async Task DeleteAssetAsync(Guid assetGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE AssetGuid = @assetGuid";
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
                var param = SqlFilterHelper.BuildFilter("a.AssetCode", "@assetCode", assetCodeSearchType, parameters, "assetCode", assetCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(assetName))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssetName", "@assetName", assetNameSearchType, parameters, "assetName", assetName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(assetCategoryId))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssetCategoryId", "@assetCategoryId", assetCategoryIdSearchType, parameters, "assetCategoryId", assetCategoryId, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(assetBrand))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssetBrand", "@assetBrand", assetBrandSearchType, parameters, "assetBrand", assetBrand, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(assetModel))
            {
                var param = SqlFilterHelper.BuildFilter("a.AssetModel", "@assetModel", assetModelSearchType, parameters, "assetModel", assetModel, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(serialNumber))
            {
                var param = SqlFilterHelper.BuildFilter("a.SerialNumber", "@serialNumber", serialNumberSearchType, parameters, "serialNumber", serialNumber, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                var param = SqlFilterHelper.BuildFilter("a.Barcode", "@barcode", barcodeSearchType, parameters, "barcode", barcode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyId", "@companyId", companyIdSearchType, parameters, "companyId", companyId, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(companyOfficeId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyOfficeId", "@companyOfficeId", companyOfficeIdSearchType, parameters, "companyOfficeId", companyOfficeId, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(costCenterId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostCenterId", "@costCenterId", costCenterIdSearchType, parameters, "costCenterId", costCenterId, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(purchaseDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.PurchaseDate", "@purchaseDate", purchaseDateSearchType, parameters, "purchaseDate", purchaseDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(purchasePrice))
            {
                var param = SqlFilterHelper.BuildFilter("a.PurchasePrice", "@purchasePrice", purchasePriceSearchType, parameters, "purchasePrice", purchasePrice, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(currencyId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CurrencyId", "@currencyId", currencyIdSearchType, parameters, "currencyId", currencyId, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(warrantyStartDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.WarrantyStartDate", "@warrantyStartDate", warrantyStartDateSearchType, parameters, "warrantyStartDate", warrantyStartDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(warrantyEndDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.WarrantyEndDate", "@warrantyEndDate", warrantyEndDateSearchType, parameters, "warrantyEndDate", warrantyEndDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srAssetCondition))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrAssetCondition", "@srAssetCondition", srAssetConditionSearchType, parameters, "srAssetCondition", srAssetCondition, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srAssetStatus))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrAssetStatus", "@srAssetStatus", srAssetStatusSearchType, parameters, "srAssetStatus", srAssetStatus, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(currentLocation))
            {
                var param = SqlFilterHelper.BuildFilter("a.CurrentLocation", "@currentLocation", currentLocationSearchType, parameters, "currentLocation", currentLocation, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AssetId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Asset>(sql, parameters, transaction);
        }
    }
}
