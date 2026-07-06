using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/audit")]
    [Produces("application/json")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        /// <summary>
        /// GET /api/audit/sessions
        /// Mendapatkan daftar audit sessions dengan filter pencarian dan pagination.
        /// </summary>
        [HttpGet("sessions")]
        public async Task<ActionResult<List<AuditSessionDto>>> GetSessions([FromQuery] AuditSearchFilter filter)
        {
            var sessions = await _auditService.GetSessionsAsync(filter);
            return Ok(sessions);
        }

        /// <summary>
        /// GET /api/audit/session/{id}
        /// Mendapatkan detail lengkap satu audit session berdasarkan id.
        /// </summary>
        [HttpGet("session/{id:long}")]
        public async Task<ActionResult<AuditSessionDto>> GetSession(long id)
        {
            var session = await _auditService.GetSessionAsync(id);
            if (session == null)
                return NotFound();

            return Ok(session);
        }

        /// <summary>
        /// GET /api/audit/entity/{table}/{key}
        /// Mendapatkan riwayat lengkap perubahan untuk entitas tertentu (dan child-child-nya).
        /// </summary>
        [HttpGet("entity/{table}/{key}")]
        public async Task<ActionResult<List<AuditSessionDto>>> GetEntityHistory(string table, string key)
        {
            var history = await _auditService.GetEntityHistoryAsync(table, key);
            return Ok(history);
        }

        /// <summary>
        /// GET /api/audit/deleted
        /// Mendapatkan daftar data/record yang sudah didelete beserta data snapshot-nya.
        /// </summary>
        [HttpGet("deleted")]
        public async Task<ActionResult<List<AuditSnapshotDto>>> GetDeletedRecords([FromQuery] AuditSearchFilter filter)
        {
            var snapshots = await _auditService.GetDeletedRecordsAsync(filter);
            return Ok(snapshots);
        }

        /// <summary>
        /// GET /api/audit/timeline
        /// Mendapatkan business activity timeline.
        /// </summary>
        [HttpGet("timeline")]
        public async Task<ActionResult<List<AuditSessionDto>>> GetTimeline([FromQuery] AuditSearchFilter filter)
        {
            var sessions = await _auditService.GetSessionsAsync(filter);
            return Ok(sessions);
        }

        /// <summary>
        /// GET /api/audit/search
        /// Melakukan pencarian global log audit berdasarkan kriteria filter.
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<List<AuditSessionDto>>> Search([FromQuery] AuditSearchFilter filter)
        {
            var sessions = await _auditService.GetSessionsAsync(filter);
            return Ok(sessions);
        }

        /// <summary>
        /// POST /api/audit/rebuild
        /// Menjalankan maintenance script untuk membangun/memperbaiki schema table & index audit di database.
        /// </summary>
        [HttpPost("rebuild")]
        public async Task<IActionResult> Rebuild()
        {
            await _auditService.RebuildAuditSchemaAsync();
            return Ok(new { Message = "Audit database schema rebuilt successfully." });
        }
    }
}
