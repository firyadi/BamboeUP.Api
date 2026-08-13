using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects
{
    public partial class PersonQuickOnboardDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public string? PreTitle { get; set; }

        public string? PostTitle { get; set; }

        public string? BirthName { get; set; }

        public string? PlaceofBirth { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public string? NationalIdNo { get; set; }

        [Required]
        public long SrGender { get; set; }

        [Required]
        public long SrReligion { get; set; }

        public long? SrSalutation { get; set; }

        public long? SrBloodType { get; set; }

        public long? SrMaritalStatus { get; set; }

        public long? FileStorageId { get; set; }

        [Required]
        public PersonAddressForCreationDto Address { get; set; } = null!;

        [Required]
        public PersonContactForCreationDto Contact { get; set; } = null!;

        [Required]
        public PersonEducationForCreationDto Education { get; set; } = null!;

        [Required]
        public PersonEmergencyContactForCreationDto EmergencyContact { get; set; } = null!;

        [Required]
        public PersonIdentificationForCreationDto Identification { get; set; } = null!;
    }
}
