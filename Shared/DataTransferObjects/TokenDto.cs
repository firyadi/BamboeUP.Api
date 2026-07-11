namespace Shared.DataTransferObjects;

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expired { get; set; }

    // User info
    public long UserId { get; set; }
    public Guid UserGuid { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }

    // Active scope (company/office yang dipilih saat login)
    public long? ActiveCompanyId { get; set; }
    public Guid? ActiveCompanyGuid { get; set; }
    public string? ActiveCompanyName { get; set; }
    public long? ActiveOfficeId { get; set; }
    public Guid? ActiveOfficeGuid { get; set; }
    public string? ActiveOfficeName { get; set; }

    /// <summary>
    /// UserGroupScopeId dari scope yang aktif saat login/switch.
    /// Digunakan NavMenu untuk matching scope secara eksplisit tanpa ambiguitas.
    /// </summary>
    public long? ActiveUserGroupScopeId { get; set; }

    /// <summary>
    /// Daftar semua company/office yang bisa diakses user ini.
    /// Digunakan frontend untuk menampilkan dropdown saat switch scope.
    /// </summary>
    public List<LoginScopeDto> AvailableScopes { get; set; } = new();
}

/// <summary>Representasi satu scope (kombinasi company + office) yang dapat diakses user.</summary>
public class LoginScopeDto
{
    public long CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public long? OfficeId { get; set; }
    public string? OfficeName { get; set; }
    public long? UserGroupScopeId { get; set; }
    public string? UserGroupName { get; set; }
}
