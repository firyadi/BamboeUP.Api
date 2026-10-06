using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ISalesTransactionRepository
    {
        Task<SalesTransaction?> GetSalesTransactionAsync(Guid salesTransactionGuid, bool trackChanges);
        Task<IEnumerable<SalesTransaction>> GetAllSalesTransactionsAsync(bool trackChanges);

        Task CreateSalesTransactionAsync(SalesTransaction salesTransaction, IDbTransaction? transaction = null);
        Task UpdateSalesTransactionAsync(SalesTransaction salesTransaction, IDbTransaction? transaction = null);
        Task DeleteSalesTransactionAsync(Guid salesTransactionGuid, IDbTransaction? transaction = null);
        Task SoftDeleteSalesTransactionAsync(SalesTransaction salesTransaction, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<SalesTransaction>> SearchSalesTransactionAsync(
            string? transactionCode,
            string? transactionCodeSearchType,
            string? companyId,
            string? companyIdSearchType,
            string? companyOfficeId,
            string? companyOfficeIdSearchType,
            string? periodYear,
            string? periodYearSearchType,
            string? periodMonth,
            string? periodMonthSearchType,
            string? transactionDate,
            string? transactionDateSearchType,
            string? regionCode,
            string? regionCodeSearchType,
            string? productCategory,
            string? productCategorySearchType,
            string? productCode,
            string? productCodeSearchType,
            string? productName,
            string? productNameSearchType,
            string? salesChannel,
            string? salesChannelSearchType,
            string? customerSegment,
            string? customerSegmentSearchType,
            string? quantity,
            string? quantitySearchType,
            string? unitPrice,
            string? unitPriceSearchType,
            string? discountAmount,
            string? discountAmountSearchType,
            string? netAmount,
            string? netAmountSearchType,
            string? costAmount,
            string? costAmountSearchType,

            IDbTransaction? transaction = null);
    }
}
