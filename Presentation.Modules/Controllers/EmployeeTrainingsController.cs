using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/employees/{headerGuid:guid}/employeeTrainings")]
    [ApiController]
    public partial class EmployeeTrainingsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public EmployeeTrainingsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.EmployeeTrainingService.GetAllByEmployeeGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.EmployeeTrainingService.GetEmployeeTrainingByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] EmployeeTrainingForCreationDto input)
        {
            var created = await _service.EmployeeTrainingService.CreateEmployeeTrainingAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.EmployeeTrainingGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] EmployeeTrainingForUpdateDto input)
        {
            await _service.EmployeeTrainingService.UpdateEmployeeTrainingAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] EmployeeTrainingForDeleteDto input)
        {
            await _service.EmployeeTrainingService.DeleteEmployeeTrainingAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.EmployeeTrainingService.DeleteEmployeeTrainingByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.EmployeeTrainingService.GetByEmployeeGuidAndEmployeeTrainingGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] EmployeeTrainingSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.EmployeeTrainingService.SearchEmployeeTrainingAsync(
                                input.StartDate, input.StartDateSearchType.ToString(),
                input.EndDate, input.EndDateSearchType.ToString(),
                input.DurationHour, input.DurationHourSearchType.ToString(),
                input.Score, input.ScoreSearchType.ToString(),
                input.Grade, input.GradeSearchType.ToString(),
                input.IsPassed, input.IsPassedSearchType.ToString(),
                input.CertificateNumber, input.CertificateNumberSearchType.ToString(),
                input.Remarks, input.RemarksSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
