using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Shared.Settings.Enums;

namespace Presentation.Modules.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public CountriesController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.CountryService.GetAllCountriesAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.CountryService.GetCountryByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CountryForCreationDto input)
        {
            var created = await _service.CountryService.CreateCountryAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.CountryGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] CountryForUpdateDto input)
        {
            await _service.CountryService.UpdateCountryAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] CountryForDeleteDto input)
        {
            await _service.CountryService.DeleteCountryAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.CountryService.DeleteCountryByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] CountrySearchDto input)
        {
            var result = await _service.CountryService.SearchCountryAsync(
                input.CountryName,
                input.CountryNameSearchType.ToString(),
                input.CountryIso,
                input.CountryIsoSearchType.ToString()
            );

            return Ok(result);
        }
    }
}
