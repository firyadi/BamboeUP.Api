using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/documentNumberRequests")]
    [ApiController]
    public class DocumentNumberRequestsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public DocumentNumberRequestsController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all DocumentNumberRequests</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Retrieve all active DocumentNumberRequests.")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<DocumentNumberRequestDto>))]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.DocumentNumberRequestService.GetAllDocumentNumberRequestsAsync(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get DocumentNumberRequest by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Retrieve a single record by its Guid.")]
        [SwaggerResponse(200, "Success", typeof(DocumentNumberRequestDto))]
        [SwaggerResponse(404, "Not found")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.DocumentNumberRequestService.GetDocumentNumberRequestByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create DocumentNumberRequest</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Create a new DocumentNumberRequest record.")]
        [SwaggerResponse(201, "Created", typeof(DocumentNumberRequestDto))]
        public async Task<IActionResult> Create([FromBody] DocumentNumberRequestForCreationDto input)
        {
            var created = await _service.DocumentNumberRequestService.CreateDocumentNumberRequestAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.DocumentNumberRequestGuid }, created);
        }

        /// <summary>Update DocumentNumberRequest</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Update a record by its Guid.")]
        [SwaggerResponse(204, "No Content")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] DocumentNumberRequestForUpdateDto input)
        {
            await _service.DocumentNumberRequestService.UpdateDocumentNumberRequestAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete DocumentNumberRequest (set Status = Void)</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Mark a record as Void (soft delete).")]
        [SwaggerResponse(204, "No Content")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] DocumentNumberRequestForDeleteDto input)
        {
            await _service.DocumentNumberRequestService.DeleteDocumentNumberRequestAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete DocumentNumberRequest (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Permanently delete a record.")]
        [SwaggerResponse(204, "No Content")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.DocumentNumberRequestService.DeleteDocumentNumberRequestByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search DocumentNumberRequests</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Flexible search using dynamic filters.")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<DocumentNumberRequestDto>))]
        public async Task<IActionResult> Search([FromQuery] DocumentNumberRequestSearchDto input)
        {
            var result = await _service.DocumentNumberRequestService.SearchDocumentNumberRequestAsync(
                input.DocumentType, input.DocumentTypeSearchType.ToString(),
                input.DocumentNo, input.DocumentNoSearchType.ToString(),
                input.ExternalReference, input.ExternalReferenceSearchType.ToString(),
                input.Description, input.DescriptionSearchType.ToString(),
                input.Status, input.StatusSearchType.ToString(),
                input.CompanyId, input.OfficeId, input.OrgUnitId
            );
            return Ok(result);
        }
    }
}
