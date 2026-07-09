using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Shell.Controllers
{
    [Authorize]
    [Route("api/companies/{companyGuid:guid}/offices")]
    [ApiController]
    public partial class CompanyOfficesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public CompanyOfficesController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get all by Company Guid</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header", Description = "Mengambil semua detail berdasarkan Guid header (companyGuid).")]
        public async Task<IActionResult> GetAll(Guid companyGuid)
        {
            var data = await _service.CompanyOfficeService.GetAllByCompanyGuidAsync(companyGuid);
            return Ok(data);
        }

        /// <summary>Get CompanyOffice by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu detail berdasarkan parameter header Guid & internal Guid.")]
        public async Task<IActionResult> GetByGuid(Guid companyGuid, Guid guid)
        {
            var data = await _service.CompanyOfficeService.GetByCompanyGuidAndCompanyOfficeGuidAsync(companyGuid, guid);
            return Ok(data);
        }

        /// <summary>Create CompanyOffice</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru detail untuk header ini.")]
        public async Task<IActionResult> Create(Guid companyGuid, [FromBody] CompanyOfficeForCreationDto input)
        {
            var claimCompanyId = User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            var claimCompanyGuid = User.Claims.FirstOrDefault(c => c.Type == "CompanyGuid")?.Value;

            if (!string.IsNullOrEmpty(claimCompanyId) && long.TryParse(claimCompanyId, out var compId))
            {
                if (input.CompanyId == 0) input.CompanyId = compId;
            }
            if (!string.IsNullOrEmpty(claimCompanyGuid) && Guid.TryParse(claimCompanyGuid, out var compGuid))
            {
                if (input.CompanyGuid == Guid.Empty) input.CompanyGuid = compGuid;
            }
            else
            {
                if (input.CompanyGuid == Guid.Empty) input.CompanyGuid = companyGuid;
            }

            var created = await _service.CompanyOfficeService.CreateCompanyOfficeAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { companyGuid = companyGuid, guid = created.CompanyOfficeGuid }, created);

        }

        /// <summary>Update CompanyOffice</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data detail berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid companyGuid, Guid guid, [FromBody] CompanyOfficeForUpdateDto input)
        {
            if (input.CompanyGuid == Guid.Empty)
            {
                input.CompanyGuid = companyGuid;
            }

            var claimCompanyId = User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            if (!string.IsNullOrEmpty(claimCompanyId) && long.TryParse(claimCompanyId, out var compId))
            {
                if (input.CompanyId == 0) input.CompanyId = compId;
            }

            await _service.CompanyOfficeService.UpdateCompanyOfficeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete CompanyOffice</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid companyGuid, Guid guid, [FromBody] CompanyOfficeForDeleteDto input)
        {
            await _service.CompanyOfficeService.DeleteCompanyOfficeAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete CompanyOffice (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid companyGuid, Guid guid)
        {
            await _service.CompanyOfficeService.DeleteCompanyOfficeByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Get detail by Header Guid & Item Guid (Admin)</summary>
        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid (Admin)", Description = "Mengambil satu detail berdasarkan Guid header dan Guid item.")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid companyGuid, Guid guid)
        {
            var data = await _service.CompanyOfficeService.GetByCompanyGuidAndCompanyOfficeGuidAsync(companyGuid, guid);
            return Ok(data);
        }

        /// <summary>Search CompanyOffice</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] CompanyOfficeSearchDto input, [FromQuery] Guid companyGuid, [FromQuery] Guid companyOfficeGuid)
        {
            var result = await _service.CompanyOfficeService.SearchCompanyOfficeAsync(
                input.CompanyOfficeName, input.CompanyOfficeNameSearchType.ToString(), 
                input.SrAddressType, 
                input.CountryId, input.StateId, input.CityId, 
                input.PostalCodeId, input.PostalCodeIdSearchType.ToString(), 
                input.Address, input.AddressSearchType.ToString(), 
                companyGuid, companyOfficeGuid);
            return Ok(result);
        }

    }
}


