using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/standardReferences")]
    [ApiController]
    public class StandardReferencesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public StandardReferencesController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all StandardReferences</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Mengambil semua data StandardReferences yang aktif.")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.StandardReferenceService.GetAllStandardReferencesAsync(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get StandardReferences for Parent Selection</summary>
        [HttpGet("for-parent-selection")]
        [SwaggerOperation(Summary = "Get For Parent Selection", Description = "Mengambil data untuk pilihan parent dropdown.")]
        public async Task<IActionResult> GetForParentSelection([FromQuery] Guid? currentRecordGuid)
        {
            var data = await _service.StandardReferenceService.GetStandardReferencesForParentSelectionAsync(currentRecordGuid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get StandardReference by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu data berdasarkan Guid.")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.StandardReferenceService.GetStandardReferenceByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create StandardReference</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru.")]
        public async Task<IActionResult> Create([FromBody] StandardReferenceForCreationDto input)
        {
            var created = await _service.StandardReferenceService.CreateStandardReferenceAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.StandardReferenceGuid }, created);
        }

        /// <summary>Update StandardReference</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] StandardReferenceForUpdateDto input)
        {
            await _service.StandardReferenceService.UpdateStandardReferenceAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete StandardReference</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] StandardReferenceForDeleteDto input)
        {
            await _service.StandardReferenceService.DeleteStandardReferenceAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete StandardReference (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.StandardReferenceService.DeleteStandardReferenceByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search StandardReferences</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] StandardReferenceSearchDto input, [FromQuery] Guid companyGuid, [FromQuery] Guid companyOfficeGuid)
        {
            var result = await _service.StandardReferenceService.SearchStandardReferenceAsync(
                input.StandardReferenceInitial, input.StandardReferenceInitialSearchType.ToString(), input.StandardReferenceName, input.StandardReferenceNameSearchType.ToString(), input.Description, input.DescriptionSearchType.ToString(), companyGuid, companyOfficeGuid
            );
            return Ok(result);
        }
    }
}
