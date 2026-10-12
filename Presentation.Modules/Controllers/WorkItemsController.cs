using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/people/{headerGuid:guid}/workItems")]
    [ApiController]
    public partial class WorkItemsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public WorkItemsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.WorkItemService.GetAllByPersonGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.WorkItemService.GetWorkItemByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] WorkItemForCreationDto input)
        {
            var created = await _service.WorkItemService.CreateWorkItemAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.WorkItemGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] WorkItemForUpdateDto input)
        {
            await _service.WorkItemService.UpdateWorkItemAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] WorkItemForDeleteDto input)
        {
            await _service.WorkItemService.DeleteWorkItemAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.WorkItemService.DeleteWorkItemByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.WorkItemService.GetByPersonGuidAndWorkItemGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] WorkItemSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.WorkItemService.SearchWorkItemAsync(
                                input.WorkItemCode, input.WorkItemCodeSearchType.ToString(),
                input.Title, input.TitleSearchType.ToString(),
                input.Description, input.DescriptionSearchType.ToString(),
                input.Module, input.ModuleSearchType.ToString(),
                input.WorkItemType, input.WorkItemTypeSearchType.ToString(),
                input.OwnerName, input.OwnerNameSearchType.ToString(),
                input.WorkflowStatus, input.WorkflowStatusSearchType.ToString(),
                input.Priority, input.PrioritySearchType.ToString(),
                input.CompletionPercent, input.CompletionPercentSearchType.ToString(),
                input.Tags, input.TagsSearchType.ToString(),
                input.Framework, input.FrameworkSearchType.ToString(),
                input.DueDate, input.DueDateSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
