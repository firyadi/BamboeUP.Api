using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Shell.Controllers
{
    [Route("api/programs")]
    [ApiController]
    public class ProgramsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public ProgramsController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.ProgramService.GetAllProgramsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.ProgramService.GetProgramByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProgramForCreationDto input)
        {
            var created = await _service.ProgramService.CreateProgramAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.ProgramGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] ProgramForUpdateDto input)
        {
            await _service.ProgramService.UpdateProgramAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] ProgramForDeleteDto input)
        {
            await _service.ProgramService.DeleteProgramAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.ProgramService.DeleteProgramByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ProgramSearchDto input)
        {
            var result = await _service.ProgramService.SearchProgramAsync(
                input.ProgramName,
                input.ProgramNameSearchType.ToString(),
                input.ProgramCode,
                input.ProgramCodeSearchType.ToString()
            );

            return Ok(result);
        }
    }
}
