using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record UserDto
    {
        public long UserId { get; set; }
        public Guid UserGuid { get; init; }
        public string UserName { get; set; } = string.Empty;
        // PasswordHash sengaja tidak disertakan di response DTO — tidak boleh dikirim ke client
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record UserForCreationDto
    {
        public string UserName { get; set; } = string.Empty;
        /// <summary>Plain-text password. Will be salted and hashed server-side before storage.</summary>
        public string PasswordHash { get; set; } = string.Empty;
        // NOTE: PasswordSalt intentionally omitted — always generated server-side
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long CreatedById { get; set; }
    }

    public record UserForUpdateDto
    {
        public string UserName { get; set; } = string.Empty;
        /// <summary>Plain-text password. Leave null/empty to keep existing password unchanged. Will be salted and hashed server-side.</summary>
        public string? PasswordHash { get; set; }
        // NOTE: PasswordSalt intentionally omitted — always generated server-side
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long? UpdatedById { get; set; }
    }

    public record UserForDeleteDto
    {
        public int? DeletedById { get; set; }
    }

    public class UserSearchDto
    {
        public string? UserName { get; set; }
        public SearchType UserNameSearchType { get; set; } = SearchType.Contains;
        // PasswordHash sengaja dihapus dari search — tidak boleh search berdasarkan hash
        public string? FullName { get; set; }
        public SearchType FullNameSearchType { get; set; } = SearchType.Contains;
        public string? Email { get; set; }
        public SearchType EmailSearchType { get; set; } = SearchType.Contains;
    }
}
