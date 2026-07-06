namespace Shared.DataTransferObjects
{
    public record UserGroupScopeDto
    {
        public long UserGroupScopeId { get; set; }
        public Guid UserGroupScopeGuid { get; init; }
        public long UserId { get; set; }
        public Guid UserGuid { get; set; }
        public string? FullName { get; set; }          // joined from core.Users
        public string? UserName { get; set; }          // joined from core.Users
        public long UserGroupId { get; set; }
        public Guid UserGroupGuid { get; set; }
        public string? UserGroupName { get; set; }     // joined from core.UserGroup
        public long CompanyId { get; set; }
        public Guid CompanyGuid { get; set; }
        public string? CompanyName { get; set; }       // joined from app.Company
        public long? CompanyOfficeId { get; set; }
        public Guid? CompanyOfficeGuid { get; set; }
        public string? CompanyOfficeName { get; set; } // joined from app.CompanyOffice
        public bool IsDefault { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record UserGroupScopeForCreationDto
    {
        public long UserId { get; set; }
        public Guid UserGuid { get; set; }
        public long UserGroupId { get; set; }
        public Guid UserGroupGuid { get; set; }
        public long CompanyId { get; set; }
        public Guid CompanyGuid { get; set; }
        /// <summary>NULL = role berlaku semua office. NOT NULL = office spesifik.</summary>
        public long? CompanyOfficeId { get; set; }
        public Guid? CompanyOfficeGuid { get; set; }
        /// <summary>Jika true, semua scope default lain milik UserId ini akan di-unset secara atomik.</summary>
        public bool IsDefault { get; set; } = false;
        public long CreatedById { get; set; }
    }

    public record UserGroupScopeForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    /// <summary>Payload untuk endpoint PATCH set-default. Hanya membutuhkan UpdatedById untuk audit.</summary>
    public record UserGroupScopeSetDefaultDto
    {
        public long UpdatedById { get; set; }
    }
}
