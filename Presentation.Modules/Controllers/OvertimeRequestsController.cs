using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/overtimeRequests")]
    [ApiController]
    public partial class OvertimeRequestsController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.OvertimeRequestService.GetAllOvertimeRequestsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.OvertimeRequestService.GetOvertimeRequestByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] OvertimeRequestForCreationDto input)
        {
            var created = await service.OvertimeRequestService.CreateOvertimeRequestAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.OvertimeRequestGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] OvertimeRequestForUpdateDto input)
        {
            await service.OvertimeRequestService.UpdateOvertimeRequestAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] OvertimeRequestForDeleteDto input)
        {
            await service.OvertimeRequestService.DeleteOvertimeRequestAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.OvertimeRequestService.DeleteOvertimeRequestByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] OvertimeRequestSearchDto input)
        {
            var result = await service.OvertimeRequestService.SearchOvertimeRequestAsync(
                input.RequestNo, input.RequestNoSearchType.ToString(),
                input.RequestDate, input.RequestDateSearchType.ToString(),
                input.OvertimeDate, input.OvertimeDateSearchType.ToString(),
                input.Reason, input.ReasonSearchType.ToString(),
                input.WorkDescription, input.WorkDescriptionSearchType.ToString(),
                input.PlannedStartTime, input.PlannedStartTimeSearchType.ToString(),
                input.PlannedEndTime, input.PlannedEndTimeSearchType.ToString(),
                input.TotalPlannedMinutes, input.TotalPlannedMinutesSearchType.ToString(),
                input.ActualStartTime, input.ActualStartTimeSearchType.ToString(),
                input.ActualEndTime, input.ActualEndTimeSearchType.ToString(),
                input.TotalActualMinutes, input.TotalActualMinutesSearchType.ToString(),
                input.ApprovedTime, input.ApprovedTimeSearchType.ToString(),
                input.RejectionReason, input.RejectionReasonSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString()
, input.ApprovedbyName, input.ApprovedbyNameSearchType.ToString()
, input.CompanyName, input.CompanyNameSearchType.ToString()
, input.CompanyOfficeName, input.CompanyOfficeNameSearchType.ToString()
, input.PersonName, input.PersonNameSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
