using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/standardReferences/{standardReferenceGuid:guid}/items")]
    [ApiController]
    public class StandardReferenceItemsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public StandardReferenceItemsController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all by StandardReference Guid</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header", Description = "Mengambil semua detail berdasarkan Guid header (standardReferenceGuid).")]
        public async Task<IActionResult> GetAll(Guid standardReferenceGuid)
        {
            var data = await _service.StandardReferenceItemService.GetAllByStandardReferenceGuidAsync(standardReferenceGuid);
            return Ok(data);
        }

        /// <summary>Get StandardReferenceItem by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu detail berdasarkan parameter header Guid & internal Guid.")]
        public async Task<IActionResult> GetByGuid(Guid standardReferenceGuid, Guid guid)
        {
            var data = await _service.StandardReferenceItemService.GetStandardReferenceItemByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create StandardReferenceItem</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru detail untuk header ini.")]
        public async Task<IActionResult> Create(Guid standardReferenceGuid, [FromBody] StandardReferenceItemForCreationDto input)
        {
            input.StandardReferenceGuid = standardReferenceGuid;
            var created = await _service.StandardReferenceItemService.CreateStandardReferenceItemAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { standardReferenceGuid = standardReferenceGuid, guid = created.StandardReferenceItemGuid }, created);
        }

        /// <summary>Update StandardReferenceItem</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data detail berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid standardReferenceGuid, Guid guid, [FromBody] StandardReferenceItemForUpdateDto input)
        {
            input.StandardReferenceGuid = standardReferenceGuid;
            await _service.StandardReferenceItemService.UpdateStandardReferenceItemAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete StandardReferenceItem</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid standardReferenceGuid, Guid guid, [FromBody] StandardReferenceItemForDeleteDto input)
        {
            await _service.StandardReferenceItemService.DeleteStandardReferenceItemAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete StandardReferenceItem (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid standardReferenceGuid, Guid guid)
        {
            await _service.StandardReferenceItemService.DeleteStandardReferenceItemByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Get detail by Header Guid & Item Guid (Admin)</summary>
        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid (Admin)", Description = "Mengambil satu detail berdasarkan Guid header dan Guid item.")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid standardReferenceGuid, Guid guid)
        {
            var data = await _service.StandardReferenceItemService.GetByStandardReferenceGuidAndStandardReferenceItemGuidAsync(standardReferenceGuid, guid);
            return Ok(data);
        }

        /// <summary>Search StandardReferenceItems</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] StandardReferenceItemSearchDto input, [FromQuery] Guid companyGuid, [FromQuery] Guid companyOfficeGuid)
        {
            var result = await _service.StandardReferenceItemService.SearchStandardReferenceItemAsync(
                string.Empty, string.Empty, input.StandardReferenceItemInitial, input.StandardReferenceItemInitialSearchType.ToString(), input.StandardReferenceItemName, input.StandardReferenceItemNameSearchType.ToString(), string.Empty, string.Empty, companyGuid, companyOfficeGuid
            );

            return Ok(result);
        }
    }
}
