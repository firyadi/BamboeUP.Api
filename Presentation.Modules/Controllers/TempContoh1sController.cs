using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/tempContoh1s")]
    [ApiController]
    public partial class TempContoh1sController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.TempContoh1Service.GetAllTempContoh1sAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.TempContoh1Service.GetTempContoh1ByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] TempContoh1ForCreationDto input)
        {
            var created = await service.TempContoh1Service.CreateTempContoh1Async(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.TempContoh1Guid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] TempContoh1ForUpdateDto input)
        {
            await service.TempContoh1Service.UpdateTempContoh1Async(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] TempContoh1ForDeleteDto input)
        {
            await service.TempContoh1Service.DeleteTempContoh1Async(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.TempContoh1Service.DeleteTempContoh1ByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] TempContoh1SearchDto input)
        {
            var result = await service.TempContoh1Service.SearchTempContoh1Async(
                input.PersonName, input.PersonNameSearchType.ToString(),
                input.AwardTypeName, input.AwardTypeNameSearchType.ToString()
            );
            return Ok(result);
        }
    }
}
