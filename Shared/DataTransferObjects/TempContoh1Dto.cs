using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record TempContoh1Dto
    {
        public long TempContoh1Id { get; set; }
        public Guid TempContoh1Guid { get; init; }
        
        public long PersonId { get; set; }
        public long? SrAwardType { get; set; }
public string? PersonName { get; set; }
public string? AwardTypeName { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record TempContoh1ForCreationDto
    {
        public long PersonId { get; set; }
        public long? SrAwardType { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record TempContoh1ForUpdateDto
    {
        public long PersonId { get; set; }
        public long? SrAwardType { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record TempContoh1ForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class TempContoh1SearchDto
    {

public string? PersonName { get; set; }
public string? AwardTypeName { get; set; }
public SearchType PersonNameSearchType { get; set; } = SearchType.Contains;
public SearchType AwardTypeNameSearchType { get; set; } = SearchType.Contains;

    }
}
