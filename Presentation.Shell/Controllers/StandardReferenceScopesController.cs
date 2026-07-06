using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/standardReferences/{headerGuid:guid}/scopes")]
    [ApiController]
    public class StandardReferenceScopesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public StandardReferenceScopesController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header", Description = "Mengambil semua detail scope berdasarkan Guid header.")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.StandardReferenceScopeService.GetAllByStandardReferenceGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu detail scope berdasarkan parameter header Guid & internal Guid.")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.StandardReferenceScopeService.GetStandardReferenceScopeByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data scope baru untuk header ini.")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] StandardReferenceScopeForCreationDto input)
        {
            input.StandardReferenceGuid = headerGuid;
            var created = await _service.StandardReferenceScopeService.CreateStandardReferenceScopeAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.StandardReferenceScopeGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data scope berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] StandardReferenceScopeForUpdateDto input)
        {
            input.StandardReferenceGuid = headerGuid;
            await _service.StandardReferenceScopeService.UpdateStandardReferenceScopeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data scope sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] StandardReferenceScopeForDeleteDto input)
        {
            await _service.StandardReferenceScopeService.DeleteStandardReferenceScopeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data scope secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.StandardReferenceScopeService.DeleteStandardReferenceScopeByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Scope Guid (Admin)", Description = "Mengambil satu detail scope berdasarkan Guid header dan Guid scope.")]
        public async Task<IActionResult> AdminGetByHeaderAndScope(Guid headerGuid, Guid guid)
        {
            var data = await _service.StandardReferenceScopeService.GetByStandardReferenceGuidAndScopeGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] Guid companyGuid, [FromQuery] Guid companyOfficeGuid)
        {
            var result = await _service.StandardReferenceScopeService.SearchStandardReferenceScopeAsync(
                companyGuid, companyOfficeGuid, Guid.Empty, Guid.Empty
            );
            return Ok(result);
        }
    }
}
