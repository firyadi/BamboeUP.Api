namespace Shared.DataTransferObjects
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public LoginContext Context { get; set; }
    }
}