using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/awards")]
    [ApiController]
    public partial class AwardsController(IServiceModulesManager service) : ControllerBase
    {

        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.AwardService.GetAllAwardsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.AwardService.GetAwardByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] AwardForCreationDto input)
        {
            var created = await service.AwardService.CreateAwardAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.AwardGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] AwardForUpdateDto input)
        {
            await service.AwardService.UpdateAwardAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] AwardForDeleteDto input)
        {
            await service.AwardService.DeleteAwardAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.AwardService.DeleteAwardByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] AwardSearchDto input)
        {
            var result = await service.AwardService.SearchAwardAsync(
                input.AwardCode, input.AwardCodeSearchType.ToString(),
                input.AwardName, input.AwardNameSearchType.ToString(),
                input.ValidFrom, input.ValidFromSearchType.ToString(),
                input.Validto, input.ValidtoSearchType.ToString(),
                input.AwardPrize, input.AwardPrizeSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString()
                // ── FK Search pass-through ──
            );
            return Ok(result);
        }
    }
}
