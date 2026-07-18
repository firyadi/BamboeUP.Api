using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class PersonService
    {
        public async Task<FileOperationResult<PersonDto>> UploadPhotoAsync(
            Guid personGuid,
            FileUploadRequest request,
            CancellationToken cancellationToken = default)
        {
            var person = await repository.Person.GetPersonAsync(personGuid, trackChanges: false);
            if (person is null)
                return FileOperationResult<PersonDto>.Fail("Person not found.", "NOT_FOUND");

            var categoryId = await fileStorageService.ResolveFileCategoryIdAsync("Photo", cancellationToken)
                             ?? await fileStorageService.ResolveFileCategoryIdAsync("photo", cancellationToken);
            if (categoryId is null or 0)
                return FileOperationResult<PersonDto>.Fail(
                    "FileCategory 'Photo' was not found in Standard Reference.",
                    "CATEGORY_MISSING");

            var upload = await fileStorageService.UploadAsync(request, categoryId.Value, cancellationToken);
            if (!upload.Success || upload.Data is null)
                return FileOperationResult<PersonDto>.Fail(
                    upload.Message,
                    upload.ErrorCode,
                    upload.ValidationErrors);

            var previousFileStorageId = person.FileStorageId;
            person.FileStorageId = upload.Data.FileStorageId;
            person.StatusId = 2;
            person.UpdatedById = userContext.UserId;
            person.UpdatedTime = DateTime.UtcNow;
            await repository.Person.UpdatePersonAsync(person);

            if (previousFileStorageId is > 0 && previousFileStorageId != upload.Data.FileStorageId)
            {
                var oldMeta = await fileStorageService.GetFileStorageByIdAsync(previousFileStorageId.Value, false);
                if (oldMeta is not null)
                    await fileStorageService.DeletePhysicalAsync(oldMeta.FileStorageGuid, userContext.UserId, cancellationToken);
            }

            var dto = person.Adapt<PersonDto>();
            dto.FileStorageGuid = upload.Data.FileStorageGuid;
            return FileOperationResult<PersonDto>.Ok(dto, "Photo uploaded.");
        }

        public async Task<FileOperationResult<PersonDto>> RemovePhotoAsync(
            Guid personGuid,
            bool deletePhysicalFile = true,
            CancellationToken cancellationToken = default)
        {
            var person = await repository.Person.GetPersonAsync(personGuid, trackChanges: false);
            if (person is null)
                return FileOperationResult<PersonDto>.Fail("Person not found.", "NOT_FOUND");

            var previousId = person.FileStorageId;
            person.FileStorageId = null;
            person.StatusId = 2;
            person.UpdatedById = userContext.UserId;
            person.UpdatedTime = DateTime.UtcNow;
            await repository.Person.UpdatePersonAsync(person);

            if (deletePhysicalFile && previousId is > 0)
            {
                var oldMeta = await fileStorageService.GetFileStorageByIdAsync(previousId.Value, false);
                if (oldMeta is not null)
                    await fileStorageService.DeletePhysicalAsync(oldMeta.FileStorageGuid, userContext.UserId, cancellationToken);
            }

            return FileOperationResult<PersonDto>.Ok(person.Adapt<PersonDto>(), "Photo removed.");
        }

        public async Task<PersonDto> OnboardPersonAsync(PersonQuickOnboardDto input)
        {
            await transactionManager.BeginTransactionAsync();
            var transaction = transactionManager.GetTransaction();
            try
            {
                // 1. Create Person
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
                        EntityDisplayName = $"{model.FirstName} {model.LastName}".Trim(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                };

                // 2. Insert Address
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
                    ActionType = "CREATE",
                    OldEntity = null,
                    NewEntity = address
                });

                // 3. Insert Identification
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
                    ActionType = "CREATE",
                    OldEntity = null,
                    NewEntity = identification
                });

                // 4. Insert Education
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
                    ActionType = "CREATE",
                    OldEntity = null,
                    NewEntity = education
                });

                // 5. Insert Emergency Contact
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
                    ActionType = "CREATE",
                    OldEntity = null,
                    NewEntity = emergencyContact
                });

                // 6. Conditional User Account Creation
                if (input.AutoCreateUserAccount)
                {
                    var saltBytes = RandomNumberGenerator.GetBytes(32);
                    var salt = Convert.ToBase64String(saltBytes);
                    var defaultPassword = "BamboeUp123!";
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(salt + defaultPassword, workFactor: 12);
                    
                    var usernameBase = input.FirstName.ToLower().Replace(" ", "");
                    var suffix = DateTime.UtcNow.Ticks.ToString();
                    var username = usernameBase + suffix.Substring(suffix.Length - 4);

                    var user = new User
                    {
                        UserName = username,
                        PasswordHash = passwordHash,
                        PasswordSalt = salt,
                        FullName = $"{input.FirstName} {input.LastName}".Trim(),
                        Email = string.IsNullOrWhiteSpace(input.UserEmail) ? $"{username}@bamboeup.local" : input.UserEmail,
                        IsAdmin = false,
                        StatusId = 1,
                        CreatedById = 0,
                        CreatedTime = DateTime.UtcNow
                    };

                    await repository.User.CreateUserAsync(user, transaction);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "Users",
                        EntityKey = user.UserGuid.ToString(),
                        EntityDisplayName = user.UserName,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = user
                    });
                }

                // 7. Log Session
                await audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "CREATE",
                    RootTableName = "Person",
                    RootEntityKey = model.PersonGuid.ToString(),
                    RootDisplayName = $"{model.FirstName} {model.LastName}".Trim(),
                    UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();

                // Load navigation properties for return
                var resultDto = model.Adapt<PersonDto>();
                resultDto.PersonAddresses = new List<PersonAddressDto> { address.Adapt<PersonAddressDto>() };
                resultDto.PersonIdentifications = new List<PersonIdentificationDto> { identification.Adapt<PersonIdentificationDto>() };
                resultDto.PersonEducations = new List<PersonEducationDto> { education.Adapt<PersonEducationDto>() };
                resultDto.PersonEmergencyContacts = new List<PersonEmergencyContactDto> { emergencyContact.Adapt<PersonEmergencyContactDto>() };

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
