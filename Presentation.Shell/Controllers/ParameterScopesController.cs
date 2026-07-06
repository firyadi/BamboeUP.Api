using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/parameters/{headerGuid:guid}/parameterscopes")]
    [ApiController]
    public partial class ParameterscopesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public ParameterscopesController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.ParameterscopeService.GetAllByParameterGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.ParameterscopeService.GetParameterscopeByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] ParameterscopeForCreationDto input)
        {
            if (input.ParameterGuid == Guid.Empty) input.ParameterGuid = headerGuid;

            var created = await _service.ParameterscopeService.CreateParameterscopeAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.ParameterscopeGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] ParameterscopeForUpdateDto input)
        {
            if (input.ParameterGuid == Guid.Empty) input.ParameterGuid = headerGuid;

            await _service.ParameterscopeService.UpdateParameterscopeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] ParameterscopeForDeleteDto input)
        {
            await _service.ParameterscopeService.DeleteParameterscopeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.ParameterscopeService.DeleteParameterscopeByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.ParameterscopeService.GetByParameterGuidAndParameterscopeGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] ParameterscopeSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.ParameterscopeService.SearchParameterscopeAsync(
                input.Overridevalue, input.OverridevalueSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
