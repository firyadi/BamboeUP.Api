using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace BamboeUp.Api.Controllers
{
    [Route("api/autoNumberCounters")]
    [ApiController]
    public class AutoNumberCountersController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public AutoNumberCountersController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all AutoNumberCounters</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Mengambil semua data AutoNumberCounters yang aktif.")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.AutoNumberCounterService.GetAllAutoNumberCountersAsync(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get AutoNumberCounter by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu data berdasarkan Guid.")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.AutoNumberCounterService.GetAutoNumberCounterByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create AutoNumberCounter</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru.")]
        public async Task<IActionResult> Create([FromBody] AutoNumberCounterForCreationDto input)
        {
            var created = await _service.AutoNumberCounterService.CreateAutoNumberCounterAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.AutoNumberCounterGuid }, created);
        }

        /// <summary>Update AutoNumberCounter</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] AutoNumberCounterForUpdateDto input)
        {
            await _service.AutoNumberCounterService.UpdateAutoNumberCounterAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete AutoNumberCounter</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] AutoNumberCounterForDeleteDto input)
        {
            await _service.AutoNumberCounterService.DeleteAutoNumberCounterAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete AutoNumberCounter (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.AutoNumberCounterService.DeleteAutoNumberCounterByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search AutoNumberCounters</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] AutoNumberCounterSearchDto input)
        {
            var result = await _service.AutoNumberCounterService.SearchAutoNumberCounterAsync(
                input.AutoNumberTemplateId, input.CompanyId, input.CompanyOfficeId,
                input.OrganizationUnitId, input.YearNo, input.MonthNo, input.DayNo
            );
            return Ok(result);
        }
    }
}
