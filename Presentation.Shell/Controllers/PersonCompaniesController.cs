using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Shell.Controllers
{
    [Authorize]
    [Route("api/people/{personGuid:guid}/companies")]
    [ApiController]
    public partial class PersonCompaniesController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public PersonCompaniesController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Get Companies led by a Person</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Person", Description = "Mengambil Company yang dipimpin oleh Person tertentu.")]
        public async Task<IActionResult> GetAllByPerson(Guid personGuid)
        {
            var data = await _service.CompanyService.GetAllByPersonGuidAsync(personGuid);
            return Ok(data);
        }
    }
}
