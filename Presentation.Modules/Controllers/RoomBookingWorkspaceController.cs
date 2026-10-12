using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/roomBookings")]
    [ApiController]
    public class RoomBookingWorkspaceController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All Room Bookings")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.RoomBookingService.GetAllRoomBookingsAsync(trackChanges: false);
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
            var data = await service.RoomBookingService.GetCalendarEventsAsync(start, end, category, search);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.RoomBookingService.GetRoomBookingByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] RoomBookingForCreationDto input)
        {
            var created = await service.RoomBookingService.CreateRoomBookingAsync(Guid.Empty, input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.RoomBookingGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] RoomBookingForUpdateDto input)
        {
            await service.RoomBookingService.UpdateRoomBookingAsync(Guid.Empty, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] RoomBookingForDeleteDto input)
        {
            await service.RoomBookingService.DeleteRoomBookingAsync(Guid.Empty, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.RoomBookingService.DeleteRoomBookingByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }
    }
}
