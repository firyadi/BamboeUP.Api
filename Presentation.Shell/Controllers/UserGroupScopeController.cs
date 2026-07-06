using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [ApiController]
    public class UserGroupScopeController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public UserGroupScopeController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet("api/usergroups/{userGroupGuid}/scope")]
        public async Task<IActionResult> GetUserGroupScopes(Guid userGroupGuid)
        {
            var result = await _service.UserGroupScopeService.GetAllByUserGroupGuidAsync(userGroupGuid, trackChanges: false);
            return Ok(result);
        }

        [HttpGet("api/usergroups/{userGroupGuid}/scope/{guid}")]
        public async Task<IActionResult> GetUserGroupScope(Guid userGroupGuid, Guid guid)
        {
            var result = await _service.UserGroupScopeService.GetByUserGroupAndGuidAsync(userGroupGuid, guid);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("api/usergroups/{userGroupGuid}/scope")]
        public async Task<IActionResult> CreateUserGroupScope(Guid userGroupGuid, [FromBody] UserGroupScopeForCreationDto input)
        {
            if (input == null) return BadRequest("UserGroupScopeForCreationDto object is null");

            var result = await _service.UserGroupScopeService.CreateAsync(userGroupGuid, input);
            return CreatedAtAction(nameof(GetUserGroupScope), new { userGroupGuid, guid = result.UserGroupScopeGuid }, result);
        }

        [HttpDelete("api/usergroups/{userGroupGuid}/scope/{guid}")]
        public async Task<IActionResult> DeleteUserGroupScope(Guid userGroupGuid, Guid guid, [FromBody] UserGroupScopeForDeleteDto input)
        {
            if (input == null) return BadRequest("UserGroupScopeForDeleteDto object is null");

            await _service.UserGroupScopeService.DeleteAsync(userGroupGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("api/usergroups/{userGroupGuid}/scope/admin/{guid}")]
        public async Task<IActionResult> AdminDeleteUserGroupScope(Guid userGroupGuid, Guid guid)
        {
            await _service.UserGroupScopeService.DeleteByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

    }
}
