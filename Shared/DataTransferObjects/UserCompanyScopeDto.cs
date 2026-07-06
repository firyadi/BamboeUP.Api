namespace Shared.DataTransferObjects
{
    public record UserCompanyScopeDto
    {
        public long UserCompanyScopeId { get; set; }
        public Guid UserCompanyScopeGuid { get; init; }
        public long UserId { get; set; }
        public long CompanyId { get; set; }
        public string? CompanyName { get; set; }       // joined from app.Company
        public long? CompanyOfficeId { get; set; }
        public string? CompanyOfficeName { get; set; } // joined from app.CompanyOffice
        public bool IsDefaultCompany { get; set; }
        public bool IsDefaultOffice { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record UserCompanyScopeForCreationDto
    {
        public long UserId { get; set; }
        public long CompanyId { get; set; }
        /// <summary>NULL = akses semua office di company. NOT NULL = office spesifik.</summary>
        public long? CompanyOfficeId { get; set; }
        public bool IsDefaultCompany { get; set; } = false;
        public bool IsDefaultOffice { get; set; } = false;
        public long CreatedById { get; set; }
    }

    public record UserCompanyScopeForUpdateDto
    {
        public long? CompanyOfficeId { get; set; }
        public bool IsDefaultCompany { get; set; }
        public bool IsDefaultOffice { get; set; }
        public long UpdatedById { get; set; }
    }

    public record UserCompanyScopeForDeleteDto
    {
        public long? DeletedById { get; set; }
    }
}
