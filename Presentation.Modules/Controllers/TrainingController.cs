using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/training")]
    [ApiController]
    public partial class TrainingController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.TrainingService.GetAllTrainingAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.TrainingService.GetTrainingByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] TrainingForCreationDto input)
        {
            var created = await service.TrainingService.CreateTrainingAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.TrainingGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] TrainingForUpdateDto input)
        {
            await service.TrainingService.UpdateTrainingAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] TrainingForDeleteDto input)
        {
            await service.TrainingService.DeleteTrainingAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.TrainingService.DeleteTrainingByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] TrainingSearchDto input)
        {
            var result = await service.TrainingService.SearchTrainingAsync(
                input.TrainingCode, input.TrainingCodeSearchType.ToString(),
                input.TrainingName, input.TrainingNameSearchType.ToString(),
                input.TrainingShortName, input.TrainingShortNameSearchType.ToString(),
                input.SrTrainingCategory, input.SrTrainingCategorySearchType.ToString(),
                input.SrTrainingType, input.SrTrainingTypeSearchType.ToString(),
                input.SrTrainingLevel, input.SrTrainingLevelSearchType.ToString(),
                input.DurationHour, input.DurationHourSearchType.ToString(),
                input.ValidityMonth, input.ValidityMonthSearchType.ToString(),
                input.SrTrainingProvider, input.SrTrainingProviderSearchType.ToString(),
                input.IsMandatory, input.IsMandatorySearchType.ToString(),
                input.IsCertification, input.IsCertificationSearchType.ToString(),
                input.IsExternal, input.IsExternalSearchType.ToString(),
                input.Description, input.DescriptionSearchType.ToString(),
                input.IsActive, input.IsActiveSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
