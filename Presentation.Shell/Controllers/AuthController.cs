using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Shell.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IDevelopmentLoginBootstrap _developmentLogin;
    private readonly ILogger<AuthController> _logger;
    private readonly IWebHostEnvironment _environment;

    public AuthController(
        IAuthenticationService authService,
        IDevelopmentLoginBootstrap developmentLogin,
        ILogger<AuthController> logger,
        IWebHostEnvironment environment)
    {
        _authService = authService;
        _developmentLogin = developmentLogin;
        _logger = logger;
        _environment = environment;
    }

    [HttpPost("login-scopes")]
    [AllowAnonymous]
    [EnableRateLimiting("LoginPolicy")]
    public async Task<IActionResult> GetLoginScopes([FromBody] LoginRequestDto login)
    {
        try
        {
            var preview = await _authService.GetLoginScopesPreviewAsync(login);
            return Ok(preview);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load login scopes for user {Username}", login?.Username);
            var message = _environment.IsDevelopment()
                ? $"Failed to load login scopes: {ex.Message}"
                : "Failed to load login scopes. Please contact the administrator.";
            return StatusCode(StatusCodes.Status500InternalServerError, new { message });
        }
    }

    [HttpPost("dev-login")]
    [AllowAnonymous]
    [EnableRateLimiting("LoginPolicy")]
    public async Task<IActionResult> DevLogin()
    {
        if (!_environment.IsDevelopment())
            return NotFound();

        try
        {
            await _developmentLogin.EnsureReadyAsync();
            var authResult = await _authService.LoginAsync(new LoginRequestDto
            {
                Username = IDevelopmentLoginBootstrap.UserName,
                Password = IDevelopmentLoginBootstrap.Password
            });
            return Ok(authResult);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Development login bootstrap failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("LoginPolicy")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto login)
    {
        try
        {
            var authResult = await _authService.LoginAsync(login);
            return Ok(authResult);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Username}", login?.Username);
            var message = _environment.IsDevelopment()
                ? $"Login failed: {ex.Message}"
                : "It seems there is a problem with your account. Please contact the administrator.";
            return StatusCode(StatusCodes.Status500InternalServerError, new { message });
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
            return Unauthorized(new { message = "Invalid token claims." });
        if (request == null)
            return BadRequest(new { message = "Request body is null." });

        try
        {
            await _authService.ChangePasswordAsync(userName, request);
            return Ok(new { message = "Password has been changed successfully." });
        }
        // 400 rather than 401: the token is valid, only the supplied current password is wrong.
        catch (Exception ex) when (ex is ArgumentException or UnauthorizedAccessException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("switch-scope")]
    [Authorize]
    public async Task<IActionResult> SwitchScope([FromBody] SwitchScopeRequestDto request)
    {
        try
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { message = "Invalid token claims." });

            var tokenDto = await _authService.SwitchScopeAsync(userName, request);
            return Ok(tokenDto);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to switch scope for user {UserName}", User.Identity?.Name);
            var message = _environment.IsDevelopment()
                ? $"Failed to switch scope: {ex.Message}"
                : "Failed to switch scope. Please contact the administrator.";
            return StatusCode(StatusCodes.Status500InternalServerError, new { message });
        }
    }
}
