using Repository.Database;
using Repository.Extensions;

namespace BamboeUp.Api.Tests.Database;

public class PlatformDialectGuardTests
{
    private static readonly string[] ClusterFiles =
    [
        "UserRepository.cs",
        "UserGroupRepository.cs",
        "ProgramRepository.cs",
        "ParameterRepository.cs",
        "StandardReferenceRepository.cs",
        "StandardReferenceItemDisplayRepository.cs",
        "AutoNumberGenerateRepository.cs",
        "Approval/ApprovalTemplateRepository.cs"
    ];

    private static string RepositoryDir => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "Repository"));

    [Fact]
    public void PlatformRepos_AreDialectAware()
    {
        foreach (var name in ClusterFiles)
        {
            var path = Path.Combine(RepositoryDir, name);
            Assert.True(File.Exists(path), path);
            var source = File.ReadAllText(path);
            Assert.Contains("ISqlDialect", source);
            Assert.DoesNotContain("fn_GetStandardReferenceItems", source);
            Assert.DoesNotContain("OUTPUT INSERTED", source);
        }
    }

    [Fact]
    public void AutoNumberGenerate_UsesPortableLockHints()
    {
        var path = Path.Combine(RepositoryDir, "AutoNumberGenerateRepository.cs");
        var source = File.ReadAllText(path);
        Assert.Contains("TableLockHint", source);
        Assert.Contains("ForUpdateSuffix", source);
        Assert.Contains("NewGuidExpression", source);
        Assert.DoesNotContain("WITH (UPDLOCK, ROWLOCK)", source);
        Assert.DoesNotContain("NEWID()", source);
    }
}
