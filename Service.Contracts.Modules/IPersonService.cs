using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonService
    {
        Task<IEnumerable<PersonDto>> GetAllPeopleAsync(bool trackChanges);
        Task<PersonDto?> GetPersonByGuidAsync(Guid personGuid, bool trackChanges);
        Task<PersonDto> CreatePersonAsync(PersonForCreationDto input);
        Task UpdatePersonAsync(Guid personGuid, PersonForUpdateDto input, bool trackChanges);
        Task DeletePersonAsync(Guid personGuid, PersonForDeleteDto input, bool trackChanges);
        Task DeletePersonByAdminAsync(Guid personGuid, bool trackChanges);

        Task<IEnumerable<PersonDto>> SearchPersonAsync(
            string? firstName, string? firstNameSearchType, string? middleName, string? middleNameSearchType, string? lastName, string? lastNameSearchType, string? preTitle, string? preTitleSearchType, string? postTitle, string? postTitleSearchType, string? birthName, string? birthNameSearchType, string? placeofBirth, string? placeofBirthSearchType, string? birthDate, string? birthDateSearchType, string? nationalIdNo, string? nationalIdNoSearchType, string? srGender, string? srGenderSearchType, string? srReligion, string? srReligionSearchType, string? srSalutation, string? srSalutationSearchType, string? srBloodType, string? srBloodTypeSearchType, string? srMaritalStatus, string? srMaritalStatusSearchType, string? photo, string? photoSearchType

        );
    }
}
