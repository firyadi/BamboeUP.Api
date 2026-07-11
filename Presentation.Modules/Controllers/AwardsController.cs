using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/awards")]
    [ApiController]
    public partial class AwardsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public AwardsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.AwardService.GetAllAwardsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.AwardService.GetAwardByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] AwardForCreationDto input)
        {
            var created = await _service.AwardService.CreateAwardAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.AwardGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] AwardForUpdateDto input)
        {
            await _service.AwardService.UpdateAwardAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] AwardForDeleteDto input)
        {
            await _service.AwardService.DeleteAwardAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.AwardService.DeleteAwardByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] AwardSearchDto input)
        {
            var result = await _service.AwardService.SearchAwardAsync(
                input.AwardCode, input.AwardCodeSearchType.ToString(),
                input.AwardName, input.AwardNameSearchType.ToString(),
                input.SrAwardCriteria, input.SrAwardCriteriaSearchType.ToString(),
                input.SrAwardType, input.SrAwardTypeSearchType.ToString(),
                input.ValidFrom, input.ValidFromSearchType.ToString(),
                input.Validto, input.ValidtoSearchType.ToString(),
                input.AwardPrize, input.AwardPrizeSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
