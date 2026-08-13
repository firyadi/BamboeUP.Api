using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class PersonService
    {
        public async Task<PersonDto> OnboardPersonAsync(PersonQuickOnboardDto input)
        {
            await transactionManager.BeginTransactionAsync();
            var transaction = transactionManager.GetTransaction();
            try
            {
                var model = input.Adapt<Person>();
                model.StatusId = 1;
                model.CreatedTime = DateTime.UtcNow;
                await repository.Person.CreatePersonAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "Person",
                        EntityKey = model.PersonGuid.ToString(),
                        EntityDisplayName = model.FirstName,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                };

                // 2. Insert PersonAddress
                var address = input.Address.Adapt<PersonAddress>();
                address.PersonId = model.PersonId;
                address.StatusId = 1;
                address.CreatedTime = DateTime.UtcNow;
                await repository.PersonAddress.CreatePersonAddressAsync(address, transaction);
                entries.Add(new AuditLogEntry
                {
                    TableName = "PersonAddress",
                    EntityKey = address.PersonAddressGuid.ToString(),
                    EntityDisplayName = address.Address,
                    ParentTableName = "Person",
                    ParentEntityKey = model.PersonGuid.ToString(),
                    ActionType = "CREATE", OldEntity = null,
                    NewEntity = address
                });

                // 3. Insert PersonContact
                var contact = input.Contact.Adapt<PersonContact>();
                contact.PersonId = model.PersonId;
                contact.StatusId = 1;
                contact.CreatedTime = DateTime.UtcNow;
                await repository.PersonContact.CreatePersonContactAsync(contact, transaction);
                entries.Add(new AuditLogEntry
                {
                    TableName = "PersonContact",
                    EntityKey = contact.PersonContactGuid.ToString(),
                    EntityDisplayName = contact.ContactValue,
                    ParentTableName = "Person",
                    ParentEntityKey = model.PersonGuid.ToString(),
                    ActionType = "CREATE", OldEntity = null,
                    NewEntity = contact
                });

                // 4. Insert PersonEducation
                var education = input.Education.Adapt<PersonEducation>();
                education.PersonId = model.PersonId;
                education.StatusId = 1;
                education.CreatedTime = DateTime.UtcNow;
                await repository.PersonEducation.CreatePersonEducationAsync(education, transaction);
                entries.Add(new AuditLogEntry
                {
                    TableName = "PersonEducation",
                    EntityKey = education.PersonEducationGuid.ToString(),
                    EntityDisplayName = education.InstitutionName,
                    ParentTableName = "Person",
                    ParentEntityKey = model.PersonGuid.ToString(),
                    ActionType = "CREATE", OldEntity = null,
                    NewEntity = education
                });

                // 5. Insert PersonEmergencyContact
                var emergencyContact = input.EmergencyContact.Adapt<PersonEmergencyContact>();
                emergencyContact.PersonId = model.PersonId;
                emergencyContact.StatusId = 1;
                emergencyContact.CreatedTime = DateTime.UtcNow;
                await repository.PersonEmergencyContact.CreatePersonEmergencyContactAsync(emergencyContact, transaction);
                entries.Add(new AuditLogEntry
                {
                    TableName = "PersonEmergencyContact",
                    EntityKey = emergencyContact.PersonEmergencyContactGuid.ToString(),
                    EntityDisplayName = emergencyContact.ContactName,
                    ParentTableName = "Person",
                    ParentEntityKey = model.PersonGuid.ToString(),
                    ActionType = "CREATE", OldEntity = null,
                    NewEntity = emergencyContact
                });

                // 6. Insert PersonIdentification
                var identification = input.Identification.Adapt<PersonIdentification>();
                identification.PersonId = model.PersonId;
                identification.StatusId = 1;
                identification.CreatedTime = DateTime.UtcNow;
                await repository.PersonIdentification.CreatePersonIdentificationAsync(identification, transaction);
                entries.Add(new AuditLogEntry
                {
                    TableName = "PersonIdentification",
                    EntityKey = identification.PersonIdentificationGuid.ToString(),
                    EntityDisplayName = identification.IdentificationValue,
                    ParentTableName = "Person",
                    ParentEntityKey = model.PersonGuid.ToString(),
                    ActionType = "CREATE", OldEntity = null,
                    NewEntity = identification
                });

                await audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "CREATE",
                    RootTableName = "Person",
                    RootEntityKey = model.PersonGuid.ToString(),
                    RootDisplayName = model.FirstName,
                    UserId = userContext.UserGuid != Guid.Empty
                        ? userContext.UserGuid.ToString()
                        : model.CreatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();

                var resultDto = model.Adapt<PersonDto>();
                resultDto.PersonAddresses = new List<PersonAddressDto> { address.Adapt<PersonAddressDto>() };
                resultDto.PersonContacts = new List<PersonContactDto> { contact.Adapt<PersonContactDto>() };
                resultDto.PersonEducations = new List<PersonEducationDto> { education.Adapt<PersonEducationDto>() };
                resultDto.PersonEmergencyContacts = new List<PersonEmergencyContactDto> { emergencyContact.Adapt<PersonEmergencyContactDto>() };
                resultDto.PersonIdentifications = new List<PersonIdentificationDto> { identification.Adapt<PersonIdentificationDto>() };
                return resultDto;
            }
            catch (Exception)
            {
                await transactionManager.RollbackAsync();
                throw;
            }
        }
    }
}
