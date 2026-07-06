using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace BamboeUp.Api.Controllers
{
    [Route("api/autoNumberComponents")]
    [ApiController]
    public class AutoNumberComponentsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public AutoNumberComponentsController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all AutoNumberComponents</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Mengambil semua data AutoNumberComponents yang aktif.")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.AutoNumberComponentService.GetAllAutoNumberComponentsAsync(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get AutoNumberComponent by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu data berdasarkan Guid.")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.AutoNumberComponentService.GetAutoNumberComponentByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create AutoNumberComponent</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru.")]
        public async Task<IActionResult> Create([FromBody] AutoNumberComponentForCreationDto input)
        {
            var created = await _service.AutoNumberComponentService.CreateAutoNumberComponentAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.AutoNumberComponentGuid }, created);
        }

        /// <summary>Update AutoNumberComponent</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] AutoNumberComponentForUpdateDto input)
        {
            await _service.AutoNumberComponentService.UpdateAutoNumberComponentAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete AutoNumberComponent</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] AutoNumberComponentForDeleteDto input)
        {
            await _service.AutoNumberComponentService.DeleteAutoNumberComponentAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete AutoNumberComponent (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.AutoNumberComponentService.DeleteAutoNumberComponentByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search AutoNumberComponents</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] AutoNumberComponentSearchDto input)
        {
            var result = await _service.AutoNumberComponentService.SearchAutoNumberComponentAsync(
                input.ComponentType, input.ComponentTypeSearchType.ToString(), 
                input.StaticValue, input.StaticValueSearchType.ToString(), 
                input.Format, input.FormatSearchType.ToString(), 
                input.AutoNumberTemplateId
            );
            return Ok(result);
        }
    }
}
