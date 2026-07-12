using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;

namespace Presentation.Shell.Controllers
{
    [Route("api/prints")]
    [ApiController]
    public class PrintsController : ControllerBase
    {
        private readonly IServiceShellManager _service;
        private readonly Contracts.IUserContext _userContext;

        public PrintsController(IServiceShellManager service, Contracts.IUserContext userContext)
        {
            _service = service;
            _userContext = userContext;
        }

        [HttpGet("allowed")]
        public async Task<IActionResult> GetAllowed(
            [FromQuery] string sourceProgramCode,
            [FromQuery] string? entityId = null)
        {
            if (string.IsNullOrWhiteSpace(sourceProgramCode))
                return BadRequest("sourceProgramCode is required.");

            var data = await _service.ReportService.GetAllowedPrintsAsync(
                sourceProgramCode.Trim(),
                _userContext.UserGuid,
                _userContext.CompanyId?.ToString(),
                _userContext.OfficeId?.ToString(),
                _userContext.IsAdmin,
                entityId);

            return Ok(data);
        }
    }
}
