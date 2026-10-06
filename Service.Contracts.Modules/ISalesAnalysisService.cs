using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface ISalesAnalysisService
    {
        Task<IEnumerable<SalesAnalysisDto>> GetAllSalesAnalysesAsync(bool trackChanges);
        Task<SalesAnalysisDto?> GetSalesAnalysisByGuidAsync(Guid salesAnalysisGuid, bool trackChanges);
        Task<SalesAnalysisDto> CreateSalesAnalysisAsync(SalesAnalysisForCreationDto input);
        Task UpdateSalesAnalysisAsync(Guid salesAnalysisGuid, SalesAnalysisForUpdateDto input, bool trackChanges);
        Task DeleteSalesAnalysisAsync(Guid salesAnalysisGuid, SalesAnalysisForDeleteDto input, bool trackChanges);
        Task DeleteSalesAnalysisByAdminAsync(Guid salesAnalysisGuid, bool trackChanges);

        Task<IEnumerable<SalesAnalysisDto>> SearchSalesAnalysisAsync(
            string? salesDate, string? salesDateSearchType, string? salesperson, string? salespersonSearchType, string? area, string? areaSearchType, string? productType, string? productTypeSearchType, string? productName, string? productNameSearchType, string? quantity, string? quantitySearchType, string? salesAmount, string? salesAmountSearchType, string? discountAmount, string? discountAmountSearchType, string? netSales, string? netSalesSearchType, string? costAmount, string? costAmountSearchType, string? profitAmount, string? profitAmountSearchType, string? salesAnalysisDescription, string? salesAnalysisDescriptionSearchType

        );
    }
}
