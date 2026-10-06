using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/salesAnalyses")]
    [ApiController]
    public partial class SalesAnalysesController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.SalesAnalysisService.GetAllSalesAnalysesAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.SalesAnalysisService.GetSalesAnalysisByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] SalesAnalysisForCreationDto input)
        {
            var created = await service.SalesAnalysisService.CreateSalesAnalysisAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.SalesAnalysisGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] SalesAnalysisForUpdateDto input)
        {
            await service.SalesAnalysisService.UpdateSalesAnalysisAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] SalesAnalysisForDeleteDto input)
        {
            await service.SalesAnalysisService.DeleteSalesAnalysisAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.SalesAnalysisService.DeleteSalesAnalysisByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] SalesAnalysisSearchDto input)
        {
            var result = await service.SalesAnalysisService.SearchSalesAnalysisAsync(
                input.SalesDate, input.SalesDateSearchType.ToString(),
                input.Salesperson, input.SalespersonSearchType.ToString(),
                input.Area, input.AreaSearchType.ToString(),
                input.ProductType, input.ProductTypeSearchType.ToString(),
                input.ProductName, input.ProductNameSearchType.ToString(),
                input.Quantity, input.QuantitySearchType.ToString(),
                input.SalesAmount, input.SalesAmountSearchType.ToString(),
                input.DiscountAmount, input.DiscountAmountSearchType.ToString(),
                input.NetSales, input.NetSalesSearchType.ToString(),
                input.CostAmount, input.CostAmountSearchType.ToString(),
                input.ProfitAmount, input.ProfitAmountSearchType.ToString(),
                input.SalesAnalysisDescription, input.SalesAnalysisDescriptionSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
