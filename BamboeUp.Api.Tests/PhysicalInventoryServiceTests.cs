using Contracts;
using Moq;
using Service.Modules;
using Shared.DataTransferObjects;
using Shared.Security;
using System.Data;
using System.Security.Cryptography;

namespace BamboeUp.Api.Tests;

public class PhysicalInventoryServiceTests
{
    [Fact]
    public async Task DuplicateOperation_ReplaysDurableReceipt()
    {
        var repository = new Mock<IPhysicalInventoryRepository>();
        var transactions = new Mock<ITransactionManager>();
        var upload = Upload();
        var existing = new PhysicalInventoryCaptureRecord(
            9, upload.CaptureGuid, upload.OperationId, upload.PayloadHash,
            PhysicalInventoryStatuses.UploadedPendingVerification, 3, 42);
        repository.Setup(x => x.GetCaptureByOperationAsync(It.IsAny<Guid>(), 7, It.IsAny<IDbTransaction?>())).ReturnsAsync(existing);

        var result = await Service(repository, transactions).UploadAsync(upload);

        Assert.Equal(existing.CaptureStatus, result.Status);
        Assert.Equal(existing.PayloadHash, result.PayloadHash);
        Assert.Equal(3, result.ConcurrencyToken);
        transactions.Verify(x => x.BeginTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task SameCaptureGuid_DifferentHash_ReturnsPayloadMismatch()
    {
        var repository = new Mock<IPhysicalInventoryRepository>();
        var transactions = new Mock<ITransactionManager>();
        var upload = Upload();
        repository.Setup(x => x.GetCaptureByOperationAsync(It.IsAny<Guid>(), 7, It.IsAny<IDbTransaction?>()))
            .ReturnsAsync((PhysicalInventoryCaptureRecord?)null);
        repository.Setup(x => x.GetCaptureByGuidAsync(It.IsAny<Guid>(), 7, It.IsAny<IDbTransaction?>()))
            .ReturnsAsync(new PhysicalInventoryCaptureRecord(
                9, upload.CaptureGuid, Guid.NewGuid(), "deadbeef",
                PhysicalInventoryStatuses.UploadedPendingVerification, 1, 42));

        var result = await Service(repository, transactions).UploadAsync(upload);

        Assert.Equal("PAYLOAD_MISMATCH", result.Code);
        Assert.Equal(PhysicalInventoryStatuses.Conflict, result.Status);
        transactions.Verify(x => x.BeginTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task UnknownDocument_IsAcceptedAsDocumentMatchRequired()
    {
        var repository = new Mock<IPhysicalInventoryRepository>();
        var transactions = Transaction();
        var upload = Upload();
        repository.Setup(x => x.GetCaptureByOperationAsync(It.IsAny<Guid>(), 7, It.IsAny<IDbTransaction?>()))
            .ReturnsAsync((PhysicalInventoryCaptureRecord?)null);
        repository.Setup(x => x.GetCaptureByGuidAsync(It.IsAny<Guid>(), 7, It.IsAny<IDbTransaction?>()))
            .ReturnsAsync((PhysicalInventoryCaptureRecord?)null);
        repository.Setup(x => x.ResolveDocumentPageAsync(upload.DocumentNumber, upload.PageNumber, 7, It.IsAny<IDbTransaction>()))
            .ReturnsAsync((null, null));
        repository.Setup(x => x.OtherCaptureExistsAsync(upload.DocumentNumber, upload.PageNumber, 7, upload.CaptureGuid, It.IsAny<IDbTransaction>()))
            .ReturnsAsync(false);
        repository.Setup(x => x.InsertCaptureAsync(It.IsAny<PhysicalInventoryCaptureInsert>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(15);
        repository.Setup(x => x.FindItemByBarcodeAsync("ABC", 7, It.IsAny<IDbTransaction>()))
            .ReturnsAsync((InventoryItemMatch?)null);

        var result = await Service(repository, transactions).UploadAsync(upload);

        Assert.Equal(PhysicalInventoryStatuses.DocumentMatchRequired, result.Status);
        transactions.Verify(x => x.CommitAsync(), Times.Once);
        repository.Verify(x => x.InsertCaptureLineAsync(
            It.Is<PhysicalInventoryCaptureLineInsert>(l => l.ValidationResult == "Unresolved"),
            It.IsAny<IDbTransaction>()), Times.Once);
    }

    [Fact]
    public async Task DuplicateDocumentPage_IsDuplicatePageReview()
    {
        var repository = new Mock<IPhysicalInventoryRepository>();
        var transactions = Transaction();
        var upload = Upload();
        repository.Setup(x => x.GetCaptureByOperationAsync(It.IsAny<Guid>(), 7, It.IsAny<IDbTransaction?>()))
            .ReturnsAsync((PhysicalInventoryCaptureRecord?)null);
        repository.Setup(x => x.GetCaptureByGuidAsync(It.IsAny<Guid>(), 7, It.IsAny<IDbTransaction?>()))
            .ReturnsAsync((PhysicalInventoryCaptureRecord?)null);
        repository.Setup(x => x.ResolveDocumentPageAsync(upload.DocumentNumber, upload.PageNumber, 7, It.IsAny<IDbTransaction>()))
            .ReturnsAsync((10L, 20L));
        repository.Setup(x => x.OtherCaptureExistsAsync(upload.DocumentNumber, upload.PageNumber, 7, upload.CaptureGuid, It.IsAny<IDbTransaction>()))
            .ReturnsAsync(true);
        repository.Setup(x => x.InsertCaptureAsync(It.IsAny<PhysicalInventoryCaptureInsert>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(15);
        repository.Setup(x => x.FindItemByBarcodeAsync("ABC", 7, It.IsAny<IDbTransaction>()))
            .ReturnsAsync(new InventoryItemMatch(3, "I-1", "Item"));
        repository.Setup(x => x.GetExpectedQuantityAsync(20, 3, It.IsAny<IDbTransaction>()))
            .ReturnsAsync(5m);

        var result = await Service(repository, transactions).UploadAsync(upload);

        Assert.Equal(PhysicalInventoryStatuses.DuplicatePageReview, result.Status);
        repository.Verify(x => x.InsertCaptureLineAsync(
            It.Is<PhysicalInventoryCaptureLineInsert>(l => l.Variance == -3m && l.ValidationResult == "Resolved"),
            It.IsAny<IDbTransaction>()), Times.Once);
    }

    [Fact]
    public async Task CounterCannotVerifyOwnCapture()
    {
        var repository = new Mock<IPhysicalInventoryRepository>();
        var transactions = new Mock<ITransactionManager>();
        repository.Setup(x => x.GetCaptureByGuidAsync(It.IsAny<Guid>(), 7, It.IsAny<IDbTransaction?>()))
            .ReturnsAsync(new PhysicalInventoryCaptureRecord(
                1, Guid.NewGuid(), Guid.NewGuid(), "hash",
                PhysicalInventoryStatuses.UploadedPendingVerification, 1, 42));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Service(repository, transactions, role: PhysicalInventoryApps.RoleVerifier)
                .VerifyAsync(Guid.NewGuid(), new PhysicalInventoryVerifyRequestDto { ExpectedConcurrencyToken = 1 }));
        Assert.Contains("cannot verify", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DifferentUserVerify_UsesConcurrencyToken()
    {
        var repository = new Mock<IPhysicalInventoryRepository>();
        var transactions = Transaction();
        var captureGuid = Guid.NewGuid();
        repository.Setup(x => x.GetCaptureByGuidAsync(captureGuid, 7, It.IsAny<IDbTransaction?>()))
            .ReturnsAsync(new PhysicalInventoryCaptureRecord(
                1, captureGuid, Guid.NewGuid(), "hash",
                PhysicalInventoryStatuses.UploadedPendingVerification, 4, 99));
        repository.Setup(x => x.TryVerifyAsync(captureGuid, 7, 42, PhysicalInventoryStatuses.Verified, "ok", 4, It.IsAny<IDbTransaction>()))
            .ReturnsAsync(true);

        await Service(repository, transactions, role: PhysicalInventoryApps.RoleVerifier)
            .VerifyAsync(captureGuid, new PhysicalInventoryVerifyRequestDto { ExpectedConcurrencyToken = 4, Note = "ok" });

        transactions.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task StaleConcurrencyToken_FailsClosed()
    {
        var repository = new Mock<IPhysicalInventoryRepository>();
        var transactions = Transaction();
        var captureGuid = Guid.NewGuid();
        repository.Setup(x => x.GetCaptureByGuidAsync(captureGuid, 7, It.IsAny<IDbTransaction?>()))
            .ReturnsAsync(new PhysicalInventoryCaptureRecord(
                1, captureGuid, Guid.NewGuid(), "hash",
                PhysicalInventoryStatuses.UploadedPendingVerification, 2, 99));
        repository.Setup(x => x.TryVerifyAsync(captureGuid, 7, 42, PhysicalInventoryStatuses.Verified, null, 1, It.IsAny<IDbTransaction>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Service(repository, transactions, role: PhysicalInventoryApps.RoleVerifier)
                .VerifyAsync(captureGuid, new PhysicalInventoryVerifyRequestDto { ExpectedConcurrencyToken = 1 }));
        transactions.Verify(x => x.RollbackAsync(), Times.Once);
    }

    [Fact]
    public async Task RejectWithoutReason_IsRejected()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            Service(new Mock<IPhysicalInventoryRepository>(), new Mock<ITransactionManager>(), role: PhysicalInventoryApps.RoleVerifier)
                .RejectAsync(Guid.NewGuid(), new PhysicalInventoryVerifyRequestDto { ExpectedConcurrencyToken = 1 }));
    }

    [Fact]
    public void PermitSigner_RejectsClockRollback()
    {
        using var rsa = RSA.Create(2048);
        var issued = DateTimeOffset.UtcNow;
        var permit = OfflinePermitSigner.Sign(new OfflineAccessPermitDto
        {
            DeviceGuid = Guid.NewGuid(),
            UserGuid = Guid.NewGuid(),
            UserId = 1,
            CompanyId = 7,
            AppCode = PhysicalInventoryApps.AppCode,
            CanCapture = true,
            IssuedAt = issued,
            ExpiresAt = issued.AddDays(7),
            ServerTime = issued
        }, rsa);
        Assert.True(OfflinePermitSigner.Verify(permit, rsa));
        Assert.False(OfflinePermitSigner.IsClockSkewSafe(permit, issued.AddHours(-1), TimeSpan.FromMinutes(5)));
        Assert.True(OfflinePermitSigner.IsClockSkewSafe(permit, issued.AddMinutes(1), TimeSpan.FromMinutes(5)));
    }

    [Fact]
    public void LocalRules_RequireDocumentPageBarcodeAndPositiveQuantity()
    {
        Assert.False(PhysicalInventoryLocalRules.TryValidateHeader(" ", 1, out _));
        Assert.False(PhysicalInventoryLocalRules.TryValidateHeader("DOC-1", 0, out _));
        Assert.True(PhysicalInventoryLocalRules.TryValidateHeader("DOC-1", 2, out _));
        Assert.False(PhysicalInventoryLocalRules.TryValidateLine("", 1, out _));
        Assert.False(PhysicalInventoryLocalRules.TryValidateLine("ABC", 0, out _));
        Assert.Equal(5m, PhysicalInventoryLocalRules.Accumulate(2, 3));
        Assert.Equal("ABC", PhysicalInventoryLocalRules.NormalizeBarcode(" abc "));
    }

    [Fact]
    public void Hasher_IsStableForAccumulatedLines()
    {
        var left = Upload();
        var right = Upload(left.OperationId, left.CaptureGuid, left.DeviceGuid);
        Assert.Equal(PhysicalInventoryPayloadHasher.Compute(left), PhysicalInventoryPayloadHasher.Compute(right));
    }

    private static PhysicalInventoryService Service(
        Mock<IPhysicalInventoryRepository> repository,
        Mock<ITransactionManager> transactions,
        string role = "InventoryCounter") =>
        new(repository.Object, transactions.Object, User(7, role), OfflinePermitKeyRing.CreateEphemeral());

    private static Mock<ITransactionManager> Transaction()
    {
        var transactions = new Mock<ITransactionManager>();
        transactions.Setup(x => x.GetTransaction()).Returns(new Mock<IDbTransaction>().Object);
        return transactions;
    }

    private static PhysicalInventoryCountUploadDto Upload(
        Guid? operationId = null, Guid? captureGuid = null, Guid? deviceGuid = null)
    {
        var dto = new PhysicalInventoryCountUploadDto
        {
            OperationId = operationId ?? Guid.NewGuid(),
            CaptureGuid = captureGuid ?? Guid.NewGuid(),
            DeviceGuid = deviceGuid ?? Guid.NewGuid(),
            DocumentNumber = "SO-1",
            PageNumber = 1,
            StartedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            FinishedAt = DateTimeOffset.UtcNow,
            PayloadHash = string.Empty,
            Lines =
            [
                new PhysicalInventoryCountLineUploadDto
                {
                    LineGuid = Guid.NewGuid(),
                    Barcode = "ABC",
                    Quantity = 2,
                    ScanCount = 2,
                    FirstCapturedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
                    LastCapturedAt = DateTimeOffset.UtcNow,
                    Source = "Camera"
                }
            ]
        };
        return dto with { PayloadHash = PhysicalInventoryPayloadHasher.Compute(dto) };
    }

    private static IUserContext User(long companyId, string role)
    {
        var user = new Mock<IUserContext>();
        user.SetupGet(x => x.IsAuthenticated).Returns(true);
        user.SetupGet(x => x.UserId).Returns(42);
        user.SetupGet(x => x.UserGuid).Returns(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        user.SetupGet(x => x.CompanyId).Returns(companyId);
        user.SetupGet(x => x.OfficeId).Returns(11);
        user.SetupGet(x => x.Role).Returns(role);
        user.SetupGet(x => x.IsAdmin).Returns(false);
        return user.Object;
    }
}
