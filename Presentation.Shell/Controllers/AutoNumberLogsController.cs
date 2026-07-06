using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/autoNumberLogs")]
    [ApiController]
    public class AutoNumberLogsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public AutoNumberLogsController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all AutoNumberLogs (with optional filter)</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All (Filtered)", Description = "Mengambil semua data histori AutoNumberLog dengan opsional filter. (View-Only)")]
        [SwaggerResponse(200, "Berhasil mendapatkan daftar log", typeof(IEnumerable<AutoNumberLogDto>))]
        public async Task<IActionResult> GetAll([FromQuery] Shared.RequestFeatures.AutoNumberLogParameters parameters)
        {
            var data = await _service.AutoNumberLogService.GetLogsAsync(parameters, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get AutoNumberLog by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu data histori berdasarkan Guid. (View-Only)")]
        [SwaggerResponse(200, "Berhasil mendapatkan log", typeof(AutoNumberLogDto))]
        [SwaggerResponse(404, "Log tidak ditemukan")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.AutoNumberLogService.GetLogByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }
    }
}
