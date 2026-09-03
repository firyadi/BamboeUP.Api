using Dapper;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Repository;
using Repository.Database;
using Shared.Settings;

namespace BamboeUp.Api.Tests.Database;

[Trait("Category", "Integration")]
[Trait("Provider", "SqlServer")]
public class PilotRepositorySqlServerTests
{
    private const string ConnectionString =
        "Server=localhost;Database=Dev.BamboeHR;User Id=sa;Password=123;Encrypt=False;TrustServerCertificate=True;";

    private static bool _sqlServerAvailable;

    static PilotRepositorySqlServerTests()
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            connection.ExecuteScalar<int>("SELECT TOP 1 1 FROM app.Hospital");
            _sqlServerAvailable = true;
        }
        catch
        {
            _sqlServerAvailable = false;
        }
    }

    private static RepositoryContext CreateContext() =>
        new(Options.Create(new DatabaseOptions
        {
            Provider = DatabaseProvider.SqlServer,
            ConnectionString = ConnectionString,
            AuditConnectionString = ConnectionString
        }));

    [Fact]
    public async Task HospitalRepository_CRUD_and_Search()
    {
        if (!_sqlServerAvailable) return;

        var repo = new HospitalRepository(CreateContext(), new SqlServerDialect());
        var guid = Guid.NewGuid();
        var code = $"TST{guid.ToString()[..6]}";
        var hospital = new Hospital
        {
            HospitalGuid = guid,
            HospitalName = $"Dialect Test Hospital {code}",
            HospitalCode = code,
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };

        try
        {
            await repo.CreateHospitalAsync(hospital);
            Assert.True(hospital.HospitalId > 0);

            var fetched = await repo.GetHospitalAsync(guid, false);
            Assert.NotNull(fetched);
            Assert.Equal(hospital.HospitalName, fetched!.HospitalName);

            var search = await repo.SearchHospitalAsync(
                "Dialect Test", "contains",
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            Assert.Contains(search, h => h.HospitalGuid == guid);
        }
        finally
        {
            await repo.DeleteHospitalAsync(guid);
        }
    }

    [Fact]
    public async Task BankRepository_CRUD()
    {
        if (!_sqlServerAvailable) return;

        var repo = new BankRepository(CreateContext(), new SqlServerDialect());
        var guid = Guid.NewGuid();
        var bank = new Bank
        {
            BankGuid = guid,
            BankName = "Dialect Test Bank",
            BankInitial = "DTB",
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };

        try
        {
            await repo.CreateBankAsync(bank);

            var fetched = await repo.GetBankAsync(guid, false);
            Assert.NotNull(fetched);
            Assert.Equal("Dialect Test Bank", fetched!.BankName);
        }
        finally
        {
            await repo.DeleteBankAsync(guid);
        }
    }

    [Fact]
    public async Task CertificationRepository_UsesOuterApplyOnSqlServer()
    {
        if (!_sqlServerAvailable) return;

        await using var connection = new SqlConnection(ConnectionString);
        var categoryItemId = await connection.ExecuteScalarAsync<long?>(@"
            SELECT TOP 1 i.StandardReferenceItemId
            FROM app.StandardReferenceItem i
            INNER JOIN app.StandardReference r ON r.StandardReferenceId = i.StandardReferenceId
            WHERE r.StandardReferenceInitial = 'CertificationCategory'");

        if (categoryItemId is null) return;

        var providerItemId = await connection.ExecuteScalarAsync<long?>(@"
            SELECT TOP 1 i.StandardReferenceItemId
            FROM app.StandardReferenceItem i
            INNER JOIN app.StandardReference r ON r.StandardReferenceId = i.StandardReferenceId
            WHERE r.StandardReferenceInitial = 'CertificationProvider'") ?? categoryItemId.Value;

        var repo = new CertificationRepository(CreateContext(), new SqlServerDialect());
        var guid = Guid.NewGuid();
        var certification = new Certification
        {
            CertificationGuid = guid,
            CertificationCode = $"DT-{guid.ToString()[..8]}",
            CertificationName = "Dialect Test Certification",
            SrCertificationCategory = categoryItemId.Value,
            SrCertificationProvider = providerItemId,
            IsActive = true,
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };

        try
        {
            await repo.CreateCertificationAsync(certification);

            var fetched = await repo.GetCertificationAsync(guid, false);
            Assert.NotNull(fetched);
            Assert.False(string.IsNullOrEmpty(fetched!.CertificationCategoryName));
        }
        finally
        {
            await repo.DeleteCertificationAsync(guid);
        }
    }

    [Fact]
    public async Task PersonRepository_CRUD()
    {
        if (!_sqlServerAvailable) return;

        var repo = new PersonRepository(CreateContext(), null!, new SqlServerDialect());
        var guid = Guid.NewGuid();
        var person = new Person
        {
            PersonGuid = guid,
            FirstName = "Dialect",
            LastName = "Person",
            BirthDate = new DateTime(1990, 1, 15),
            SrGender = 1,
            SrReligion = 1,
            StatusId = 1,
            CreatedById = 1,
            CreatedTime = DateTime.UtcNow
        };

        try
        {
            await repo.CreatePersonAsync(person);
            Assert.True(person.PersonId > 0);

            var fetched = await repo.GetPersonAsync(guid, false);
            Assert.NotNull(fetched);
            Assert.Equal("Dialect", fetched!.FirstName);
        }
        finally
        {
            await repo.DeletePersonAsync(guid);
        }
    }
}
