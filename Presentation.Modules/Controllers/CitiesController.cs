using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Shared.Settings.Enums;

namespace Presentation.Modules.Controllers
{
    [Route("api/provinces/{provinceGuid:guid}/cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public CitiesController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid provinceGuid)
        {
            var data = await _service.CityService.GetAllCitiesAsync(provinceGuid, trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid provinceGuid, Guid guid)
        {
            var data = await _service.CityService.GetCityByGuidAsync(provinceGuid, guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid provinceGuid, [FromBody] CityForCreationDto input)
        {
            var created = await _service.CityService.CreateCityAsync(provinceGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { provinceGuid, guid = created.CityGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid provinceGuid, Guid guid, [FromBody] CityForUpdateDto input)
        {
            await _service.CityService.UpdateCityAsync(provinceGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid provinceGuid, Guid guid, [FromBody] CityForDeleteDto input)
        {
            await _service.CityService.DeleteCityAsync(provinceGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid provinceGuid, Guid guid)
        {
            await _service.CityService.DeleteCityByAdminAsync(provinceGuid, guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(Guid provinceGuid, [FromQuery] CitySearchDto input)
        {
            var result = await _service.CityService.SearchCityAsync(
                provinceGuid,
                input.CityName,
                input.CityNameSearchType.ToString()
            );

            return Ok(result);
        }
    }
}
