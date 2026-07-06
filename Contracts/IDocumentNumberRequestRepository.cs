using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDocumentNumberRequestRepository
    {
        Task<DocumentNumberRequest> GetDocumentNumberRequestAsync(Guid requestGuid, bool trackChanges);
        Task<IEnumerable<DocumentNumberRequest>> GetAllDocumentNumberRequestsAsync(bool trackChanges);
        Task CreateDocumentNumberRequestAsync(DocumentNumberRequest request, IDbTransaction? transaction = null);
        Task UpdateDocumentNumberRequestAsync(DocumentNumberRequest request, IDbTransaction? transaction = null);
        Task SoftDeleteDocumentNumberRequestAsync(DocumentNumberRequest request, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteDocumentNumberRequestAsync(Guid requestGuid, IDbTransaction? transaction = null);
        Task<IEnumerable<DocumentNumberRequest>> SearchDocumentNumberRequestAsync(
            string? documentType, string? documentTypeSearchType,
            string? documentNo, string? documentNoSearchType,
            string? externalReference, string? externalReferenceSearchType,
            string? description, string? descriptionSearchType,
            string? status, string? statusSearchType,
            long? companyId, long? officeId, long? orgUnitId,
            IDbTransaction? transaction = null);
    }
}
