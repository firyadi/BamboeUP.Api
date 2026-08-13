using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/assets")]
    [ApiController]
    public partial class AssetsController(IServiceModulesManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.AssetService.GetAllAssetsAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.AssetService.GetAssetByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] AssetForCreationDto input)
        {
            var created = await service.AssetService.CreateAssetAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.AssetGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] AssetForUpdateDto input)
        {
            await service.AssetService.UpdateAssetAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] AssetForDeleteDto input)
        {
            await service.AssetService.DeleteAssetAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.AssetService.DeleteAssetByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] AssetSearchDto input)
        {
            var result = await service.AssetService.SearchAssetAsync(
                input.AssetCode, input.AssetCodeSearchType.ToString(),
                input.AssetName, input.AssetNameSearchType.ToString(),
                input.AssetCategoryId, input.AssetCategoryIdSearchType.ToString(),
                input.AssetBrand, input.AssetBrandSearchType.ToString(),
                input.AssetModel, input.AssetModelSearchType.ToString(),
                input.SerialNumber, input.SerialNumberSearchType.ToString(),
                input.Barcode, input.BarcodeSearchType.ToString(),
                input.CompanyId, input.CompanyIdSearchType.ToString(),
                input.CompanyOfficeId, input.CompanyOfficeIdSearchType.ToString(),
                input.CostCenterId, input.CostCenterIdSearchType.ToString(),
                input.PurchaseDate, input.PurchaseDateSearchType.ToString(),
                input.PurchasePrice, input.PurchasePriceSearchType.ToString(),
                input.CurrencyId, input.CurrencyIdSearchType.ToString(),
                input.WarrantyStartDate, input.WarrantyStartDateSearchType.ToString(),
                input.WarrantyEndDate, input.WarrantyEndDateSearchType.ToString(),
                input.SrAssetCondition, input.SrAssetConditionSearchType.ToString(),
                input.SrAssetStatus, input.SrAssetStatusSearchType.ToString(),
                input.CurrentLocation, input.CurrentLocationSearchType.ToString(),
                input.Description, input.DescriptionSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
