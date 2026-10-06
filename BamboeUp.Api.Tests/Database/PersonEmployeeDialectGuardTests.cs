using Repository.Database;
using Repository.Extensions;

namespace BamboeUp.Api.Tests.Database;

public class PersonEmployeeDialectGuardTests
{
    private static readonly string[] ClusterFiles =
    [
        "PersonRepository.cs",
        "PersonAddressRepository.cs",
        "PersonContactRepository.cs",
        "PersonEducationRepository.cs",
        "PersonFamilyRepository.cs",
        "PersonIdentificationRepository.cs",
        "PersonEmergencyContactRepository.cs",
        "PersonBankAccountRepository.cs",
        "PersonPhysicalCharacteristicRepository.cs",
        "EmployeeRepository.cs",
        "EmployeeAssignmentRepository.cs",
        "EmployeeCareerRepository.cs",
        "EmployeeSkillRepository.cs",
        "EmployeeLanguageRepository.cs",
        "EmployeeContractRepository.cs"
    ];

    private static string RepositoryDir => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "Repository"));

    [Fact]
    public void PersonEmployeeRepos_DoNotCallStandardReferenceUdf()
    {
        foreach (var name in ClusterFiles)
        {
            var path = Path.Combine(RepositoryDir, name);
            Assert.True(File.Exists(path), path);
            var source = File.ReadAllText(path);
            Assert.DoesNotContain("fn_GetStandardReferenceItems", source);
            Assert.DoesNotContain("OUTER APPLY", source);
            Assert.Contains("ISqlDialect dialect", source);
        }
    }

    [Fact]
    public void PersonAddress_JoinsCoreProvince()
    {
        var path = Path.Combine(RepositoryDir, "PersonAddressRepository.cs");
        var source = File.ReadAllText(path);
        Assert.Contains("Qualify(\"core\", \"Province\")", source);
        Assert.DoesNotContain("Qualify(\"core\", \"rovince\")", source);
    }

    [Fact]
    public void StandardReferenceLookup_IsSharedByBothProviders()
    {
        var sqlServer = SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(
            new SqlServerDialect(), "v_SrGender", "SrGender", "Gender");
        var postgres = SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(
            new PostgreSqlDialect(), "v_SrGender", "SrGender", "Gender");

        Assert.Contains("LEFT JOIN", sqlServer);
        Assert.Contains("LEFT JOIN", postgres);
        Assert.Contains("[app].[StandardReferenceItem]", sqlServer);
        Assert.Contains("\"app\".\"StandardReferenceItem\"", postgres);
    }
}
