using Contracts;
using Entities.Models;
using Mapster;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Shell
{
    public class DocumentNumberRequestService : IDocumentNumberRequestService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        public DocumentNumberRequestService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<DocumentNumberRequestDto>> GetAllDocumentNumberRequestsAsync(bool trackChanges)
        {
            var data = await _repository.DocumentNumberRequest.GetAllDocumentNumberRequestsAsync(trackChanges);
            return data.Adapt<IEnumerable<DocumentNumberRequestDto>>();
        }

        public async Task<DocumentNumberRequestDto> GetDocumentNumberRequestByGuidAsync(Guid guid, bool trackChanges)
        {
            var entity = await _repository.DocumentNumberRequest.GetDocumentNumberRequestAsync(guid, trackChanges);
            if (entity is null)
                throw new KeyNotFoundException($"DocumentNumberRequest with Guid '{guid}' not found.");
            return entity.Adapt<DocumentNumberRequestDto>();
        }

        public async Task<DocumentNumberRequestDto> CreateDocumentNumberRequestAsync(DocumentNumberRequestForCreationDto input)
        {
            var entity = input.Adapt<DocumentNumberRequest>();
            entity.DocumentNumberRequestGuid = Guid.NewGuid();
            entity.CreatedTime = DateTime.UtcNow;
            entity.Status = string.IsNullOrWhiteSpace(input.Status) ? "Used" : input.Status;

            await _repository.DocumentNumberRequest.CreateDocumentNumberRequestAsync(entity);
            return entity.Adapt<DocumentNumberRequestDto>();
        }

        public async Task UpdateDocumentNumberRequestAsync(Guid guid, DocumentNumberRequestForUpdateDto input, bool trackChanges)
        {
            var entity = await _repository.DocumentNumberRequest.GetDocumentNumberRequestAsync(guid, trackChanges);
            if (entity is null)
                throw new KeyNotFoundException($"DocumentNumberRequest with Guid '{guid}' not found.");

            entity.DocumentType      = input.DocumentType;
            entity.DocumentNo        = input.DocumentNo;
            entity.AutoNumberLogId   = input.AutoNumberLogId;
            entity.ExternalReference = input.ExternalReference;
            entity.Description       = input.Description;
            entity.Status            = input.Status;
            entity.CompanyId         = input.CompanyId;
            entity.OfficeId          = input.OfficeId;
            entity.OrgUnitId         = input.OrgUnitId;
            entity.UpdatedById       = input.UpdatedById;
            entity.UpdatedTime       = DateTime.UtcNow;

            await _repository.DocumentNumberRequest.UpdateDocumentNumberRequestAsync(entity);
        }

        public async Task DeleteDocumentNumberRequestAsync(Guid guid, DocumentNumberRequestForDeleteDto input, bool trackChanges)
        {
            var entity = await _repository.DocumentNumberRequest.GetDocumentNumberRequestAsync(guid, trackChanges);
            if (entity is null)
                throw new KeyNotFoundException($"DocumentNumberRequest with Guid '{guid}' not found.");

            await _repository.DocumentNumberRequest.SoftDeleteDocumentNumberRequestAsync(
                entity, input.DeletedById ?? 0);
        }

        public async Task DeleteDocumentNumberRequestByAdminAsync(Guid guid, bool trackChanges)
        {
            var entity = await _repository.DocumentNumberRequest.GetDocumentNumberRequestAsync(guid, trackChanges);
            if (entity is null)
                throw new KeyNotFoundException($"DocumentNumberRequest with Guid '{guid}' not found.");

            await _repository.DocumentNumberRequest.DeleteDocumentNumberRequestAsync(guid);
        }

        public async Task<IEnumerable<DocumentNumberRequestDto>> SearchDocumentNumberRequestAsync(
            string? documentType, string? documentTypeSearchType,
            string? documentNo, string? documentNoSearchType,
            string? externalReference, string? externalReferenceSearchType,
            string? description, string? descriptionSearchType,
            string? status, string? statusSearchType,
            long? companyId, long? officeId, long? orgUnitId)
        {
            var data = await _repository.DocumentNumberRequest.SearchDocumentNumberRequestAsync(
                documentType, documentTypeSearchType,
                documentNo, documentNoSearchType,
                externalReference, externalReferenceSearchType,
                description, descriptionSearchType,
                status, statusSearchType,
                companyId, officeId, orgUnitId);

            return data.Adapt<IEnumerable<DocumentNumberRequestDto>>();
        }
    }
}
