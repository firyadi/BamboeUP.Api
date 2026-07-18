using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/people/{headerGuid:guid}/personFamilies")]
    [ApiController]
    public partial class PersonFamiliesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public PersonFamiliesController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.PersonFamilyService.GetAllByPersonGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonFamilyService.GetPersonFamilyByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] PersonFamilyForCreationDto input)
        {
            var created = await _service.PersonFamilyService.CreatePersonFamilyAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.PersonFamilyGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] PersonFamilyForUpdateDto input)
        {
            await _service.PersonFamilyService.UpdatePersonFamilyAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] PersonFamilyForDeleteDto input)
        {
            await _service.PersonFamilyService.DeletePersonFamilyAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.PersonFamilyService.DeletePersonFamilyByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonFamilyService.GetByPersonGuidAndPersonFamilyGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] PersonFamilySearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.PersonFamilyService.SearchPersonFamilyAsync(
                                input.SrFamilyRelation, input.SrFamilyRelationSearchType.ToString(),
                input.FamilyName, input.FamilyNameSearchType.ToString(),
                input.DateBirth, input.DateBirthSearchType.ToString(),
                input.SrEducationLevel, input.SrEducationLevelSearchType.ToString(),
                input.Address, input.AddressSearchType.ToString(),
                input.StateId, input.StateIdSearchType.ToString(),
                input.CityId, input.CityIdSearchType.ToString(),
                input.ZipCode, input.ZipCodeSearchType.ToString(),
                input.Phone, input.PhoneSearchType.ToString(),
                input.SrMaritalStatus, input.SrMaritalStatusSearchType.ToString(),
                input.SrGender, input.SrGenderSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
