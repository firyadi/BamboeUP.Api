using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Shared.Settings.Enums;

namespace Presentation.Modules.Controllers
{
    [Route("api/cities/{cityGuid:guid}/districts")]
    [ApiController]
    public class DistrictsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public DistrictsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid cityGuid)
        {
            var data = await _service.DistrictService.GetAllDistrictsAsync(cityGuid, trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid cityGuid, Guid guid)
        {
            var data = await _service.DistrictService.GetDistrictByGuidAsync(cityGuid, guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid cityGuid, [FromBody] DistrictForCreationDto input)
        {
            var created = await _service.DistrictService.CreateDistrictAsync(cityGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { cityGuid, guid = created.DistrictGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid cityGuid, Guid guid, [FromBody] DistrictForUpdateDto input)
        {
            await _service.DistrictService.UpdateDistrictAsync(cityGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid cityGuid, Guid guid, [FromBody] DistrictForDeleteDto input)
        {
            await _service.DistrictService.DeleteDistrictAsync(cityGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid cityGuid, Guid guid)
        {
            await _service.DistrictService.DeleteDistrictByAdminAsync(cityGuid, guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(Guid cityGuid, [FromQuery] DistrictSearchDto input)
        {
            var result = await _service.DistrictService.SearchDistrictAsync(
                cityGuid,
                input.DistrictName,
                input.DistrictNameSearchType.ToString()
            );

            return Ok(result);
        }
    }
}
