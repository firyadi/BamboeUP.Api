using System.Security.Cryptography;
using System.Text.Json;
using Contracts;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Shared.Security;

namespace Service.Modules;

public sealed class PhysicalInventoryService(
    IPhysicalInventoryRepository repository,
    ITransactionManager transactionManager,
    IUserContext userContext,
    OfflinePermitKeyRing keyRing) : IPhysicalInventoryService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task<string> GetPublicKeyAsync() => Task.FromResult(keyRing.PublicKeyBase64);

    public async Task<OfflineAccessPermitDto> IssuePermitAsync(Guid deviceGuid)
    {
        EnsureUser();
        if (deviceGuid == Guid.Empty)
            throw new ArgumentException("DeviceGuid is required.");
        if (userContext.CompanyId is not > 0)
            throw new UnauthorizedAccessException("An active company is required for an offline permit.");

        var now = DateTimeOffset.UtcNow;
        var canVerify = userContext.IsAdmin
            || userContext.Role.Contains(PhysicalInventoryApps.RoleVerifier, StringComparison.OrdinalIgnoreCase);
        var canCapture = userContext.IsAdmin
            || userContext.Role.Contains(PhysicalInventoryApps.RoleCounter, StringComparison.OrdinalIgnoreCase)
            || !canVerify;

        var unsigned = new OfflineAccessPermitDto
        {
            DeviceGuid = deviceGuid,
            UserGuid = userContext.UserGuid,
            UserId = userContext.UserId,
            CompanyId = userContext.CompanyId.Value,
            OfficeId = userContext.OfficeId,
            AppCode = PhysicalInventoryApps.AppCode,
            CanCapture = canCapture,
            CanVerify = canVerify,
            PermissionVersion = 1,
            IssuedAt = now,
            ExpiresAt = now.AddDays(PhysicalInventoryApps.PermitDays),
            ServerTime = now
        };
        var signed = OfflinePermitSigner.Sign(unsigned, keyRing.PrivateKey);
        var json = JsonSerializer.Serialize(signed, JsonOptions);

        await transactionManager.BeginTransactionAsync();
        try
        {
            await repository.UpsertPermitAsync(signed, json, transactionManager.GetTransaction());
            await transactionManager.CommitAsync();
        }
        catch
        {
            await transactionManager.RollbackAsync();
            throw;
        }

        return signed;
    }

    public async Task<PhysicalInventoryCountReceiptDto> UploadAsync(PhysicalInventoryCountUploadDto upload)
    {
        EnsureUser();
        if (userContext.CompanyId is not long companyId)
            throw new UnauthorizedAccessException("An active company is required.");
        ValidateUpload(upload);
        var expectedHash = PhysicalInventoryPayloadHasher.Compute(upload);
        if (!string.Equals(expectedHash, upload.PayloadHash, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("PAYLOAD_HASH_MISMATCH");

        var existingByOperation = await repository.GetCaptureByOperationAsync(upload.OperationId, companyId);
        if (existingByOperation is not null)
        {
            if (!string.Equals(existingByOperation.PayloadHash, expectedHash, StringComparison.OrdinalIgnoreCase))
                return Mismatch(upload, existingByOperation.ConcurrencyToken);
            return Receipt(existingByOperation, null, "Replay of a durable receipt.");
        }

        var existingByCapture = await repository.GetCaptureByGuidAsync(upload.CaptureGuid, companyId);
        if (existingByCapture is not null)
        {
            if (!string.Equals(existingByCapture.PayloadHash, expectedHash, StringComparison.OrdinalIgnoreCase))
                return Mismatch(upload, existingByCapture.ConcurrencyToken);
            return Receipt(existingByCapture, null, "Replay of a durable receipt.");
        }

        await transactionManager.BeginTransactionAsync();
        var transaction = transactionManager.GetTransaction();
        try
        {
            var (documentId, pageId) = await repository.ResolveDocumentPageAsync(
                upload.DocumentNumber.Trim(), upload.PageNumber, companyId, transaction);
            var duplicate = await repository.OtherCaptureExistsAsync(
                upload.DocumentNumber.Trim(), upload.PageNumber, companyId, upload.CaptureGuid, transaction);

            var status = documentId is null || pageId is null
                ? PhysicalInventoryStatuses.DocumentMatchRequired
                : duplicate
                    ? PhysicalInventoryStatuses.DuplicatePageReview
                    : PhysicalInventoryStatuses.UploadedPendingVerification;

            var captureId = await repository.InsertCaptureAsync(new PhysicalInventoryCaptureInsert(
                upload.CaptureGuid,
                upload.OperationId,
                companyId,
                userContext.OfficeId,
                upload.DeviceGuid,
                upload.DocumentNumber.Trim(),
                upload.PageNumber,
                documentId,
                pageId,
                userContext.UserId,
                userContext.UserGuid,
                status,
                expectedHash,
                upload.StartedAt,
                upload.FinishedAt,
                userContext.UserId), transaction);

            foreach (var line in upload.Lines)
            {
                var barcode = line.Barcode.Trim();
                var item = await repository.FindItemByBarcodeAsync(barcode, companyId, transaction);
                decimal? expected = null;
                var validation = "Unresolved";
                if (item is not null)
                {
                    validation = "Resolved";
                    if (pageId is long resolvedPage)
                        expected = await repository.GetExpectedQuantityAsync(resolvedPage, item.InventoryItemId, transaction);
                    if (expected is null && pageId is not null)
                        validation = "NoExpectedQuantity";
                }

                decimal? variance = expected is null ? null : line.Quantity - expected;
                await repository.InsertCaptureLineAsync(new PhysicalInventoryCaptureLineInsert(
                    line.LineGuid,
                    captureId,
                    barcode,
                    line.Quantity,
                    Math.Max(1, line.ScanCount),
                    string.IsNullOrWhiteSpace(line.Source) ? "Manual" : line.Source,
                    item?.InventoryItemId,
                    expected,
                    variance,
                    validation,
                    line.FirstCapturedAt,
                    line.LastCapturedAt,
                    userContext.UserId), transaction);
            }

            await transactionManager.CommitAsync();
            return new PhysicalInventoryCountReceiptDto
            {
                OperationId = upload.OperationId,
                CaptureGuid = upload.CaptureGuid,
                Status = status,
                PayloadHash = expectedHash,
                ConcurrencyToken = 1,
                CommittedAt = DateTimeOffset.UtcNow,
                Message = "Capture stored for verification."
            };
        }
        catch
        {
            await transactionManager.RollbackAsync();
            throw;
        }
    }

    public async Task<IReadOnlyList<PhysicalInventoryCaptureDto>> GetQueueAsync()
    {
        EnsureUser();
        if (userContext.CompanyId is not long companyId)
            throw new UnauthorizedAccessException("An active company is required.");
        var pending = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            PhysicalInventoryStatuses.UploadedPendingVerification,
            PhysicalInventoryStatuses.DocumentMatchRequired,
            PhysicalInventoryStatuses.DuplicatePageReview
        };
        return (await repository.GetQueueAsync(companyId))
            .Where(x => pending.Contains(x.CaptureStatus))
            .ToList();
    }

    public async Task<PhysicalInventoryCaptureDto?> GetCaptureAsync(Guid captureGuid)
    {
        EnsureUser();
        if (userContext.CompanyId is not long companyId)
            throw new UnauthorizedAccessException("An active company is required.");
        var header = (await repository.GetQueueAsync(companyId))
            .FirstOrDefault(x => x.PhysicalInventoryCaptureGuid == captureGuid);
        if (header is null)
        {
            var record = await repository.GetCaptureByGuidAsync(captureGuid, companyId);
            if (record is null)
                return null;
            header = new PhysicalInventoryCaptureDto
            {
                PhysicalInventoryCaptureGuid = record.PhysicalInventoryCaptureGuid,
                CaptureStatus = record.CaptureStatus,
                PayloadHash = record.PayloadHash,
                ConcurrencyToken = record.ConcurrencyToken,
                CounterUserId = record.CounterUserId
            };
        }

        var lines = await repository.GetLinesAsync(
            (await repository.GetCaptureByGuidAsync(captureGuid, companyId))!.PhysicalInventoryCaptureId);
        return header with { Lines = lines };
    }

    public Task VerifyAsync(Guid captureGuid, PhysicalInventoryVerifyRequestDto input) =>
        CompleteAsync(captureGuid, input, PhysicalInventoryStatuses.Verified);

    public Task RejectAsync(Guid captureGuid, PhysicalInventoryVerifyRequestDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Note))
            throw new ArgumentException("A rejection reason is required.");
        return CompleteAsync(captureGuid, input, PhysicalInventoryStatuses.Rejected);
    }

    private async Task CompleteAsync(Guid captureGuid, PhysicalInventoryVerifyRequestDto input, string status)
    {
        EnsureUser();
        if (userContext.CompanyId is not long companyId)
            throw new UnauthorizedAccessException("An active company is required.");
        var canVerify = userContext.IsAdmin
            || userContext.Role.Contains(PhysicalInventoryApps.RoleVerifier, StringComparison.OrdinalIgnoreCase);
        if (!canVerify)
            throw new UnauthorizedAccessException("The current user cannot verify physical inventory captures.");

        var capture = await repository.GetCaptureByGuidAsync(captureGuid, companyId)
            ?? throw new KeyNotFoundException("Capture was not found.");
        if (capture.CounterUserId == userContext.UserId)
            throw new InvalidOperationException("A counter cannot verify their own capture.");
        if (capture.CaptureStatus is PhysicalInventoryStatuses.Verified or PhysicalInventoryStatuses.Rejected)
            throw new InvalidOperationException("The capture has already been completed.");

        await transactionManager.BeginTransactionAsync();
        try
        {
            var ok = await repository.TryVerifyAsync(
                captureGuid, companyId, userContext.UserId, status, input.Note,
                input.ExpectedConcurrencyToken, transactionManager.GetTransaction());
            if (!ok)
                throw new InvalidOperationException("CONCURRENCY_CONFLICT");
            await transactionManager.CommitAsync();
        }
        catch
        {
            await transactionManager.RollbackAsync();
            throw;
        }
    }

    private void EnsureUser()
    {
        if (!userContext.IsAuthenticated || userContext.UserId <= 0)
            throw new UnauthorizedAccessException("An authenticated user is required.");
    }

    private static void ValidateUpload(PhysicalInventoryCountUploadDto upload)
    {
        if (upload.Version != PhysicalInventoryApps.PayloadVersion)
            throw new ArgumentException($"Unsupported payload version '{upload.Version}'.");
        if (upload.OperationId == Guid.Empty || upload.CaptureGuid == Guid.Empty || upload.DeviceGuid == Guid.Empty)
            throw new ArgumentException("OperationId, CaptureGuid, and DeviceGuid are required.");
        if (string.IsNullOrWhiteSpace(upload.DocumentNumber) || upload.PageNumber <= 0)
            throw new ArgumentException("Document number and a positive page number are required.");
        if (upload.Lines.Count == 0)
            throw new ArgumentException("At least one counted line is required.");
        if (upload.Lines.Any(x => string.IsNullOrWhiteSpace(x.Barcode) || x.Quantity <= 0))
            throw new ArgumentException("Every line requires a barcode and a positive quantity.");
        if (upload.Lines.Select(x => x.Barcode.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != upload.Lines.Count)
            throw new ArgumentException("Duplicate barcodes must be accumulated before upload.");
    }

    private static PhysicalInventoryCountReceiptDto Receipt(
        PhysicalInventoryCaptureRecord record, string? code, string message) =>
        new()
        {
            OperationId = record.OperationGuid,
            CaptureGuid = record.PhysicalInventoryCaptureGuid,
            Status = record.CaptureStatus,
            Code = code,
            Message = message,
            ConcurrencyToken = record.ConcurrencyToken,
            PayloadHash = record.PayloadHash,
            CommittedAt = DateTimeOffset.UtcNow
        };

    private static PhysicalInventoryCountReceiptDto Mismatch(PhysicalInventoryCountUploadDto upload, long token) =>
        new()
        {
            OperationId = upload.OperationId,
            CaptureGuid = upload.CaptureGuid,
            Status = PhysicalInventoryStatuses.Conflict,
            Code = "PAYLOAD_MISMATCH",
            Message = "The capture GUID already exists with a different payload hash.",
            ConcurrencyToken = token,
            PayloadHash = upload.PayloadHash,
            CommittedAt = DateTimeOffset.UtcNow
        };
}

public sealed class OfflinePermitKeyRing
{
    public OfflinePermitKeyRing(string privateKeyBase64, string publicKeyBase64)
    {
        PrivateKey = RSA.Create();
        PrivateKey.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
        PublicKey = RSA.Create();
        PublicKey.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64), out _);
        PublicKeyBase64 = publicKeyBase64;
    }

    public RSA PrivateKey { get; }
    public RSA PublicKey { get; }
    public string PublicKeyBase64 { get; }

    public static OfflinePermitKeyRing CreateEphemeral()
    {
        using var rsa = RSA.Create(2048);
        return new OfflinePermitKeyRing(
            Convert.ToBase64String(rsa.ExportPkcs8PrivateKey()),
            Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()));
    }
}
