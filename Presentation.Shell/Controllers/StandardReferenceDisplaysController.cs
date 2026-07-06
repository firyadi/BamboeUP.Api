using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;

namespace Presentation.Shell.Controllers
{
    [Route("api/standardReferenceDisplays")]
    [ApiController]
    public class StandardReferenceDisplaysController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public StandardReferenceDisplaysController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Shared.RequestFeatures.StandardReferenceDisplayParameters parameters)
        {
            var data = await _service.StandardReferenceDisplayService.GetAllDisplaysAsync(parameters, trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetOne(long id)
        {
            var data = await _service.StandardReferenceDisplayService.GetOneAsync(id, trackChanges: false);
            return Ok(data);
        }
    }
}
