using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/workTeams")]
    [ApiController]
    public partial class WorkTeamsController(IServiceModulesManager service) : ControllerBase
    {

        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.WorkTeamService.GetAllWorkTeamsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.WorkTeamService.GetWorkTeamByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] WorkTeamForCreationDto input)
        {
            var created = await service.WorkTeamService.CreateWorkTeamAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.WorkTeamGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] WorkTeamForUpdateDto input)
        {
            await service.WorkTeamService.UpdateWorkTeamAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] WorkTeamForDeleteDto input)
        {
            await service.WorkTeamService.DeleteWorkTeamAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.WorkTeamService.DeleteWorkTeamByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] WorkTeamSearchDto input)
        {
            var result = await service.WorkTeamService.SearchWorkTeamAsync(
                input.WorkTeamCode, input.WorkTeamCodeSearchType.ToString(),
                input.WorkTeamName, input.WorkTeamNameSearchType.ToString(),
                input.IsEditAble, input.IsEditAbleSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString()
                // ── FK Search pass-through ──
            );
            return Ok(result);
        }
    }
}
