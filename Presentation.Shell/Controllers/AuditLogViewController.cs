using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/auditlogview")]
    [Produces("application/json")]
    public class AuditLogViewController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditLogViewController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        /// <summary>
        /// Mendapatkan riwayat audit suatu entity (termasuk child/detail entities)
        /// berdasarkan nama tabel dan primary key (EntityKey).
        /// </summary>
        [HttpGet("table/{tableName}/key/{entityKey}")]
        public async Task<ActionResult<List<AuditSessionDto>>> GetEntityHistory(string tableName, string entityKey)
        {
            var sessions = await _auditService.GetEntityHistoryAsync(tableName, entityKey);
            if (sessions == null || sessions.Count == 0)
                return NotFound();

            return Ok(sessions);
        }

        /// <summary>
        /// Mendapatkan detail satu audit session beserta semua log dan perubahannya.
        /// </summary>
        [HttpGet("session/{auditSessionId:long}")]
        public async Task<ActionResult<AuditSessionDto>> GetSession(long auditSessionId)
        {
            var session = await _auditService.GetSessionAsync(auditSessionId);
            if (session == null)
                return NotFound();

            return Ok(session);
        }
    }
}
