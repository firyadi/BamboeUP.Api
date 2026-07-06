using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Shell.Controllers
{
    [Route("api/usergroups/{userGroupGuid:guid}/programs")]
    [ApiController]
    public class UserGroupProgramsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public UserGroupProgramsController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserGroup(Guid userGroupGuid)
        {
            var data = await _service.UserGroupProgramService.GetUserGroupProgramsByUserGroupAsync(userGroupGuid, trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid userGroupGuid, Guid guid)
        {
            var data = await _service.UserGroupProgramService.GetUserGroupProgramAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid userGroupGuid, [FromBody] UserGroupProgramForCreationDto input)
        {
            var created = await _service.UserGroupProgramService.CreateUserGroupProgramAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { userGroupGuid, guid = created.UserGroupProgramGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid userGroupGuid, Guid guid, [FromBody] UserGroupProgramForUpdateDto input)
        {
            await _service.UserGroupProgramService.UpdateUserGroupProgramAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid userGroupGuid, Guid guid, [FromBody] UserGroupProgramForDeleteDto input)
        {
            await _service.UserGroupProgramService.DeleteUserGroupProgramAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid userGroupGuid, Guid guid)
        {
            await _service.UserGroupProgramService.DeleteUserGroupProgramByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }
    }
}
