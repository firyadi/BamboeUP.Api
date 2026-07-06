using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public interface IDocumentNumberRequestService
    {
        Task<IEnumerable<DocumentNumberRequestDto>> GetAllDocumentNumberRequestsAsync(bool trackChanges);
        Task<DocumentNumberRequestDto> GetDocumentNumberRequestByGuidAsync(Guid guid, bool trackChanges);
        Task<DocumentNumberRequestDto> CreateDocumentNumberRequestAsync(DocumentNumberRequestForCreationDto input);
        Task UpdateDocumentNumberRequestAsync(Guid guid, DocumentNumberRequestForUpdateDto input, bool trackChanges);
        Task DeleteDocumentNumberRequestAsync(Guid guid, DocumentNumberRequestForDeleteDto input, bool trackChanges);
        Task DeleteDocumentNumberRequestByAdminAsync(Guid guid, bool trackChanges);
        Task<IEnumerable<DocumentNumberRequestDto>> SearchDocumentNumberRequestAsync(
            string? documentType, string? documentTypeSearchType,
            string? documentNo, string? documentNoSearchType,
            string? externalReference, string? externalReferenceSearchType,
            string? description, string? descriptionSearchType,
            string? status, string? statusSearchType,
            long? companyId, long? officeId, long? orgUnitId);
    }
}
