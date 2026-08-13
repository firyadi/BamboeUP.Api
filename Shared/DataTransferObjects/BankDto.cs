using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record BankDto
    {
        public long BankId { get; set; }
        public Guid BankGuid { get; init; }
        public string BankName { get; set; } = string.Empty;
        public string BankInitial { get; set; } = string.Empty;
        public int StatusId { get; init; }
        public byte[]? RowVersion { get; init; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record BankForCreationDto
    {
        public string BankName { get; set; } = string.Empty;
        public string BankInitial { get; set; } = string.Empty;
        public long CreatedById { get; init; } = 0;
        public int StatusId { get; init; } = 1;
    }

    public record BankForUpdateDto
    {
        public string BankName { get; set; } = string.Empty;
        public string BankInitial { get; set; } = string.Empty;
        public long UpdatedById { get; init; }
        public int StatusId { get; init; }
        public byte[]? RowVersion { get; init; }
    }

    public record BankForDeleteDto
    {
        public long DeletedById { get; init; }

    }

    public class BankSearchDto
    {
        public string? BankName { get; set; }
        public SearchType BankNameSearchType { get; set; } = SearchType.Contains;

        public string? BankInitial { get; set; }
        public SearchType BankInitialSearchType { get; set; } = SearchType.Contains;
    }
}
