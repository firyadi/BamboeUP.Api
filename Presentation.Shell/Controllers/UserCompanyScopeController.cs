using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/users/{userGuid}/companyscope")]
    public class UserCompanyScopeController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public UserCompanyScopeController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCompanyScopes(Guid userGuid)
        {
            var result = await _service.UserCompanyScopeService.GetAllByUserGuidAsync(userGuid);
            return Ok(result);
        }

        [HttpGet("{guid}")]
        public async Task<IActionResult> GetUserCompanyScope(Guid userGuid, Guid guid)
        {
            var result = await _service.UserCompanyScopeService.GetByUserAndGuidAsync(userGuid, guid);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserCompanyScope(Guid userGuid, [FromBody] UserCompanyScopeForCreationDto input)
        {
            if (input == null) return BadRequest("UserCompanyScopeForCreationDto object is null");

            var result = await _service.UserCompanyScopeService.CreateAsync(userGuid, input);
            return CreatedAtAction(nameof(GetUserCompanyScope), new { userGuid, guid = result.UserCompanyScopeGuid }, result);
        }

        [HttpPut("{guid}")]
        public async Task<IActionResult> UpdateUserCompanyScope(Guid userGuid, Guid guid, [FromBody] UserCompanyScopeForUpdateDto input)
        {
            if (input == null) return BadRequest("UserCompanyScopeForUpdateDto object is null");

            await _service.UserCompanyScopeService.UpdateAsync(userGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid}")]
        public async Task<IActionResult> DeleteUserCompanyScope(Guid userGuid, Guid guid, [FromBody] UserCompanyScopeForDeleteDto input)
        {
            if (input == null) return BadRequest("UserCompanyScopeForDeleteDto object is null");

            await _service.UserCompanyScopeService.DeleteAsync(userGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid}")]
        public async Task<IActionResult> AdminDeleteUserCompanyScope(Guid userGuid, Guid guid)
        {
            await _service.UserCompanyScopeService.DeleteByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }
    }
}
