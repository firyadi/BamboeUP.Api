namespace Shared.DataTransferObjects
{
    public class SalesTransactionPieRowDto
    {
        public int PeriodYear { get; set; }
        public int PeriodMonth { get; set; }
        public string? PeriodYearMonth { get; set; }
        public string? RegionCode { get; set; }
        public string? ProductCategory { get; set; }
        public string? ProductName { get; set; }
        public string? SalesChannel { get; set; }
        public string? CustomerSegment { get; set; }
        public int Quantity { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CostAmount { get; set; }
        public int TransactionCount { get; set; }
    }

    public class SalesTransactionPieResultDto
    {
        public string? Box1Caption { get; set; }
        public string? Box2Caption { get; set; }
        public string? Box3Caption { get; set; }
        public decimal Box1Value { get; set; }
        public long Box2Value { get; set; }
        public long Box3Value { get; set; }
        public List<SalesTransactionPieRowDto> Rows { get; set; } = [];
    }

    public class SalesTransactionPieQueryDto
    {
        public long CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
