using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;

namespace Presentation.Shell.Controllers
{
    [Route("api/vwStandardReferenceItems")]
    [ApiController]
    public class VwStandardReferenceItemsController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public VwStandardReferenceItemsController(IServiceShellManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? StandardReferenceInitial)
        {
            var data = await _service.VwStandardReferenceItemService.GetAllAsync(StandardReferenceInitial, trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetOne(long id)
        {
            var data = await _service.VwStandardReferenceItemService.GetOneAsync(id, trackChanges: false);
            return Ok(data);
        }
    }
}
