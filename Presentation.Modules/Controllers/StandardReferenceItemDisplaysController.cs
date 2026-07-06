using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Modules.Controllers
{
    /// <summary>
    /// Exposes Standard Reference items resolved by scope/template priority
    /// through app.fn_GetStandardReferenceItems.
    /// </summary>
    [Route("api/standard-reference-items")]
    [ApiController]
    public partial class StandardReferenceItemDisplaysController : ControllerBase
    {
        private readonly IServiceModulesManager _service;

        public StandardReferenceItemDisplaysController(IServiceModulesManager service)
            => _service = service;

        /// <summary>
        /// Returns the resolved list of Standard Reference items for a given company scope and reference initial.
        /// Scope resolution priority: Company+Office > Company-only > Global Template.
        /// </summary>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="companyOfficeId">Company office identifier.</param>
        /// <param name="standardReferenceInitial">Standard reference initial key (e.g. "EMP_STATUS").</param>
        /// <param name="departmentId">Optional department identifier (reserved for future use).</param>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get Standard Reference Items",
            Description = "Resolves items from the most specific scope (Company+Office > Company-only) or falls back to the global template.")]
        public async Task<IActionResult> GetItems(
            [FromQuery] long companyId,
            [FromQuery] long companyOfficeId,
            [FromQuery] string standardReferenceInitial,
            [FromQuery] long? departmentId = null)
        {
            var data = await _service.StandardReferenceItemDisplayService.GetItemsAsync(
                companyId,
                companyOfficeId,
                departmentId,
                standardReferenceInitial);

            return Ok(data);
        }
    }
}
