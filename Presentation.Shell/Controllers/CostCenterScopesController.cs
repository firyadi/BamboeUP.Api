using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/costCenters/{headerGuid:guid}/costCenterScopes")]
    [ApiController]
    public partial class CostCenterScopesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public CostCenterScopesController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.CostCenterScopeService.GetAllByCostCenterGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.CostCenterScopeService.GetCostCenterScopeByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] CostCenterScopeForCreationDto input)
        {
            var created = await _service.CostCenterScopeService.CreateCostCenterScopeAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.CostCenterScopeGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] CostCenterScopeForUpdateDto input)
        {
            await _service.CostCenterScopeService.UpdateCostCenterScopeAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] CostCenterScopeForDeleteDto input)
        {
            await _service.CostCenterScopeService.DeleteCostCenterScopeAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.CostCenterScopeService.DeleteCostCenterScopeByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.CostCenterScopeService.GetByCostCenterGuidAndCostCenterScopeGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] CostCenterScopeSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.CostCenterScopeService.SearchCostCenterScopeAsync(
                                input.CompanyId, input.CompanyIdSearchType.ToString(),
                input.CompanyOfficeId, input.CompanyOfficeIdSearchType.ToString(),
                input.ScopeType, input.ScopeTypeSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
