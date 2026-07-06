using BamboeUp.Audit.Contracts;
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
    public class ApprovalRequestRepository : IApprovalRequestRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public ApprovalRequestRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<ApprovalRequest?> GetAsync(Guid guid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalRequest] WHERE ApprovalRequestGuid = @guid";
            return await connection.QuerySingleOrDefaultAsync<ApprovalRequest>(sql, new { guid });
        }

        public async Task<ApprovalRequest?> GetByDocumentAsync(string moduleCode, Guid referenceGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [apv].[ApprovalRequest] WHERE ModuleCode = @moduleCode AND ReferenceGuid = @referenceGuid AND StatusId != 5"; // 5 = Cancelled
            return await connection.QuerySingleOrDefaultAsync<ApprovalRequest>(sql, new { moduleCode, referenceGuid });
        }

        public async Task<IEnumerable<ApprovalRequest>> GetPendingRequestsByUserIdAsync(long userId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            // Cari ApprovalRequest yang memiliki ApprovalStep (status Pending=1) untuk user tersebut
            const string sql = @"
                SELECT DISTINCT r.* 
                FROM [apv].[ApprovalRequest] r
                INNER JOIN [apv].[ApprovalStep] s ON r.ApprovalRequestId = s.ApprovalRequestId
                WHERE s.StatusId = 1 
                  AND r.StatusId IN (1, 2) 
                  AND (s.ApproverUserId = @userId OR s.DelegatedFromUserId = @userId)";
                  
            return await connection.QueryAsync<ApprovalRequest>(sql, new { userId });
        }

        public async Task CreateAsync(ApprovalRequest request, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(request);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [apv].[ApprovalRequest]
                (ApprovalRequestGuid, ApprovalTemplateId, RequestNumber, ModuleCode, ReferenceGuid, ReferenceNumber, 
                 RequestedByUserId, CurrentLevelOrder, StatusId, Notes, RequestedTime, CreatedById, CreatedTime)
                OUTPUT INSERTED.ApprovalRequestId
                VALUES
                (@ApprovalRequestGuid, @ApprovalTemplateId, @RequestNumber, @ModuleCode, @ReferenceGuid, @ReferenceNumber, 
                 @RequestedByUserId, @CurrentLevelOrder, @StatusId, @Notes, @RequestedTime, @CreatedById, @CreatedTime)";
                 
            request.ApprovalRequestId = await conn.ExecuteScalarAsync<long>(sql, request, transaction);
            await _auditService.LogAsync("CREATE", "ApprovalRequest", request.ApprovalRequestGuid.ToString(), request.CreatedById.ToString(), null, request);
        }

        public async Task UpdateAsync(ApprovalRequest request, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(request);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [apv].[ApprovalRequest]
                SET CurrentLevelOrder = @CurrentLevelOrder, StatusId = @StatusId, Notes = @Notes, 
                    CompletedTime = @CompletedTime, UpdatedById = @UpdatedById, UpdatedTime = @UpdatedTime
                WHERE ApprovalRequestId = @ApprovalRequestId";
                
            await conn.ExecuteAsync(sql, request, transaction);
            await _auditService.LogAsync("UPDATE", "ApprovalRequest", request.ApprovalRequestGuid.ToString(), request.UpdatedById?.ToString(), null, request);
        }
    }
}
