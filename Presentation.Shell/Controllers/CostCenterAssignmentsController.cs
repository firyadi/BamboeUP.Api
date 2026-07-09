using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/costCenters/{headerGuid:guid}/costCenterAssignments")]
    [ApiController]
    public partial class CostCenterAssignmentsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public CostCenterAssignmentsController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.CostCenterAssignmentService.GetAllByCostCenterGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.CostCenterAssignmentService.GetCostCenterAssignmentByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] CostCenterAssignmentForCreationDto input)
        {
            var created = await _service.CostCenterAssignmentService.CreateCostCenterAssignmentAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.CostCenterAssignmentGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] CostCenterAssignmentForUpdateDto input)
        {
            await _service.CostCenterAssignmentService.UpdateCostCenterAssignmentAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] CostCenterAssignmentForDeleteDto input)
        {
            await _service.CostCenterAssignmentService.DeleteCostCenterAssignmentAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.CostCenterAssignmentService.DeleteCostCenterAssignmentByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.CostCenterAssignmentService.GetByCostCenterGuidAndCostCenterAssignmentGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] CostCenterAssignmentSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.CostCenterAssignmentService.SearchCostCenterAssignmentAsync(
                                input.CompanyId, input.CompanyIdSearchType.ToString(),
                input.CompanyOfficeId, input.CompanyOfficeIdSearchType.ToString(),
                input.ProfitCenterId, input.ProfitCenterIdSearchType.ToString(),
                input.CostCenterManagerEmployeeId, input.CostCenterManagerEmployeeIdSearchType.ToString(),
                input.BudgetControlType, input.BudgetControlTypeSearchType.ToString(),
                input.EffectiveDate, input.EffectiveDateSearchType.ToString(),
                input.ExpiredDate, input.ExpiredDateSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
