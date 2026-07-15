using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonRepository
    {
        Task<Person?> GetPersonAsync(Guid personGuid, bool trackChanges);
        Task<Person?> GetPersonByIdAsync(long personId, bool trackChanges);
        Task<IEnumerable<Person>> GetAllPeopleAsync(bool trackChanges);

        Task CreatePersonAsync(Person person, IDbTransaction? transaction = null);
        Task UpdatePersonAsync(Person person, IDbTransaction? transaction = null);
        Task DeletePersonAsync(Guid personGuid, IDbTransaction? transaction = null);
        Task SoftDeletePersonAsync(Person person, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<Person>> SearchPersonAsync(
            string? firstName,
            string? firstNameSearchType,
            string? middleName,
            string? middleNameSearchType,
            string? lastName,
            string? lastNameSearchType,
            string? preTitle,
            string? preTitleSearchType,
            string? postTitle,
            string? postTitleSearchType,
            string? birthName,
            string? birthNameSearchType,
            string? placeofBirth,
            string? placeofBirthSearchType,
            string? birthDate,
            string? birthDateSearchType,
            string? nationalIdNo,
            string? nationalIdNoSearchType,
            string? srGender,
            string? srGenderSearchType,
            string? srReligion,
            string? srReligionSearchType,
            string? srSalutation,
            string? srSalutationSearchType,
            string? srBloodType,
            string? srBloodTypeSearchType,
            string? srMaritalStatus,
            string? srMaritalStatusSearchType,
            string? photo,
            string? photoSearchType,
            IDbTransaction? transaction = null);
    }
}
