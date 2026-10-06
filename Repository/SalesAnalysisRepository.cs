using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class SalesAnalysisRepository(RepositoryContext context, ISqlDialect dialect) : ISalesAnalysisRepository
    {
        private readonly string _table = dialect.Qualify("app", "SalesAnalysis");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<SalesAnalysis?> GetSalesAnalysisAsync(Guid salesAnalysisGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE a.SalesAnalysisGuid = @salesAnalysisGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<SalesAnalysis>(sql, new { salesAnalysisGuid });
        }

        public async Task<IEnumerable<SalesAnalysis>> GetAllSalesAnalysesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}



                FROM {_table} a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.SalesAnalysisId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<SalesAnalysis>(sql);
        }

        public async Task CreateSalesAnalysisAsync(SalesAnalysis salesAnalysis, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (SalesAnalysisGuid, CreatedById, StatusId, CreatedTime, SalesDate, Salesperson, Area, ProductType, ProductName, Quantity, SalesAmount, DiscountAmount, NetSales, CostAmount, ProfitAmount, SalesAnalysisDescription
                )
                VALUES
                (@SalesAnalysisGuid, @CreatedById, @StatusId, @CreatedTime, @SalesDate, @Salesperson, @Area, @ProductType, @ProductName, @Quantity, @SalesAmount, @DiscountAmount, @NetSales, @CostAmount, @ProfitAmount, @SalesAnalysisDescription
                ){dialect.ReturnIdentitySuffix("SalesAnalysisId")}";
            salesAnalysis.SalesAnalysisId = await conn.QuerySingleAsync<long>(sql, salesAnalysis, transaction);
        }

        public async Task UpdateSalesAnalysisAsync(SalesAnalysis salesAnalysis, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     SalesDate = @SalesDate,
                    Salesperson = @Salesperson,
                    Area = @Area,
                    ProductType = @ProductType,
                    ProductName = @ProductName,
                    Quantity = @Quantity,
                    SalesAmount = @SalesAmount,
                    DiscountAmount = @DiscountAmount,
                    NetSales = @NetSales,
                    CostAmount = @CostAmount,
                    ProfitAmount = @ProfitAmount,
                    SalesAnalysisDescription = @SalesAnalysisDescription,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE SalesAnalysisGuid = @SalesAnalysisGuid";
            await conn.ExecuteAsync(sql, salesAnalysis, transaction);
        }

        public async Task SoftDeleteSalesAnalysisAsync(SalesAnalysis salesAnalysis, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE SalesAnalysisGuid = @SalesAnalysisGuid";

            await conn.ExecuteAsync(sql, salesAnalysis, transaction);
        }

        public async Task DeleteSalesAnalysisAsync(Guid salesAnalysisGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE SalesAnalysisGuid = @salesAnalysisGuid";
            await conn.ExecuteAsync(sql, new { salesAnalysisGuid }, transaction);
        }

        public async Task<IEnumerable<SalesAnalysis>> SearchSalesAnalysisAsync(
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

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(salesDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.SalesDate", "@salesDate", salesDateSearchType, parameters, "salesDate", salesDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(salesperson))
            {
                var param = SqlFilterHelper.BuildFilter("a.Salesperson", "@salesperson", salespersonSearchType, parameters, "salesperson", salesperson, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(area))
            {
                var param = SqlFilterHelper.BuildFilter("a.Area", "@area", areaSearchType, parameters, "area", area, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(productType))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProductType", "@productType", productTypeSearchType, parameters, "productType", productType, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(productName))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProductName", "@productName", productNameSearchType, parameters, "productName", productName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(quantity))
            {
                var param = SqlFilterHelper.BuildFilter("a.Quantity", "@quantity", quantitySearchType, parameters, "quantity", quantity, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(salesAmount))
            {
                var param = SqlFilterHelper.BuildFilter("a.SalesAmount", "@salesAmount", salesAmountSearchType, parameters, "salesAmount", salesAmount, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(discountAmount))
            {
                var param = SqlFilterHelper.BuildFilter("a.DiscountAmount", "@discountAmount", discountAmountSearchType, parameters, "discountAmount", discountAmount, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(netSales))
            {
                var param = SqlFilterHelper.BuildFilter("a.NetSales", "@netSales", netSalesSearchType, parameters, "netSales", netSales, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(costAmount))
            {
                var param = SqlFilterHelper.BuildFilter("a.CostAmount", "@costAmount", costAmountSearchType, parameters, "costAmount", costAmount, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(profitAmount))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProfitAmount", "@profitAmount", profitAmountSearchType, parameters, "profitAmount", profitAmount, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(salesAnalysisDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.SalesAnalysisDescription", "@salesAnalysisDescription", salesAnalysisDescriptionSearchType, parameters, "salesAnalysisDescription", salesAnalysisDescription, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.SalesAnalysisId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<SalesAnalysis>(sql, parameters, transaction);
        }
    }
}
