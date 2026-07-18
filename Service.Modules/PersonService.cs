using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Service.Contracts.Shell;
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
        IUserContext userContext,
        IFileStorageService fileStorageService) : IPersonService
    {
        public PersonService(IRepositoryManager repository, ILoggerManager logger)
            : this(repository, logger, null!, null!, null!, null!)
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
            var dto = entity.Adapt<PersonDto>();
            if (entity.FileStorageId is > 0)
            {
                var file = await fileStorageService.GetFileStorageByIdAsync(entity.FileStorageId.Value, false);
                dto.FileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
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

                                if (input.PersonContacts != null && input.PersonContacts.Any())
                {
                    foreach (var detailDto in input.PersonContacts)
                    {
                        var detail = detailDto.Adapt<PersonContact>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.PersonContact.CreatePersonContactAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "PersonContact",
                            EntityKey = detail.PersonContactGuid.ToString(),
                            EntityDisplayName = detail.ContactValue,
                            ParentTableName = "Person",
                            ParentEntityKey = model.PersonGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.PersonFamilies != null && input.PersonFamilies.Any())
                {
                    foreach (var detailDto in input.PersonFamilies)
                    {
                        var detail = detailDto.Adapt<PersonFamily>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.PersonFamily.CreatePersonFamilyAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "PersonFamily",
                            EntityKey = detail.PersonFamilyGuid.ToString(),
                            EntityDisplayName = detail.FamilyName,
                            ParentTableName = "Person",
                            ParentEntityKey = model.PersonGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.PersonPhysicalCharacteristics != null && input.PersonPhysicalCharacteristics.Any())
                {
                    foreach (var detailDto in input.PersonPhysicalCharacteristics)
                    {
                        var detail = detailDto.Adapt<PersonPhysicalCharacteristic>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.PersonPhysicalCharacteristic.CreatePersonPhysicalCharacteristicAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "PersonPhysicalCharacteristic",
                            EntityKey = detail.PersonPhysicalCharacteristicGuid.ToString(),
                            EntityDisplayName = detail.PhysicalValue,
                            ParentTableName = "Person",
                            ParentEntityKey = model.PersonGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.PersonWorkExperiences != null && input.PersonWorkExperiences.Any())
                {
                    foreach (var detailDto in input.PersonWorkExperiences)
                    {
                        var detail = detailDto.Adapt<PersonWorkExperience>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.PersonWorkExperience.CreatePersonWorkExperienceAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "PersonWorkExperience",
                            EntityKey = detail.PersonWorkExperienceGuid.ToString(),
                            EntityDisplayName = detail.CompanyName,
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
                        var oldPersonContacts = (await repository.PersonContact.GetAllByPersonGuidAsync(personGuid)).ToList();
                        var oldPersonFamilies = (await repository.PersonFamily.GetAllByPersonGuidAsync(personGuid)).ToList();
                        var oldPersonPhysicalCharacteristics = (await repository.PersonPhysicalCharacteristic.GetAllByPersonGuidAsync(personGuid)).ToList();
                        var oldPersonWorkExperiences = (await repository.PersonWorkExperience.GetAllByPersonGuidAsync(personGuid)).ToList();
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

                                if (input.PersonContacts != null)
                {
                    var newPersonContacts = input.PersonContacts.ToList();
                    var oldPersonContactDict = oldPersonContacts.ToDictionary(o => o.PersonContactGuid);

                    foreach (var detailDto in newPersonContacts)
                    {
                        var detail = detailDto.Adapt<PersonContact>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.PersonContactGuid != Guid.Empty && oldPersonContactDict.ContainsKey(detailDto.PersonContactGuid))
                        {
                            detail.PersonContactGuid = detailDto.PersonContactGuid;
                            detail.PersonContactId = oldPersonContactDict[detailDto.PersonContactGuid].PersonContactId;
                            await repository.PersonContact.UpdatePersonContactAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonContact",
                                EntityKey = detail.PersonContactGuid.ToString(),
                                EntityDisplayName = detail.ContactValue,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldPersonContactDict[detailDto.PersonContactGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.PersonContact.CreatePersonContactAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonContact",
                                EntityKey = detail.PersonContactGuid.ToString(),
                                EntityDisplayName = detail.ContactValue,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldPersonContact in oldPersonContacts)
                    {
                        if (!newPersonContacts.Any(o => o.PersonContactGuid == oldPersonContact.PersonContactGuid))
                        {
                            await repository.PersonContact.SoftDeletePersonContactAsync(
                                new PersonContact
                                {
                                    PersonContactGuid = oldPersonContact.PersonContactGuid,
                                    PersonContactId = oldPersonContact.PersonContactId,
                                    PersonId = oldPersonContact.PersonId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonContact",
                                EntityKey = oldPersonContact.PersonContactGuid.ToString(),
                                EntityDisplayName = oldPersonContact.ContactValue,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldPersonContact,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.PersonFamilies != null)
                {
                    var newPersonFamilies = input.PersonFamilies.ToList();
                    var oldPersonFamilyDict = oldPersonFamilies.ToDictionary(o => o.PersonFamilyGuid);

                    foreach (var detailDto in newPersonFamilies)
                    {
                        var detail = detailDto.Adapt<PersonFamily>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.PersonFamilyGuid != Guid.Empty && oldPersonFamilyDict.ContainsKey(detailDto.PersonFamilyGuid))
                        {
                            detail.PersonFamilyGuid = detailDto.PersonFamilyGuid;
                            detail.PersonFamilyId = oldPersonFamilyDict[detailDto.PersonFamilyGuid].PersonFamilyId;
                            await repository.PersonFamily.UpdatePersonFamilyAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonFamily",
                                EntityKey = detail.PersonFamilyGuid.ToString(),
                                EntityDisplayName = detail.FamilyName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldPersonFamilyDict[detailDto.PersonFamilyGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.PersonFamily.CreatePersonFamilyAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonFamily",
                                EntityKey = detail.PersonFamilyGuid.ToString(),
                                EntityDisplayName = detail.FamilyName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldPersonFamily in oldPersonFamilies)
                    {
                        if (!newPersonFamilies.Any(o => o.PersonFamilyGuid == oldPersonFamily.PersonFamilyGuid))
                        {
                            await repository.PersonFamily.SoftDeletePersonFamilyAsync(
                                new PersonFamily
                                {
                                    PersonFamilyGuid = oldPersonFamily.PersonFamilyGuid,
                                    PersonFamilyId = oldPersonFamily.PersonFamilyId,
                                    PersonId = oldPersonFamily.PersonId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonFamily",
                                EntityKey = oldPersonFamily.PersonFamilyGuid.ToString(),
                                EntityDisplayName = oldPersonFamily.FamilyName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldPersonFamily,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.PersonPhysicalCharacteristics != null)
                {
                    var newPersonPhysicalCharacteristics = input.PersonPhysicalCharacteristics.ToList();
                    var oldPersonPhysicalCharacteristicDict = oldPersonPhysicalCharacteristics.ToDictionary(o => o.PersonPhysicalCharacteristicGuid);

                    foreach (var detailDto in newPersonPhysicalCharacteristics)
                    {
                        var detail = detailDto.Adapt<PersonPhysicalCharacteristic>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.PersonPhysicalCharacteristicGuid != Guid.Empty && oldPersonPhysicalCharacteristicDict.ContainsKey(detailDto.PersonPhysicalCharacteristicGuid))
                        {
                            detail.PersonPhysicalCharacteristicGuid = detailDto.PersonPhysicalCharacteristicGuid;
                            detail.PersonPhysicalCharacteristicId = oldPersonPhysicalCharacteristicDict[detailDto.PersonPhysicalCharacteristicGuid].PersonPhysicalCharacteristicId;
                            await repository.PersonPhysicalCharacteristic.UpdatePersonPhysicalCharacteristicAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonPhysicalCharacteristic",
                                EntityKey = detail.PersonPhysicalCharacteristicGuid.ToString(),
                                EntityDisplayName = detail.PhysicalValue,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldPersonPhysicalCharacteristicDict[detailDto.PersonPhysicalCharacteristicGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.PersonPhysicalCharacteristic.CreatePersonPhysicalCharacteristicAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonPhysicalCharacteristic",
                                EntityKey = detail.PersonPhysicalCharacteristicGuid.ToString(),
                                EntityDisplayName = detail.PhysicalValue,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldPersonPhysicalCharacteristic in oldPersonPhysicalCharacteristics)
                    {
                        if (!newPersonPhysicalCharacteristics.Any(o => o.PersonPhysicalCharacteristicGuid == oldPersonPhysicalCharacteristic.PersonPhysicalCharacteristicGuid))
                        {
                            await repository.PersonPhysicalCharacteristic.SoftDeletePersonPhysicalCharacteristicAsync(
                                new PersonPhysicalCharacteristic
                                {
                                    PersonPhysicalCharacteristicGuid = oldPersonPhysicalCharacteristic.PersonPhysicalCharacteristicGuid,
                                    PersonPhysicalCharacteristicId = oldPersonPhysicalCharacteristic.PersonPhysicalCharacteristicId,
                                    PersonId = oldPersonPhysicalCharacteristic.PersonId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonPhysicalCharacteristic",
                                EntityKey = oldPersonPhysicalCharacteristic.PersonPhysicalCharacteristicGuid.ToString(),
                                EntityDisplayName = oldPersonPhysicalCharacteristic.PhysicalValue,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldPersonPhysicalCharacteristic,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.PersonWorkExperiences != null)
                {
                    var newPersonWorkExperiences = input.PersonWorkExperiences.ToList();
                    var oldPersonWorkExperienceDict = oldPersonWorkExperiences.ToDictionary(o => o.PersonWorkExperienceGuid);

                    foreach (var detailDto in newPersonWorkExperiences)
                    {
                        var detail = detailDto.Adapt<PersonWorkExperience>();
                        detail.PersonId = model.PersonId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.PersonWorkExperienceGuid != Guid.Empty && oldPersonWorkExperienceDict.ContainsKey(detailDto.PersonWorkExperienceGuid))
                        {
                            detail.PersonWorkExperienceGuid = detailDto.PersonWorkExperienceGuid;
                            detail.PersonWorkExperienceId = oldPersonWorkExperienceDict[detailDto.PersonWorkExperienceGuid].PersonWorkExperienceId;
                            await repository.PersonWorkExperience.UpdatePersonWorkExperienceAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonWorkExperience",
                                EntityKey = detail.PersonWorkExperienceGuid.ToString(),
                                EntityDisplayName = detail.CompanyName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldPersonWorkExperienceDict[detailDto.PersonWorkExperienceGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.PersonWorkExperience.CreatePersonWorkExperienceAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonWorkExperience",
                                EntityKey = detail.PersonWorkExperienceGuid.ToString(),
                                EntityDisplayName = detail.CompanyName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldPersonWorkExperience in oldPersonWorkExperiences)
                    {
                        if (!newPersonWorkExperiences.Any(o => o.PersonWorkExperienceGuid == oldPersonWorkExperience.PersonWorkExperienceGuid))
                        {
                            await repository.PersonWorkExperience.SoftDeletePersonWorkExperienceAsync(
                                new PersonWorkExperience
                                {
                                    PersonWorkExperienceGuid = oldPersonWorkExperience.PersonWorkExperienceGuid,
                                    PersonWorkExperienceId = oldPersonWorkExperience.PersonWorkExperienceId,
                                    PersonId = oldPersonWorkExperience.PersonId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "PersonWorkExperience",
                                EntityKey = oldPersonWorkExperience.PersonWorkExperienceGuid.ToString(),
                                EntityDisplayName = oldPersonWorkExperience.CompanyName,
                                ParentTableName = "Person",
                                ParentEntityKey = model.PersonGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldPersonWorkExperience,
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
            string? firstName, string? firstNameSearchType, string? middleName, string? middleNameSearchType, string? lastName, string? lastNameSearchType, string? preTitle, string? preTitleSearchType, string? postTitle, string? postTitleSearchType, string? personName, string? personNameSearchType, string? birthName, string? birthNameSearchType, string? placeofBirth, string? placeofBirthSearchType, string? birthDate, string? birthDateSearchType, string? nationalIdNo, string? nationalIdNoSearchType, string? srGender, string? srGenderSearchType, string? srReligion, string? srReligionSearchType, string? srSalutation, string? srSalutationSearchType, string? srBloodType, string? srBloodTypeSearchType, string? srMaritalStatus, string? srMaritalStatusSearchType
            // ── FK Virtual Search Params ──
        )
        {
            var data = await repository.Person.SearchPersonAsync(
firstName, firstNameSearchType, middleName, middleNameSearchType, lastName, lastNameSearchType, preTitle, preTitleSearchType, postTitle, postTitleSearchType, personName, personNameSearchType, birthName, birthNameSearchType, placeofBirth, placeofBirthSearchType, birthDate, birthDateSearchType, nationalIdNo, nationalIdNoSearchType, srGender, srGenderSearchType, srReligion, srReligionSearchType, srSalutation, srSalutationSearchType, srBloodType, srBloodTypeSearchType, srMaritalStatus, srMaritalStatusSearchType
                // ── FK Virtual Search Args ──
            );
            return data.Adapt<IEnumerable<PersonDto>>();
        }
    }
}
