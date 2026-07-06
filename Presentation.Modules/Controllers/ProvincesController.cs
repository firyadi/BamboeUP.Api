using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Shared.Settings.Enums;

namespace Presentation.Modules.Controllers
{
    [Route("api/countries/{countryGuid:guid}/provinces")]
    [ApiController]
    public class ProvincesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public ProvincesController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid countryGuid)
        {
            var data = await _service.ProvinceService.GetAllProvincesAsync(countryGuid, trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid countryGuid, Guid guid)
        {
            var data = await _service.ProvinceService.GetProvinceByGuidAsync(countryGuid, guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid countryGuid, [FromBody] ProvinceForCreationDto input)
        {
            var created = await _service.ProvinceService.CreateProvinceAsync(countryGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { countryGuid, guid = created.ProvinceGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid countryGuid, Guid guid, [FromBody] ProvinceForUpdateDto input)
        {
            await _service.ProvinceService.UpdateProvinceAsync(countryGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid countryGuid, Guid guid, [FromBody] ProvinceForDeleteDto input)
        {
            await _service.ProvinceService.DeleteProvinceAsync(countryGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid countryGuid, Guid guid)
        {
            await _service.ProvinceService.DeleteProvinceByAdminAsync(countryGuid, guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(Guid countryGuid, [FromQuery] ProvinceSearchDto input)
        {
            var result = await _service.ProvinceService.SearchProvinceAsync(
                countryGuid,
                input.ProvinceName,
                input.ProvinceNameSearchType.ToString()
            );

            return Ok(result);
        }
    }
}
