using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/people/{headerGuid:guid}/personPhysicalCharacteristics")]
    [ApiController]
    public partial class PersonPhysicalCharacteristicsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public PersonPhysicalCharacteristicsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.PersonPhysicalCharacteristicService.GetAllByPersonGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonPhysicalCharacteristicService.GetPersonPhysicalCharacteristicByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] PersonPhysicalCharacteristicForCreationDto input)
        {
            var created = await _service.PersonPhysicalCharacteristicService.CreatePersonPhysicalCharacteristicAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.PersonPhysicalCharacteristicGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] PersonPhysicalCharacteristicForUpdateDto input)
        {
            await _service.PersonPhysicalCharacteristicService.UpdatePersonPhysicalCharacteristicAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] PersonPhysicalCharacteristicForDeleteDto input)
        {
            await _service.PersonPhysicalCharacteristicService.DeletePersonPhysicalCharacteristicAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.PersonPhysicalCharacteristicService.DeletePersonPhysicalCharacteristicByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonPhysicalCharacteristicService.GetByPersonGuidAndPersonPhysicalCharacteristicGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] PersonPhysicalCharacteristicSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.PersonPhysicalCharacteristicService.SearchPersonPhysicalCharacteristicAsync(
                                input.SrPhysicalCharacteristic, input.SrPhysicalCharacteristicSearchType.ToString(),
                input.PhysicalValue, input.PhysicalValueSearchType.ToString(),
                input.SrMeasurementUnit, input.SrMeasurementUnitSearchType.ToString(),
                input.RecordedDate, input.RecordedDateSearchType.ToString(),
                input.Remarks, input.RemarksSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
