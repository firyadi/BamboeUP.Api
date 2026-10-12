using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/employees/{headerGuid:guid}/employeeAssets")]
    [ApiController]
    public partial class EmployeeAssetsController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public EmployeeAssetsController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.EmployeeAssetService.GetAllByEmployeeGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.EmployeeAssetService.GetEmployeeAssetByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] EmployeeAssetForCreationDto input)
        {
            var created = await _service.EmployeeAssetService.CreateEmployeeAssetAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.EmployeeAssetGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] EmployeeAssetForUpdateDto input)
        {
            await _service.EmployeeAssetService.UpdateEmployeeAssetAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] EmployeeAssetForDeleteDto input)
        {
            await _service.EmployeeAssetService.DeleteEmployeeAssetAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.EmployeeAssetService.DeleteEmployeeAssetByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.EmployeeAssetService.GetByEmployeeGuidAndEmployeeAssetGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] EmployeeAssetSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.EmployeeAssetService.SearchEmployeeAssetAsync(
                                input.AssignedDate, input.AssignedDateSearchType.ToString(),
                input.ReturnedDate, input.ReturnedDateSearchType.ToString(),
                input.ExpectedReturnDate, input.ExpectedReturnDateSearchType.ToString(),
                input.IsCurrent, input.IsCurrentSearchType.ToString(),
                input.ConditionWhenAssigned, input.ConditionWhenAssignedSearchType.ToString(),
                input.ConditionWhenReturned, input.ConditionWhenReturnedSearchType.ToString(),
                input.Remarks, input.RemarksSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
