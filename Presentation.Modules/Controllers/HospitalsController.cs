using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/hospitals")]
    [ApiController]
    public partial class HospitalsController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.HospitalService.GetAllHospitalsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.HospitalService.GetHospitalByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] HospitalForCreationDto input)
        {
            var created = await service.HospitalService.CreateHospitalAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.HospitalGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] HospitalForUpdateDto input)
        {
            await service.HospitalService.UpdateHospitalAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] HospitalForDeleteDto input)
        {
            await service.HospitalService.DeleteHospitalAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.HospitalService.DeleteHospitalByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] HospitalSearchDto input)
        {
            var result = await service.HospitalService.SearchHospitalAsync(
                input.HospitalName, input.HospitalNameSearchType.ToString(),
                input.HospitalCode, input.HospitalCodeSearchType.ToString(),
                input.ShortName, input.ShortNameSearchType.ToString(),
                input.LicenseNo, input.LicenseNoSearchType.ToString(),
                input.HospitalType, input.HospitalTypeSearchType.ToString(),
                input.HospitalClass, input.HospitalClassSearchType.ToString(),
                input.PhoneNo, input.PhoneNoSearchType.ToString(),
                input.Email, input.EmailSearchType.ToString(),
                input.Website, input.WebsiteSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
