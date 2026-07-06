using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects.Approval;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/approval-delegations")]
    [ApiController]
    public class ApprovalDelegationsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public ApprovalDelegationsController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet("user/{userId:long}")]
        public async Task<IActionResult> GetMyDelegations(long userId)
        {
            var delegations = await _service.ApprovalDelegationService.GetMyDelegationsAsync(userId, trackChanges: false);
            return Ok(delegations);
        }

        [HttpGet("{guid:guid}", Name = "ApprovalDelegationById")]
        public async Task<IActionResult> GetDelegation(Guid guid)
        {
            var delegation = await _service.ApprovalDelegationService.GetAsync(guid, trackChanges: false);
            if (delegation == null) return NotFound();
            
            return Ok(delegation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDelegation([FromBody] ApprovalDelegationForCreationDto input)
        {
            if (input == null) return BadRequest("DelegationForCreationDto object is null");
            
            var createdDelegation = await _service.ApprovalDelegationService.CreateAsync(input);

            return CreatedAtRoute("ApprovalDelegationById", new { guid = createdDelegation.ApprovalDelegationGuid }, createdDelegation);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> UpdateDelegation(Guid guid, [FromBody] ApprovalDelegationForUpdateDto input)
        {
            if (input == null) return BadRequest("DelegationForUpdateDto object is null");

            await _service.ApprovalDelegationService.UpdateAsync(guid, input, trackChanges: true);

            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> DeleteDelegation(Guid guid)
        {
            await _service.ApprovalDelegationService.DeleteAsync(guid, trackChanges: false);
            return NoContent();
        }
    }
}
