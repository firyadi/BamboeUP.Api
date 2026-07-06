using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/organizationUnits/{headerGuid:guid}/organizationUnitScopes")]
    [ApiController]
    public partial class OrganizationUnitScopesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public OrganizationUnitScopesController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.OrganizationUnitScopeService.GetAllByOrganizationUnitGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.OrganizationUnitScopeService.GetOrganizationUnitScopeByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] OrganizationUnitScopeForCreationDto input)
        {
            var created = await _service.OrganizationUnitScopeService.CreateOrganizationUnitScopeAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.OrganizationUnitScopeGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] OrganizationUnitScopeForUpdateDto input)
        {
            await _service.OrganizationUnitScopeService.UpdateOrganizationUnitScopeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] OrganizationUnitScopeForDeleteDto input)
        {
            await _service.OrganizationUnitScopeService.DeleteOrganizationUnitScopeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.OrganizationUnitScopeService.DeleteOrganizationUnitScopeByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.OrganizationUnitScopeService.GetByOrganizationUnitGuidAndOrganizationUnitScopeGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] OrganizationUnitScopeSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.OrganizationUnitScopeService.SearchOrganizationUnitScopeAsync(
                input.ScopeType, input.ScopeTypeSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
