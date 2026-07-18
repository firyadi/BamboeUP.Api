using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/people/{headerGuid:guid}/personContacts")]
    [ApiController]
    public partial class PersonContactsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public PersonContactsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.PersonContactService.GetAllByPersonGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonContactService.GetPersonContactByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] PersonContactForCreationDto input)
        {
            var created = await _service.PersonContactService.CreatePersonContactAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.PersonContactGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] PersonContactForUpdateDto input)
        {
            await _service.PersonContactService.UpdatePersonContactAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] PersonContactForDeleteDto input)
        {
            await _service.PersonContactService.DeletePersonContactAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.PersonContactService.DeletePersonContactByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonContactService.GetByPersonGuidAndPersonContactGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] PersonContactSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.PersonContactService.SearchPersonContactAsync(
                                input.SrContactType, input.SrContactTypeSearchType.ToString(),
                input.ContactValue, input.ContactValueSearchType.ToString(),
                input.IsPrimary, input.IsPrimarySearchType.ToString(),
                input.Remark, input.RemarkSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
