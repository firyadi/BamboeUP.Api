using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class DocumentNumberRequestRepository : IDocumentNumberRequestRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public DocumentNumberRequestRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<DocumentNumberRequest> GetDocumentNumberRequestAsync(Guid requestGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[DocumentNumberRequest] a
                WHERE a.DocumentNumberRequestGuid = @requestGuid
                  AND a.Status != 'Void'";
            return await connection.QuerySingleOrDefaultAsync<DocumentNumberRequest>(sql, new { requestGuid });
        }

        public async Task<IEnumerable<DocumentNumberRequest>> GetAllDocumentNumberRequestsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[DocumentNumberRequest] a
                WHERE a.Status != 'Void'
                ORDER BY a.DocumentNumberRequestId DESC";
            return await connection.QueryAsync<DocumentNumberRequest>(sql);
        }

        public async Task CreateDocumentNumberRequestAsync(DocumentNumberRequest request, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(request);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[DocumentNumberRequest]
                (DocumentNumberRequestGuid, DocumentType, DocumentNo, AutoNumberLogId, ExternalReference,
                 Description, Status, CompanyId, OfficeId, OrgUnitId, CreatedById, CreatedTime)
                VALUES
                (@DocumentNumberRequestGuid, @DocumentType, @DocumentNo, @AutoNumberLogId, @ExternalReference,
                 @Description, @Status, @CompanyId, @OfficeId, @OrgUnitId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, request, transaction);

            await _audit.LogAsync(
                actionType: "CREATE",
                tableName: "DocumentNumberRequest",
                primaryKey: request.DocumentNumberRequestGuid.ToString(),
                userId: request.CreatedById.ToString(),
                oldEntity: null,
                newEntity: request);
        }

        public async Task UpdateDocumentNumberRequestAsync(DocumentNumberRequest request, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(request);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetDocumentNumberRequestAsync(request.DocumentNumberRequestGuid, false);

            const string sql = @"
                UPDATE [app].[DocumentNumberRequest]
                SET DocumentType = @DocumentType,
                    DocumentNo = @DocumentNo,
                    AutoNumberLogId = @AutoNumberLogId,
                    ExternalReference = @ExternalReference,
                    Description = @Description,
                    Status = @Status,
                    CompanyId = @CompanyId,
                    OfficeId = @OfficeId,
                    OrgUnitId = @OrgUnitId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE DocumentNumberRequestGuid = @DocumentNumberRequestGuid";
            await conn.ExecuteAsync(sql, request, transaction);

            await _audit.LogAsync(
                actionType: "UPDATE",
                tableName: "DocumentNumberRequest",
                primaryKey: request.DocumentNumberRequestGuid.ToString(),
                userId: request.UpdatedById?.ToString() ?? "0",
                oldEntity: oldData,
                newEntity: request);
        }

        public async Task SoftDeleteDocumentNumberRequestAsync(DocumentNumberRequest request, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(request);
            request.UpdatedById = deletedBy;
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetDocumentNumberRequestAsync(request.DocumentNumberRequestGuid, false);

            const string sql = @"
                UPDATE [app].[DocumentNumberRequest]
                SET Status = 'Void',
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE DocumentNumberRequestGuid = @DocumentNumberRequestGuid";

            await conn.ExecuteAsync(sql, request, transaction);

            await _audit.LogAsync(
                actionType: "DELETE",
                tableName: "DocumentNumberRequest",
                primaryKey: request.DocumentNumberRequestGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task DeleteDocumentNumberRequestAsync(Guid requestGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetDocumentNumberRequestAsync(requestGuid, false);

            const string sql = @"DELETE FROM [app].[DocumentNumberRequest] WHERE DocumentNumberRequestGuid = @requestGuid";
            await conn.ExecuteAsync(sql, new { requestGuid }, transaction);

            await _audit.LogAsync(
                actionType: "DELETE_ADMIN",
                tableName: "DocumentNumberRequest",
                primaryKey: requestGuid.ToString(),
                userId: oldData?.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<DocumentNumberRequest>> SearchDocumentNumberRequestAsync(
            string? documentType, string? documentTypeSearchType,
            string? documentNo, string? documentNoSearchType,
            string? externalReference, string? externalReferenceSearchType,
            string? description, string? descriptionSearchType,
            string? status, string? statusSearchType,
            long? companyId, long? officeId, long? orgUnitId,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string> { "a.Status != 'Void'" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(documentType))
            {
                var param = SqlFilterHelper.BuildFilter("a.DocumentType", "@documentType", documentTypeSearchType, parameters, "documentType", documentType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(documentNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.DocumentNo", "@documentNo", documentNoSearchType, parameters, "documentNo", documentNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(externalReference))
            {
                var param = SqlFilterHelper.BuildFilter("a.ExternalReference", "@externalReference", externalReferenceSearchType, parameters, "externalReference", externalReference);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var param = SqlFilterHelper.BuildFilter("a.Status", "@status", statusSearchType, parameters, "status", status);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (companyId.HasValue)
            {
                whereClauses.Add("a.CompanyId = @CompanyId");
                parameters.Add("CompanyId", companyId.Value);
            }

            if (officeId.HasValue)
            {
                whereClauses.Add("a.OfficeId = @OfficeId");
                parameters.Add("OfficeId", officeId.Value);
            }

            if (orgUnitId.HasValue)
            {
                whereClauses.Add("a.OrgUnitId = @OrgUnitId");
                parameters.Add("OrgUnitId", orgUnitId.Value);
            }

            var sql = $@"
                SELECT a.*
                FROM [app].[DocumentNumberRequest] a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.DocumentNumberRequestId DESC";

            return await connection.QueryAsync<DocumentNumberRequest>(sql, parameters, transaction);
        }
    }
}
