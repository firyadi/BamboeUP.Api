using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects.Approval;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/approval-templates")]
    [ApiController]
    public class ApprovalTemplatesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public ApprovalTemplatesController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTemplates()
        {
            var templates = await _service.ApprovalTemplateService.GetAllAsync(trackChanges: false);
            return Ok(templates);
        }

        [HttpGet("{guid:guid}", Name = "ApprovalTemplateById")]
        public async Task<IActionResult> GetTemplate(Guid guid)
        {
            var template = await _service.ApprovalTemplateService.GetAsync(guid, trackChanges: false);
            if (template == null) return NotFound();
            
            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] ApprovalTemplateForCreationDto input)
        {
            if (input == null) return BadRequest("TemplateForCreationDto object is null");
            
            var createdTemplate = await _service.ApprovalTemplateService.CreateAsync(input);

            return CreatedAtRoute("ApprovalTemplateById", new { guid = createdTemplate.ApprovalTemplateGuid }, createdTemplate);
        }

        [HttpPut("{guid:guid}")]
        public async Task<IActionResult> UpdateTemplate(Guid guid, [FromBody] ApprovalTemplateForUpdateDto input)
        {
            if (input == null) return BadRequest("TemplateForUpdateDto object is null");

            await _service.ApprovalTemplateService.UpdateAsync(guid, input, trackChanges: true);

            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        public async Task<IActionResult> DeleteTemplate(Guid guid, [FromQuery] long deletedBy)
        {
            await _service.ApprovalTemplateService.DeleteAsync(guid, deletedBy, trackChanges: false);
            return NoContent();
        }
    }
}
