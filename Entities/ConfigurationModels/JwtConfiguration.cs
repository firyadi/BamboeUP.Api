namespace Entities.ConfigurationModels;

public class JwtConfiguration
{
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
    public int Expires { get; set; }
    public string SecretKey { get; set; } = string.Empty;
}
