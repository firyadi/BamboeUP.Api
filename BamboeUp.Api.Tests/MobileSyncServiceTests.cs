using Contracts;
using Moq;
using Service.Modules;
using Shared.DataTransferObjects;
using System.Data;
using System.Text.Json;

namespace BamboeUp.Api.Tests;

public class MobileSyncServiceTests
{
    [Fact]
    public async Task DuplicateBatchReplay_ReturnsDurableReceiptsWithoutNewTransaction()
    {
        var repository = new Mock<IMobileSyncRepository>();
        var transactions = new Mock<ITransactionManager>();
        var user = User(companyId: 7);
        var batch = Batch(companyId: 7);
        var durable = new MobileSyncBatchResultV1
        {
            BatchGuid = batch.BatchGuid,
            Receipts =
            [
                new MobileSyncReceiptV1
                {
                    OperationGuid = batch.Operations[0].OperationGuid,
                    EntityGuid = batch.Operations[0].EntityGuid,
                    Outcome = MobileSyncOutcome.Applied,
                    ServerVersion = 1,
                    ProcessedAt = DateTimeOffset.UtcNow
                }
            ]
        };
        repository.Setup(x => x.GetCompletedBatchAsync(batch.BatchGuid, 7, 42)).ReturnsAsync(durable);

        var result = await new MobileSyncService(repository.Object, transactions.Object, user).PushAsync(batch);

        Assert.Same(durable, result);
        transactions.Verify(x => x.BeginTransactionAsync(), Times.Never);
        repository.Verify(x => x.TryReserveOperationAsync(
            It.IsAny<Guid>(), It.IsAny<MobileSyncOperationV1>(), It.IsAny<long>(),
            It.IsAny<long>(), It.IsAny<IDbTransaction>()), Times.Never);
    }

    [Fact]
    public async Task StaleAttendanceVersion_ReturnsDurableConflict()
    {
        var repository = new Mock<IMobileSyncRepository>();
        var transactions = new Mock<ITransactionManager>();
        var transaction = new Mock<IDbTransaction>().Object;
        var batch = Batch(companyId: 7, baseVersion: 1, operationType: MobileSyncV1.CheckOutOperation);
        MobileSyncReceiptV1? saved = null;

        transactions.Setup(x => x.GetTransaction()).Returns(transaction);
        repository.Setup(x => x.GetCompletedBatchAsync(batch.BatchGuid, 7, 42))
            .ReturnsAsync((MobileSyncBatchResultV1?)null);
        repository.Setup(x => x.TryReserveOperationAsync(
            batch.BatchGuid, batch.Operations[0], 7, 42, transaction)).ReturnsAsync(true);
        repository.Setup(x => x.PersonBelongsToTenantAsync(100, 7, transaction)).ReturnsAsync(true);
        repository.Setup(x => x.OfficeBelongsToTenantAsync(11, 7, transaction)).ReturnsAsync(true);
        repository.Setup(x => x.GetAttendanceAsync(batch.Operations[0].EntityGuid, 7, transaction, true))
            .ReturnsAsync(new AttendanceSyncState(
                batch.Operations[0].EntityGuid, 7, 100, DateTime.UtcNow.Date,
                DateTime.UtcNow.AddHours(-8), null, 2));
        repository.Setup(x => x.SaveReceiptAsync(
                batch.Operations[0].OperationGuid, It.IsAny<MobileSyncReceiptV1>(), 7, 42, transaction))
            .Callback<Guid, MobileSyncReceiptV1, long, long, IDbTransaction>((_, receipt, _, _, _) => saved = receipt)
            .Returns(Task.CompletedTask);

        var result = await new MobileSyncService(repository.Object, transactions.Object, User(7)).PushAsync(batch);

        Assert.Equal(MobileSyncOutcome.Conflict, result.Receipts[0].Outcome);
        Assert.Equal("VERSION_CONFLICT", result.Receipts[0].Code);
        Assert.Equal(2, result.Receipts[0].ServerVersion);
        Assert.Equal(result.Receipts[0], saved);
        repository.Verify(x => x.ApplyAttendanceEventAsync(
            It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<AttendanceSyncEventV1>(), It.IsAny<long?>(),
            It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<long>(), It.IsAny<IDbTransaction>()), Times.Never);
        transactions.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task BatchForDifferentTenant_IsRejectedBeforeRepositoryAccess()
    {
        var repository = new Mock<IMobileSyncRepository>();
        var transactions = new Mock<ITransactionManager>();
        var service = new MobileSyncService(repository.Object, transactions.Object, User(companyId: 7));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.PushAsync(Batch(companyId: 8)));

        repository.VerifyNoOtherCalls();
        transactions.VerifyNoOtherCalls();
    }

    private static MobileSyncBatchV1 Batch(
        long companyId,
        long? baseVersion = null,
        string operationType = MobileSyncV1.CheckInOperation)
    {
        var payload = JsonSerializer.SerializeToElement(new AttendanceSyncEventV1
        {
            PersonId = 100,
            CompanyOfficeId = 11,
            OccurredAt = DateTimeOffset.UtcNow,
            Channel = "Mobile"
        });
        return new MobileSyncBatchV1
        {
            BatchGuid = Guid.NewGuid(),
            DeviceGuid = Guid.NewGuid(),
            CompanyId = companyId,
            ClientCreatedAt = DateTimeOffset.UtcNow,
            Operations =
            [
                new MobileSyncOperationV1
                {
                    OperationGuid = Guid.NewGuid(),
                    EntityGuid = Guid.NewGuid(),
                    EntityType = MobileSyncV1.AttendanceEntity,
                    OperationType = operationType,
                    BaseVersion = baseVersion,
                    Payload = payload
                }
            ]
        };
    }

    private static IUserContext User(long companyId)
    {
        var user = new Mock<IUserContext>();
        user.SetupGet(x => x.IsAuthenticated).Returns(true);
        user.SetupGet(x => x.UserId).Returns(42);
        user.SetupGet(x => x.CompanyId).Returns(companyId);
        return user.Object;
    }
}
