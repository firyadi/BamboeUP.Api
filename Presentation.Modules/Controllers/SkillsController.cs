using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/skills")]
    [ApiController]
    public partial class SkillsController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.SkillService.GetAllSkillsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.SkillService.GetSkillByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] SkillForCreationDto input)
        {
            var created = await service.SkillService.CreateSkillAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.SkillGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] SkillForUpdateDto input)
        {
            await service.SkillService.UpdateSkillAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] SkillForDeleteDto input)
        {
            await service.SkillService.DeleteSkillAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.SkillService.DeleteSkillByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] SkillSearchDto input)
        {
            var result = await service.SkillService.SearchSkillAsync(
                input.SkillCode, input.SkillCodeSearchType.ToString(),
                input.SkillName, input.SkillNameSearchType.ToString(),
                input.SkillShortName, input.SkillShortNameSearchType.ToString(),
                input.SrSkillCategory, input.SrSkillCategorySearchType.ToString(),
                input.SrSkillType, input.SrSkillTypeSearchType.ToString(),
                input.SrDefaultSkillLevel, input.SrDefaultSkillLevelSearchType.ToString(),
                input.IsCertificationRequired, input.IsCertificationRequiredSearchType.ToString(),
                input.IsMandatory, input.IsMandatorySearchType.ToString(),
                input.Description, input.DescriptionSearchType.ToString(),
                input.IsActive, input.IsActiveSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
