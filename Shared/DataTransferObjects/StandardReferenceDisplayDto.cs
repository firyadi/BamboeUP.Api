namespace Shared.DataTransferObjects
{
    public record StandardReferenceDisplayDto
    {
        public long StandardReferenceItemId { get; init; }
        public long CompanyId { get; init; }
        public long CompanyOfficeId { get; init; }
        public long? StandardReferenceGroupId { get; init; }
        public string StandardReferenceInitial { get; init; } = string.Empty;
        public string StandardReferenceItemName { get; init; } = string.Empty;
    }
}
