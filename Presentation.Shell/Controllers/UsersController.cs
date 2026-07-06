using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Shell.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public UsersController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.UserService.GetAllUsersAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.UserService.GetUserByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserForCreationDto input)
        {
            var created = await _service.UserService.CreateUserAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.UserGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] UserForUpdateDto input)
        {
            await _service.UserService.UpdateUserAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] UserForDeleteDto input)
        {
            await _service.UserService.DeleteUserAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.UserService.DeleteUserByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] UserSearchDto input)
        {
            var result = await _service.UserService.SearchUserAsync(
                input.UserName, input.UserNameSearchType.ToString(), 
                input.FullName, input.FullNameSearchType.ToString(), 
                input.Email, input.EmailSearchType.ToString());

            return Ok(result);
        }

        [HttpGet("{userId:guid}/usergroupscopes")]
        public async Task<IActionResult> GetUserGroupScopesByUser(Guid userId)
        {
            var user = await _service.UserService.GetUserByGuidAsync(userId, trackChanges: false);
            if (user == null) return NotFound();

            var result = await _service.UserGroupScopeService.GetAllByUserIdAsync(user.UserId);
            return Ok(result);
        }

        [HttpGet("{userGuid:guid}/allowed-menus")]
        public async Task<IActionResult> GetAllowedMenus(Guid userGuid, [FromQuery] string? companyId, [FromQuery] string? officeId)
        {
            try
            {
                var result = await _service.UserService.GetAllowedMenusAsync(userGuid, companyId, officeId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{userId:guid}/usergroupscopes")]
        public async Task<IActionResult> CreateUserGroupScopeByUser(Guid userId, [FromBody] UserGroupScopeForCreationDto input)
        {
            if (input == null) return BadRequest("UserGroupScopeForCreationDto object is null");

            var result = await _service.UserGroupScopeService.CreateByUserAsync(userId, input);
            // Returning Ok or CreatedAtAction. Since we don't have GetUserGroupScopeByUser, we can return Ok.
            return Ok(result);
        }

        [HttpDelete("{userId:guid}/usergroupscopes/{id:guid}")]
        public async Task<IActionResult> DeleteUserGroupScopeByUser(Guid userId, Guid id, [FromBody] UserGroupScopeForDeleteDto input)
        {
            if (input == null) return BadRequest("UserGroupScopeForDeleteDto object is null");

            await _service.UserGroupScopeService.DeleteByUserAsync(userId, id, input, trackChanges: true);
            return NoContent();
        }
        
        /// <summary>
        /// Set scope tertentu sebagai default login untuk user.
        /// Semua scope default lain milik user yang sama akan di-unset secara atomik.
        /// </summary>
        [HttpPatch("{userId:guid}/usergroupscopes/{scopeGuid:guid}/set-default")]
        public async Task<IActionResult> SetDefaultUserGroupScope(Guid userId, Guid scopeGuid, [FromBody] UserGroupScopeSetDefaultDto input)
        {
            if (input == null) return BadRequest("UserGroupScopeSetDefaultDto object is null");

            var result = await _service.UserGroupScopeService.SetDefaultAsync(scopeGuid, input);
            return Ok(result);
        }

        /// <summary>
        /// Reset password pengguna berdasarkan username. Tidak memerlukan autentikasi (AllowAnonymous).
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDto dto)
        {
            if (dto == null) return BadRequest("Request body is null.");
            if (string.IsNullOrWhiteSpace(dto.UserName)) return BadRequest("Username is required.");
            if (string.IsNullOrWhiteSpace(dto.NewPassword)) return BadRequest("New password is required.");

            try
            {
                await _service.UserService.ResetPasswordAsync(dto);
                return Ok(new { message = "Password has been reset successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
