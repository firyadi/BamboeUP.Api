namespace Shared.DataTransferObjects
{
    public class ApiErrorDto
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}
