using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/parameters")]
    [ApiController]
    public partial class ParametersController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public ParametersController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.ParameterService.GetAllParametersAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.ParameterService.GetParameterByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] ParameterForCreationDto input)
        {
            var created = await _service.ParameterService.CreateParameterAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.ParameterGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] ParameterForUpdateDto input)
        {
            await _service.ParameterService.UpdateParameterAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] ParameterForDeleteDto input)
        {
            await _service.ParameterService.DeleteParameterAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.ParameterService.DeleteParameterByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] ParameterSearchDto input)
        {
            var result = await _service.ParameterService.SearchParameterAsync(
                input.Parametername, input.ParameternameSearchType.ToString(), input.Parametervalue, input.ParametervalueSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
