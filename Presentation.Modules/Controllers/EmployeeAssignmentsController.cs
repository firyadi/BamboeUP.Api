using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/employees/{headerGuid:guid}/employeeAssignments")]
    [ApiController]
    public partial class EmployeeAssignmentsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public EmployeeAssignmentsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.EmployeeAssignmentService.GetAllByEmployeeGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.EmployeeAssignmentService.GetEmployeeAssignmentByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] EmployeeAssignmentForCreationDto input)
        {
            var created = await _service.EmployeeAssignmentService.CreateEmployeeAssignmentAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.EmployeeAssignmentGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] EmployeeAssignmentForUpdateDto input)
        {
            await _service.EmployeeAssignmentService.UpdateEmployeeAssignmentAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] EmployeeAssignmentForDeleteDto input)
        {
            await _service.EmployeeAssignmentService.DeleteEmployeeAssignmentAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.EmployeeAssignmentService.DeleteEmployeeAssignmentByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.EmployeeAssignmentService.GetByEmployeeGuidAndEmployeeAssignmentGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] EmployeeAssignmentSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.EmployeeAssignmentService.SearchEmployeeAssignmentAsync(
                                input.EffectiveFrom, input.EffectiveFromSearchType.ToString(),
                input.Effectiveto, input.EffectivetoSearchType.ToString(),
                input.IsPrimary, input.IsPrimarySearchType.ToString(),
                input.IsCurrent, input.IsCurrentSearchType.ToString(),
                input.Remarks, input.RemarksSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
