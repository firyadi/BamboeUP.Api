using Shared.DataTransferObjects;

namespace Service.Contracts.Modules;

public interface IMobileSyncService
{
    Task<MobileSyncBatchResultV1> PushAsync(MobileSyncBatchV1 batch);
}
