using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/costCenters")]
    [ApiController]
    public partial class CostCentersController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public CostCentersController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.CostCenterService.GetAllCostCentersAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.CostCenterService.GetCostCenterByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] CostCenterForCreationDto input)
        {
            var created = await _service.CostCenterService.CreateCostCenterAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.CostCenterGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] CostCenterForUpdateDto input)
        {
            await _service.CostCenterService.UpdateCostCenterAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] CostCenterForDeleteDto input)
        {
            await _service.CostCenterService.DeleteCostCenterAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.CostCenterService.DeleteCostCenterByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] CostCenterSearchDto input)
        {
            var result = await _service.CostCenterService.SearchCostCenterAsync(
                input.CostCenterCode, input.CostCenterCodeSearchType.ToString(),
                input.CostCenterName, input.CostCenterNameSearchType.ToString(),
                input.CostCenterDescription, input.CostCenterDescriptionSearchType.ToString(),
                input.ParentCostCenterId, input.ParentCostCenterIdSearchType.ToString(),
                input.LevelDepth, input.LevelDepthSearchType.ToString(),
                input.HierarchyPath, input.HierarchyPathSearchType.ToString()
                // ── FK Search pass-through ──
            );
            return Ok(result);
        }
    }
}
