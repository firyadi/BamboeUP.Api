using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class SalesAnalysisService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : ISalesAnalysisService
    {
        public async Task<IEnumerable<SalesAnalysisDto>> GetAllSalesAnalysesAsync(bool trackChanges)
        {
            var entities = await repository.SalesAnalysis.GetAllSalesAnalysesAsync(trackChanges);
            return entities.Adapt<IEnumerable<SalesAnalysisDto>>();
        }

        public async Task<SalesAnalysisDto?> GetSalesAnalysisByGuidAsync(Guid salesAnalysisGuid, bool trackChanges)
        {
            var entity = await repository.SalesAnalysis.GetSalesAnalysisAsync(salesAnalysisGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<SalesAnalysisDto>();
            return dto;
        }

        public async Task<SalesAnalysisDto> CreateSalesAnalysisAsync(SalesAnalysisForCreationDto input)
        {
            var model = input.Adapt<SalesAnalysis>();

            if (model.SalesAnalysisGuid == Guid.Empty)
                model.SalesAnalysisGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.SalesAnalysis.CreateSalesAnalysisAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "SalesAnalysis",
                RootEntityKey = model.SalesAnalysisGuid.ToString(),
                RootDisplayName = model.Salesperson,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "SalesAnalysis",
                        EntityKey = model.SalesAnalysisGuid.ToString(),
                        EntityDisplayName = model.Salesperson,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<SalesAnalysisDto>();
        }

        public async Task UpdateSalesAnalysisAsync(Guid salesAnalysisGuid, SalesAnalysisForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.SalesAnalysis.GetSalesAnalysisAsync(salesAnalysisGuid, false)
                ?? throw new KeyNotFoundException($"SalesAnalysis '{{salesAnalysisGuid}}' was not found.");

            var model = input.Adapt<SalesAnalysis>();
            model.SalesAnalysisGuid = salesAnalysisGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.SalesAnalysis.UpdateSalesAnalysisAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "SalesAnalysis",
                RootEntityKey = model.SalesAnalysisGuid.ToString(),
                RootDisplayName = model.Salesperson,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "SalesAnalysis",
                        EntityKey = model.SalesAnalysisGuid.ToString(),
                        EntityDisplayName = model.Salesperson,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteSalesAnalysisAsync(Guid salesAnalysisGuid, SalesAnalysisForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.SalesAnalysis.GetSalesAnalysisAsync(salesAnalysisGuid, false);
            var model = new SalesAnalysis
            {
                SalesAnalysisGuid = salesAnalysisGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.SalesAnalysis.SoftDeleteSalesAnalysisAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "SalesAnalysis",
                RootEntityKey = salesAnalysisGuid.ToString(),
                RootDisplayName = oldEntity?.Salesperson,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "SalesAnalysis",
                        EntityKey = salesAnalysisGuid.ToString(),
                        EntityDisplayName = oldEntity?.Salesperson,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteSalesAnalysisByAdminAsync(Guid salesAnalysisGuid, bool trackChanges)
        {
            await repository.SalesAnalysis.DeleteSalesAnalysisAsync(salesAnalysisGuid);
        }

        public async Task<IEnumerable<SalesAnalysisDto>> SearchSalesAnalysisAsync(
            string? salesDate, string? salesDateSearchType, string? salesperson, string? salespersonSearchType, string? area, string? areaSearchType, string? productType, string? productTypeSearchType, string? productName, string? productNameSearchType, string? quantity, string? quantitySearchType, string? salesAmount, string? salesAmountSearchType, string? discountAmount, string? discountAmountSearchType, string? netSales, string? netSalesSearchType, string? costAmount, string? costAmountSearchType, string? profitAmount, string? profitAmountSearchType, string? salesAnalysisDescription, string? salesAnalysisDescriptionSearchType

            )
        {
            var data = await repository.SalesAnalysis.SearchSalesAnalysisAsync(
                salesDate, salesDateSearchType, salesperson, salespersonSearchType, area, areaSearchType, productType, productTypeSearchType, productName, productNameSearchType, quantity, quantitySearchType, salesAmount, salesAmountSearchType, discountAmount, discountAmountSearchType, netSales, netSalesSearchType, costAmount, costAmountSearchType, profitAmount, profitAmountSearchType, salesAnalysisDescription, salesAnalysisDescriptionSearchType

                );
            return data.Adapt<IEnumerable<SalesAnalysisDto>>();
        }
    }
}
