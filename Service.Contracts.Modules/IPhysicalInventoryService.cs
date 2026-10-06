using Shared.DataTransferObjects;

namespace Service.Contracts.Modules;

public interface IPhysicalInventoryService
{
    Task<string> GetPublicKeyAsync();
    Task<OfflineAccessPermitDto> IssuePermitAsync(Guid deviceGuid);
    Task<PhysicalInventoryCountReceiptDto> UploadAsync(PhysicalInventoryCountUploadDto upload);
    Task<IReadOnlyList<PhysicalInventoryCaptureDto>> GetQueueAsync();
    Task<PhysicalInventoryCaptureDto?> GetCaptureAsync(Guid captureGuid);
    Task VerifyAsync(Guid captureGuid, PhysicalInventoryVerifyRequestDto input);
    Task RejectAsync(Guid captureGuid, PhysicalInventoryVerifyRequestDto input);
}
