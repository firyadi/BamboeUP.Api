using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Shell.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public partial class CompaniesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public CompaniesController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all Companies</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Mengambil semua data Companies yang aktif.")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get Company by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu data berdasarkan Guid.")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.CompanyService.GetCompanyByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create Company</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru.")]
        public async Task<IActionResult> Create([FromBody] CompanyForCreationDto input)
        {
            var created = await _service.CompanyService.CreateCompanyAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.CompanyGuid }, created);
        }

        /// <summary>Update Company</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] CompanyForUpdateDto input)
        {
            await _service.CompanyService.UpdateCompanyAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete Company</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] CompanyForDeleteDto input)
        {
            await _service.CompanyService.DeleteCompanyAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete Company (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.CompanyService.DeleteCompanyByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search Companies</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] CompanySearchDto input)
        {
            var result = await _service.CompanyService.SearchCompanyAsync(
                input.CompanyName, input.CompanyNameSearchType.ToString(), input.InitialName, input.InitialNameSearchType.ToString(), input.TaxCompulsionNo, input.TaxCompulsionNoSearchType.ToString(), input.RegistrationNo, input.RegistrationNoSearchType.ToString(), input.DefaultCurrency, input.DefaultCurrencySearchType.ToString()
        );
            return Ok(result);
        }
    }
}
