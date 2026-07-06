using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace BamboeUp.Api.Controllers
{
    [Route("api/autoNumberTemplates")]
    [ApiController]
    public class AutoNumberTemplatesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public AutoNumberTemplatesController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all AutoNumberTemplates</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Retrieve all active AutoNumberTemplates.")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.AutoNumberTemplateService.GetAllAutoNumberTemplatesAsync(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get AutoNumberTemplate by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Retrieve a single record by its Guid.")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.AutoNumberTemplateService.GetAutoNumberTemplateByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create AutoNumberTemplate</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Create a new record.")]
        public async Task<IActionResult> Create([FromBody] AutoNumberTemplateForCreationDto input)
        {
            var created = await _service.AutoNumberTemplateService.CreateAutoNumberTemplateAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.AutoNumberTemplateGuid }, created);
        }

        /// <summary>Update AutoNumberTemplate</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Update a record by its Guid.")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] AutoNumberTemplateForUpdateDto input)
        {
            await _service.AutoNumberTemplateService.UpdateAutoNumberTemplateAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete AutoNumberTemplate</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Mark a record as deleted (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] AutoNumberTemplateForDeleteDto input)
        {
            await _service.AutoNumberTemplateService.DeleteAutoNumberTemplateAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete AutoNumberTemplate (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Permanently delete a record.")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.AutoNumberTemplateService.DeleteAutoNumberTemplateByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search AutoNumberTemplates</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Flexible search using dynamic filters.")]
        public async Task<IActionResult> Search([FromQuery] AutoNumberTemplateSearchDto input)
        {
            var result = await _service.AutoNumberTemplateService.SearchAutoNumberTemplateAsync(
                input.TemplateName, input.TemplateNameSearchType.ToString(), 
                input.Description, input.DescriptionSearchType.ToString(), 
                input.EffectiveDateFrom, input.EffectiveDateTo,
                input.TemplateScopeType, input.CompanyId, input.CompanyOfficeId
            );
            return Ok(result);
        }

        // ─── Detail: AutoNumberComponent ─────────────────────────────────────────

        /// <summary>Get all AutoNumberComponents by AutoNumberTemplate Guid</summary>
        [HttpGet("{guid:guid}/components")]
        [SwaggerOperation(Summary = "Get Components by Template", Description = "Retrieve all AutoNumberComponents related to a specific AutoNumberTemplate.")]
        public async Task<IActionResult> GetComponents(Guid guid)
        {
            var data = await _service.AutoNumberComponentService.GetAllByAutoNumberTemplateGuidAsync(guid);
            return Ok(data);
        }

        // ─── Detail: AutoNumberCounter ────────────────────────────────────────────

        /// <summary>Get all AutoNumberCounters by AutoNumberTemplate Guid</summary>
        [HttpGet("{guid:guid}/counters")]
        [SwaggerOperation(Summary = "Get Counters by Template", Description = "Retrieve all AutoNumberCounters related to a specific AutoNumberTemplate.")]
        public async Task<IActionResult> GetCounters(Guid guid)
        {
            var data = await _service.AutoNumberCounterService.GetAllByAutoNumberTemplateGuidAsync(guid);
            return Ok(data);
        }
    }
}
