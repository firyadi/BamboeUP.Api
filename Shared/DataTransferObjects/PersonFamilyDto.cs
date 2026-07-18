using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PersonFamilyDto
    {
        public long PersonFamilyId { get; set; }
        public Guid PersonFamilyGuid { get; init; }
        public long PersonId { get; set; }
        public long SrFamilyRelation { get; set; }
        public string FamilyName { get; set; } = string.Empty;
        public DateTime DateBirth { get; set; }
        public long? SrEducationLevel { get; set; }
        public string? Address { get; set; }
        public long? StateId { get; set; }
        public long? CityId { get; set; }
        public string? ZipCode { get; set; }
        public string? Phone { get; set; }
        public long? SrMaritalStatus { get; set; }
        public long? SrGender { get; set; }
public string? PersonName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record PersonFamilyForCreationDto
    {
        public long PersonId { get; set; }
        public long SrFamilyRelation { get; set; }
        public string FamilyName { get; set; } = string.Empty;
        public DateTime DateBirth { get; set; }
        public long? SrEducationLevel { get; set; }
        public string? Address { get; set; }
        public long? StateId { get; set; }
        public long? CityId { get; set; }
        public string? ZipCode { get; set; }
        public string? Phone { get; set; }
        public long? SrMaritalStatus { get; set; }
        public long? SrGender { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record PersonFamilyForUpdateDto
    {
        public Guid PersonFamilyGuid { get; set; }
        public long PersonId { get; set; }
        public long SrFamilyRelation { get; set; }
        public string FamilyName { get; set; } = string.Empty;
        public DateTime DateBirth { get; set; }
        public long? SrEducationLevel { get; set; }
        public string? Address { get; set; }
        public long? StateId { get; set; }
        public long? CityId { get; set; }
        public string? ZipCode { get; set; }
        public string? Phone { get; set; }
        public long? SrMaritalStatus { get; set; }
        public long? SrGender { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record PersonFamilyForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PersonFamilySearchDto
    {
        public string? SrFamilyRelation { get; set; }
        public SearchType SrFamilyRelationSearchType { get; set; } = SearchType.Contains;

        public string? FamilyName { get; set; }
        public SearchType FamilyNameSearchType { get; set; } = SearchType.Contains;

        public string? DateBirth { get; set; }
        public SearchType DateBirthSearchType { get; set; } = SearchType.Contains;

        public string? SrEducationLevel { get; set; }
        public SearchType SrEducationLevelSearchType { get; set; } = SearchType.Contains;

        public string? Address { get; set; }
        public SearchType AddressSearchType { get; set; } = SearchType.Contains;

        public string? StateId { get; set; }
        public SearchType StateIdSearchType { get; set; } = SearchType.Contains;

        public string? CityId { get; set; }
        public SearchType CityIdSearchType { get; set; } = SearchType.Contains;

        public string? ZipCode { get; set; }
        public SearchType ZipCodeSearchType { get; set; } = SearchType.Contains;

        public string? Phone { get; set; }
        public SearchType PhoneSearchType { get; set; } = SearchType.Contains;

        public string? SrMaritalStatus { get; set; }
        public SearchType SrMaritalStatusSearchType { get; set; } = SearchType.Contains;

        public string? SrGender { get; set; }
        public SearchType SrGenderSearchType { get; set; } = SearchType.Contains;
    }
}
