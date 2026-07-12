using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    // Old-style: does NOT inherit RepositoryBase<T>
    public partial class BankRepository : IBankRepository
    {
        private readonly RepositoryContext _context;

        public BankRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Bank?> GetBankAsync(Guid bankGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [core].[Bank] a
                WHERE a.BankGuid = @bankGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Bank>(sql, new { bankGuid });
        }

        public async Task<IEnumerable<Bank>> GetAllBanksAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [core].[Bank] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.BankId DESC";
            return await connection.QueryAsync<Bank>(sql);
        }

        public async Task CreateBankAsync(Bank bank, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(bank);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [core].[Bank]
                (BankGuid, CreatedById, StatusId, CreatedTime, BankName, BankInitial)
                VALUES
                (@BankGuid, @CreatedById, @StatusId, @CreatedTime, @BankName, @BankInitial)";
            await conn.ExecuteAsync(sql, bank, transaction);
        }

        public async Task UpdateBankAsync(Bank bank, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(bank);
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string sql = @"
                UPDATE [core].[Bank]
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

            const string sql = @"
                UPDATE [core].[Bank]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE BankGuid = @BankGuid";

            await conn.ExecuteAsync(sql, bank, transaction);
        }

        public async Task DeleteBankAsync(Guid bankGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string sql = @"DELETE FROM [core].[Bank] WHERE BankGuid = @bankGuid";
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
    var param = SqlFilterHelper.BuildFilter("a.BankName", "@bankName", bankNameSearchType, parameters, "bankName", bankName);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 BankInitial
if (!string.IsNullOrWhiteSpace(bankInitial))
{
    var param = SqlFilterHelper.BuildFilter("a.BankInitial", "@bankInitial", bankInitialSearchType, parameters, "bankInitial", bankInitial);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}
            

            var sql = $@"
                SELECT a.*
                
                FROM [core].[Bank] a
                
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.BankId DESC";

            return await connection.QueryAsync<Bank>(sql, parameters, transaction);
        }

        // Detail helpers (only emitted if entity has a parent)
        
    }
}
