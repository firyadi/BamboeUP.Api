using Contracts;
using Dapper;
using Npgsql;
using Repository;
using Repository.Database;
using Shared.DataTransferObjects;
using Shared.Settings;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Xunit;

namespace BamboeUp.Api.Tests.Database;

[Trait("Category", "Integration")]
public class PhysicalInventoryPostgreSqlTests : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private string _connectionString = string.Empty;
    private bool _dockerAvailable;

    public async Task InitializeAsync()
    {
        try
        {
            _container = new PostgreSqlBuilder().WithImage("postgres:16-alpine").Build();
            await _container.StartAsync();
            _connectionString = _container.GetConnectionString();
            var sqlPath = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory, "..", "..", "..", "..", "Table", "PhysicalInventory.PostgreSQL.sql"));
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(await File.ReadAllTextAsync(sqlPath));
            _dockerAvailable = true;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            _dockerAvailable = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync();
    }

    [Fact]
    public async Task UniqueOperationAndCapture_AreTenantScoped()
    {
        if (!_dockerAvailable) return;
        var repo = Repository();
        var captureA = Guid.NewGuid();
        var operation = Guid.NewGuid();

        using (var connection = CreateContext().CreateConnection())
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var first = await repo.InsertCaptureAsync(Insert(captureA, operation, companyId: 7, counter: 1), transaction);
            Assert.True(first > 0);
            await repo.InsertCaptureLineAsync(Line(first, "AAA", 2), transaction);
            transaction.Commit();
        }

        using (var connection = CreateContext().CreateConnection())
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            await Assert.ThrowsAnyAsync<Exception>(() =>
                repo.InsertCaptureAsync(Insert(captureA, Guid.NewGuid(), companyId: 7, counter: 1), transaction));
        }

        using (var connection = CreateContext().CreateConnection())
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var otherTenant = await repo.InsertCaptureAsync(
                Insert(Guid.NewGuid(), Guid.NewGuid(), companyId: 8, counter: 2), transaction);
            Assert.True(otherTenant > 0);
            Assert.Null(await repo.GetCaptureByGuidAsync(captureA, 8, transaction));
            transaction.Commit();
        }
    }

    [Fact]
    public async Task VerificationConcurrency_AllowsOnlyOneWinner()
    {
        if (!_dockerAvailable) return;
        var repo = Repository();
        using var connection = CreateContext().CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        var captureGuid = Guid.NewGuid();
        await repo.InsertCaptureAsync(Insert(captureGuid, Guid.NewGuid(), 7, 1), transaction);
        transaction.Commit();

        using var one = CreateContext().CreateConnection();
        one.Open();
        using var tx1 = one.BeginTransaction();
        using var two = CreateContext().CreateConnection();
        two.Open();
        using var tx2 = two.BeginTransaction();

        var first = await repo.TryVerifyAsync(captureGuid, 7, 9, PhysicalInventoryStatuses.Verified, "a", 1, tx1);
        var second = await repo.TryVerifyAsync(captureGuid, 7, 10, PhysicalInventoryStatuses.Verified, "b", 1, tx2);
        if (first) tx1.Commit(); else tx1.Rollback();
        if (second) tx2.Commit(); else tx2.Rollback();
        Assert.True(first ^ second);
    }

    private PhysicalInventoryRepository Repository() =>
        new(CreateContext(), new PostgreSqlDialect());

    private RepositoryContext CreateContext() =>
        new(Options.Create(new DatabaseOptions
        {
            Provider = DatabaseProvider.PostgreSql,
            ConnectionString = _connectionString,
            AuditConnectionString = _connectionString
        }));

    private static PhysicalInventoryCaptureInsert Insert(Guid capture, Guid operation, long companyId, long counter) =>
        new(capture, operation, companyId, 1, Guid.NewGuid(), "SO-1", 1, null, null, counter, Guid.NewGuid(),
            PhysicalInventoryStatuses.UploadedPendingVerification, "hash", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, counter);

    private static PhysicalInventoryCaptureLineInsert Line(long captureId, string barcode, decimal qty) =>
        new(Guid.NewGuid(), captureId, barcode, qty, 1, "Manual", null, null, null, "Unresolved",
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, 1);
}
