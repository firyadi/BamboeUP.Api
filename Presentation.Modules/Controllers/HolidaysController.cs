using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Modules.Controllers
{
    [Route("api/holidays")]
    [ApiController]
    public class HolidaysController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public HolidaysController(IServiceModulesManager service)
        {
            _service = service;
        }

        /// <summary>Get all Holidays</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Mengambil semua data Holidays yang aktif.")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.HolidayService.GetAllHolidaysAsync(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get Holiday by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu data berdasarkan Guid.")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.HolidayService.GetHolidayByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create Holiday</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru.")]
        public async Task<IActionResult> Create([FromBody] HolidayForCreationDto input)
        {
            var created = await _service.HolidayService.CreateHolidayAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.HolidayGuid }, created);
        }

        /// <summary>Update Holiday</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] HolidayForUpdateDto input)
        {
            await _service.HolidayService.UpdateHolidayAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete Holiday</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] HolidayForDeleteDto input)
        {
            await _service.HolidayService.DeleteHolidayAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete Holiday (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.HolidayService.DeleteHolidayByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search Holidays</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] HolidaySearchDto input)
        {
            var result = await _service.HolidayService.SearchHolidayAsync(
                input.YearPeriode, input.YearPeriodeSearchType.ToString(), input.Note, input.NoteSearchType.ToString(), input.HolidayDatesFrom, input.HolidayDatesTo
            );

            return Ok(result);
        }
    }
}
