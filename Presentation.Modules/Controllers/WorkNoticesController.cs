using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/workNotices")]
    [ApiController]
    public partial class WorkNoticesController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.WorkNoticeService.GetAllWorkNoticesAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.WorkNoticeService.GetWorkNoticeByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] WorkNoticeForCreationDto input)
        {
            var created = await service.WorkNoticeService.CreateWorkNoticeAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.WorkNoticeGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] WorkNoticeForUpdateDto input)
        {
            await service.WorkNoticeService.UpdateWorkNoticeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] WorkNoticeForDeleteDto input)
        {
            await service.WorkNoticeService.DeleteWorkNoticeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.WorkNoticeService.DeleteWorkNoticeByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] WorkNoticeSearchDto input)
        {
            var result = await service.WorkNoticeService.SearchWorkNoticeAsync(
                input.WorkNoticeCode, input.WorkNoticeCodeSearchType.ToString(),
                input.WorkNoticeName, input.WorkNoticeNameSearchType.ToString(),
                input.WorkNoticeDate, input.WorkNoticeDateSearchType.ToString(),
                input.IsEditAble, input.IsEditAbleSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
