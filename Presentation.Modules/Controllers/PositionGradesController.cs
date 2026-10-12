using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/positionGrades")]
    [ApiController]
    public partial class PositionGradesController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.PositionGradeService.GetAllPositionGradesAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.PositionGradeService.GetPositionGradeByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] PositionGradeForCreationDto input)
        {
            var created = await service.PositionGradeService.CreatePositionGradeAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.PositionGradeGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] PositionGradeForUpdateDto input)
        {
            await service.PositionGradeService.UpdatePositionGradeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] PositionGradeForDeleteDto input)
        {
            await service.PositionGradeService.DeletePositionGradeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.PositionGradeService.DeletePositionGradeByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] PositionGradeSearchDto input)
        {
            var result = await service.PositionGradeService.SearchPositionGradeAsync(
                input.PositionGradeCode, input.PositionGradeCodeSearchType.ToString(),
                input.PositionGradeName, input.PositionGradeNameSearchType.ToString(),
                input.Interval, input.IntervalSearchType.ToString(),
                input.Ranking, input.RankingSearchType.ToString(),
                input.PositionGradeDescription, input.PositionGradeDescriptionSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
