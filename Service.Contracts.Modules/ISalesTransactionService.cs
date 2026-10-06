using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface ISalesTransactionService
    {
        Task<IEnumerable<SalesTransactionDto>> GetAllSalesTransactionsAsync(bool trackChanges);
        Task<SalesTransactionDto?> GetSalesTransactionByGuidAsync(Guid salesTransactionGuid, bool trackChanges);
        Task<SalesTransactionDto> CreateSalesTransactionAsync(SalesTransactionForCreationDto input);
        Task UpdateSalesTransactionAsync(Guid salesTransactionGuid, SalesTransactionForUpdateDto input, bool trackChanges);
        Task DeleteSalesTransactionAsync(Guid salesTransactionGuid, SalesTransactionForDeleteDto input, bool trackChanges);
        Task DeleteSalesTransactionByAdminAsync(Guid salesTransactionGuid, bool trackChanges);

        Task<IEnumerable<SalesTransactionDto>> SearchSalesTransactionAsync(
            string? transactionCode, string? transactionCodeSearchType, string? companyId, string? companyIdSearchType, string? companyOfficeId, string? companyOfficeIdSearchType, string? periodYear, string? periodYearSearchType, string? periodMonth, string? periodMonthSearchType, string? transactionDate, string? transactionDateSearchType, string? regionCode, string? regionCodeSearchType, string? productCategory, string? productCategorySearchType, string? productCode, string? productCodeSearchType, string? productName, string? productNameSearchType, string? salesChannel, string? salesChannelSearchType, string? customerSegment, string? customerSegmentSearchType, string? quantity, string? quantitySearchType, string? unitPrice, string? unitPriceSearchType, string? discountAmount, string? discountAmountSearchType, string? netAmount, string? netAmountSearchType, string? costAmount, string? costAmountSearchType

        );
    }
}
