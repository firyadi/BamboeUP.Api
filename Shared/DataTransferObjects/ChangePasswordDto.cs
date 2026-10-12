namespace Shared.DataTransferObjects
{
    public record ChangePasswordDto
    {
        public string CurrentPassword { get; init; } = string.Empty;
        public string NewPassword { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
    }
}
