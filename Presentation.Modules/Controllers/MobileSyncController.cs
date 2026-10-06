using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace BamboeUp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/mobile-sync")]
public sealed class MobileSyncController(
    IMobileSyncService service,
    IPhysicalInventoryService inventory) : ControllerBase
{
    [HttpPost("push")]
    [SwaggerOperation(Summary = "Push a durable mobile sync batch")]
    [ProducesResponseType(typeof(MobileSyncBatchResultV1), StatusCodes.Status200OK)]
    public async Task<ActionResult<MobileSyncBatchResultV1>> Push(
        [FromBody] MobileSyncBatchV1 batch,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(await service.PushAsync(batch));
    }

    [HttpGet("offline-permit/public-key")]
    [SwaggerOperation(Summary = "Public key used to verify offline permits")]
    public async Task<IActionResult> PublicKey() =>
        Ok(new { publicKey = await inventory.GetPublicKeyAsync() });

    [HttpPost("offline-permit")]
    [SwaggerOperation(Summary = "Issue a 7-day device-bound offline permit")]
    [ProducesResponseType(typeof(OfflineAccessPermitDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OfflineAccessPermitDto>> IssuePermit(
        [FromBody] OfflinePermitIssueRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(await inventory.IssuePermitAsync(request.DeviceGuid));
    }

    [HttpPost("physical-inventory-counts")]
    [SwaggerOperation(Summary = "Upload a finished Physical Inventory Count page")]
    [ProducesResponseType(typeof(PhysicalInventoryCountReceiptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PhysicalInventoryCountReceiptDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PhysicalInventoryCountReceiptDto>> UploadCount(
        [FromBody] PhysicalInventoryCountUploadDto upload,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var receipt = await inventory.UploadAsync(upload);
        if (string.Equals(receipt.Code, "PAYLOAD_MISMATCH", StringComparison.Ordinal))
            return Conflict(receipt);
        return Ok(receipt);
    }

    [HttpGet("physical-inventory-counts")]
    [SwaggerOperation(Summary = "Verification queue for Physical Inventory Count")]
    public async Task<ActionResult<IReadOnlyList<PhysicalInventoryCaptureDto>>> Queue() =>
        Ok(await inventory.GetQueueAsync());

    [HttpGet("physical-inventory-counts/{captureGuid:guid}")]
    public async Task<ActionResult<PhysicalInventoryCaptureDto>> Get(Guid captureGuid)
    {
        var item = await inventory.GetCaptureAsync(captureGuid);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("physical-inventory-counts/{captureGuid:guid}/verify")]
    public async Task<IActionResult> Verify(Guid captureGuid, [FromBody] PhysicalInventoryVerifyRequestDto input)
    {
        await inventory.VerifyAsync(captureGuid, input);
        return NoContent();
    }

    [HttpPost("physical-inventory-counts/{captureGuid:guid}/reject")]
    public async Task<IActionResult> Reject(Guid captureGuid, [FromBody] PhysicalInventoryVerifyRequestDto input)
    {
        await inventory.RejectAsync(captureGuid, input);
        return NoContent();
    }
}
