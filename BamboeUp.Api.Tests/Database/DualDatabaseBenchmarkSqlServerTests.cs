using System.Diagnostics;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Repository;
using Repository.Database;
using Shared.Settings;
using Xunit.Abstractions;

namespace BamboeUp.Api.Tests.Database;

/// <summary>
/// Smoke benchmarks for Person list + Employee search on SQL Server.
/// Skips when Dev.BamboeHR is unavailable. Not a hard SLA gate.
/// </summary>
[Trait("Category", "Integration")]
[Trait("Category", "Benchmark")]
[Trait("Provider", "SqlServer")]
public class DualDatabaseBenchmarkSqlServerTests
{
    private const string ConnectionString =
        "Server=localhost;Database=Dev.BamboeHR;User Id=sa;Password=123;Encrypt=False;TrustServerCertificate=True;";

    private static readonly bool SqlServerAvailable;
    private readonly ITestOutputHelper _output;

    static DualDatabaseBenchmarkSqlServerTests()
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            connection.ExecuteScalar<int>("SELECT TOP 1 1 FROM app.Person");
            SqlServerAvailable = true;
        }
        catch
        {
            SqlServerAvailable = false;
        }
    }

    public DualDatabaseBenchmarkSqlServerTests(ITestOutputHelper output) => _output = output;

    private static RepositoryContext CreateContext() =>
        new(Options.Create(new DatabaseOptions
        {
            Provider = DatabaseProvider.SqlServer,
            ConnectionString = ConnectionString,
            AuditConnectionString = ConnectionString
        }));

    [Fact]
    public async Task PersonList_And_EmployeeSearch_CompleteUnderSoftBudget()
    {
        if (!SqlServerAvailable)
        {
            _output.WriteLine("Benchmark skipped — SQL Server / app.Person unavailable.");
            return;
        }

        var dialect = new SqlServerDialect();
        var context = CreateContext();
        var personRepo = new PersonRepository(context, null!, dialect);
        var employeeRepo = new EmployeeRepository(context, null!, dialect);

        var listSw = Stopwatch.StartNew();
        var people = (await personRepo.GetAllPeopleAsync(false)).ToList();
        listSw.Stop();
        _output.WriteLine($"SQL Server Person GetAllPeople: {listSw.ElapsedMilliseconds} ms ({people.Count} rows)");

        var searchSw = Stopwatch.StartNew();
        var employees = (await employeeRepo.SearchEmployeeAsync(
            null, null,
            null, null, null, null, null, null, null, null)).ToList();
        searchSw.Stop();
        _output.WriteLine($"SQL Server Employee SearchEmployee: {searchSw.ElapsedMilliseconds} ms ({employees.Count} rows)");

        Assert.True(listSw.Elapsed < TimeSpan.FromSeconds(10),
            $"Person list took {listSw.Elapsed}; expected under 10s.");
        Assert.True(searchSw.Elapsed < TimeSpan.FromSeconds(10),
            $"Employee search took {searchSw.Elapsed}; expected under 10s.");
    }
}
