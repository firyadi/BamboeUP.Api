using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record SalesTransactionDto
    {
        public long SalesTransactionId { get; set; }
        public Guid SalesTransactionGuid { get; init; }
        
        public string TransactionCode { get; set; } = string.Empty;
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public int PeriodYear { get; set; }
        public int PeriodMonth { get; set; }
        public DateTime TransactionDate { get; set; }
        public string RegionCode { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SalesChannel { get; set; } = string.Empty;
        public string CustomerSegment { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal CostAmount { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record SalesTransactionForCreationDto
    {
        public string TransactionCode { get; set; } = string.Empty;
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public int PeriodYear { get; set; }
        public int PeriodMonth { get; set; }
        public DateTime TransactionDate { get; set; }
        public string RegionCode { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SalesChannel { get; set; } = string.Empty;
        public string CustomerSegment { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal CostAmount { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record SalesTransactionForUpdateDto
    {
        public string TransactionCode { get; set; } = string.Empty;
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public int PeriodYear { get; set; }
        public int PeriodMonth { get; set; }
        public DateTime TransactionDate { get; set; }
        public string RegionCode { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SalesChannel { get; set; } = string.Empty;
        public string CustomerSegment { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal CostAmount { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record SalesTransactionForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class SalesTransactionSearchDto
    {
        public string? TransactionCode { get; set; }
        public SearchType TransactionCodeSearchType { get; set; } = SearchType.Contains;

        public string? CompanyId { get; set; }
        public SearchType CompanyIdSearchType { get; set; } = SearchType.Contains;

        public string? CompanyOfficeId { get; set; }
        public SearchType CompanyOfficeIdSearchType { get; set; } = SearchType.Contains;

        public string? PeriodYear { get; set; }
        public SearchType PeriodYearSearchType { get; set; } = SearchType.Contains;

        public string? PeriodMonth { get; set; }
        public SearchType PeriodMonthSearchType { get; set; } = SearchType.Contains;

        public string? TransactionDate { get; set; }
        public SearchType TransactionDateSearchType { get; set; } = SearchType.Contains;

        public string? RegionCode { get; set; }
        public SearchType RegionCodeSearchType { get; set; } = SearchType.Contains;

        public string? ProductCategory { get; set; }
        public SearchType ProductCategorySearchType { get; set; } = SearchType.Contains;

        public string? ProductCode { get; set; }
        public SearchType ProductCodeSearchType { get; set; } = SearchType.Contains;

        public string? ProductName { get; set; }
        public SearchType ProductNameSearchType { get; set; } = SearchType.Contains;

        public string? SalesChannel { get; set; }
        public SearchType SalesChannelSearchType { get; set; } = SearchType.Contains;

        public string? CustomerSegment { get; set; }
        public SearchType CustomerSegmentSearchType { get; set; } = SearchType.Contains;

        public string? Quantity { get; set; }
        public SearchType QuantitySearchType { get; set; } = SearchType.Contains;

        public string? UnitPrice { get; set; }
        public SearchType UnitPriceSearchType { get; set; } = SearchType.Contains;

        public string? DiscountAmount { get; set; }
        public SearchType DiscountAmountSearchType { get; set; } = SearchType.Contains;

        public string? NetAmount { get; set; }
        public SearchType NetAmountSearchType { get; set; } = SearchType.Contains;

        public string? CostAmount { get; set; }
        public SearchType CostAmountSearchType { get; set; } = SearchType.Contains;

    }
}
