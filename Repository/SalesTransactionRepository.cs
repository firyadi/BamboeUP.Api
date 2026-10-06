using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class SalesTransactionRepository(RepositoryContext context, ISqlDialect dialect) : ISalesTransactionRepository
    {
        private readonly string _table = dialect.Qualify("app", "SalesTransaction");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<SalesTransaction?> GetSalesTransactionAsync(Guid salesTransactionGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE a.SalesTransactionGuid = @salesTransactionGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<SalesTransaction>(sql, new { salesTransactionGuid });
        }

        public async Task<IEnumerable<SalesTransaction>> GetAllSalesTransactionsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}



                FROM {_table} a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.SalesTransactionId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<SalesTransaction>(sql);
        }

        public async Task CreateSalesTransactionAsync(SalesTransaction salesTransaction, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (SalesTransactionGuid, CreatedById, StatusId, CreatedTime, TransactionCode, CompanyId, CompanyOfficeId, PeriodYear, PeriodMonth, TransactionDate, RegionCode, ProductCategory, ProductCode, ProductName, SalesChannel, CustomerSegment, Quantity, UnitPrice, DiscountAmount, NetAmount, CostAmount
                )
                VALUES
                (@SalesTransactionGuid, @CreatedById, @StatusId, @CreatedTime, @TransactionCode, @CompanyId, @CompanyOfficeId, @PeriodYear, @PeriodMonth, @TransactionDate, @RegionCode, @ProductCategory, @ProductCode, @ProductName, @SalesChannel, @CustomerSegment, @Quantity, @UnitPrice, @DiscountAmount, @NetAmount, @CostAmount
                ){dialect.ReturnIdentitySuffix("SalesTransactionId")}";
            salesTransaction.SalesTransactionId = await conn.QuerySingleAsync<long>(sql, salesTransaction, transaction);
        }

        public async Task UpdateSalesTransactionAsync(SalesTransaction salesTransaction, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     TransactionCode = @TransactionCode,
                    CompanyId = @CompanyId,
                    CompanyOfficeId = @CompanyOfficeId,
                    PeriodYear = @PeriodYear,
                    PeriodMonth = @PeriodMonth,
                    TransactionDate = @TransactionDate,
                    RegionCode = @RegionCode,
                    ProductCategory = @ProductCategory,
                    ProductCode = @ProductCode,
                    ProductName = @ProductName,
                    SalesChannel = @SalesChannel,
                    CustomerSegment = @CustomerSegment,
                    Quantity = @Quantity,
                    UnitPrice = @UnitPrice,
                    DiscountAmount = @DiscountAmount,
                    NetAmount = @NetAmount,
                    CostAmount = @CostAmount,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE SalesTransactionGuid = @SalesTransactionGuid";
            await conn.ExecuteAsync(sql, salesTransaction, transaction);
        }

        public async Task SoftDeleteSalesTransactionAsync(SalesTransaction salesTransaction, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE SalesTransactionGuid = @SalesTransactionGuid";

            await conn.ExecuteAsync(sql, salesTransaction, transaction);
        }

        public async Task DeleteSalesTransactionAsync(Guid salesTransactionGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE SalesTransactionGuid = @salesTransactionGuid";
            await conn.ExecuteAsync(sql, new { salesTransactionGuid }, transaction);
        }

        public async Task<IEnumerable<SalesTransaction>> SearchSalesTransactionAsync(
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

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(transactionCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.TransactionCode", "@transactionCode", transactionCodeSearchType, parameters, "transactionCode", transactionCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyId", "@companyId", companyIdSearchType, parameters, "companyId", companyId, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(companyOfficeId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyOfficeId", "@companyOfficeId", companyOfficeIdSearchType, parameters, "companyOfficeId", companyOfficeId, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(periodYear))
            {
                var param = SqlFilterHelper.BuildFilter("a.PeriodYear", "@periodYear", periodYearSearchType, parameters, "periodYear", periodYear, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(periodMonth))
            {
                var param = SqlFilterHelper.BuildFilter("a.PeriodMonth", "@periodMonth", periodMonthSearchType, parameters, "periodMonth", periodMonth, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(transactionDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.TransactionDate", "@transactionDate", transactionDateSearchType, parameters, "transactionDate", transactionDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(regionCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.RegionCode", "@regionCode", regionCodeSearchType, parameters, "regionCode", regionCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(productCategory))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProductCategory", "@productCategory", productCategorySearchType, parameters, "productCategory", productCategory, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(productCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProductCode", "@productCode", productCodeSearchType, parameters, "productCode", productCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(productName))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProductName", "@productName", productNameSearchType, parameters, "productName", productName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(salesChannel))
            {
                var param = SqlFilterHelper.BuildFilter("a.SalesChannel", "@salesChannel", salesChannelSearchType, parameters, "salesChannel", salesChannel, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(customerSegment))
            {
                var param = SqlFilterHelper.BuildFilter("a.CustomerSegment", "@customerSegment", customerSegmentSearchType, parameters, "customerSegment", customerSegment, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(quantity))
            {
                var param = SqlFilterHelper.BuildFilter("a.Quantity", "@quantity", quantitySearchType, parameters, "quantity", quantity, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(unitPrice))
            {
                var param = SqlFilterHelper.BuildFilter("a.UnitPrice", "@unitPrice", unitPriceSearchType, parameters, "unitPrice", unitPrice, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(discountAmount))
            {
                var param = SqlFilterHelper.BuildFilter("a.DiscountAmount", "@discountAmount", discountAmountSearchType, parameters, "discountAmount", discountAmount, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(netAmount))
            {
                var param = SqlFilterHelper.BuildFilter("a.NetAmount", "@netAmount", netAmountSearchType, parameters, "netAmount", netAmount, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(costAmount))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostAmount", "@costAmount", costAmountSearchType, parameters, "costAmount", costAmount, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.SalesTransactionId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<SalesTransaction>(sql, parameters, transaction);
        }
    }
}
