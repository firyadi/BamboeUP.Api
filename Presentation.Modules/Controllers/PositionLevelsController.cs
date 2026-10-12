using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/positionLevels")]
    [ApiController]
    public partial class PositionLevelsController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.PositionLevelService.GetAllPositionLevelsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.PositionLevelService.GetPositionLevelByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] PositionLevelForCreationDto input)
        {
            var created = await service.PositionLevelService.CreatePositionLevelAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.PositionLevelGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] PositionLevelForUpdateDto input)
        {
            await service.PositionLevelService.UpdatePositionLevelAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] PositionLevelForDeleteDto input)
        {
            await service.PositionLevelService.DeletePositionLevelAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.PositionLevelService.DeletePositionLevelByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] PositionLevelSearchDto input)
        {
            var result = await service.PositionLevelService.SearchPositionLevelAsync(
                input.PositionLevelCode, input.PositionLevelCodeSearchType.ToString(),
                input.PositionLevelName, input.PositionLevelNameSearchType.ToString(),
                input.Ranking, input.RankingSearchType.ToString(),
                input.PositionLevelDescription, input.PositionLevelDescriptionSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
