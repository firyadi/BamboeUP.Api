using System;
using System.Collections.Generic;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record PersonDto
    {
        public long PersonId { get; set; }
        public Guid PersonGuid { get; init; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PreTitle { get; set; }
        public string? PostTitle { get; set; }
        public string? BirthName { get; set; }
        public string? PlaceofBirth { get; set; }
        public DateTime BirthDate { get; set; }
        public string? NationalIdNo { get; set; }
        public long SrGender { get; set; }
        public long SrReligion { get; set; }
        public long? SrSalutation { get; set; }
        public long? SrBloodType { get; set; }
        public long? SrMaritalStatus { get; set; }
        public string? Photo { get; set; }

                public IEnumerable<PersonIdentificationDto>? PersonIdentifications { get; set; }
                public IEnumerable<PersonEducationDto>? PersonEducations { get; set; }
                public IEnumerable<PersonEmergencyContactDto>? PersonEmergencyContacts { get; set; }
        // ##HeaderDetailCollections##
        public IEnumerable<PersonAddressDto>? PersonAddresses { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record PersonForCreationDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PreTitle { get; set; }
        public string? PostTitle { get; set; }
        public string? BirthName { get; set; }
        public string? PlaceofBirth { get; set; }
        public DateTime BirthDate { get; set; }
        public string? NationalIdNo { get; set; }
        public long SrGender { get; set; }
        public long SrReligion { get; set; }
        public long? SrSalutation { get; set; }
        public long? SrBloodType { get; set; }
        public long? SrMaritalStatus { get; set; }
        public string? Photo { get; set; }
                public IEnumerable<PersonIdentificationForCreationDto>? PersonIdentifications { get; set; }
                public IEnumerable<PersonEducationForCreationDto>? PersonEducations { get; set; }
                public IEnumerable<PersonEmergencyContactForCreationDto>? PersonEmergencyContacts { get; set; }
        // ##HeaderDetailCreateCollections##
        public IEnumerable<PersonAddressForCreationDto>? PersonAddresses { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record PersonForUpdateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PreTitle { get; set; }
        public string? PostTitle { get; set; }
        public string? BirthName { get; set; }
        public string? PlaceofBirth { get; set; }
        public DateTime BirthDate { get; set; }
        public string? NationalIdNo { get; set; }
        public long SrGender { get; set; }
        public long SrReligion { get; set; }
        public long? SrSalutation { get; set; }
        public long? SrBloodType { get; set; }
        public long? SrMaritalStatus { get; set; }
        public string? Photo { get; set; }
                public IEnumerable<PersonIdentificationForUpdateDto>? PersonIdentifications { get; set; }
                public IEnumerable<PersonEducationForUpdateDto>? PersonEducations { get; set; }
                public IEnumerable<PersonEmergencyContactForUpdateDto>? PersonEmergencyContacts { get; set; }
        // ##HeaderDetailUpdateCollections##
        public IEnumerable<PersonAddressForUpdateDto>? PersonAddresses { get; set; }
        public long UpdatedById { get; set; }
    }

    public record PersonForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class PersonSearchDto
    {
        public string? FirstName { get; set; }
        public SearchType FirstNameSearchType { get; set; } = SearchType.Contains;

        public string? MiddleName { get; set; }
        public SearchType MiddleNameSearchType { get; set; } = SearchType.Contains;

        public string? LastName { get; set; }
        public SearchType LastNameSearchType { get; set; } = SearchType.Contains;

        public string? PreTitle { get; set; }
        public SearchType PreTitleSearchType { get; set; } = SearchType.Contains;

        public string? PostTitle { get; set; }
        public SearchType PostTitleSearchType { get; set; } = SearchType.Contains;

        public string? BirthName { get; set; }
        public SearchType BirthNameSearchType { get; set; } = SearchType.Contains;

        public string? PlaceofBirth { get; set; }
        public SearchType PlaceofBirthSearchType { get; set; } = SearchType.Contains;

        public string? BirthDate { get; set; }
        public SearchType BirthDateSearchType { get; set; } = SearchType.Contains;

        public string? NationalIdNo { get; set; }
        public SearchType NationalIdNoSearchType { get; set; } = SearchType.Contains;

        public string? SrGender { get; set; }
        public SearchType SrGenderSearchType { get; set; } = SearchType.Contains;

        public string? SrReligion { get; set; }
        public SearchType SrReligionSearchType { get; set; } = SearchType.Contains;

        public string? SrSalutation { get; set; }
        public SearchType SrSalutationSearchType { get; set; } = SearchType.Contains;

        public string? SrBloodType { get; set; }
        public SearchType SrBloodTypeSearchType { get; set; } = SearchType.Contains;

        public string? SrMaritalStatus { get; set; }
        public SearchType SrMaritalStatusSearchType { get; set; } = SearchType.Contains;

        public string? Photo { get; set; }
        public SearchType PhotoSearchType { get; set; } = SearchType.Contains;

    }
}
