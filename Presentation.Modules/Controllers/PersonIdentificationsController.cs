using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/people/{headerGuid:guid}/personIdentifications")]
    [ApiController]
    public partial class PersonIdentificationsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public PersonIdentificationsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.PersonIdentificationService.GetAllByPersonGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonIdentificationService.GetPersonIdentificationByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] PersonIdentificationForCreationDto input)
        {
            var created = await _service.PersonIdentificationService.CreatePersonIdentificationAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.PersonIdentificationGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] PersonIdentificationForUpdateDto input)
        {
            await _service.PersonIdentificationService.UpdatePersonIdentificationAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] PersonIdentificationForDeleteDto input)
        {
            await _service.PersonIdentificationService.DeletePersonIdentificationAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.PersonIdentificationService.DeletePersonIdentificationByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonIdentificationService.GetByPersonGuidAndPersonIdentificationGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] PersonIdentificationSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.PersonIdentificationService.SearchPersonIdentificationAsync(
                                input.SrIdentificationTypeId, input.SrIdentificationTypeIdSearchType.ToString(),
                input.IdentificationValue, input.IdentificationValueSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
