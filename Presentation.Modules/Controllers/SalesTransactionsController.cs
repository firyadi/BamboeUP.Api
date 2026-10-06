using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/salesTransactions")]
    [ApiController]
    public partial class SalesTransactionsController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.SalesTransactionService.GetAllSalesTransactionsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.SalesTransactionService.GetSalesTransactionByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] SalesTransactionForCreationDto input)
        {
            var created = await service.SalesTransactionService.CreateSalesTransactionAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.SalesTransactionGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] SalesTransactionForUpdateDto input)
        {
            await service.SalesTransactionService.UpdateSalesTransactionAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] SalesTransactionForDeleteDto input)
        {
            await service.SalesTransactionService.DeleteSalesTransactionAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.SalesTransactionService.DeleteSalesTransactionByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] SalesTransactionSearchDto input)
        {
            var result = await service.SalesTransactionService.SearchSalesTransactionAsync(
                input.TransactionCode, input.TransactionCodeSearchType.ToString(),
                input.CompanyId, input.CompanyIdSearchType.ToString(),
                input.CompanyOfficeId, input.CompanyOfficeIdSearchType.ToString(),
                input.PeriodYear, input.PeriodYearSearchType.ToString(),
                input.PeriodMonth, input.PeriodMonthSearchType.ToString(),
                input.TransactionDate, input.TransactionDateSearchType.ToString(),
                input.RegionCode, input.RegionCodeSearchType.ToString(),
                input.ProductCategory, input.ProductCategorySearchType.ToString(),
                input.ProductCode, input.ProductCodeSearchType.ToString(),
                input.ProductName, input.ProductNameSearchType.ToString(),
                input.SalesChannel, input.SalesChannelSearchType.ToString(),
                input.CustomerSegment, input.CustomerSegmentSearchType.ToString(),
                input.Quantity, input.QuantitySearchType.ToString(),
                input.UnitPrice, input.UnitPriceSearchType.ToString(),
                input.DiscountAmount, input.DiscountAmountSearchType.ToString(),
                input.NetAmount, input.NetAmountSearchType.ToString(),
                input.CostAmount, input.CostAmountSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
