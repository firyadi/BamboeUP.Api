using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/workTeams/{headerGuid:guid}/workTeamCapabilities")]
    [ApiController]
    public partial class WorkTeamCapabilitiesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public WorkTeamCapabilitiesController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.WorkTeamCapabilityService.GetAllByWorkTeamGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.WorkTeamCapabilityService.GetWorkTeamCapabilityByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] WorkTeamCapabilityForCreationDto input)
        {
            var created = await _service.WorkTeamCapabilityService.CreateWorkTeamCapabilityAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.WorkTeamCapabilityGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] WorkTeamCapabilityForUpdateDto input)
        {
            await _service.WorkTeamCapabilityService.UpdateWorkTeamCapabilityAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] WorkTeamCapabilityForDeleteDto input)
        {
            await _service.WorkTeamCapabilityService.DeleteWorkTeamCapabilityAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.WorkTeamCapabilityService.DeleteWorkTeamCapabilityByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.WorkTeamCapabilityService.GetByWorkTeamGuidAndWorkTeamCapabilityGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] WorkTeamCapabilitySearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.WorkTeamCapabilityService.SearchWorkTeamCapabilityAsync(
                                input.IsViewAble, input.IsViewAbleSearchType.ToString(),
                input.IsAddAble, input.IsAddAbleSearchType.ToString(),
                input.IsEditAble, input.IsEditAbleSearchType.ToString(),
                input.IsApproveAble, input.IsApproveAbleSearchType.ToString(),
                input.IsExportAble, input.IsExportAbleSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
