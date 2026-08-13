using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/awards/{headerGuid:guid}/awardCriteria")]
    [ApiController]
    public partial class AwardCriteriaController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public AwardCriteriaController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.AwardCriteriaService.GetAllByAwardGuidAsync(headerGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.AwardCriteriaService.GetAwardCriteriaByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] AwardCriteriaForCreationDto input)
        {
            var created = await _service.AwardCriteriaService.CreateAwardCriteriaAsync(headerGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.AwardCriteriaGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] AwardCriteriaForUpdateDto input)
        {
            await _service.AwardCriteriaService.UpdateAwardCriteriaAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] AwardCriteriaForDeleteDto input)
        {
            await _service.AwardCriteriaService.DeleteAwardCriteriaAsync(headerGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.AwardCriteriaService.DeleteAwardCriteriaByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.AwardCriteriaService.GetByAwardGuidAndAwardCriteriaGuidAsync(headerGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] AwardCriteriaSearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.AwardCriteriaService.SearchAwardCriteriaAsync(
                                input.LineNo, input.LineNoSearchType.ToString(),
                input.CriteriaCode, input.CriteriaCodeSearchType.ToString(),
                input.CriteriaName, input.CriteriaNameSearchType.ToString(),
                input.MinimumScore, input.MinimumScoreSearchType.ToString(),
                input.MaximumScore, input.MaximumScoreSearchType.ToString(),
                input.Weight, input.WeightSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString(),
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
