using Dapper;
using Entities.Models;
using Microsoft.Extensions.Options;
using Npgsql;
using Repository;
using Repository.Database;
using Shared.Settings;
using Testcontainers.PostgreSql;
using Xunit;

namespace BamboeUp.Api.Tests.Database;

[Trait("Category", "Integration")]
public class PilotRepositoryPostgreSqlTests : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private string _connectionString = string.Empty;
    private bool _dockerAvailable;

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
                AppContext.BaseDirectory,
                "..", "..", "..", "..",
                "Database", "PostgreSQL", "init-pilot.sql"));

            var initSql = await File.ReadAllTextAsync(initPath);
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(initSql);
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

    private RepositoryContext CreateContext() =>
        new(Options.Create(new DatabaseOptions
        {
            Provider = DatabaseProvider.PostgreSql,
            ConnectionString = _connectionString,
            AuditConnectionString = _connectionString
        }));

    [Fact]
    public async Task HospitalRepository_CRUD_and_Search()
    {
        if (!_dockerAvailable) return;

        var repo = new HospitalRepository(CreateContext(), new PostgreSqlDialect());
        var guid = Guid.NewGuid();
        var hospital = new Hospital
        {
            HospitalGuid = guid,
            HospitalName = "RS Pilot PostgreSQL",
            HospitalCode = $"PG{guid.ToString()[..6]}",
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };

        await repo.CreateHospitalAsync(hospital);
        Assert.True(hospital.HospitalId > 0);

        var fetched = await repo.GetHospitalAsync(guid, false);
        Assert.NotNull(fetched);
        Assert.Equal(hospital.HospitalName, fetched!.HospitalName);

        var search = await repo.SearchHospitalAsync(
            "pilot", "contains",
            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        Assert.Contains(search, h => h.HospitalGuid == guid);

        await repo.DeleteHospitalAsync(guid);
        Assert.Null(await repo.GetHospitalAsync(guid, false));
    }

    [Fact]
    public async Task BankRepository_CRUD()
    {
        if (!_dockerAvailable) return;

        var repo = new BankRepository(CreateContext(), new PostgreSqlDialect());
        var guid = Guid.NewGuid();
        var bank = new Bank
        {
            BankGuid = guid,
            BankName = "Bank Pilot PG",
            BankInitial = "BPP",
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };

        await repo.CreateBankAsync(bank);

        var fetched = await repo.GetBankAsync(guid, false);
        Assert.NotNull(fetched);
        Assert.Equal("Bank Pilot PG", fetched!.BankName);

        await repo.DeleteBankAsync(guid);
    }

    [Fact]
    public async Task CertificationRepository_ReadsStandardReferenceNames()
    {
        if (!_dockerAvailable) return;

        await using var connection = new NpgsqlConnection(_connectionString);
        var categoryItemId = await connection.ExecuteScalarAsync<long>(
            @"SELECT i.""StandardReferenceItemId""
              FROM app.""StandardReferenceItem"" i
              INNER JOIN app.""StandardReference"" r ON r.""StandardReferenceId"" = i.""StandardReferenceId""
              WHERE r.""StandardReferenceInitial"" = 'CertificationCategory'
              LIMIT 1");
        var providerItemId = await connection.ExecuteScalarAsync<long>(
            @"SELECT i.""StandardReferenceItemId""
              FROM app.""StandardReferenceItem"" i
              INNER JOIN app.""StandardReference"" r ON r.""StandardReferenceId"" = i.""StandardReferenceId""
              WHERE r.""StandardReferenceInitial"" = 'CertificationProvider'
              LIMIT 1");

        var repo = new CertificationRepository(CreateContext(), new PostgreSqlDialect());
        var guid = Guid.NewGuid();
        var certification = new Certification
        {
            CertificationGuid = guid,
            CertificationCode = $"CERT-{guid.ToString()[..8]}",
            CertificationName = "Pilot Certification",
            SrCertificationCategory = categoryItemId,
            SrCertificationProvider = providerItemId,
            IsActive = true,
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };

        await repo.CreateCertificationAsync(certification);

        var fetched = await repo.GetCertificationAsync(guid, false);
        Assert.NotNull(fetched);
        Assert.Equal("Professional", fetched!.CertificationCategoryName);
        Assert.Equal("International Body", fetched.CertificationProviderName);

        await repo.DeleteCertificationAsync(guid);
    }

    [Fact]
    public async Task PersonRepository_CRUD_and_Search()
    {
        if (!_dockerAvailable) return;

        var dialect = new PostgreSqlDialect();
        var repo = new PersonRepository(CreateContext(), null!, dialect);
        var guid = Guid.NewGuid();
        var genderId = await GetStandardReferenceItemId("Gender");
        var religionId = await GetStandardReferenceItemId("Religion");

        var person = new Person
        {
            PersonGuid = guid,
            FirstName = "Pilot",
            LastName = "Person",
            BirthDate = new DateTime(1990, 1, 15),
            SrGender = genderId,
            SrReligion = religionId,
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };

        await repo.CreatePersonAsync(person);
        Assert.True(person.PersonId > 0);

        var fetched = await repo.GetPersonAsync(guid, false);
        Assert.NotNull(fetched);
        Assert.Equal("Pilot", fetched!.FirstName);
        Assert.Equal("Male", fetched.GenderName);

        var search = await repo.SearchPersonAsync(
            "Pilot", "contains",
            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null, null, null);
        Assert.Contains(search, p => p.PersonGuid == guid);

        await repo.DeletePersonAsync(guid);
        Assert.Null(await repo.GetPersonAsync(guid, false));
    }

    [Fact]
    public async Task EmployeeRepository_CRUD()
    {
        if (!_dockerAvailable) return;

        var dialect = new PostgreSqlDialect();
        var context = CreateContext();
        var personRepo = new PersonRepository(context, null!, dialect);
        var employeeRepo = new EmployeeRepository(context, null!, dialect);

        var personGuid = Guid.NewGuid();
        var employeeGuid = Guid.NewGuid();
        var person = new Person
        {
            PersonGuid = personGuid,
            FirstName = "Emp",
            LastName = "Pilot",
            BirthDate = new DateTime(1988, 6, 1),
            SrGender = await GetStandardReferenceItemId("Gender"),
            SrReligion = await GetStandardReferenceItemId("Religion"),
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };
        await personRepo.CreatePersonAsync(person);

        var employee = new Employee
        {
            EmployeeGuid = employeeGuid,
            PersonId = person.PersonId,
            EmployeeNo = $"PG{employeeGuid.ToString()[..8]}",
            HireDate = new DateTime(2020, 1, 1),
            SrEmploymentStatus = await GetStandardReferenceItemId("EmploymentStatus"),
            SrEmploymentType = await GetStandardReferenceItemId("EmploymentType"),
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };

        try
        {
            await employeeRepo.CreateEmployeeAsync(employee);
            Assert.True(employee.EmployeeId > 0);

            var fetched = await employeeRepo.GetEmployeeAsync(employeeGuid, false);
            Assert.NotNull(fetched);
            Assert.Equal(employee.EmployeeNo, fetched!.EmployeeNo);
            Assert.Equal("Active", fetched.EmploymentStatusName);
        }
        finally
        {
            await employeeRepo.DeleteEmployeeAsync(employeeGuid);
            await personRepo.DeletePersonAsync(personGuid);
        }
    }

    private async Task<long> GetStandardReferenceItemId(string referenceInitial)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<long>(
            @"SELECT i.""StandardReferenceItemId""
              FROM app.""StandardReferenceItem"" i
              INNER JOIN app.""StandardReference"" r ON r.""StandardReferenceId"" = i.""StandardReferenceId""
              WHERE r.""StandardReferenceInitial"" = @referenceInitial
              LIMIT 1",
            new { referenceInitial });
    }
}
