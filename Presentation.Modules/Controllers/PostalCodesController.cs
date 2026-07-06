using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Shared.Settings.Enums;

namespace Presentation.Modules.Controllers
{
    [Route("api/subdistricts/{subDistrictGuid:guid}/postalcodes")]
    [ApiController]
    public class PostalCodesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public PostalCodesController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid subDistrictGuid)
        {
            var data = await _service.PostalCodeService.GetAllPostalCodesAsync(subDistrictGuid, trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid subDistrictGuid, Guid guid)
        {
            var data = await _service.PostalCodeService.GetPostalCodeByGuidAsync(subDistrictGuid, guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid subDistrictGuid, [FromBody] PostalCodeForCreationDto input)
        {
            var created = await _service.PostalCodeService.CreatePostalCodeAsync(subDistrictGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { subDistrictGuid, guid = created.PostalCodeGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid subDistrictGuid, Guid guid, [FromBody] PostalCodeForUpdateDto input)
        {
            await _service.PostalCodeService.UpdatePostalCodeAsync(subDistrictGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid subDistrictGuid, Guid guid, [FromBody] PostalCodeForDeleteDto input)
        {
            await _service.PostalCodeService.DeletePostalCodeAsync(subDistrictGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid subDistrictGuid, Guid guid)
        {
            await _service.PostalCodeService.DeletePostalCodeByAdminAsync(subDistrictGuid, guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(Guid subDistrictGuid, [FromQuery] PostalCodeSearchDto input)
        {
            var result = await _service.PostalCodeService.SearchPostalCodeAsync(
                subDistrictGuid,
                input.PostalCodeValue,
                input.PostalCodeValueSearchType.ToString()
            );

            return Ok(result);
        }
    }
}
