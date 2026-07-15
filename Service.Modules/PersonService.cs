using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class PersonService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IPersonService
    {
        public PersonService(IRepositoryManager repository, ILoggerManager logger)
            : this(repository, logger, null!, null!, null!)
        {
        }

        public async Task<IEnumerable<PersonDto>> GetAllPeopleAsync(bool trackChanges)
        {
            var entities = await repository.Person.GetAllPeopleAsync(trackChanges);
            return entities.Adapt<IEnumerable<PersonDto>>();
        }

        public async Task<PersonDto?> GetPersonByGuidAsync(Guid personGuid, bool trackChanges)
        {
            var entity = await repository.Person.GetPersonAsync(personGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<PersonDto>();
        }

        public async Task<PersonDto> CreatePersonAsync(PersonForCreationDto input)
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
                        EntityDisplayName = model.MiddleName,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                };

                // Dapper doesn't automatically insert children, so we must manually insert nested Details
                                if (input.PersonIdentifications != null && input.PersonIdentifications.Any())
                {
                    foreach (var detailDto in input.PersonIdentifications)
                    {
                        var detail = detailDto.Adapt<PersonIdentification>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.PersonIdentification.CreatePersonIdentificationAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "PersonIdentification",
                            EntityKey = detail.PersonIdentificationGuid.ToString(),
                            EntityDisplayName = detail.IdentificationValue,
                            ParentTableName = "Person",
                            ParentEntityKey = model.PersonGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.PersonEducations != null && input.PersonEducations.Any())
                {
                    foreach (var detailDto in input.PersonEducations)
                    {
                        var detail = detailDto.Adapt<PersonEducation>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.PersonEducation.CreatePersonEducationAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "PersonEducation",
                            EntityKey = detail.PersonEducationGuid.ToString(),
                            EntityDisplayName = detail.InstitutionName,
                            ParentTableName = "Person",
                            ParentEntityKey = model.PersonGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.PersonEmergencyContacts != null && input.PersonEmergencyContacts.Any())
                {
                    foreach (var detailDto in input.PersonEmergencyContacts)
                    {
                        var detail = detailDto.Adapt<PersonEmergencyContact>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.PersonEmergencyContact.CreatePersonEmergencyContactAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "PersonEmergencyContact",
                            EntityKey = detail.PersonEmergencyContactGuid.ToString(),
                            EntityDisplayName = detail.ContactName,
                            ParentTableName = "Person",
                            ParentEntityKey = model.PersonGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                // ##HeaderDetailCreateBlock##
                if (input.PersonAddresses != null && input.PersonAddresses.Any())
                {
                    foreach (var detailDto in input.PersonAddresses)
                    {
                        var detail = detailDto.Adapt<PersonAddress>();
                        detail.PersonId = model.PersonId;
                    detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.PersonAddress.CreatePersonAddressAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "PersonAddress",
                            EntityKey = detail.PersonAddressGuid.ToString(),
                            EntityDisplayName = detail.Address,
                            ParentTableName = "Person",
                            ParentEntityKey = model.PersonGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                // Single audit session for all entities
                await audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "CREATE",
                    RootTableName = "Person",
                    RootEntityKey = model.PersonGuid.ToString(),
                    RootDisplayName = model.MiddleName,
                    UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();
                return model.Adapt<PersonDto>();
            }
            catch (Exception ex)
            {
                await transactionManager.RollbackAsync();
                logger.LogError($"Error in CreatePersonAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdatePersonAsync(Guid personGuid, PersonForUpdateDto input, bool trackChanges)
        {
            // 1. Fetch old data for audit diff
            var oldPerson = await repository.Person.GetPersonAsync(personGuid, false)
                ?? throw new KeyNotFoundException($"Person '{{personGuid}}' was not found.");
            var oldPersonAddresses = (await repository.PersonAddress.GetAllByPersonGuidAsync(personGuid)).ToList();
                        var oldPersonIdentifications = (await repository.PersonIdentification.GetAllByPersonGuidAsync(personGuid)).ToList();
                        var oldPersonEducations = (await repository.PersonEducation.GetAllByPersonGuidAsync(personGuid)).ToList();
                        var oldPersonEmergencyContacts = (await repository.PersonEmergencyContact.GetAllByPersonGuidAsync(personGuid)).ToList();
            // ##HeaderDetailUpdateFetch##

            await transactionManager.BeginTransactionAsync();
            var transaction = transactionManager.GetTransaction();
            try
            {
                // 2. Update Header
                var model = input.Adapt<Person>();

                model.PersonGuid = personGuid;
                model.PersonId = oldPerson.PersonId;

                model.StatusId = 2;
                model.UpdatedById = input.UpdatedById;
                model.UpdatedTime = DateTime.UtcNow;
                await repository.Person.UpdatePersonAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "Person",
                        EntityKey = model.PersonGuid.ToString(),
                        EntityDisplayName = model.MiddleName,
                        ActionType = "UPDATE",
                        OldEntity = oldPerson,
                        NewEntity = model
                    }
                };

                // 3. Child alignment/diff logic
                                if (input.PersonIdentifications != null)
                {
                    var newPersonIdentifications = input.PersonIdentifications.ToList();
                    var oldPersonIdentificationDict = oldPersonIdentifications.ToDictionary(o => o.PersonIdentificationGuid);

                    foreach (var detailDto in newPersonIdentifications)
                    {
                        var detail = detailDto.Adapt<PersonIdentification>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.PersonIdentificationGuid != Guid.Empty && oldPersonIdentificationDict.ContainsKey(detailDto.PersonIdentificationGuid))
                        {
                            detail.PersonIdentificationGuid = detailDto.PersonIdentificationGuid;
                            detail.PersonIdentificationId = oldPersonIdentificationDict[detailDto.PersonIdentificationGuid].PersonIdentificationId;
                            await repository.PersonIdentification.UpdatePersonIdentificationAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonIdentification",
                                EntityKey = detail.PersonIdentificationGuid.ToString(),
                                EntityDisplayName = detail.IdentificationValue,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldPersonIdentificationDict[detailDto.PersonIdentificationGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.PersonIdentification.CreatePersonIdentificationAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonIdentification",
                                EntityKey = detail.PersonIdentificationGuid.ToString(),
                                EntityDisplayName = detail.IdentificationValue,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldPersonIdentification in oldPersonIdentifications)
                    {
                        if (!newPersonIdentifications.Any(o => o.PersonIdentificationGuid == oldPersonIdentification.PersonIdentificationGuid))
                        {
                            await repository.PersonIdentification.SoftDeletePersonIdentificationAsync(
                                new PersonIdentification
                                {
                                    PersonIdentificationGuid = oldPersonIdentification.PersonIdentificationGuid,
                                    PersonIdentificationId = oldPersonIdentification.PersonIdentificationId,
                                    PersonId = oldPersonIdentification.PersonId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonIdentification",
                                EntityKey = oldPersonIdentification.PersonIdentificationGuid.ToString(),
                                EntityDisplayName = oldPersonIdentification.IdentificationValue,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldPersonIdentification,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.PersonEducations != null)
                {
                    var newPersonEducations = input.PersonEducations.ToList();
                    var oldPersonEducationDict = oldPersonEducations.ToDictionary(o => o.PersonEducationGuid);

                    foreach (var detailDto in newPersonEducations)
                    {
                        var detail = detailDto.Adapt<PersonEducation>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.PersonEducationGuid != Guid.Empty && oldPersonEducationDict.ContainsKey(detailDto.PersonEducationGuid))
                        {
                            detail.PersonEducationGuid = detailDto.PersonEducationGuid;
                            detail.PersonEducationId = oldPersonEducationDict[detailDto.PersonEducationGuid].PersonEducationId;
                            await repository.PersonEducation.UpdatePersonEducationAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonEducation",
                                EntityKey = detail.PersonEducationGuid.ToString(),
                                EntityDisplayName = detail.InstitutionName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldPersonEducationDict[detailDto.PersonEducationGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.PersonEducation.CreatePersonEducationAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonEducation",
                                EntityKey = detail.PersonEducationGuid.ToString(),
                                EntityDisplayName = detail.InstitutionName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldPersonEducation in oldPersonEducations)
                    {
                        if (!newPersonEducations.Any(o => o.PersonEducationGuid == oldPersonEducation.PersonEducationGuid))
                        {
                            await repository.PersonEducation.SoftDeletePersonEducationAsync(
                                new PersonEducation
                                {
                                    PersonEducationGuid = oldPersonEducation.PersonEducationGuid,
                                    PersonEducationId = oldPersonEducation.PersonEducationId,
                                    PersonId = oldPersonEducation.PersonId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonEducation",
                                EntityKey = oldPersonEducation.PersonEducationGuid.ToString(),
                                EntityDisplayName = oldPersonEducation.InstitutionName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldPersonEducation,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.PersonEmergencyContacts != null)
                {
                    var newPersonEmergencyContacts = input.PersonEmergencyContacts.ToList();
                    var oldPersonEmergencyContactDict = oldPersonEmergencyContacts.ToDictionary(o => o.PersonEmergencyContactGuid);

                    foreach (var detailDto in newPersonEmergencyContacts)
                    {
                        var detail = detailDto.Adapt<PersonEmergencyContact>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.PersonEmergencyContactGuid != Guid.Empty && oldPersonEmergencyContactDict.ContainsKey(detailDto.PersonEmergencyContactGuid))
                        {
                            detail.PersonEmergencyContactGuid = detailDto.PersonEmergencyContactGuid;
                            detail.PersonEmergencyContactId = oldPersonEmergencyContactDict[detailDto.PersonEmergencyContactGuid].PersonEmergencyContactId;
                            await repository.PersonEmergencyContact.UpdatePersonEmergencyContactAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonEmergencyContact",
                                EntityKey = detail.PersonEmergencyContactGuid.ToString(),
                                EntityDisplayName = detail.ContactName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldPersonEmergencyContactDict[detailDto.PersonEmergencyContactGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.PersonEmergencyContact.CreatePersonEmergencyContactAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonEmergencyContact",
                                EntityKey = detail.PersonEmergencyContactGuid.ToString(),
                                EntityDisplayName = detail.ContactName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldPersonEmergencyContact in oldPersonEmergencyContacts)
                    {
                        if (!newPersonEmergencyContacts.Any(o => o.PersonEmergencyContactGuid == oldPersonEmergencyContact.PersonEmergencyContactGuid))
                        {
                            await repository.PersonEmergencyContact.SoftDeletePersonEmergencyContactAsync(
                                new PersonEmergencyContact
                                {
                                    PersonEmergencyContactGuid = oldPersonEmergencyContact.PersonEmergencyContactGuid,
                                    PersonEmergencyContactId = oldPersonEmergencyContact.PersonEmergencyContactId,
                                    PersonId = oldPersonEmergencyContact.PersonId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonEmergencyContact",
                                EntityKey = oldPersonEmergencyContact.PersonEmergencyContactGuid.ToString(),
                                EntityDisplayName = oldPersonEmergencyContact.ContactName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldPersonEmergencyContact,
                                NewEntity = null
                            });
                        }
                    }
                }

                // ##HeaderDetailUpdateDiff##
                if (input.PersonAddresses != null)
                {
                    var newPersonAddresses = input.PersonAddresses.ToList();
                    var oldPersonAddressDict = oldPersonAddresses.ToDictionary(o => o.PersonAddressGuid);

                    foreach (var detailDto in newPersonAddresses)
                    {
                        var detail = detailDto.Adapt<PersonAddress>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.PersonAddressGuid != Guid.Empty && oldPersonAddressDict.TryGetValue(detailDto.PersonAddressGuid, out var oldDetailRow))
                        {
                            detail.PersonAddressGuid = detailDto.PersonAddressGuid;
                            detail.UpdatedById = input.UpdatedById;
                            await repository.PersonAddress.UpdatePersonAddressAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonAddress",
                                EntityKey = detail.PersonAddressGuid.ToString(),
                                EntityDisplayName = detail.Address,
                                ParentTableName = "Person",
                                ParentEntityKey = personGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldDetailRow,
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.PersonAddressGuid = Guid.NewGuid();
                            detail.CreatedById = input.UpdatedById;
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.PersonAddress.CreatePersonAddressAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonAddress",
                                EntityKey = detail.PersonAddressGuid.ToString(),
                                EntityDisplayName = detail.Address,
                                ParentTableName = "Person",
                                ParentEntityKey = personGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldPersonAddress in oldPersonAddresses)
                    {
                        if (!newPersonAddresses.Any(o => o.PersonAddressGuid == oldPersonAddress.PersonAddressGuid))
                        {
                            await repository.PersonAddress.SoftDeletePersonAddressAsync(
                                new PersonAddress
                                {
                                    PersonAddressGuid = oldPersonAddress.PersonAddressGuid,
                                    DeletedById = input.UpdatedById,
                                    DeletedTime = DateTime.UtcNow
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonAddress",
                                EntityKey = oldPersonAddress.PersonAddressGuid.ToString(),
                                EntityDisplayName = oldPersonAddress.Address,
                                ParentTableName = "Person",
                                ParentEntityKey = personGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldPersonAddress,
                                NewEntity = null
                            });
                        }
                    }
                }

                // 4. Single audit session for all entities
                await audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "UPDATE",
                    RootTableName = "Person",
                    RootEntityKey = model.PersonGuid.ToString(),
                    RootDisplayName = model.MiddleName,
                    UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.UpdatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();
            }
            catch (Exception ex)
            {
                await transactionManager.RollbackAsync();
                logger.LogError($"Error in UpdatePersonAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeletePersonAsync(Guid personGuid, PersonForDeleteDto input, bool trackChanges)
        {
            var oldPerson = await repository.Person.GetPersonAsync(personGuid, false);
            var model = new Person
            {
                PersonGuid = personGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Person.SoftDeletePersonAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Person",
                RootEntityKey = personGuid.ToString(),
                RootDisplayName = oldPerson?.MiddleName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Person",
                        EntityKey = personGuid.ToString(),
                        EntityDisplayName = oldPerson?.MiddleName,
                        ActionType = "DELETE",
                        OldEntity = oldPerson,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeletePersonByAdminAsync(Guid personGuid, bool trackChanges)
        {
            await repository.Person.DeletePersonAsync(personGuid);
        }

        public async Task<IEnumerable<PersonDto>> SearchPersonAsync(
            string? firstName, string? firstNameSearchType, string? middleName, string? middleNameSearchType, string? lastName, string? lastNameSearchType, string? preTitle, string? preTitleSearchType, string? postTitle, string? postTitleSearchType, string? birthName, string? birthNameSearchType, string? placeofBirth, string? placeofBirthSearchType, string? birthDate, string? birthDateSearchType, string? nationalIdNo, string? nationalIdNoSearchType, string? srGender, string? srGenderSearchType, string? srReligion, string? srReligionSearchType, string? srSalutation, string? srSalutationSearchType, string? srBloodType, string? srBloodTypeSearchType, string? srMaritalStatus, string? srMaritalStatusSearchType, string? photo, string? photoSearchType
            // ── FK Virtual Search Params ──
        )
        {
            var data = await repository.Person.SearchPersonAsync(
firstName, firstNameSearchType, middleName, middleNameSearchType, lastName, lastNameSearchType, preTitle, preTitleSearchType, postTitle, postTitleSearchType, birthName, birthNameSearchType, placeofBirth, placeofBirthSearchType, birthDate, birthDateSearchType, nationalIdNo, nationalIdNoSearchType, srGender, srGenderSearchType, srReligion, srReligionSearchType, srSalutation, srSalutationSearchType, srBloodType, srBloodTypeSearchType, srMaritalStatus, srMaritalStatusSearchType, photo, photoSearchType
                // ── FK Virtual Search Args ──
            );
            return data.Adapt<IEnumerable<PersonDto>>();
        }
    }
}
