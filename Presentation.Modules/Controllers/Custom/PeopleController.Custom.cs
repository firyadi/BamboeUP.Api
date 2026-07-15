using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    public partial class PeopleController
    {
        [HttpPost("onboard")]
        public async Task<IActionResult> Onboard([FromBody] PersonQuickOnboardDto input)
        {
            if (input == null) return BadRequest("Input cannot be null");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await service.PersonService.OnboardPersonAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.PersonGuid }, created);
        }
    }
}
