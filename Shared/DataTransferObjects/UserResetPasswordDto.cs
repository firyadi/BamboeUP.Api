namespace Shared.DataTransferObjects
{
    public record UserResetPasswordDto
    {
        public string UserName { get; init; } = string.Empty;
        public string NewPassword { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
    }
}
