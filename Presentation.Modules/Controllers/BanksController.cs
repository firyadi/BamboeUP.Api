using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace BamboeUp.Api.Controllers
{
    [Route("api/banks")]
    [ApiController]
    public partial class BanksController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public BanksController(IServiceModulesManager service)
        {
            _service = service;
        }

        /// <summary>Get all Banks</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Mengambil semua data Banks yang aktif.")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.BankService.GetAllBanksAsync(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get Bank by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu data berdasarkan Guid.")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.BankService.GetBankByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create Bank</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru.")]
        public async Task<IActionResult> Create([FromBody] BankForCreationDto input)
        {
            var created = await _service.BankService.CreateBankAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.BankGuid }, created);
        }

        /// <summary>Update Bank</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] BankForUpdateDto input)
        {
            await _service.BankService.UpdateBankAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete Bank</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] BankForDeleteDto input)
        {
            await _service.BankService.DeleteBankAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete Bank (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.BankService.DeleteBankByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search Banks</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] BankSearchDto input)
        {
            var result = await _service.BankService.SearchBankAsync(
                input.BankName, input.BankNameSearchType.ToString(), input.BankInitial, input.BankInitialSearchType.ToString()
        );
            return Ok(result);
        }
    }
}
