using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/standardReferences/{headerGuid:guid}/scopes/{scopeGuid:guid}/items")]
    [ApiController]
    public class StandardReferenceScopeItemsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public StandardReferenceScopeItemsController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Scope", Description = "Mengambil semua detail scope item berdasarkan Guid scope.")]
        public async Task<IActionResult> GetAll(Guid headerGuid, Guid scopeGuid)
        {
            var data = await _service.StandardReferenceScopeItemService.GetAllByStandardReferenceScopeGuidAsync(scopeGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu detail scope item berdasarkan parameter scope Guid & internal Guid.")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid scopeGuid, Guid guid)
        {
            var data = await _service.StandardReferenceScopeItemService.GetStandardReferenceScopeItemByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data scope item baru untuk scope ini.")]
        public async Task<IActionResult> Create(Guid headerGuid, Guid scopeGuid, [FromBody] StandardReferenceScopeItemForCreationDto input)
        {
            input.StandardReferenceScopeGuid = scopeGuid;
            var created = await _service.StandardReferenceScopeItemService.CreateStandardReferenceScopeItemAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, scopeGuid = scopeGuid, guid = created.StandardReferenceScopeItemGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data scope item berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid scopeGuid, Guid guid, [FromBody] StandardReferenceScopeItemForUpdateDto input)
        {
            input.StandardReferenceScopeGuid = scopeGuid;
            await _service.StandardReferenceScopeItemService.UpdateStandardReferenceScopeItemAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data scope item sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid scopeGuid, Guid guid, [FromBody] StandardReferenceScopeItemForDeleteDto input)
        {
            await _service.StandardReferenceScopeItemService.DeleteStandardReferenceScopeItemAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data scope item secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid scopeGuid, Guid guid)
        {
            await _service.StandardReferenceScopeItemService.DeleteStandardReferenceScopeItemByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Scope & Item Guid (Admin)", Description = "Mengambil satu detail scope item berdasarkan Guid scope dan Guid item.")]
        public async Task<IActionResult> AdminGetByScopeAndItem(Guid headerGuid, Guid scopeGuid, Guid guid)
        {
            var data = await _service.StandardReferenceScopeItemService.GetByScopeGuidAndItemGuidAsync(scopeGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search(Guid headerGuid, Guid scopeGuid, [FromQuery] StandardReferenceScopeItemSearchDto input)
        {
            var result = await _service.StandardReferenceScopeItemService.SearchStandardReferenceScopeItemAsync(
                input.StandardReferenceScopeItemInitial, input.StandardReferenceScopeItemInitialSearchType.ToString(), scopeGuid, Guid.Empty
            );
            return Ok(result);
        }
    }
}
