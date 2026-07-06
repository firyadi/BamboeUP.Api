using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.RequestFeatures;

namespace Presentation.Modules.Controllers
{
    [Route("api/administrativeAreaDisplays")]
    [ApiController]
    public class AdministrativeAreaDisplaysController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public AdministrativeAreaDisplaysController(IServiceModulesManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AdministrativeAreaParameters parameters)
        {
            var data = await _service.AdministrativeAreaDisplayService.GetAllAsync(parameters, trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            var data = await _service.AdministrativeAreaDisplayService.GetOneAsync(id, trackChanges: false);
            return Ok(data);
        }
    }
}
