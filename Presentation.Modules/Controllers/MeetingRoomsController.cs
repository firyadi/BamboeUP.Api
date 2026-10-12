using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/meetingRooms")]
    [ApiController]
    public partial class MeetingRoomsController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.MeetingRoomService.GetAllMeetingRoomsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.MeetingRoomService.GetMeetingRoomByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] MeetingRoomForCreationDto input)
        {
            var created = await service.MeetingRoomService.CreateMeetingRoomAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.MeetingRoomGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] MeetingRoomForUpdateDto input)
        {
            await service.MeetingRoomService.UpdateMeetingRoomAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] MeetingRoomForDeleteDto input)
        {
            await service.MeetingRoomService.DeleteMeetingRoomAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.MeetingRoomService.DeleteMeetingRoomByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] MeetingRoomSearchDto input)
        {
            var result = await service.MeetingRoomService.SearchMeetingRoomAsync(
                input.RoomCode, input.RoomCodeSearchType.ToString(),
                input.RoomName, input.RoomNameSearchType.ToString(),
                input.Capacity, input.CapacitySearchType.ToString(),
                input.Location, input.LocationSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
