using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/holidays")]
    [ApiController]
    public partial class HolidaysController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.HolidayService.GetAllHolidaysAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("calendar/events")]
        [SwaggerOperation(Summary = "Calendar events by date range (UI-08)")]
        public async Task<IActionResult> GetCalendarEvents(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] string? category = null,
            [FromQuery] string? search = null)
        {
            if (end <= start)
                return BadRequest("end must be greater than start.");
            var data = await service.HolidayService.GetCalendarEventsAsync(start, end, category, search);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.HolidayService.GetHolidayByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] HolidayForCreationDto input)
        {
            var created = await service.HolidayService.CreateHolidayAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.HolidayGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] HolidayForUpdateDto input)
        {
            await service.HolidayService.UpdateHolidayAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] HolidayForDeleteDto input)
        {
            await service.HolidayService.DeleteHolidayAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.HolidayService.DeleteHolidayByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] HolidaySearchDto input)
        {
            var result = await service.HolidayService.SearchHolidayAsync(
                input.HolidayDate, input.HolidayDateSearchType.ToString(),
                input.HolidayName, input.HolidayNameSearchType.ToString(),
                input.HolidayDescription, input.HolidayDescriptionSearchType.ToString(),
                input.SrHolidayType, input.SrHolidayTypeSearchType.ToString(),
                input.IsNational, input.IsNationalSearchType.ToString(),
                input.IsCompanyHoliday, input.IsCompanyHolidaySearchType.ToString()

            );
            return Ok(result);
        }
    }
}
