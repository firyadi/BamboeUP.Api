using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    // Old-style: does NOT inherit RepositoryBase<T>
    public partial class BankRepository : IBankRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly string _table;
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public BankRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
            _table = dialect.Qualify("core", "Bank");
        }

        public async Task<Bank?> GetBankAsync(Guid bankGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_table} a
                WHERE a.BankGuid = @bankGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Bank>(sql, new { bankGuid });
        }

        public async Task<IEnumerable<Bank>> GetAllBanksAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_table} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.BankId DESC{_dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Bank>(sql);
        }

        public async Task CreateBankAsync(Bank bank, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(bank);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (BankGuid, CreatedById, StatusId, CreatedTime, BankName, BankInitial)
                VALUES
                (@BankGuid, @CreatedById, @StatusId, @CreatedTime, @BankName, @BankInitial)";
            await conn.ExecuteAsync(sql, bank, transaction);
        }

        public async Task UpdateBankAsync(Bank bank, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(bank);
            var conn = transaction?.Connection ?? _context.CreateConnection();

            var sql = $@"
                UPDATE {_table}
                SET BankName = @BankName,
                                      BankInitial = @BankInitial,
                                      StatusId = @StatusId,
                                      UpdatedById = @UpdatedById,
                                      UpdatedTime = @UpdatedTime
                WHERE BankGuid = @BankGuid";
            await conn.ExecuteAsync(sql, bank, transaction);
        }

        public async Task SoftDeleteBankAsync(Bank bank, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(bank, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();

            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE BankGuid = @BankGuid";

            await conn.ExecuteAsync(sql, bank, transaction);
        }

        public async Task DeleteBankAsync(Guid bankGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();

            var sql = $@"DELETE FROM {_table} WHERE BankGuid = @bankGuid";
            await conn.ExecuteAsync(sql, new { bankGuid }, transaction);
        }


        public async Task<IEnumerable<Bank>> SearchBankAsync(
            string? bankName, string? bankNameSearchType,
string? bankInitial, string? bankInitialSearchType,
IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            // 🔍 BankName
if (!string.IsNullOrWhiteSpace(bankName))
{
    var param = SqlFilterHelper.BuildFilter("a.BankName", "@bankName", bankNameSearchType, parameters, "bankName", bankName, _dialect);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 BankInitial
if (!string.IsNullOrWhiteSpace(bankInitial))
{
    var param = SqlFilterHelper.BuildFilter("a.BankInitial", "@bankInitial", bankInitialSearchType, parameters, "bankInitial", bankInitial, _dialect);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}
            

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_table} a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.BankId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Bank>(sql, parameters, transaction);
        }

        // Detail helpers (only emitted if entity has a parent)
        
    }
}
