namespace Shared.Settings;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SqlServer;

    public string ConnectionString { get; set; } = string.Empty;

    public string AuditConnectionString { get; set; } = string.Empty;
}
