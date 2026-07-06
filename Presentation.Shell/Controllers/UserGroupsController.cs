using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Shell.Controllers
{
    [Route("api/usergroups")]
    [ApiController]
    public class UserGroupsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public UserGroupsController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.UserGroupService.GetAllUserGroupsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.UserGroupService.GetUserGroupAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserGroupForCreationDto input)
        {
            var created = await _service.UserGroupService.CreateUserGroupAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.UserGroupGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] UserGroupForUpdateDto input)
        {
            await _service.UserGroupService.UpdateUserGroupAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] UserGroupForDeleteDto input)
        {
            await _service.UserGroupService.DeleteUserGroupAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.UserGroupService.DeleteUserGroupByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] UserGroupSearchDto input)
        {
            var result = await _service.UserGroupService.SearchUserGroupAsync(
                input.UserGroupName, input.UserGroupNameSearchType.ToString());

            return Ok(result);
        }
    }
}
