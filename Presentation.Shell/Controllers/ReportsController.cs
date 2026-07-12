using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Shell.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IServiceShellManager _service;
        private readonly Contracts.IUserContext _userContext;

        public ReportsController(IServiceShellManager service, Contracts.IUserContext userContext)
        {
            _service = service;
            _userContext = userContext;
        }

        [HttpGet("allowed")]
        public async Task<IActionResult> GetAllowed([FromQuery] string tp, [FromQuery] string? companyId, [FromQuery] string? officeId)
        {
            var data = await _service.ReportService.GetAllowedReportsAsync(
                tp,
                _userContext.UserGuid,
                companyId ?? _userContext.CompanyId?.ToString(),
                officeId ?? _userContext.OfficeId?.ToString(),
                _userContext.IsAdmin);
            return Ok(data);
        }

        [HttpGet("{programId:long}/parameters")]
        public async Task<IActionResult> GetParameters(
            long programId,
            [FromQuery] string tp,
            [FromQuery] long? companyId,
            [FromQuery] long? officeId,
            [FromQuery] string? companyName,
            [FromQuery] string? officeName)
        {
            var data = await _service.ReportService.GetReportParameterSchemaAsync(
                programId,
                companyId ?? _userContext.CompanyId,
                officeId ?? _userContext.OfficeId,
                tp,
                companyName,
                officeName);
            return Ok(data);
        }

        [HttpGet("lookups/{lookupType}")]
        public async Task<IActionResult> Lookup(
            string lookupType,
            [FromQuery] string? q,
            [FromQuery] long? companyId,
            [FromQuery] long? officeId,
            [FromQuery] string? lookupConfig,
            [FromQuery] int take = 20)
        {
            var data = await _service.ReportService.LookupAsync(
                lookupType,
                q,
                companyId ?? _userContext.CompanyId,
                officeId ?? _userContext.OfficeId,
                lookupConfig,
                take);
            return Ok(data);
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run([FromBody] ReportRunRequestDto input)
        {
            var result = await _service.ReportService.RunReportAsync(
                input,
                _userContext.UserGuid,
                _userContext.UserId,
                _userContext.CompanyId?.ToString(),
                _userContext.OfficeId?.ToString(),
                _userContext.IsAdmin);
            return Ok(result);
        }

        [HttpGet("execution")]
        public async Task<IActionResult> GetExecutionByPrintId([FromQuery] string printId)
        {
            if (!_userContext.IsAdmin)
                return Forbid();

            var data = await _service.ReportService.GetExecutionByPrintIdAsync(printId, includeFullPrintId: true);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [HttpGet("execution-logs")]
        public async Task<IActionResult> GetExecutionLogs([FromQuery] ReportExecutionLogQueryDto query)
        {
            var data = await _service.ReportService.GetExecutionLogsAsync(
                query,
                _userContext.UserGuid,
                _userContext.CompanyId?.ToString(),
                _userContext.OfficeId?.ToString(),
                _userContext.IsAdmin);
            return Ok(data);
        }
    }
}
