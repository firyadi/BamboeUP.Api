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
    public partial class SalesTransactionService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : ISalesTransactionService
    {
        public async Task<IEnumerable<SalesTransactionDto>> GetAllSalesTransactionsAsync(bool trackChanges)
        {
            var entities = await repository.SalesTransaction.GetAllSalesTransactionsAsync(trackChanges);
            return entities.Adapt<IEnumerable<SalesTransactionDto>>();
        }

        public async Task<SalesTransactionDto?> GetSalesTransactionByGuidAsync(Guid salesTransactionGuid, bool trackChanges)
        {
            var entity = await repository.SalesTransaction.GetSalesTransactionAsync(salesTransactionGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<SalesTransactionDto>();
            return dto;
        }

        public async Task<SalesTransactionDto> CreateSalesTransactionAsync(SalesTransactionForCreationDto input)
        {
            var model = input.Adapt<SalesTransaction>();

            if (model.SalesTransactionGuid == Guid.Empty)
                model.SalesTransactionGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.SalesTransaction.CreateSalesTransactionAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "SalesTransaction",
                RootEntityKey = model.SalesTransactionGuid.ToString(),
                RootDisplayName = model.TransactionCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "SalesTransaction",
                        EntityKey = model.SalesTransactionGuid.ToString(),
                        EntityDisplayName = model.TransactionCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<SalesTransactionDto>();
        }

        public async Task UpdateSalesTransactionAsync(Guid salesTransactionGuid, SalesTransactionForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.SalesTransaction.GetSalesTransactionAsync(salesTransactionGuid, false)
                ?? throw new KeyNotFoundException($"SalesTransaction '{{salesTransactionGuid}}' was not found.");

            var model = input.Adapt<SalesTransaction>();
            model.SalesTransactionGuid = salesTransactionGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.SalesTransaction.UpdateSalesTransactionAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "SalesTransaction",
                RootEntityKey = model.SalesTransactionGuid.ToString(),
                RootDisplayName = model.TransactionCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "SalesTransaction",
                        EntityKey = model.SalesTransactionGuid.ToString(),
                        EntityDisplayName = model.TransactionCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteSalesTransactionAsync(Guid salesTransactionGuid, SalesTransactionForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.SalesTransaction.GetSalesTransactionAsync(salesTransactionGuid, false);
            var model = new SalesTransaction
            {
                SalesTransactionGuid = salesTransactionGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.SalesTransaction.SoftDeleteSalesTransactionAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "SalesTransaction",
                RootEntityKey = salesTransactionGuid.ToString(),
                RootDisplayName = oldEntity?.TransactionCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "SalesTransaction",
                        EntityKey = salesTransactionGuid.ToString(),
                        EntityDisplayName = oldEntity?.TransactionCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteSalesTransactionByAdminAsync(Guid salesTransactionGuid, bool trackChanges)
        {
            await repository.SalesTransaction.DeleteSalesTransactionAsync(salesTransactionGuid);
        }

        public async Task<IEnumerable<SalesTransactionDto>> SearchSalesTransactionAsync(
            string? transactionCode, string? transactionCodeSearchType, string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? periodYear, string? periodYearSearchType, string? periodMonth, string? periodMonthSearchType, string? transactionDate, string? transactionDateSearchType, string? regionCode, string? regionCodeSearchType, string? productCategory, string? productCategorySearchType, string? productCode, string? productCodeSearchType, string? productName, string? productNameSearchType, string? salesChannel, string? salesChannelSearchType, string? customerSegment, string? customerSegmentSearchType, string? quantity, string? quantitySearchType, string? unitPrice, string? unitPriceSearchType, string? discountAmount, string? discountAmountSearchType, string? netAmount, string? netAmountSearchType, string? costAmount, string? costAmountSearchType

            )
        {
            var data = await repository.SalesTransaction.SearchSalesTransactionAsync(
                transactionCode, transactionCodeSearchType, companyId, companyIdSearchType, companyOfficeId, companyOfficeIdSearchType, periodYear, periodYearSearchType, periodMonth, periodMonthSearchType, transactionDate, transactionDateSearchType, regionCode, regionCodeSearchType, productCategory, productCategorySearchType, productCode, productCodeSearchType, productName, productNameSearchType, salesChannel, salesChannelSearchType, customerSegment, customerSegmentSearchType, quantity, quantitySearchType, unitPrice, unitPriceSearchType, discountAmount, discountAmountSearchType, netAmount, netAmountSearchType, costAmount, costAmountSearchType

                );
            return data.Adapt<IEnumerable<SalesTransactionDto>>();
        }
    }
}
