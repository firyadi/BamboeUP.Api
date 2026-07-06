namespace Shared.DataTransferObjects;

public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public long? CompanyId { get; set; }
    public long? OfficeId { get; set; }
}
