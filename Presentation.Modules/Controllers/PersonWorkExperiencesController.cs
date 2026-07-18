using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/people/{headerGuid:guid}/personWorkExperiences")]
    [ApiController]
    public partial class PersonWorkExperiencesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public PersonWorkExperiencesController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.PersonWorkExperienceService.GetAllByPersonGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonWorkExperienceService.GetPersonWorkExperienceByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] PersonWorkExperienceForCreationDto input)
        {
            var created = await _service.PersonWorkExperienceService.CreatePersonWorkExperienceAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.PersonWorkExperienceGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] PersonWorkExperienceForUpdateDto input)
        {
            await _service.PersonWorkExperienceService.UpdatePersonWorkExperienceAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] PersonWorkExperienceForDeleteDto input)
        {
            await _service.PersonWorkExperienceService.DeletePersonWorkExperienceAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.PersonWorkExperienceService.DeletePersonWorkExperienceByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.PersonWorkExperienceService.GetByPersonGuidAndPersonWorkExperienceGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] PersonWorkExperienceSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.PersonWorkExperienceService.SearchPersonWorkExperienceAsync(
                                input.SrIndustry, input.SrIndustrySearchType.ToString(),
                input.SrEmploymentType, input.SrEmploymentTypeSearchType.ToString(),
                input.CompanyName, input.CompanyNameSearchType.ToString(),
                input.JobTitle, input.JobTitleSearchType.ToString(),
                input.Department, input.DepartmentSearchType.ToString(),
                input.Location, input.LocationSearchType.ToString(),
                input.Supervisor, input.SupervisorSearchType.ToString(),
                input.JobDescription, input.JobDescriptionSearchType.ToString(),
                input.StartDate, input.StartDateSearchType.ToString(),
                input.EndDate, input.EndDateSearchType.ToString(),
                input.IsCurrentEmployment, input.IsCurrentEmploymentSearchType.ToString(),
                input.LastSalary, input.LastSalarySearchType.ToString(),
                input.ReasonforLeaving, input.ReasonforLeavingSearchType.ToString(),
                input.Remarks, input.RemarksSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
