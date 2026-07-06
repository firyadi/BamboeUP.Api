using Contracts.Approval;
using Dapper;
using Entities.Models.Approval;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository.Approval
{
    public class ApprovalDelegationRepository : IApprovalDelegationRepository
    {
        private readonly RepositoryContext _context;

        public ApprovalDelegationRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<ApprovalDelegation?> GetAsync(Guid guid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalDelegation] WHERE ApprovalDelegationGuid = @guid AND StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<ApprovalDelegation>(sql, new { guid });
        }

        public async Task<IEnumerable<ApprovalDelegation>> GetActiveDelegationsAsync(long delegatorUserId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalDelegation] 
                WHERE DelegatorUserId = @delegatorUserId 
                  AND StatusId > 0 AND IsActive = 1
                  AND EndDate >= CAST(GETUTCDATE() AS DATE)
                ORDER BY StartDate DESC";
            return await connection.QueryAsync<ApprovalDelegation>(sql, new { delegatorUserId });
        }

        public async Task<ApprovalDelegation?> GetActiveDelegationForUserAsync(long delegatorUserId, DateTime dateToCheck, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT TOP 1 * FROM [apv].[ApprovalDelegation] 
                WHERE DelegatorUserId = @delegatorUserId 
                  AND StatusId > 0 AND IsActive = 1
                  AND CAST(@dateToCheck AS DATE) BETWEEN StartDate AND EndDate
                ORDER BY StartDate DESC";
            return await connection.QuerySingleOrDefaultAsync<ApprovalDelegation>(sql, new { delegatorUserId, dateToCheck });
        }

        public async Task CreateAsync(ApprovalDelegation delegation, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(delegation);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [apv].[ApprovalDelegation]
                (ApprovalDelegationGuid, DelegatorUserId, DelegateUserId, StartDate, EndDate, IsActive, Notes, StatusId, CreatedById, CreatedTime)
                OUTPUT INSERTED.ApprovalDelegationId
                VALUES
                (@ApprovalDelegationGuid, @DelegatorUserId, @DelegateUserId, @StartDate, @EndDate, @IsActive, @Notes, @StatusId, @CreatedById, @CreatedTime)";
                 
            delegation.ApprovalDelegationId = await conn.ExecuteScalarAsync<long>(sql, delegation, transaction);
        }

        public async Task UpdateAsync(ApprovalDelegation delegation, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(delegation);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [apv].[ApprovalDelegation]
                SET DelegateUserId = @DelegateUserId, StartDate = @StartDate, EndDate = @EndDate, 
                    IsActive = @IsActive, Notes = @Notes, UpdatedById = @UpdatedById, UpdatedTime = @UpdatedTime
                WHERE ApprovalDelegationId = @ApprovalDelegationId";
                
            await conn.ExecuteAsync(sql, delegation, transaction);
        }

        public async Task DeleteAsync(Guid guid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = "UPDATE [apv].[ApprovalDelegation] SET StatusId = 0, DeletedTime = @DeletedTime WHERE ApprovalDelegationGuid = @guid";
            await conn.ExecuteAsync(sql, new { guid, DeletedTime = DateTime.UtcNow }, transaction);
        }
    }
}
