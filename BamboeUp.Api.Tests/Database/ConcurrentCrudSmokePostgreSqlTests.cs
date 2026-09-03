using Entities.Models;
using Microsoft.Extensions.Options;
using Npgsql;
using Dapper;
using Repository;
using Repository.Database;
using Shared.Settings;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace BamboeUp.Api.Tests.Database;

/// <summary>
/// Concurrent CRUD smoke on PostgreSQL (Hospital). Skips without Docker.
/// </summary>
[Trait("Category", "Integration")]
[Trait("Category", "Load")]
public class ConcurrentCrudSmokePostgreSqlTests : IAsyncLifetime
{
    private const int ParallelWorkers = 12;
    private PostgreSqlContainer? _container;
    private string _connectionString = string.Empty;
    private bool _dockerAvailable;
    private readonly ITestOutputHelper _output;

    public ConcurrentCrudSmokePostgreSqlTests(ITestOutputHelper output) => _output = output;

    public async Task InitializeAsync()
    {
        try
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .Build();

            await _container.StartAsync();
            _connectionString = _container.GetConnectionString();

            var initPath = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory, "..", "..", "..", "..",
                "Database", "PostgreSQL", "init-pilot.sql"));

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(await File.ReadAllTextAsync(initPath));
            _dockerAvailable = true;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            _dockerAvailable = false;
            _output.WriteLine($"Load smoke skipped — Docker unavailable: {ex.GetType().Name}");
        }
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync();
    }

    private RepositoryContext CreateContext() =>
        new(Options.Create(new DatabaseOptions
        {
            Provider = DatabaseProvider.PostgreSql,
            ConnectionString = _connectionString,
            AuditConnectionString = _connectionString
        }));

    [Fact]
    public async Task Hospital_Parallel_CreateGetDelete_NoFailures()
    {
        if (!_dockerAvailable) return;

        var dialect = new PostgreSqlDialect();
        var errors = new System.Collections.Concurrent.ConcurrentBag<string>();

        await Parallel.ForEachAsync(
            Enumerable.Range(0, ParallelWorkers),
            new ParallelOptions { MaxDegreeOfParallelism = ParallelWorkers },
            async (i, _) =>
            {
                try
                {
                    var repo = new HospitalRepository(CreateContext(), dialect);
                    var guid = Guid.NewGuid();
                    var hospital = new Hospital
                    {
                        HospitalGuid = guid,
                        HospitalName = $"Load Hospital {i}",
                        HospitalCode = $"L{i:D2}{guid.ToString()[..6]}",
                        StatusId = 1,
                        CreatedById = 1,
                        CreatedTime = DateTime.UtcNow
                    };

                    await repo.CreateHospitalAsync(hospital);
                    var fetched = await repo.GetHospitalAsync(guid, false);
                    if (fetched is null)
                        errors.Add($"Get returned null for {guid}");
                    await repo.DeleteHospitalAsync(guid);
                }
                catch (Exception ex)
                {
                    errors.Add($"{i}: {ex.GetType().Name}: {ex.Message}");
                }
            });

        Assert.True(errors.IsEmpty, string.Join(Environment.NewLine, errors));
        _output.WriteLine($"Completed {ParallelWorkers} parallel Hospital CRUD cycles with no errors.");
    }
}
