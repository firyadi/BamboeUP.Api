namespace Shared.DataTransferObjects;

/// <summary>Preview scope options before completing login (credentials validated, no token issued).</summary>
public class LoginScopesPreviewDto
{
    public bool IsAdmin { get; set; }
    public long? DefaultCompanyId { get; set; }
    public long? DefaultOfficeId { get; set; }
    public List<LoginScopeDto> AvailableScopes { get; set; } = new();
}
