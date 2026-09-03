using System.Diagnostics;
using Dapper;
using Entities.Models;
using Microsoft.Extensions.Options;
using Npgsql;
using Repository;
using Repository.Database;
using Shared.Settings;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace BamboeUp.Api.Tests.Database;

/// <summary>
/// Smoke benchmarks for Person list + Employee search on PostgreSQL.
/// Skips when Docker/Testcontainers is unavailable. Not a hard SLA gate.
/// </summary>
[Trait("Category", "Integration")]
[Trait("Category", "Benchmark")]
public class DualDatabaseBenchmarkPostgreSqlTests : IAsyncLifetime
{
    private const int SeedCount = 40;
    private PostgreSqlContainer? _container;
    private string _connectionString = string.Empty;
    private bool _dockerAvailable;
    private readonly ITestOutputHelper _output;

    public DualDatabaseBenchmarkPostgreSqlTests(ITestOutputHelper output) => _output = output;

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
            _output.WriteLine($"Benchmark skipped — Docker unavailable: {ex.GetType().Name}");
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
    public async Task PersonList_And_EmployeeSearch_CompleteUnderSoftBudget()
    {
        if (!_dockerAvailable) return;

        var dialect = new PostgreSqlDialect();
        var context = CreateContext();
        var personRepo = new PersonRepository(context, null!, dialect);
        var employeeRepo = new EmployeeRepository(context, null!, dialect);

        await using var connection = new NpgsqlConnection(_connectionString);
        var genderId = await connection.ExecuteScalarAsync<long>(
            @"SELECT i.""StandardReferenceItemId"" FROM app.""StandardReferenceItem"" i
              INNER JOIN app.""StandardReference"" r ON r.""StandardReferenceId"" = i.""StandardReferenceId""
              WHERE r.""StandardReferenceInitial"" = 'Gender' LIMIT 1");
        var religionId = await connection.ExecuteScalarAsync<long>(
            @"SELECT i.""StandardReferenceItemId"" FROM app.""StandardReferenceItem"" i
              INNER JOIN app.""StandardReference"" r ON r.""StandardReferenceId"" = i.""StandardReferenceId""
              WHERE r.""StandardReferenceInitial"" = 'Religion' LIMIT 1");
        var employmentStatusId = await connection.ExecuteScalarAsync<long>(
            @"SELECT i.""StandardReferenceItemId"" FROM app.""StandardReferenceItem"" i
              INNER JOIN app.""StandardReference"" r ON r.""StandardReferenceId"" = i.""StandardReferenceId""
              WHERE r.""StandardReferenceInitial"" = 'EmploymentStatus' LIMIT 1");
        var employmentTypeId = await connection.ExecuteScalarAsync<long>(
            @"SELECT i.""StandardReferenceItemId"" FROM app.""StandardReferenceItem"" i
              INNER JOIN app.""StandardReference"" r ON r.""StandardReferenceId"" = i.""StandardReferenceId""
              WHERE r.""StandardReferenceInitial"" = 'EmploymentType' LIMIT 1");

        var personGuids = new List<Guid>(SeedCount);
        var employeeGuids = new List<Guid>(SeedCount);

        for (var i = 0; i < SeedCount; i++)
        {
            var personGuid = Guid.NewGuid();
            var person = new Person
            {
                PersonGuid = personGuid,
                FirstName = $"Bench{i:D3}",
                LastName = "Person",
                BirthDate = new DateTime(1990, 1, 1),
                SrGender = genderId,
                SrReligion = religionId,
                StatusId = 1,
                CreatedById = 1,
                CreatedTime = DateTime.UtcNow
            };
            await personRepo.CreatePersonAsync(person);
            personGuids.Add(personGuid);

            var employeeGuid = Guid.NewGuid();
            await employeeRepo.CreateEmployeeAsync(new Employee
            {
                EmployeeGuid = employeeGuid,
                PersonId = person.PersonId,
                EmployeeNo = $"B{i:D4}{employeeGuid.ToString()[..4]}",
                HireDate = new DateTime(2020, 1, 1),
                SrEmploymentStatus = employmentStatusId,
                SrEmploymentType = employmentTypeId,
                StatusId = 1,
                CreatedById = 1,
                CreatedTime = DateTime.UtcNow
            });
            employeeGuids.Add(employeeGuid);
        }

        var listSw = Stopwatch.StartNew();
        var people = (await personRepo.GetAllPeopleAsync(false)).ToList();
        listSw.Stop();
        _output.WriteLine($"PostgreSQL Person GetAllPeople: {listSw.ElapsedMilliseconds} ms ({people.Count} rows)");

        var searchSw = Stopwatch.StartNew();
        var employees = (await employeeRepo.SearchEmployeeAsync(
            "B00", "startswith",
            null, null, null, null, null, null, null, null)).ToList();
        searchSw.Stop();
        _output.WriteLine($"PostgreSQL Employee SearchEmployee: {searchSw.ElapsedMilliseconds} ms ({employees.Count} rows)");

        // Soft budgets — smoke only; tighten once staging baselines exist.
        Assert.True(listSw.Elapsed < TimeSpan.FromSeconds(5),
            $"Person list took {listSw.Elapsed}; expected under 5s for {SeedCount} seed rows.");
        Assert.True(searchSw.Elapsed < TimeSpan.FromSeconds(5),
            $"Employee search took {searchSw.Elapsed}; expected under 5s.");

        foreach (var g in employeeGuids)
            await employeeRepo.DeleteEmployeeAsync(g);
        foreach (var g in personGuids)
            await personRepo.DeletePersonAsync(g);
    }
}
