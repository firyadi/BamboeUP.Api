using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/positions")]
    [ApiController]
    public partial class PositionsController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.PositionService.GetAllPositionsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.PositionService.GetPositionByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] PositionForCreationDto input)
        {
            var created = await service.PositionService.CreatePositionAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.PositionGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] PositionForUpdateDto input)
        {
            await service.PositionService.UpdatePositionAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] PositionForDeleteDto input)
        {
            await service.PositionService.DeletePositionAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.PositionService.DeletePositionByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] PositionSearchDto input)
        {
            var result = await service.PositionService.SearchPositionAsync(
                input.PositionCode, input.PositionCodeSearchType.ToString(),
                input.PositionName, input.PositionNameSearchType.ToString(),
                input.Summary, input.SummarySearchType.ToString(),
                input.ValidFrom, input.ValidFromSearchType.ToString(),
                input.Validto, input.ValidtoSearchType.ToString(),
                input.PositionDescription, input.PositionDescriptionSearchType.ToString()
, input.PositionGradeName, input.PositionGradeNameSearchType.ToString()
, input.PositionLevelName, input.PositionLevelNameSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
