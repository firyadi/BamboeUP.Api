using Shared.DataTransferObjects;
using Entities.Models;
using Mapster;

namespace BamboeUp.Api
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            // ##MapsterConfig##
            TypeAdapterConfig<FileStorage, FileStorageDto>.NewConfig();
            TypeAdapterConfig<FileStorageForCreationDto, FileStorage>.NewConfig();
            TypeAdapterConfig<FileStorageForUpdateDto, FileStorage>.NewConfig();
            TypeAdapterConfig<PersonWorkExperience, PersonWorkExperienceDto>.NewConfig();
            TypeAdapterConfig<PersonWorkExperienceForCreationDto, PersonWorkExperience>.NewConfig();
            TypeAdapterConfig<PersonWorkExperienceForUpdateDto, PersonWorkExperience>.NewConfig();
            TypeAdapterConfig<PersonPhysicalCharacteristic, PersonPhysicalCharacteristicDto>.NewConfig();
            TypeAdapterConfig<PersonPhysicalCharacteristicForCreationDto, PersonPhysicalCharacteristic>.NewConfig();
            TypeAdapterConfig<PersonPhysicalCharacteristicForUpdateDto, PersonPhysicalCharacteristic>.NewConfig();
            TypeAdapterConfig<PersonFamily, PersonFamilyDto>.NewConfig();
            TypeAdapterConfig<PersonFamilyForCreationDto, PersonFamily>.NewConfig();
            TypeAdapterConfig<PersonFamilyForUpdateDto, PersonFamily>.NewConfig();
            TypeAdapterConfig<PersonContact, PersonContactDto>.NewConfig();
            TypeAdapterConfig<PersonContactForCreationDto, PersonContact>.NewConfig();
            TypeAdapterConfig<PersonContactForUpdateDto, PersonContact>.NewConfig();
            TypeAdapterConfig<PersonEmergencyContact, PersonEmergencyContactDto>.NewConfig();
            TypeAdapterConfig<PersonEmergencyContactForCreationDto, PersonEmergencyContact>.NewConfig();
            TypeAdapterConfig<PersonEmergencyContactForUpdateDto, PersonEmergencyContact>.NewConfig();
            TypeAdapterConfig<PersonEducation, PersonEducationDto>.NewConfig();
            TypeAdapterConfig<PersonEducationForCreationDto, PersonEducation>.NewConfig();
            TypeAdapterConfig<PersonEducationForUpdateDto, PersonEducation>.NewConfig();
            TypeAdapterConfig<PersonIdentification, PersonIdentificationDto>.NewConfig();
            TypeAdapterConfig<PersonIdentificationForCreationDto, PersonIdentification>.NewConfig();
            TypeAdapterConfig<PersonIdentificationForUpdateDto, PersonIdentification>.NewConfig();
            TypeAdapterConfig<PersonAddress, PersonAddressDto>.NewConfig();
            TypeAdapterConfig<PersonAddressForCreationDto, PersonAddress>.NewConfig();
            TypeAdapterConfig<PersonAddressForUpdateDto, PersonAddress>.NewConfig();
            TypeAdapterConfig<Person, PersonDto>.NewConfig();
            TypeAdapterConfig<PersonForCreationDto, Person>.NewConfig();
            TypeAdapterConfig<PersonForUpdateDto, Person>.NewConfig();
            TypeAdapterConfig<Award, AwardDto>.NewConfig();
            TypeAdapterConfig<AwardForCreationDto, Award>.NewConfig();
            TypeAdapterConfig<AwardForUpdateDto, Award>.NewConfig();
            TypeAdapterConfig<CostCenterScope, CostCenterScopeDto>.NewConfig();
            TypeAdapterConfig<CostCenterScopeForCreationDto, CostCenterScope>.NewConfig();
            TypeAdapterConfig<CostCenterScopeForUpdateDto, CostCenterScope>.NewConfig();
            TypeAdapterConfig<CostCenterAssignment, CostCenterAssignmentDto>.NewConfig();
            TypeAdapterConfig<CostCenterAssignmentForCreationDto, CostCenterAssignment>.NewConfig();
            TypeAdapterConfig<CostCenterAssignmentForUpdateDto, CostCenterAssignment>.NewConfig();
            TypeAdapterConfig<CostCenter, CostCenterDto>.NewConfig();
            TypeAdapterConfig<CostCenterForCreationDto, CostCenter>.NewConfig();
            TypeAdapterConfig<CostCenterForUpdateDto, CostCenter>.NewConfig();
            TypeAdapterConfig<OrganizationUnitScope, OrganizationUnitScopeDto>.NewConfig();
            TypeAdapterConfig<OrganizationUnitScopeForCreationDto, OrganizationUnitScope>.NewConfig();
            TypeAdapterConfig<OrganizationUnitScopeForUpdateDto, OrganizationUnitScope>.NewConfig();
            TypeAdapterConfig<OrganizationUnit, OrganizationUnitDto>.NewConfig();
            TypeAdapterConfig<OrganizationUnitForCreationDto, OrganizationUnit>.NewConfig()
                .Map(dest => dest.ParentOrganizationUnitId, src => !src.ParentOrganizationUnitId.HasValue || src.ParentOrganizationUnitId.Value == 0 ? null : src.ParentOrganizationUnitId);
            TypeAdapterConfig<OrganizationUnitForUpdateDto, OrganizationUnit>.NewConfig()
                .Map(dest => dest.ParentOrganizationUnitId, src => !src.ParentOrganizationUnitId.HasValue || src.ParentOrganizationUnitId.Value == 0 ? null : src.ParentOrganizationUnitId);
            TypeAdapterConfig<Hospital, HospitalDto>.NewConfig();
            TypeAdapterConfig<HospitalForCreationDto, Hospital>.NewConfig();
            TypeAdapterConfig<HospitalForUpdateDto, Hospital>.NewConfig();
            TypeAdapterConfig<Parameterscope, ParameterscopeDto>.NewConfig();
            TypeAdapterConfig<ParameterscopeForCreationDto, Parameterscope>.NewConfig();
            TypeAdapterConfig<ParameterscopeForUpdateDto, Parameterscope>.NewConfig();
            TypeAdapterConfig<Parameter, ParameterDto>.NewConfig();
            TypeAdapterConfig<ParameterForCreationDto, Parameter>.NewConfig();
            TypeAdapterConfig<ParameterForUpdateDto, Parameter>.NewConfig();
        }
    }
}