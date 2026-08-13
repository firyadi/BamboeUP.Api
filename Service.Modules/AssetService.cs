using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class AssetService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext,
        IFileStorageService fileStorageService) : IAssetService
    {
        public async Task<IEnumerable<AssetDto>> GetAllAssetsAsync(bool trackChanges)
        {
            var entities = await repository.Asset.GetAllAssetsAsync(trackChanges);
            return entities.Adapt<IEnumerable<AssetDto>>();
        }

        public async Task<AssetDto?> GetAssetByGuidAsync(Guid assetGuid, bool trackChanges)
        {
            var entity = await repository.Asset.GetAssetAsync(assetGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<AssetDto>();
            if (entity.FileStorageId is > 0)
            {
                var file = await fileStorageService.GetFileStorageByIdAsync(entity.FileStorageId.Value, false);
                dto.FileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<AssetDto> CreateAssetAsync(AssetForCreationDto input)
        {
            var model = input.Adapt<Asset>();

            if (model.AssetGuid == Guid.Empty)
                model.AssetGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.Asset.CreateAssetAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Asset",
                RootEntityKey = model.AssetGuid.ToString(),
                RootDisplayName = model.AssetCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Asset",
                        EntityKey = model.AssetGuid.ToString(),
                        EntityDisplayName = model.AssetCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<AssetDto>();
        }

        public async Task UpdateAssetAsync(Guid assetGuid, AssetForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.Asset.GetAssetAsync(assetGuid, false)
                ?? throw new KeyNotFoundException($"Asset '{{assetGuid}}' was not found.");

            var model = input.Adapt<Asset>();
            model.AssetGuid = assetGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.Asset.UpdateAssetAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Asset",
                RootEntityKey = model.AssetGuid.ToString(),
                RootDisplayName = model.AssetCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Asset",
                        EntityKey = model.AssetGuid.ToString(),
                        EntityDisplayName = model.AssetCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteAssetAsync(Guid assetGuid, AssetForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.Asset.GetAssetAsync(assetGuid, false);
            var model = new Asset
            {
                AssetGuid = assetGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Asset.SoftDeleteAssetAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Asset",
                RootEntityKey = assetGuid.ToString(),
                RootDisplayName = oldEntity?.AssetCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Asset",
                        EntityKey = assetGuid.ToString(),
                        EntityDisplayName = oldEntity?.AssetCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteAssetByAdminAsync(Guid assetGuid, bool trackChanges)
        {
            await repository.Asset.DeleteAssetAsync(assetGuid);
        }

        public async Task<IEnumerable<AssetDto>> SearchAssetAsync(
            string? assetCode, string? assetCodeSearchType, string? assetName, string? assetNameSearchType, string? assetCategoryId, string? assetCategoryIdSearchType, string? assetBrand, string? assetBrandSearchType, string? assetModel, string? assetModelSearchType, string? serialNumber, string? serialNumberSearchType, string? barcode, string? barcodeSearchType, string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? costCenterId, string? costCenterIdSearchType, string? purchaseDate, string? purchaseDateSearchType, string? purchasePrice, string? purchasePriceSearchType, string? currencyId, string? currencyIdSearchType, string? warrantyStartDate, string? warrantyStartDateSearchType, string? warrantyEndDate, string? warrantyEndDateSearchType, string? srAssetCondition, string? srAssetConditionSearchType, string? srAssetStatus, string? srAssetStatusSearchType, string? currentLocation, string? currentLocationSearchType, string? description, string? descriptionSearchType

            )
        {
            var data = await repository.Asset.SearchAssetAsync(
                assetCode, assetCodeSearchType, assetName, assetNameSearchType, assetCategoryId, assetCategoryIdSearchType, assetBrand, assetBrandSearchType, assetModel, assetModelSearchType, serialNumber, serialNumberSearchType, barcode, barcodeSearchType, companyId, companyIdSearchType, companyOfficeId, companyOfficeIdSearchType, costCenterId, costCenterIdSearchType, purchaseDate, purchaseDateSearchType, purchasePrice, purchasePriceSearchType, currencyId, currencyIdSearchType, warrantyStartDate, warrantyStartDateSearchType, warrantyEndDate, warrantyEndDateSearchType, srAssetCondition, srAssetConditionSearchType, srAssetStatus, srAssetStatusSearchType, currentLocation, currentLocationSearchType, description, descriptionSearchType

                );
            return data.Adapt<IEnumerable<AssetDto>>();
        }
    }
}
