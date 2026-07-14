using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Shell.Controllers
{
    [Route("api/report-definitions")]
    [ApiController]
    public class ReportDefinitionsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public ReportDefinitionsController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.ReportDefinitionService.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.ReportDefinitionService.GetByGuidAsync(guid);
            return Ok(data);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ReportDefinitionSearchDto input)
        {
            var data = await _service.ReportDefinitionService.SearchAsync(input);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReportDefinitionForCreationDto input)
        {
            var created = await _service.ReportDefinitionService.CreateAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.ReportDefinitionGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] ReportDefinitionForUpdateDto input)
        {
            await _service.ReportDefinitionService.UpdateAsync(guid, input);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> Delete(Guid guid, [FromBody] ReportDefinitionForDeleteDto input)
        {
            await _service.ReportDefinitionService.DeleteAsync(guid, input);
            return NoContent();
        }

        [HttpGet("{guid:guid}/parameters")]
        public async Task<IActionResult> GetParameters(Guid guid)
        {
            var data = await _service.ReportDefinitionService.GetParametersAsync(guid);
            return Ok(data);
        }

        [HttpPut("{guid:guid}/parameters")]
        public async Task<IActionResult> ReplaceParameters(Guid guid, [FromBody] ReportParameterBatchReplaceDto input)
        {
            await _service.ReportDefinitionService.ReplaceParametersAsync(guid, input);
            return NoContent();
        }
    }
}
