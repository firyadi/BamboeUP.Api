using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Presentation.Shell.Controllers
{
    [Route("api/organizationUnits")]
    [ApiController]
    public partial class OrganizationUnitsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public OrganizationUnitsController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.OrganizationUnitService.GetAllOrganizationUnitsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.OrganizationUnitService.GetOrganizationUnitByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] OrganizationUnitForCreationDto input)
        {
            var created = await _service.OrganizationUnitService.CreateOrganizationUnitAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.OrganizationUnitGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] OrganizationUnitForUpdateDto input)
        {
            await _service.OrganizationUnitService.UpdateOrganizationUnitAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] OrganizationUnitForDeleteDto input)
        {
            await _service.OrganizationUnitService.DeleteOrganizationUnitAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.OrganizationUnitService.DeleteOrganizationUnitByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] OrganizationUnitSearchDto input)
        {
            var result = await _service.OrganizationUnitService.SearchOrganizationUnitAsync(
                input.OrganizationUnitCode, input.OrganizationUnitCodeSearchType.ToString(), input.OrganizationUnitName, input.OrganizationUnitNameSearchType.ToString()
, input.ParentOrganizationUnitName, input.ParentOrganizationUnitNameSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
