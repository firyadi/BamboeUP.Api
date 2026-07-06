using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects.Approval;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/approvals")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public ApprovalsController(IServiceShellManager service)
        {
            _service = service;
        }

        // --- Core Engine endpoints ---

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitRequest([FromBody] ApprovalRequestForCreationDto input)
        {
            if (input == null) return BadRequest("RequestForCreationDto object is null");

            var createdRequest = await _service.ApprovalService.SubmitRequestAsync(input);

            return CreatedAtRoute("ApprovalRequestById", new { guid = createdRequest.ApprovalRequestGuid }, createdRequest);
        }

        [HttpGet("{guid:guid}", Name = "ApprovalRequestById")]
        public async Task<IActionResult> GetRequest(Guid guid)
        {
            var req = await _service.ApprovalService.GetRequestAsync(guid, trackChanges: false);
            if (req == null) return NotFound();
            
            return Ok(req);
        }

        [HttpGet("pending/{userId:long}")]
        public async Task<IActionResult> GetMyPendingRequests(long userId)
        {
            var reqs = await _service.ApprovalService.GetMyPendingRequestsAsync(userId, trackChanges: false);
            return Ok(reqs);
        }

        [HttpPost("{requestGuid:guid}/steps/{stepGuid:guid}/approve")]
        public async Task<IActionResult> ApproveStep(Guid requestGuid, Guid stepGuid, [FromBody] ApprovalRequestForActionDto input)
        {
            if (input == null) return BadRequest("Action details cannot be null");
            
            await _service.ApprovalService.ApproveStepAsync(requestGuid, stepGuid, input);
            return NoContent();
        }

        [HttpPost("{requestGuid:guid}/steps/{stepGuid:guid}/reject")]
        public async Task<IActionResult> RejectStep(Guid requestGuid, Guid stepGuid, [FromBody] ApprovalRequestForActionDto input)
        {
            if (input == null) return BadRequest("Action details cannot be null");
            
            await _service.ApprovalService.RejectStepAsync(requestGuid, stepGuid, input);
            return NoContent();
        }

        [HttpPost("{requestGuid:guid}/cancel")]
        public async Task<IActionResult> CancelRequest(Guid requestGuid, [FromQuery] long cancelledByUserId)
        {
            await _service.ApprovalService.CancelRequestAsync(requestGuid, cancelledByUserId);
            return NoContent();
        }
    }
}
