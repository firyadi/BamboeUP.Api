using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Shared.Settings.Enums;

namespace Presentation.Modules.Controllers
{
    [Route("api/districts/{districtGuid:guid}/subdistricts")]
    [ApiController]
    public class SubDistrictsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public SubDistrictsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid districtGuid)
        {
            var data = await _service.SubDistrictService.GetAllSubDistrictsAsync(districtGuid, trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid districtGuid, Guid guid)
        {
            var data = await _service.SubDistrictService.GetSubDistrictByGuidAsync(districtGuid, guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid districtGuid, [FromBody] SubDistrictForCreationDto input)
        {
            var created = await _service.SubDistrictService.CreateSubDistrictAsync(districtGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { districtGuid, guid = created.SubDistrictGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid districtGuid, Guid guid, [FromBody] SubDistrictForUpdateDto input)
        {
            await _service.SubDistrictService.UpdateSubDistrictAsync(districtGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid districtGuid, Guid guid, [FromBody] SubDistrictForDeleteDto input)
        {
            await _service.SubDistrictService.DeleteSubDistrictAsync(districtGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid districtGuid, Guid guid)
        {
            await _service.SubDistrictService.DeleteSubDistrictByAdminAsync(districtGuid, guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(Guid districtGuid, [FromQuery] SubDistrictSearchDto input)
        {
            var result = await _service.SubDistrictService.SearchSubDistrictAsync(
                districtGuid,
                input.SubDistrictName,
                input.SubDistrictNameSearchType.ToString()
            );

            return Ok(result);
        }
    }
}
