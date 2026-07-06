using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Shell.Controllers
{
    [Route("api/autoNumbers")]
    [ApiController]
    public class AutoNumbersController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public AutoNumbersController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all AutoNumbers</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Mengambil semua data AutoNumbers yang aktif.")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.AutoNumberService.GetAllAutoNumbersAsync(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get AutoNumber by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu data berdasarkan Guid.")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.AutoNumberService.GetAutoNumberByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create AutoNumber</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru.")]
        public async Task<IActionResult> Create([FromBody] AutoNumberForCreationDto input)
        {
            var created = await _service.AutoNumberService.CreateAutoNumberAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.AutoNumberGuid }, created);
        }

        /// <summary>Update AutoNumber</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] AutoNumberForUpdateDto input)
        {
            await _service.AutoNumberService.UpdateAutoNumberAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete AutoNumber</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] AutoNumberForDeleteDto input)
        {
            await _service.AutoNumberService.DeleteAutoNumberAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete AutoNumber (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.AutoNumberService.DeleteAutoNumberByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search AutoNumbers</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] AutoNumberSearchDto input)
        {
            var result = await _service.AutoNumberService.SearchAutoNumberAsync(
                input.Prefik, input.PrefikSearchType.ToString(), input.SeparatorAfterPrefik, input.SeparatorAfterPrefikSearchType.ToString(), input.SeparatorAfterDept, input.SeparatorAfterDeptSearchType.ToString(), input.SeparatorAfterYear, input.SeparatorAfterYearSearchType.ToString(), input.SeparatorAfterMonth, input.SeparatorAfterMonthSearchType.ToString(), input.SeparatorAfterDay, input.SeparatorAfterDaySearchType.ToString(), input.NumberGroupSeparator, input.NumberGroupSeparatorSearchType.ToString(), input.NumberFormat, input.NumberFormatSearchType.ToString()
            );

            return Ok(result);
        }
    }
}
