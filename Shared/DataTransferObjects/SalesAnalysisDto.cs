using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record SalesAnalysisDto
    {
        public long SalesAnalysisId { get; set; }
        public Guid SalesAnalysisGuid { get; init; }
        
        public DateTime SalesDate { get; set; }
        public string Salesperson { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetSales { get; set; }
        public decimal CostAmount { get; set; }
        public decimal ProfitAmount { get; set; }
        public string? SalesAnalysisDescription { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record SalesAnalysisForCreationDto
    {
        public DateTime SalesDate { get; set; }
        public string Salesperson { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetSales { get; set; }
        public decimal CostAmount { get; set; }
        public decimal ProfitAmount { get; set; }
        public string? SalesAnalysisDescription { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record SalesAnalysisForUpdateDto
    {
        public DateTime SalesDate { get; set; }
        public string Salesperson { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetSales { get; set; }
        public decimal CostAmount { get; set; }
        public decimal ProfitAmount { get; set; }
        public string? SalesAnalysisDescription { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record SalesAnalysisForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class SalesAnalysisSearchDto
    {
        public string? SalesDate { get; set; }
        public SearchType SalesDateSearchType { get; set; } = SearchType.Contains;

        public string? Salesperson { get; set; }
        public SearchType SalespersonSearchType { get; set; } = SearchType.Contains;

        public string? Area { get; set; }
        public SearchType AreaSearchType { get; set; } = SearchType.Contains;

        public string? ProductType { get; set; }
        public SearchType ProductTypeSearchType { get; set; } = SearchType.Contains;

        public string? ProductName { get; set; }
        public SearchType ProductNameSearchType { get; set; } = SearchType.Contains;

        public string? Quantity { get; set; }
        public SearchType QuantitySearchType { get; set; } = SearchType.Contains;

        public string? SalesAmount { get; set; }
        public SearchType SalesAmountSearchType { get; set; } = SearchType.Contains;

        public string? DiscountAmount { get; set; }
        public SearchType DiscountAmountSearchType { get; set; } = SearchType.Contains;

        public string? NetSales { get; set; }
        public SearchType NetSalesSearchType { get; set; } = SearchType.Contains;

        public string? CostAmount { get; set; }
        public SearchType CostAmountSearchType { get; set; } = SearchType.Contains;

        public string? ProfitAmount { get; set; }
        public SearchType ProfitAmountSearchType { get; set; } = SearchType.Contains;

        public string? SalesAnalysisDescription { get; set; }
        public SearchType SalesAnalysisDescriptionSearchType { get; set; } = SearchType.Contains;

    }
}
