using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/certifications")]
    [ApiController]
    public partial class CertificationsController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.CertificationService.GetAllCertificationsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.CertificationService.GetCertificationByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] CertificationForCreationDto input)
        {
            var created = await service.CertificationService.CreateCertificationAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.CertificationGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] CertificationForUpdateDto input)
        {
            await service.CertificationService.UpdateCertificationAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] CertificationForDeleteDto input)
        {
            await service.CertificationService.DeleteCertificationAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.CertificationService.DeleteCertificationByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] CertificationSearchDto input)
        {
            var result = await service.CertificationService.SearchCertificationAsync(
                input.CertificationCode, input.CertificationCodeSearchType.ToString(),
                input.CertificationName, input.CertificationNameSearchType.ToString(),
                input.CertificationShortName, input.CertificationShortNameSearchType.ToString(),
                input.DefaultValidityMonth, input.DefaultValidityMonthSearchType.ToString(),
                input.IsLifetime, input.IsLifetimeSearchType.ToString(),
                input.IsRenewable, input.IsRenewableSearchType.ToString(),
                input.Description, input.DescriptionSearchType.ToString(),
                input.IsActive, input.IsActiveSearchType.ToString()
, input.CertificationCategoryName, input.CertificationCategoryNameSearchType.ToString()
, input.CertificationProviderName, input.CertificationProviderNameSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
