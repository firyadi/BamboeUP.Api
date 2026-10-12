using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public partial class EmployeesController(IServiceModulesManager service) : ControllerBase
    {

        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.EmployeeService.GetAllEmployeesAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.EmployeeService.GetEmployeeByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] EmployeeForCreationDto input)
        {
            var created = await service.EmployeeService.CreateEmployeeAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.EmployeeGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] EmployeeForUpdateDto input)
        {
            await service.EmployeeService.UpdateEmployeeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] EmployeeForDeleteDto input)
        {
            await service.EmployeeService.DeleteEmployeeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.EmployeeService.DeleteEmployeeByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] EmployeeSearchDto input)
        {
            var result = await service.EmployeeService.SearchEmployeeAsync(
                input.EmployeeNo, input.EmployeeNoSearchType.ToString(),
                input.HireDate, input.HireDateSearchType.ToString(),
                input.ProbationEndDate, input.ProbationEndDateSearchType.ToString(),
                input.ConfirmationDate, input.ConfirmationDateSearchType.ToString(),
                input.TerminationDate, input.TerminationDateSearchType.ToString()
                // ── FK Search pass-through ──
            );
            return Ok(result);
        }
    }
}
