using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/attendances")]
    [ApiController]
    public partial class AttendancesController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.AttendanceService.GetAllAttendancesAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.AttendanceService.GetAttendanceByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] AttendanceForCreationDto input)
        {
            var created = await service.AttendanceService.CreateAttendanceAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.AttendanceGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] AttendanceForUpdateDto input)
        {
            await service.AttendanceService.UpdateAttendanceAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] AttendanceForDeleteDto input)
        {
            await service.AttendanceService.DeleteAttendanceAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.AttendanceService.DeleteAttendanceByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] AttendanceSearchDto input)
        {
            var result = await service.AttendanceService.SearchAttendanceAsync(
                input.AttendanceDate, input.AttendanceDateSearchType.ToString(),
                input.CheckinTime, input.CheckinTimeSearchType.ToString(),
                input.CheckOutTime, input.CheckOutTimeSearchType.ToString(),
                input.CheckinChannel, input.CheckinChannelSearchType.ToString(),
                input.CheckOutChannel, input.CheckOutChannelSearchType.ToString(),
                input.CheckinLatitude, input.CheckinLatitudeSearchType.ToString(),
                input.CheckinLongitude, input.CheckinLongitudeSearchType.ToString(),
                input.CheckinAccuracyMeters, input.CheckinAccuracyMetersSearchType.ToString(),
                input.CheckOutLatitude, input.CheckOutLatitudeSearchType.ToString(),
                input.CheckOutLongitude, input.CheckOutLongitudeSearchType.ToString(),
                input.CheckOutAccuracyMeters, input.CheckOutAccuracyMetersSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString()
, input.CompanyName, input.CompanyNameSearchType.ToString()
, input.CompanyOfficeName, input.CompanyOfficeNameSearchType.ToString()
, input.PersonName, input.PersonNameSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
