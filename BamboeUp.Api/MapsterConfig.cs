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
            TypeAdapterConfig<EmployeeTraining, EmployeeTrainingDto>.NewConfig();
            TypeAdapterConfig<EmployeeTrainingForCreationDto, EmployeeTraining>.NewConfig();
            TypeAdapterConfig<EmployeeTrainingForUpdateDto, EmployeeTraining>.NewConfig();
            TypeAdapterConfig<EmployeeSkill, EmployeeSkillDto>.NewConfig();
            TypeAdapterConfig<EmployeeSkillForCreationDto, EmployeeSkill>.NewConfig();
            TypeAdapterConfig<EmployeeSkillForUpdateDto, EmployeeSkill>.NewConfig();
            TypeAdapterConfig<EmployeeLanguage, EmployeeLanguageDto>.NewConfig();
            TypeAdapterConfig<EmployeeLanguageForCreationDto, EmployeeLanguage>.NewConfig();
            TypeAdapterConfig<EmployeeLanguageForUpdateDto, EmployeeLanguage>.NewConfig();
            TypeAdapterConfig<EmployeeContract, EmployeeContractDto>.NewConfig();
            TypeAdapterConfig<EmployeeContractForCreationDto, EmployeeContract>.NewConfig();
            TypeAdapterConfig<EmployeeContractForUpdateDto, EmployeeContract>.NewConfig();
            TypeAdapterConfig<EmployeeCertification, EmployeeCertificationDto>.NewConfig();
            TypeAdapterConfig<EmployeeCertificationForCreationDto, EmployeeCertification>.NewConfig();
            TypeAdapterConfig<EmployeeCertificationForUpdateDto, EmployeeCertification>.NewConfig();
            TypeAdapterConfig<EmployeeCareer, EmployeeCareerDto>.NewConfig();
            TypeAdapterConfig<EmployeeCareerForCreationDto, EmployeeCareer>.NewConfig();
            TypeAdapterConfig<EmployeeCareerForUpdateDto, EmployeeCareer>.NewConfig();
            TypeAdapterConfig<EmployeeAssignment, EmployeeAssignmentDto>.NewConfig();
            TypeAdapterConfig<EmployeeAssignmentForCreationDto, EmployeeAssignment>.NewConfig();
            TypeAdapterConfig<EmployeeAssignmentForUpdateDto, EmployeeAssignment>.NewConfig();
            TypeAdapterConfig<EmployeeAsset, EmployeeAssetDto>.NewConfig();
            TypeAdapterConfig<EmployeeAssetForCreationDto, EmployeeAsset>.NewConfig();
            TypeAdapterConfig<EmployeeAssetForUpdateDto, EmployeeAsset>.NewConfig();
            TypeAdapterConfig<Employee, EmployeeDto>.NewConfig();
            TypeAdapterConfig<EmployeeForCreationDto, Employee>.NewConfig();
            TypeAdapterConfig<EmployeeForUpdateDto, Employee>.NewConfig();
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
            TypeAdapterConfig<CostCenterAssignment, CostCenterAssignmentDto>.NewConfig();
            TypeAdapterConfig<CostCenterAssignmentForCreationDto, CostCenterAssignment>.NewConfig();
            TypeAdapterConfig<CostCenterAssignmentForUpdateDto, CostCenterAssignment>.NewConfig();
            // CompanyLogo is varbinary(max) in SQL. Keep binary data in the
            // entity and use Base64 only at the DTO/API boundary.
            TypeAdapterConfig<Company, CompanyDto>.NewConfig()
                .Map(dest => dest.CompanyLogo, src => ToBase64(src.CompanyLogo));
            TypeAdapterConfig<CompanyForCreationDto, Company>.NewConfig()
                .Map(dest => dest.CompanyLogo, src => FromBase64(src.CompanyLogo));
            TypeAdapterConfig<CompanyForUpdateDto, Company>.NewConfig()
                .Map(dest => dest.CompanyLogo, src => FromBase64(src.CompanyLogo));
            TypeAdapterConfig<AwardCriteriaRule, AwardCriteriaRuleDto>.NewConfig();
            TypeAdapterConfig<AwardCriteriaRuleForCreationDto, AwardCriteriaRule>.NewConfig();
            TypeAdapterConfig<AwardCriteriaRuleForUpdateDto, AwardCriteriaRule>.NewConfig();
            TypeAdapterConfig<AwardCriteria, AwardCriteriaDto>.NewConfig();
            TypeAdapterConfig<AwardCriteriaForCreationDto, AwardCriteria>.NewConfig();
            TypeAdapterConfig<AwardCriteriaForUpdateDto, AwardCriteria>.NewConfig();
            TypeAdapterConfig<Award, AwardDto>.NewConfig();
            TypeAdapterConfig<AwardForCreationDto, Award>.NewConfig();
            TypeAdapterConfig<AwardForUpdateDto, Award>.NewConfig();
            TypeAdapterConfig<Holiday, HolidayDto>.NewConfig();
            TypeAdapterConfig<HolidayForCreationDto, Holiday>.NewConfig();
            TypeAdapterConfig<HolidayForUpdateDto, Holiday>.NewConfig();
            TypeAdapterConfig<Certification, CertificationDto>.NewConfig();
            TypeAdapterConfig<CertificationForCreationDto, Certification>.NewConfig();
            TypeAdapterConfig<CertificationForUpdateDto, Certification>.NewConfig();
            TypeAdapterConfig<Asset, AssetDto>.NewConfig();
            TypeAdapterConfig<AssetForCreationDto, Asset>.NewConfig();
            TypeAdapterConfig<AssetForUpdateDto, Asset>.NewConfig();
            TypeAdapterConfig<Training, TrainingDto>.NewConfig();
            TypeAdapterConfig<TrainingForCreationDto, Training>.NewConfig();
            TypeAdapterConfig<TrainingForUpdateDto, Training>.NewConfig();
            TypeAdapterConfig<Skill, SkillDto>.NewConfig();
            TypeAdapterConfig<SkillForCreationDto, Skill>.NewConfig();
            TypeAdapterConfig<SkillForUpdateDto, Skill>.NewConfig();
            TypeAdapterConfig<PersonBankAccount, PersonBankAccountDto>.NewConfig();
            TypeAdapterConfig<PersonBankAccountForCreationDto, PersonBankAccount>.NewConfig();
            TypeAdapterConfig<PersonBankAccountForUpdateDto, PersonBankAccount>.NewConfig();
            TypeAdapterConfig<FileStorage, FileStorageDto>.NewConfig();
            TypeAdapterConfig<FileStorageForCreationDto, FileStorage>.NewConfig();
            TypeAdapterConfig<FileStorageForUpdateDto, FileStorage>.NewConfig();
            TypeAdapterConfig<CostCenterScope, CostCenterScopeDto>.NewConfig();
            TypeAdapterConfig<CostCenterScopeForCreationDto, CostCenterScope>.NewConfig();
            TypeAdapterConfig<CostCenterScopeForUpdateDto, CostCenterScope>.NewConfig();
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

        private static string? ToBase64(byte[]? bytes) =>
            bytes is { Length: > 0 } ? Convert.ToBase64String(bytes) : null;

        private static byte[]? FromBase64(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var raw = value.Trim();
            var comma = raw.IndexOf(',');
            if (raw.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma > 0)
                raw = raw[(comma + 1)..];

            try
            {
                return Convert.FromBase64String(raw);
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}