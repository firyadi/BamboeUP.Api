using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/employees/{headerGuid:guid}/employeeLanguages")]
    [ApiController]
    public partial class EmployeeLanguagesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public EmployeeLanguagesController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.EmployeeLanguageService.GetAllByEmployeeGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.EmployeeLanguageService.GetEmployeeLanguageByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] EmployeeLanguageForCreationDto input)
        {
            var created = await _service.EmployeeLanguageService.CreateEmployeeLanguageAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.EmployeeLanguageGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] EmployeeLanguageForUpdateDto input)
        {
            await _service.EmployeeLanguageService.UpdateEmployeeLanguageAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] EmployeeLanguageForDeleteDto input)
        {
            await _service.EmployeeLanguageService.DeleteEmployeeLanguageAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.EmployeeLanguageService.DeleteEmployeeLanguageByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.EmployeeLanguageService.GetByEmployeeGuidAndEmployeeLanguageGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] EmployeeLanguageSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.EmployeeLanguageService.SearchEmployeeLanguageAsync(
                                input.CertificationName, input.CertificationNameSearchType.ToString(),
                input.Score, input.ScoreSearchType.ToString(),
                input.IssuedDate, input.IssuedDateSearchType.ToString(),
                input.ExpiredDate, input.ExpiredDateSearchType.ToString(),
                input.IsPrimary, input.IsPrimarySearchType.ToString(),
                input.Remarks, input.RemarksSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
