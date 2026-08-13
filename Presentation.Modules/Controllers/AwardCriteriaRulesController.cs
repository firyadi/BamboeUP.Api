using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/awardCriteria/{detailGuid:guid}/awardCriteriaRules")]
    [ApiController]
    public partial class AwardCriteriaRulesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public AwardCriteriaRulesController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Detail")]
        public async Task<IActionResult> GetAll(Guid detailGuid)
        {
            var data = await _service.AwardCriteriaRuleService.GetAllByAwardCriteriaGuidAsync(detailGuid);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid detailGuid, Guid guid)
        {
            var data = await _service.AwardCriteriaRuleService.GetAwardCriteriaRuleByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create(Guid detailGuid, [FromBody] AwardCriteriaRuleForCreationDto input)
        {
            var created = await _service.AwardCriteriaRuleService.CreateAwardCriteriaRuleAsync(detailGuid, input);
            return CreatedAtAction(nameof(GetByGuid), new { detailGuid = detailGuid, guid = created.AwardCriteriaRuleGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid detailGuid, Guid guid, [FromBody] AwardCriteriaRuleForUpdateDto input)
        {
            await _service.AwardCriteriaRuleService.UpdateAwardCriteriaRuleAsync(detailGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid detailGuid, Guid guid, [FromBody] AwardCriteriaRuleForDeleteDto input)
        {
            await _service.AwardCriteriaRuleService.DeleteAwardCriteriaRuleAsync(detailGuid, guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid detailGuid, Guid guid)
        {
            await _service.AwardCriteriaRuleService.DeleteAwardCriteriaRuleByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("admin/item/{guid:guid}")]
        [SwaggerOperation(Summary = "Get SubDetail By Detail & Item Guid")]
        public async Task<IActionResult> AdminGetByDetailAndItem(Guid detailGuid, Guid guid)
        {
            var data = await _service.AwardCriteriaRuleService.GetByAwardCriteriaGuidAndAwardCriteriaRuleGuidAsync(detailGuid, guid);
            return Ok(data);
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] AwardCriteriaRuleSearchDto input, [FromQuery] Guid detailGuid, [FromQuery] Guid subDetailGuid)
        {
            var result = await _service.AwardCriteriaRuleService.SearchAwardCriteriaRuleAsync(
                                input.RuleCode, input.RuleCodeSearchType.ToString(),
                input.RuleName, input.RuleNameSearchType.ToString(),
                input.Operator, input.OperatorSearchType.ToString(),
                input.ValueFrom, input.ValueFromSearchType.ToString(),
                input.Valueto, input.ValuetoSearchType.ToString(),
                input.Score, input.ScoreSearchType.ToString(),
                input.Note, input.NoteSearchType.ToString(),
                detailGuid, subDetailGuid);
            return Ok(result);
        }
    }
}
