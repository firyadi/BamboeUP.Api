using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/workCapabilities")]
    [ApiController]
    public class WorkCapabilitiesController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public WorkCapabilitiesController(IServiceModulesManager service) => _service = service;

        [HttpGet]
        [SwaggerOperation(Summary = "List catalog items for WorkTeam assignment")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.WorkTeamCapabilityService.GetWorkCapabilitiesAsync();
            return Ok(data);
        }
    }
}
