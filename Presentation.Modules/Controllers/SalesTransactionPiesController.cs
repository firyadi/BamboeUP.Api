using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/salesTransactionPies")]
    [ApiController]
    public partial class SalesTransactionPiesController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Read SalesTransactionPie")]
        public async Task<IActionResult> Query([FromQuery] SalesTransactionPieQueryDto query)
        {
            var data = await service.SalesTransactionPieService.QueryAsync(query);
            return Ok(data);
        }
    }
}