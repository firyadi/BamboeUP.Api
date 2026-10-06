using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ISalesAnalysisRepository
    {
        Task<SalesAnalysis?> GetSalesAnalysisAsync(Guid salesAnalysisGuid, bool trackChanges);
        Task<IEnumerable<SalesAnalysis>> GetAllSalesAnalysesAsync(bool trackChanges);

        Task CreateSalesAnalysisAsync(SalesAnalysis salesAnalysis, IDbTransaction? transaction = null);
        Task UpdateSalesAnalysisAsync(SalesAnalysis salesAnalysis, IDbTransaction? transaction = null);
        Task DeleteSalesAnalysisAsync(Guid salesAnalysisGuid, IDbTransaction? transaction = null);
        Task SoftDeleteSalesAnalysisAsync(SalesAnalysis salesAnalysis, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<SalesAnalysis>> SearchSalesAnalysisAsync(
            string? salesDate,
            string? salesDateSearchType,
            string? salesperson,
            string? salespersonSearchType,
            string? area,
            string? areaSearchType,
            string? productType,
            string? productTypeSearchType,
            string? productName,
            string? productNameSearchType,
            string? quantity,
            string? quantitySearchType,
            string? salesAmount,
            string? salesAmountSearchType,
            string? discountAmount,
            string? discountAmountSearchType,
            string? netSales,
            string? netSalesSearchType,
            string? costAmount,
            string? costAmountSearchType,
            string? profitAmount,
            string? profitAmountSearchType,
            string? salesAnalysisDescription,
            string? salesAnalysisDescriptionSearchType,

            IDbTransaction? transaction = null);
    }
}
