namespace Shared.DataTransferObjects
{
    public class AuthResultDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public required LoginContext Context { get; set; }
    }
}
