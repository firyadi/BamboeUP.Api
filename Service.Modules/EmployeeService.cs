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
    public partial class EmployeeService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IEmployeeService
    {
        public EmployeeService(IRepositoryManager repository, ILoggerManager logger)
            : this(repository, logger, null!, null!, null!)
        {
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(bool trackChanges)
        {
            var entities = await repository.Employee.GetAllEmployeesAsync(trackChanges);
            return entities.Adapt<IEnumerable<EmployeeDto>>();
        }

        public async Task<EmployeeDto?> GetEmployeeByGuidAsync(Guid employeeGuid, bool trackChanges)
        {
            var entity = await repository.Employee.GetEmployeeAsync(employeeGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<EmployeeDto>();
            return dto;
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeForCreationDto input)
        {
            await transactionManager.BeginTransactionAsync();
            var transaction = transactionManager.GetTransaction();
            try
            {
                var model = input.Adapt<Employee>();
            model.SrRemunerationType = input.SrRemunerationType is null or 0 ? null : input.SrRemunerationType;

                model.StatusId = 1;
                model.CreatedTime = DateTime.UtcNow;
                await repository.Employee.CreateEmployeeAsync(model, transaction);


                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "Employee",
                        EntityKey = model.EmployeeGuid.ToString(),
                        EntityDisplayName = model.EmployeeNo,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                };

                // Dapper doesn't automatically insert children, so we must manually insert nested Details
                                if (input.EmployeeAssignments != null && input.EmployeeAssignments.Any())
                {
                    foreach (var detailDto in input.EmployeeAssignments)
                    {
                        var detail = detailDto.Adapt<EmployeeAssignment>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.EmployeeAssignment.CreateEmployeeAssignmentAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "EmployeeAssignment",
                            EntityKey = detail.EmployeeAssignmentGuid.ToString(),
                            EntityDisplayName = detail.Remarks,
                            ParentTableName = "Employee",
                            ParentEntityKey = model.EmployeeGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.EmployeeCareers != null && input.EmployeeCareers.Any())
                {
                    foreach (var detailDto in input.EmployeeCareers)
                    {
                        var detail = detailDto.Adapt<EmployeeCareer>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.EmployeeCareer.CreateEmployeeCareerAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "EmployeeCareer",
                            EntityKey = detail.EmployeeCareerGuid.ToString(),
                            EntityDisplayName = detail.ReferenceNo,
                            ParentTableName = "Employee",
                            ParentEntityKey = model.EmployeeGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.EmployeeCertifications != null && input.EmployeeCertifications.Any())
                {
                    foreach (var detailDto in input.EmployeeCertifications)
                    {
                        var detail = detailDto.Adapt<EmployeeCertification>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.EmployeeCertification.CreateEmployeeCertificationAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "EmployeeCertification",
                            EntityKey = detail.EmployeeCertificationGuid.ToString(),
                            EntityDisplayName = detail.CertificateNumber,
                            ParentTableName = "Employee",
                            ParentEntityKey = model.EmployeeGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.EmployeeContracts != null && input.EmployeeContracts.Any())
                {
                    foreach (var detailDto in input.EmployeeContracts)
                    {
                        var detail = detailDto.Adapt<EmployeeContract>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.EmployeeContract.CreateEmployeeContractAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "EmployeeContract",
                            EntityKey = detail.EmployeeContractGuid.ToString(),
                            EntityDisplayName = detail.ContractNo,
                            ParentTableName = "Employee",
                            ParentEntityKey = model.EmployeeGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.EmployeeLanguages != null && input.EmployeeLanguages.Any())
                {
                    foreach (var detailDto in input.EmployeeLanguages)
                    {
                        var detail = detailDto.Adapt<EmployeeLanguage>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.EmployeeLanguage.CreateEmployeeLanguageAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "EmployeeLanguage",
                            EntityKey = detail.EmployeeLanguageGuid.ToString(),
                            EntityDisplayName = detail.CertificationName,
                            ParentTableName = "Employee",
                            ParentEntityKey = model.EmployeeGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.EmployeeSkills != null && input.EmployeeSkills.Any())
                {
                    foreach (var detailDto in input.EmployeeSkills)
                    {
                        var detail = detailDto.Adapt<EmployeeSkill>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.EmployeeSkill.CreateEmployeeSkillAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "EmployeeSkill",
                            EntityKey = detail.EmployeeSkillGuid.ToString(),
                            EntityDisplayName = detail.CertificateNumber,
                            ParentTableName = "Employee",
                            ParentEntityKey = model.EmployeeGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                                if (input.EmployeeTrainings != null && input.EmployeeTrainings.Any())
                {
                    foreach (var detailDto in input.EmployeeTrainings)
                    {
                        var detail = detailDto.Adapt<EmployeeTraining>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.EmployeeTraining.CreateEmployeeTrainingAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "EmployeeTraining",
                            EntityKey = detail.EmployeeTrainingGuid.ToString(),
                            EntityDisplayName = detail.Grade,
                            ParentTableName = "Employee",
                            ParentEntityKey = model.EmployeeGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                // ##HeaderDetailCreateBlock##
                if (input.EmployeeAssets != null && input.EmployeeAssets.Any())
                {
                    foreach (var detailDto in input.EmployeeAssets)
                    {
                        var detail = detailDto.Adapt<EmployeeAsset>();
                        detail.EmployeeId = model.EmployeeId;
                    detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.EmployeeAsset.CreateEmployeeAssetAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "EmployeeAsset",
                            EntityKey = detail.EmployeeAssetGuid.ToString(),
                            EntityDisplayName = detail.ConditionWhenAssigned,
                            ParentTableName = "Employee",
                            ParentEntityKey = model.EmployeeGuid.ToString(),
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
                    RootTableName = "Employee",
                    RootEntityKey = model.EmployeeGuid.ToString(),
                    RootDisplayName = model.EmployeeNo,
                    UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();
                return model.Adapt<EmployeeDto>();
            }
            catch (Exception ex)
            {
                await transactionManager.RollbackAsync();
                logger.LogError($"Error in CreateEmployeeAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateEmployeeAsync(Guid employeeGuid, EmployeeForUpdateDto input, bool trackChanges)
        {
            // 1. Fetch old data for audit diff
            var oldEmployee = await repository.Employee.GetEmployeeAsync(employeeGuid, false)
                ?? throw new KeyNotFoundException($"Employee '{{employeeGuid}}' was not found.");
            var oldEmployeeAssets = (await repository.EmployeeAsset.GetAllByEmployeeGuidAsync(employeeGuid)).ToList();
                        var oldEmployeeAssignments = (await repository.EmployeeAssignment.GetAllByEmployeeGuidAsync(employeeGuid)).ToList();
                        var oldEmployeeCareers = (await repository.EmployeeCareer.GetAllByEmployeeGuidAsync(employeeGuid)).ToList();
                        var oldEmployeeCertifications = (await repository.EmployeeCertification.GetAllByEmployeeGuidAsync(employeeGuid)).ToList();
                        var oldEmployeeContracts = (await repository.EmployeeContract.GetAllByEmployeeGuidAsync(employeeGuid)).ToList();
                        var oldEmployeeLanguages = (await repository.EmployeeLanguage.GetAllByEmployeeGuidAsync(employeeGuid)).ToList();
                        var oldEmployeeSkills = (await repository.EmployeeSkill.GetAllByEmployeeGuidAsync(employeeGuid)).ToList();
                        var oldEmployeeTrainings = (await repository.EmployeeTraining.GetAllByEmployeeGuidAsync(employeeGuid)).ToList();
            // ##HeaderDetailUpdateFetch##

            await transactionManager.BeginTransactionAsync();
            var transaction = transactionManager.GetTransaction();
            try
            {
                // 2. Update Header
                var model = input.Adapt<Employee>();
            model.SrRemunerationType = input.SrRemunerationType is null or 0 ? null : input.SrRemunerationType;
                model.EmployeeGuid = employeeGuid;
                model.EmployeeId = oldEmployee.EmployeeId;

                model.StatusId = 2;
                model.UpdatedById = input.UpdatedById;
                model.UpdatedTime = DateTime.UtcNow;
                await repository.Employee.UpdateEmployeeAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "Employee",
                        EntityKey = model.EmployeeGuid.ToString(),
                        EntityDisplayName = model.EmployeeNo,
                        ActionType = "UPDATE",
                        OldEntity = oldEmployee,
                        NewEntity = model
                    }
                };

                // 3. Child alignment/diff logic
                                if (input.EmployeeAssignments != null)
                {
                    var newEmployeeAssignments = input.EmployeeAssignments.ToList();
                    var oldEmployeeAssignmentDict = oldEmployeeAssignments.ToDictionary(o => o.EmployeeAssignmentGuid);

                    foreach (var detailDto in newEmployeeAssignments)
                    {
                        var detail = detailDto.Adapt<EmployeeAssignment>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.EmployeeAssignmentGuid != Guid.Empty && oldEmployeeAssignmentDict.ContainsKey(detailDto.EmployeeAssignmentGuid))
                        {
                            detail.EmployeeAssignmentGuid = detailDto.EmployeeAssignmentGuid;
                            detail.EmployeeAssignmentId = oldEmployeeAssignmentDict[detailDto.EmployeeAssignmentGuid].EmployeeAssignmentId;
                            await repository.EmployeeAssignment.UpdateEmployeeAssignmentAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeAssignment",
                                EntityKey = detail.EmployeeAssignmentGuid.ToString(),
                                EntityDisplayName = detail.Remarks,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldEmployeeAssignmentDict[detailDto.EmployeeAssignmentGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.EmployeeAssignment.CreateEmployeeAssignmentAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeAssignment",
                                EntityKey = detail.EmployeeAssignmentGuid.ToString(),
                                EntityDisplayName = detail.Remarks,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldEmployeeAssignment in oldEmployeeAssignments)
                    {
                        if (!newEmployeeAssignments.Any(o => o.EmployeeAssignmentGuid == oldEmployeeAssignment.EmployeeAssignmentGuid))
                        {
                            await repository.EmployeeAssignment.SoftDeleteEmployeeAssignmentAsync(
                                new EmployeeAssignment
                                {
                                    EmployeeAssignmentGuid = oldEmployeeAssignment.EmployeeAssignmentGuid,
                                    EmployeeAssignmentId = oldEmployeeAssignment.EmployeeAssignmentId,
                                    EmployeeId = oldEmployeeAssignment.EmployeeId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeAssignment",
                                EntityKey = oldEmployeeAssignment.EmployeeAssignmentGuid.ToString(),
                                EntityDisplayName = oldEmployeeAssignment.Remarks,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldEmployeeAssignment,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.EmployeeCareers != null)
                {
                    var newEmployeeCareers = input.EmployeeCareers.ToList();
                    var oldEmployeeCareerDict = oldEmployeeCareers.ToDictionary(o => o.EmployeeCareerGuid);

                    foreach (var detailDto in newEmployeeCareers)
                    {
                        var detail = detailDto.Adapt<EmployeeCareer>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.EmployeeCareerGuid != Guid.Empty && oldEmployeeCareerDict.ContainsKey(detailDto.EmployeeCareerGuid))
                        {
                            detail.EmployeeCareerGuid = detailDto.EmployeeCareerGuid;
                            detail.EmployeeCareerId = oldEmployeeCareerDict[detailDto.EmployeeCareerGuid].EmployeeCareerId;
                            await repository.EmployeeCareer.UpdateEmployeeCareerAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeCareer",
                                EntityKey = detail.EmployeeCareerGuid.ToString(),
                                EntityDisplayName = detail.ReferenceNo,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldEmployeeCareerDict[detailDto.EmployeeCareerGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.EmployeeCareer.CreateEmployeeCareerAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeCareer",
                                EntityKey = detail.EmployeeCareerGuid.ToString(),
                                EntityDisplayName = detail.ReferenceNo,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldEmployeeCareer in oldEmployeeCareers)
                    {
                        if (!newEmployeeCareers.Any(o => o.EmployeeCareerGuid == oldEmployeeCareer.EmployeeCareerGuid))
                        {
                            await repository.EmployeeCareer.SoftDeleteEmployeeCareerAsync(
                                new EmployeeCareer
                                {
                                    EmployeeCareerGuid = oldEmployeeCareer.EmployeeCareerGuid,
                                    EmployeeCareerId = oldEmployeeCareer.EmployeeCareerId,
                                    EmployeeId = oldEmployeeCareer.EmployeeId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeCareer",
                                EntityKey = oldEmployeeCareer.EmployeeCareerGuid.ToString(),
                                EntityDisplayName = oldEmployeeCareer.ReferenceNo,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldEmployeeCareer,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.EmployeeCertifications != null)
                {
                    var newEmployeeCertifications = input.EmployeeCertifications.ToList();
                    var oldEmployeeCertificationDict = oldEmployeeCertifications.ToDictionary(o => o.EmployeeCertificationGuid);

                    foreach (var detailDto in newEmployeeCertifications)
                    {
                        var detail = detailDto.Adapt<EmployeeCertification>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.EmployeeCertificationGuid != Guid.Empty && oldEmployeeCertificationDict.ContainsKey(detailDto.EmployeeCertificationGuid))
                        {
                            detail.EmployeeCertificationGuid = detailDto.EmployeeCertificationGuid;
                            detail.EmployeeCertificationId = oldEmployeeCertificationDict[detailDto.EmployeeCertificationGuid].EmployeeCertificationId;
                            await repository.EmployeeCertification.UpdateEmployeeCertificationAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeCertification",
                                EntityKey = detail.EmployeeCertificationGuid.ToString(),
                                EntityDisplayName = detail.CertificateNumber,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldEmployeeCertificationDict[detailDto.EmployeeCertificationGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.EmployeeCertification.CreateEmployeeCertificationAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeCertification",
                                EntityKey = detail.EmployeeCertificationGuid.ToString(),
                                EntityDisplayName = detail.CertificateNumber,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldEmployeeCertification in oldEmployeeCertifications)
                    {
                        if (!newEmployeeCertifications.Any(o => o.EmployeeCertificationGuid == oldEmployeeCertification.EmployeeCertificationGuid))
                        {
                            await repository.EmployeeCertification.SoftDeleteEmployeeCertificationAsync(
                                new EmployeeCertification
                                {
                                    EmployeeCertificationGuid = oldEmployeeCertification.EmployeeCertificationGuid,
                                    EmployeeCertificationId = oldEmployeeCertification.EmployeeCertificationId,
                                    EmployeeId = oldEmployeeCertification.EmployeeId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeCertification",
                                EntityKey = oldEmployeeCertification.EmployeeCertificationGuid.ToString(),
                                EntityDisplayName = oldEmployeeCertification.CertificateNumber,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldEmployeeCertification,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.EmployeeContracts != null)
                {
                    var newEmployeeContracts = input.EmployeeContracts.ToList();
                    var oldEmployeeContractDict = oldEmployeeContracts.ToDictionary(o => o.EmployeeContractGuid);

                    foreach (var detailDto in newEmployeeContracts)
                    {
                        var detail = detailDto.Adapt<EmployeeContract>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.EmployeeContractGuid != Guid.Empty && oldEmployeeContractDict.ContainsKey(detailDto.EmployeeContractGuid))
                        {
                            detail.EmployeeContractGuid = detailDto.EmployeeContractGuid;
                            detail.EmployeeContractId = oldEmployeeContractDict[detailDto.EmployeeContractGuid].EmployeeContractId;
                            await repository.EmployeeContract.UpdateEmployeeContractAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeContract",
                                EntityKey = detail.EmployeeContractGuid.ToString(),
                                EntityDisplayName = detail.ContractNo,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldEmployeeContractDict[detailDto.EmployeeContractGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.EmployeeContract.CreateEmployeeContractAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeContract",
                                EntityKey = detail.EmployeeContractGuid.ToString(),
                                EntityDisplayName = detail.ContractNo,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldEmployeeContract in oldEmployeeContracts)
                    {
                        if (!newEmployeeContracts.Any(o => o.EmployeeContractGuid == oldEmployeeContract.EmployeeContractGuid))
                        {
                            await repository.EmployeeContract.SoftDeleteEmployeeContractAsync(
                                new EmployeeContract
                                {
                                    EmployeeContractGuid = oldEmployeeContract.EmployeeContractGuid,
                                    EmployeeContractId = oldEmployeeContract.EmployeeContractId,
                                    EmployeeId = oldEmployeeContract.EmployeeId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeContract",
                                EntityKey = oldEmployeeContract.EmployeeContractGuid.ToString(),
                                EntityDisplayName = oldEmployeeContract.ContractNo,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldEmployeeContract,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.EmployeeLanguages != null)
                {
                    var newEmployeeLanguages = input.EmployeeLanguages.ToList();
                    var oldEmployeeLanguageDict = oldEmployeeLanguages.ToDictionary(o => o.EmployeeLanguageGuid);

                    foreach (var detailDto in newEmployeeLanguages)
                    {
                        var detail = detailDto.Adapt<EmployeeLanguage>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.EmployeeLanguageGuid != Guid.Empty && oldEmployeeLanguageDict.ContainsKey(detailDto.EmployeeLanguageGuid))
                        {
                            detail.EmployeeLanguageGuid = detailDto.EmployeeLanguageGuid;
                            detail.EmployeeLanguageId = oldEmployeeLanguageDict[detailDto.EmployeeLanguageGuid].EmployeeLanguageId;
                            await repository.EmployeeLanguage.UpdateEmployeeLanguageAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeLanguage",
                                EntityKey = detail.EmployeeLanguageGuid.ToString(),
                                EntityDisplayName = detail.CertificationName,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldEmployeeLanguageDict[detailDto.EmployeeLanguageGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.EmployeeLanguage.CreateEmployeeLanguageAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeLanguage",
                                EntityKey = detail.EmployeeLanguageGuid.ToString(),
                                EntityDisplayName = detail.CertificationName,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldEmployeeLanguage in oldEmployeeLanguages)
                    {
                        if (!newEmployeeLanguages.Any(o => o.EmployeeLanguageGuid == oldEmployeeLanguage.EmployeeLanguageGuid))
                        {
                            await repository.EmployeeLanguage.SoftDeleteEmployeeLanguageAsync(
                                new EmployeeLanguage
                                {
                                    EmployeeLanguageGuid = oldEmployeeLanguage.EmployeeLanguageGuid,
                                    EmployeeLanguageId = oldEmployeeLanguage.EmployeeLanguageId,
                                    EmployeeId = oldEmployeeLanguage.EmployeeId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeLanguage",
                                EntityKey = oldEmployeeLanguage.EmployeeLanguageGuid.ToString(),
                                EntityDisplayName = oldEmployeeLanguage.CertificationName,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldEmployeeLanguage,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.EmployeeSkills != null)
                {
                    var newEmployeeSkills = input.EmployeeSkills.ToList();
                    var oldEmployeeSkillDict = oldEmployeeSkills.ToDictionary(o => o.EmployeeSkillGuid);

                    foreach (var detailDto in newEmployeeSkills)
                    {
                        var detail = detailDto.Adapt<EmployeeSkill>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.EmployeeSkillGuid != Guid.Empty && oldEmployeeSkillDict.ContainsKey(detailDto.EmployeeSkillGuid))
                        {
                            detail.EmployeeSkillGuid = detailDto.EmployeeSkillGuid;
                            detail.EmployeeSkillId = oldEmployeeSkillDict[detailDto.EmployeeSkillGuid].EmployeeSkillId;
                            await repository.EmployeeSkill.UpdateEmployeeSkillAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeSkill",
                                EntityKey = detail.EmployeeSkillGuid.ToString(),
                                EntityDisplayName = detail.CertificateNumber,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldEmployeeSkillDict[detailDto.EmployeeSkillGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.EmployeeSkill.CreateEmployeeSkillAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeSkill",
                                EntityKey = detail.EmployeeSkillGuid.ToString(),
                                EntityDisplayName = detail.CertificateNumber,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldEmployeeSkill in oldEmployeeSkills)
                    {
                        if (!newEmployeeSkills.Any(o => o.EmployeeSkillGuid == oldEmployeeSkill.EmployeeSkillGuid))
                        {
                            await repository.EmployeeSkill.SoftDeleteEmployeeSkillAsync(
                                new EmployeeSkill
                                {
                                    EmployeeSkillGuid = oldEmployeeSkill.EmployeeSkillGuid,
                                    EmployeeSkillId = oldEmployeeSkill.EmployeeSkillId,
                                    EmployeeId = oldEmployeeSkill.EmployeeId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeSkill",
                                EntityKey = oldEmployeeSkill.EmployeeSkillGuid.ToString(),
                                EntityDisplayName = oldEmployeeSkill.CertificateNumber,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldEmployeeSkill,
                                NewEntity = null
                            });
                        }
                    }
                }

                                if (input.EmployeeTrainings != null)
                {
                    var newEmployeeTrainings = input.EmployeeTrainings.ToList();
                    var oldEmployeeTrainingDict = oldEmployeeTrainings.ToDictionary(o => o.EmployeeTrainingGuid);

                    foreach (var detailDto in newEmployeeTrainings)
                    {
                        var detail = detailDto.Adapt<EmployeeTraining>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.EmployeeTrainingGuid != Guid.Empty && oldEmployeeTrainingDict.ContainsKey(detailDto.EmployeeTrainingGuid))
                        {
                            detail.EmployeeTrainingGuid = detailDto.EmployeeTrainingGuid;
                            detail.EmployeeTrainingId = oldEmployeeTrainingDict[detailDto.EmployeeTrainingGuid].EmployeeTrainingId;
                            await repository.EmployeeTraining.UpdateEmployeeTrainingAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeTraining",
                                EntityKey = detail.EmployeeTrainingGuid.ToString(),
                                EntityDisplayName = detail.Grade,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldEmployeeTrainingDict[detailDto.EmployeeTrainingGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.EmployeeTraining.CreateEmployeeTrainingAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeTraining",
                                EntityKey = detail.EmployeeTrainingGuid.ToString(),
                                EntityDisplayName = detail.Grade,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldEmployeeTraining in oldEmployeeTrainings)
                    {
                        if (!newEmployeeTrainings.Any(o => o.EmployeeTrainingGuid == oldEmployeeTraining.EmployeeTrainingGuid))
                        {
                            await repository.EmployeeTraining.SoftDeleteEmployeeTrainingAsync(
                                new EmployeeTraining
                                {
                                    EmployeeTrainingGuid = oldEmployeeTraining.EmployeeTrainingGuid,
                                    EmployeeTrainingId = oldEmployeeTraining.EmployeeTrainingId,
                                    EmployeeId = oldEmployeeTraining.EmployeeId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeTraining",
                                EntityKey = oldEmployeeTraining.EmployeeTrainingGuid.ToString(),
                                EntityDisplayName = oldEmployeeTraining.Grade,
                                ParentTableName = "Employee",
                                ParentEntityKey = model.EmployeeGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldEmployeeTraining,
                                NewEntity = null
                            });
                        }
                    }
                }

                // ##HeaderDetailUpdateDiff##
                if (input.EmployeeAssets != null)
                {
                    var newEmployeeAssets = input.EmployeeAssets.ToList();
                    var oldEmployeeAssetDict = oldEmployeeAssets.ToDictionary(o => o.EmployeeAssetGuid);

                    foreach (var detailDto in newEmployeeAssets)
                    {
                        var detail = detailDto.Adapt<EmployeeAsset>();
                        detail.EmployeeId = model.EmployeeId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.EmployeeAssetGuid != Guid.Empty && oldEmployeeAssetDict.TryGetValue(detailDto.EmployeeAssetGuid, out var oldDetailRow))
                        {
                            detail.EmployeeAssetGuid = detailDto.EmployeeAssetGuid;
                            detail.UpdatedById = input.UpdatedById;
                            await repository.EmployeeAsset.UpdateEmployeeAssetAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeAsset",
                                EntityKey = detail.EmployeeAssetGuid.ToString(),
                                EntityDisplayName = detail.ConditionWhenAssigned,
                                ParentTableName = "Employee",
                                ParentEntityKey = employeeGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldDetailRow,
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.EmployeeAssetGuid = Guid.NewGuid();
                            detail.CreatedById = input.UpdatedById;
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.EmployeeAsset.CreateEmployeeAssetAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeAsset",
                                EntityKey = detail.EmployeeAssetGuid.ToString(),
                                EntityDisplayName = detail.ConditionWhenAssigned,
                                ParentTableName = "Employee",
                                ParentEntityKey = employeeGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldEmployeeAsset in oldEmployeeAssets)
                    {
                        if (!newEmployeeAssets.Any(o => o.EmployeeAssetGuid == oldEmployeeAsset.EmployeeAssetGuid))
                        {
                            await repository.EmployeeAsset.SoftDeleteEmployeeAssetAsync(
                                new EmployeeAsset
                                {
                                    EmployeeAssetGuid = oldEmployeeAsset.EmployeeAssetGuid,
                                    DeletedById = input.UpdatedById,
                                    DeletedTime = DateTime.UtcNow
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "EmployeeAsset",
                                EntityKey = oldEmployeeAsset.EmployeeAssetGuid.ToString(),
                                EntityDisplayName = oldEmployeeAsset.ConditionWhenAssigned,
                                ParentTableName = "Employee",
                                ParentEntityKey = employeeGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldEmployeeAsset,
                                NewEntity = null
                            });
                        }
                    }
                }

                // 4. Single audit session for all entities
                await audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "UPDATE",
                    RootTableName = "Employee",
                    RootEntityKey = model.EmployeeGuid.ToString(),
                    RootDisplayName = model.EmployeeNo,
                    UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.UpdatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();
            }
            catch (Exception ex)
            {
                await transactionManager.RollbackAsync();
                logger.LogError($"Error in UpdateEmployeeAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteEmployeeAsync(Guid employeeGuid, EmployeeForDeleteDto input, bool trackChanges)
        {
            var oldEmployee = await repository.Employee.GetEmployeeAsync(employeeGuid, false);
            var model = new Employee
            {
                EmployeeGuid = employeeGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Employee.SoftDeleteEmployeeAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Employee",
                RootEntityKey = employeeGuid.ToString(),
                RootDisplayName = oldEmployee?.EmployeeNo,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Employee",
                        EntityKey = employeeGuid.ToString(),
                        EntityDisplayName = oldEmployee?.EmployeeNo,
                        ActionType = "DELETE",
                        OldEntity = oldEmployee,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteEmployeeByAdminAsync(Guid employeeGuid, bool trackChanges)
        {
            await repository.Employee.DeleteEmployeeAsync(employeeGuid);
        }

        public async Task<IEnumerable<EmployeeDto>> SearchEmployeeAsync(
            string? employeeNo, string? employeeNoSearchType, string? hireDate, string? hireDateSearchType, string? probationEndDate, string? probationEndDateSearchType, string? confirmationDate, string? confirmationDateSearchType, string? terminationDate, string? terminationDateSearchType
            // ── FK Virtual Search Params ──
        )
        {
            var data = await repository.Employee.SearchEmployeeAsync(
employeeNo, employeeNoSearchType, hireDate, hireDateSearchType, probationEndDate, probationEndDateSearchType, confirmationDate, confirmationDateSearchType, terminationDate, terminationDateSearchType
                // ── FK Virtual Search Args ──
            );
            return data.Adapt<IEnumerable<EmployeeDto>>();
        }
    }
}
