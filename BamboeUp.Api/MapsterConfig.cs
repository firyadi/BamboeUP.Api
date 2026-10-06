using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Approval;
using Entities.Models;
using Entities.Models.Approval;
using Mapster;

namespace BamboeUp.Api
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            // ##MapsterConfig##
            TypeAdapterConfig<AdministrativeAreaDisplay, AdministrativeAreaDisplayDto>.NewConfig();
            TypeAdapterConfig<ApprovalDelegation, ApprovalDelegationDto>.NewConfig();
            TypeAdapterConfig<ApprovalDelegationForCreationDto, ApprovalDelegation>.NewConfig();
            TypeAdapterConfig<ApprovalDelegationForUpdateDto, ApprovalDelegation>.NewConfig();
            TypeAdapterConfig<ApprovalHistory, ApprovalHistoryDto>.NewConfig();
            TypeAdapterConfig<ApprovalRequest, ApprovalRequestDto>.NewConfig();
            TypeAdapterConfig<ApprovalRequestForCreationDto, ApprovalRequest>.NewConfig();
            TypeAdapterConfig<ApprovalStep, ApprovalStepDto>.NewConfig();
            TypeAdapterConfig<ApprovalTemplate, ApprovalTemplateDto>.NewConfig();
            TypeAdapterConfig<ApprovalTemplateForCreationDto, ApprovalTemplate>.NewConfig();
            TypeAdapterConfig<ApprovalTemplateForUpdateDto, ApprovalTemplate>.NewConfig();
            TypeAdapterConfig<ApprovalTemplateLevel, ApprovalTemplateLevelDto>.NewConfig();
            TypeAdapterConfig<ApprovalTemplateLevelForCreationDto, ApprovalTemplateLevel>.NewConfig();
            TypeAdapterConfig<ApprovalTemplateLevelApprover, ApprovalTemplateLevelApproverDto>.NewConfig();
            TypeAdapterConfig<ApprovalTemplateLevelApproverForCreationDto, ApprovalTemplateLevelApprover>.NewConfig();
            TypeAdapterConfig<Asset, AssetDto>.NewConfig();
            TypeAdapterConfig<AssetForCreationDto, Asset>.NewConfig();
            TypeAdapterConfig<AssetForUpdateDto, Asset>.NewConfig();
            TypeAdapterConfig<Attendance, AttendanceDto>.NewConfig();
            TypeAdapterConfig<AttendanceForCreationDto, Attendance>.NewConfig();
            TypeAdapterConfig<AttendanceForUpdateDto, Attendance>.NewConfig();
            TypeAdapterConfig<AutoNumber, AutoNumberDto>.NewConfig();
            TypeAdapterConfig<AutoNumberForCreationDto, AutoNumber>.NewConfig();
            TypeAdapterConfig<AutoNumberForUpdateDto, AutoNumber>.NewConfig();
            TypeAdapterConfig<AutoNumberComponent, AutoNumberComponentDto>.NewConfig();
            TypeAdapterConfig<AutoNumberComponentForCreationDto, AutoNumberComponent>.NewConfig();
            TypeAdapterConfig<AutoNumberComponentForUpdateDto, AutoNumberComponent>.NewConfig();
            TypeAdapterConfig<AutoNumberCounter, AutoNumberCounterDto>.NewConfig();
            TypeAdapterConfig<AutoNumberCounterForCreationDto, AutoNumberCounter>.NewConfig();
            TypeAdapterConfig<AutoNumberCounterForUpdateDto, AutoNumberCounter>.NewConfig();
            TypeAdapterConfig<AutoNumberLog, AutoNumberLogDto>.NewConfig();
            TypeAdapterConfig<AutoNumberTemplate, AutoNumberTemplateDto>.NewConfig();
            TypeAdapterConfig<AutoNumberTemplateForCreationDto, AutoNumberTemplate>.NewConfig();
            TypeAdapterConfig<AutoNumberTemplateForUpdateDto, AutoNumberTemplate>.NewConfig();
            TypeAdapterConfig<Award, AwardDto>.NewConfig();
            TypeAdapterConfig<AwardForCreationDto, Award>.NewConfig();
            TypeAdapterConfig<AwardForUpdateDto, Award>.NewConfig();
            TypeAdapterConfig<AwardCriteria, AwardCriteriaDto>.NewConfig();
            TypeAdapterConfig<AwardCriteriaForCreationDto, AwardCriteria>.NewConfig();
            TypeAdapterConfig<AwardCriteriaForUpdateDto, AwardCriteria>.NewConfig();
            TypeAdapterConfig<AwardCriteriaRule, AwardCriteriaRuleDto>.NewConfig();
            TypeAdapterConfig<AwardCriteriaRuleForCreationDto, AwardCriteriaRule>.NewConfig();
            TypeAdapterConfig<AwardCriteriaRuleForUpdateDto, AwardCriteriaRule>.NewConfig();
            TypeAdapterConfig<Bank, BankDto>.NewConfig();
            TypeAdapterConfig<BankForCreationDto, Bank>.NewConfig();
            TypeAdapterConfig<BankForUpdateDto, Bank>.NewConfig();
            TypeAdapterConfig<Certification, CertificationDto>.NewConfig();
            TypeAdapterConfig<CertificationForCreationDto, Certification>.NewConfig();
            TypeAdapterConfig<CertificationForUpdateDto, Certification>.NewConfig();
            TypeAdapterConfig<City, CityDto>.NewConfig();
            TypeAdapterConfig<CityForCreationDto, City>.NewConfig();
            TypeAdapterConfig<CityForUpdateDto, City>.NewConfig();
            TypeAdapterConfig<Company, CompanyDto>.NewConfig();
            TypeAdapterConfig<CompanyForCreationDto, Company>.NewConfig();
            TypeAdapterConfig<CompanyForUpdateDto, Company>.NewConfig();
            TypeAdapterConfig<CompanyOffice, CompanyOfficeDto>.NewConfig();
            TypeAdapterConfig<CompanyOfficeForCreationDto, CompanyOffice>.NewConfig();
            TypeAdapterConfig<CompanyOfficeForUpdateDto, CompanyOffice>.NewConfig();
            TypeAdapterConfig<CostCenter, CostCenterDto>.NewConfig();
            TypeAdapterConfig<CostCenterForCreationDto, CostCenter>.NewConfig();
            TypeAdapterConfig<CostCenterForUpdateDto, CostCenter>.NewConfig();
            TypeAdapterConfig<CostCenterAssignment, CostCenterAssignmentDto>.NewConfig();
            TypeAdapterConfig<CostCenterAssignmentForCreationDto, CostCenterAssignment>.NewConfig();
            TypeAdapterConfig<CostCenterAssignmentForUpdateDto, CostCenterAssignment>.NewConfig();
            TypeAdapterConfig<CostCenterScope, CostCenterScopeDto>.NewConfig();
            TypeAdapterConfig<CostCenterScopeForCreationDto, CostCenterScope>.NewConfig();
            TypeAdapterConfig<CostCenterScopeForUpdateDto, CostCenterScope>.NewConfig();
            TypeAdapterConfig<Country, CountryDto>.NewConfig();
            TypeAdapterConfig<CountryForCreationDto, Country>.NewConfig();
            TypeAdapterConfig<CountryForUpdateDto, Country>.NewConfig();
            TypeAdapterConfig<District, DistrictDto>.NewConfig();
            TypeAdapterConfig<DistrictForCreationDto, District>.NewConfig();
            TypeAdapterConfig<DistrictForUpdateDto, District>.NewConfig();
            TypeAdapterConfig<DocumentNumberRequest, DocumentNumberRequestDto>.NewConfig();
            TypeAdapterConfig<DocumentNumberRequestForCreationDto, DocumentNumberRequest>.NewConfig();
            TypeAdapterConfig<DocumentNumberRequestForUpdateDto, DocumentNumberRequest>.NewConfig();
            TypeAdapterConfig<Employee, EmployeeDto>.NewConfig();
            TypeAdapterConfig<EmployeeForCreationDto, Employee>.NewConfig();
            TypeAdapterConfig<EmployeeForUpdateDto, Employee>.NewConfig();
            TypeAdapterConfig<EmployeeAsset, EmployeeAssetDto>.NewConfig();
            TypeAdapterConfig<EmployeeAssetForCreationDto, EmployeeAsset>.NewConfig();
            TypeAdapterConfig<EmployeeAssetForUpdateDto, EmployeeAsset>.NewConfig();
            TypeAdapterConfig<EmployeeAssignment, EmployeeAssignmentDto>.NewConfig();
            TypeAdapterConfig<EmployeeAssignmentForCreationDto, EmployeeAssignment>.NewConfig();
            TypeAdapterConfig<EmployeeAssignmentForUpdateDto, EmployeeAssignment>.NewConfig();
            TypeAdapterConfig<EmployeeCareer, EmployeeCareerDto>.NewConfig();
            TypeAdapterConfig<EmployeeCareerForCreationDto, EmployeeCareer>.NewConfig();
            TypeAdapterConfig<EmployeeCareerForUpdateDto, EmployeeCareer>.NewConfig();
            TypeAdapterConfig<EmployeeCertification, EmployeeCertificationDto>.NewConfig();
            TypeAdapterConfig<EmployeeCertificationForCreationDto, EmployeeCertification>.NewConfig();
            TypeAdapterConfig<EmployeeCertificationForUpdateDto, EmployeeCertification>.NewConfig();
            TypeAdapterConfig<EmployeeContract, EmployeeContractDto>.NewConfig();
            TypeAdapterConfig<EmployeeContractForCreationDto, EmployeeContract>.NewConfig();
            TypeAdapterConfig<EmployeeContractForUpdateDto, EmployeeContract>.NewConfig();
            TypeAdapterConfig<EmployeeLanguage, EmployeeLanguageDto>.NewConfig();
            TypeAdapterConfig<EmployeeLanguageForCreationDto, EmployeeLanguage>.NewConfig();
            TypeAdapterConfig<EmployeeLanguageForUpdateDto, EmployeeLanguage>.NewConfig();
            TypeAdapterConfig<EmployeeSkill, EmployeeSkillDto>.NewConfig();
            TypeAdapterConfig<EmployeeSkillForCreationDto, EmployeeSkill>.NewConfig();
            TypeAdapterConfig<EmployeeSkillForUpdateDto, EmployeeSkill>.NewConfig();
            TypeAdapterConfig<EmployeeTraining, EmployeeTrainingDto>.NewConfig();
            TypeAdapterConfig<EmployeeTrainingForCreationDto, EmployeeTraining>.NewConfig();
            TypeAdapterConfig<EmployeeTrainingForUpdateDto, EmployeeTraining>.NewConfig();
            TypeAdapterConfig<FileStorage, FileStorageDto>.NewConfig();
            TypeAdapterConfig<FileStorageForCreationDto, FileStorage>.NewConfig();
            TypeAdapterConfig<FileStorageForUpdateDto, FileStorage>.NewConfig();
            TypeAdapterConfig<Holiday, HolidayDto>.NewConfig();
            TypeAdapterConfig<HolidayForCreationDto, Holiday>.NewConfig();
            TypeAdapterConfig<HolidayForUpdateDto, Holiday>.NewConfig();
            TypeAdapterConfig<Hospital, HospitalDto>.NewConfig();
            TypeAdapterConfig<HospitalForCreationDto, Hospital>.NewConfig();
            TypeAdapterConfig<HospitalForUpdateDto, Hospital>.NewConfig();
            TypeAdapterConfig<MeetingRoom, MeetingRoomDto>.NewConfig();
            TypeAdapterConfig<MeetingRoomForCreationDto, MeetingRoom>.NewConfig();
            TypeAdapterConfig<MeetingRoomForUpdateDto, MeetingRoom>.NewConfig();
            TypeAdapterConfig<OrganizationUnit, OrganizationUnitDto>.NewConfig()
                .Map(dest => dest.ParentOrganizationUnitId, src => !src.ParentOrganizationUnitId.HasValue || src.ParentOrganizationUnitId.Value == 0 ? null : src.ParentOrganizationUnitId);
            TypeAdapterConfig<OrganizationUnitForCreationDto, OrganizationUnit>.NewConfig();
            TypeAdapterConfig<OrganizationUnitForUpdateDto, OrganizationUnit>.NewConfig()
                .Map(dest => dest.ParentOrganizationUnitId, src => !src.ParentOrganizationUnitId.HasValue || src.ParentOrganizationUnitId.Value == 0 ? null : src.ParentOrganizationUnitId);
            TypeAdapterConfig<OrganizationUnitScope, OrganizationUnitScopeDto>.NewConfig();
            TypeAdapterConfig<OrganizationUnitScopeForCreationDto, OrganizationUnitScope>.NewConfig();
            TypeAdapterConfig<OrganizationUnitScopeForUpdateDto, OrganizationUnitScope>.NewConfig();
            TypeAdapterConfig<OvertimeRequest, OvertimeRequestDto>.NewConfig();
            TypeAdapterConfig<OvertimeRequestForCreationDto, OvertimeRequest>.NewConfig();
            TypeAdapterConfig<OvertimeRequestForUpdateDto, OvertimeRequest>.NewConfig();
            TypeAdapterConfig<Parameter, ParameterDto>.NewConfig();
            TypeAdapterConfig<ParameterForCreationDto, Parameter>.NewConfig();
            TypeAdapterConfig<ParameterForUpdateDto, Parameter>.NewConfig();
            TypeAdapterConfig<Parameterscope, ParameterscopeDto>.NewConfig();
            TypeAdapterConfig<ParameterscopeForCreationDto, Parameterscope>.NewConfig();
            TypeAdapterConfig<ParameterscopeForUpdateDto, Parameterscope>.NewConfig();
            TypeAdapterConfig<Person, PersonDto>.NewConfig();
            TypeAdapterConfig<PersonForCreationDto, Person>.NewConfig();
            TypeAdapterConfig<PersonForUpdateDto, Person>.NewConfig();
            TypeAdapterConfig<PersonAddress, PersonAddressDto>.NewConfig();
            TypeAdapterConfig<PersonAddressForCreationDto, PersonAddress>.NewConfig();
            TypeAdapterConfig<PersonAddressForUpdateDto, PersonAddress>.NewConfig();
            TypeAdapterConfig<PersonBankAccount, PersonBankAccountDto>.NewConfig();
            TypeAdapterConfig<PersonBankAccountForCreationDto, PersonBankAccount>.NewConfig();
            TypeAdapterConfig<PersonBankAccountForUpdateDto, PersonBankAccount>.NewConfig();
            TypeAdapterConfig<PersonContact, PersonContactDto>.NewConfig();
            TypeAdapterConfig<PersonContactForCreationDto, PersonContact>.NewConfig();
            TypeAdapterConfig<PersonContactForUpdateDto, PersonContact>.NewConfig();
            TypeAdapterConfig<PersonEducation, PersonEducationDto>.NewConfig();
            TypeAdapterConfig<PersonEducationForCreationDto, PersonEducation>.NewConfig();
            TypeAdapterConfig<PersonEducationForUpdateDto, PersonEducation>.NewConfig();
            TypeAdapterConfig<PersonEmergencyContact, PersonEmergencyContactDto>.NewConfig();
            TypeAdapterConfig<PersonEmergencyContactForCreationDto, PersonEmergencyContact>.NewConfig();
            TypeAdapterConfig<PersonEmergencyContactForUpdateDto, PersonEmergencyContact>.NewConfig();
            TypeAdapterConfig<PersonFamily, PersonFamilyDto>.NewConfig();
            TypeAdapterConfig<PersonFamilyForCreationDto, PersonFamily>.NewConfig();
            TypeAdapterConfig<PersonFamilyForUpdateDto, PersonFamily>.NewConfig();
            TypeAdapterConfig<PersonIdentification, PersonIdentificationDto>.NewConfig();
            TypeAdapterConfig<PersonIdentificationForCreationDto, PersonIdentification>.NewConfig();
            TypeAdapterConfig<PersonIdentificationForUpdateDto, PersonIdentification>.NewConfig();
            TypeAdapterConfig<PersonPhysicalCharacteristic, PersonPhysicalCharacteristicDto>.NewConfig();
            TypeAdapterConfig<PersonPhysicalCharacteristicForCreationDto, PersonPhysicalCharacteristic>.NewConfig();
            TypeAdapterConfig<PersonPhysicalCharacteristicForUpdateDto, PersonPhysicalCharacteristic>.NewConfig();
            TypeAdapterConfig<Position, PositionDto>.NewConfig();
            TypeAdapterConfig<PositionForCreationDto, Position>.NewConfig();
            TypeAdapterConfig<PositionForUpdateDto, Position>.NewConfig();
            TypeAdapterConfig<PositionGrade, PositionGradeDto>.NewConfig();
            TypeAdapterConfig<PositionGradeForCreationDto, PositionGrade>.NewConfig();
            TypeAdapterConfig<PositionGradeForUpdateDto, PositionGrade>.NewConfig();
            TypeAdapterConfig<PositionLevel, PositionLevelDto>.NewConfig();
            TypeAdapterConfig<PositionLevelForCreationDto, PositionLevel>.NewConfig();
            TypeAdapterConfig<PositionLevelForUpdateDto, PositionLevel>.NewConfig();
            TypeAdapterConfig<PostalCode, PostalCodeDto>.NewConfig();
            TypeAdapterConfig<PostalCodeForCreationDto, PostalCode>.NewConfig();
            TypeAdapterConfig<PostalCodeForUpdateDto, PostalCode>.NewConfig();
            TypeAdapterConfig<Province, ProvinceDto>.NewConfig();
            TypeAdapterConfig<ProvinceForCreationDto, Province>.NewConfig();
            TypeAdapterConfig<ProvinceForUpdateDto, Province>.NewConfig();
            TypeAdapterConfig<RoomBooking, RoomBookingDto>.NewConfig();
            TypeAdapterConfig<RoomBookingForCreationDto, RoomBooking>.NewConfig();
            TypeAdapterConfig<RoomBookingForUpdateDto, RoomBooking>.NewConfig();
            TypeAdapterConfig<SalesAnalysis, SalesAnalysisDto>.NewConfig();
            TypeAdapterConfig<SalesAnalysisForCreationDto, SalesAnalysis>.NewConfig();
            TypeAdapterConfig<SalesAnalysisForUpdateDto, SalesAnalysis>.NewConfig();
            TypeAdapterConfig<SalesTransaction, SalesTransactionDto>.NewConfig();
            TypeAdapterConfig<SalesTransactionForCreationDto, SalesTransaction>.NewConfig();
            TypeAdapterConfig<SalesTransactionForUpdateDto, SalesTransaction>.NewConfig();
            TypeAdapterConfig<Skill, SkillDto>.NewConfig();
            TypeAdapterConfig<SkillForCreationDto, Skill>.NewConfig();
            TypeAdapterConfig<SkillForUpdateDto, Skill>.NewConfig();
            TypeAdapterConfig<StandardReference, StandardReferenceDto>.NewConfig();
            TypeAdapterConfig<StandardReferenceForCreationDto, StandardReference>.NewConfig();
            TypeAdapterConfig<StandardReferenceForUpdateDto, StandardReference>.NewConfig();
            TypeAdapterConfig<StandardReferenceDisplay, StandardReferenceDisplayDto>.NewConfig();
            TypeAdapterConfig<StandardReferenceItem, StandardReferenceItemDto>.NewConfig();
            TypeAdapterConfig<StandardReferenceItemForCreationDto, StandardReferenceItem>.NewConfig();
            TypeAdapterConfig<StandardReferenceItemForUpdateDto, StandardReferenceItem>.NewConfig();
            TypeAdapterConfig<StandardReferenceItemDisplay, StandardReferenceItemDisplayDto>.NewConfig();
            TypeAdapterConfig<StandardReferenceScope, StandardReferenceScopeDto>.NewConfig();
            TypeAdapterConfig<StandardReferenceScopeForCreationDto, StandardReferenceScope>.NewConfig();
            TypeAdapterConfig<StandardReferenceScopeForUpdateDto, StandardReferenceScope>.NewConfig();
            TypeAdapterConfig<StandardReferenceScopeItem, StandardReferenceScopeItemDto>.NewConfig();
            TypeAdapterConfig<StandardReferenceScopeItemForCreationDto, StandardReferenceScopeItem>.NewConfig();
            TypeAdapterConfig<StandardReferenceScopeItemForUpdateDto, StandardReferenceScopeItem>.NewConfig();
            TypeAdapterConfig<SubDistrict, SubDistrictDto>.NewConfig();
            TypeAdapterConfig<SubDistrictForCreationDto, SubDistrict>.NewConfig();
            TypeAdapterConfig<SubDistrictForUpdateDto, SubDistrict>.NewConfig();
            TypeAdapterConfig<TempContoh1, TempContoh1Dto>.NewConfig();
            TypeAdapterConfig<TempContoh1ForCreationDto, TempContoh1>.NewConfig();
            TypeAdapterConfig<TempContoh1ForUpdateDto, TempContoh1>.NewConfig();
            TypeAdapterConfig<Training, TrainingDto>.NewConfig();
            TypeAdapterConfig<TrainingForCreationDto, Training>.NewConfig();
            TypeAdapterConfig<TrainingForUpdateDto, Training>.NewConfig();
            TypeAdapterConfig<User, UserDto>.NewConfig();
            TypeAdapterConfig<UserForCreationDto, User>.NewConfig();
            TypeAdapterConfig<UserForUpdateDto, User>.NewConfig();
            TypeAdapterConfig<UserCompanyScope, UserCompanyScopeDto>.NewConfig();
            TypeAdapterConfig<UserCompanyScopeForCreationDto, UserCompanyScope>.NewConfig();
            TypeAdapterConfig<UserCompanyScopeForUpdateDto, UserCompanyScope>.NewConfig();
            TypeAdapterConfig<UserGroup, UserGroupDto>.NewConfig();
            TypeAdapterConfig<UserGroupForCreationDto, UserGroup>.NewConfig();
            TypeAdapterConfig<UserGroupForUpdateDto, UserGroup>.NewConfig();
            TypeAdapterConfig<UserGroupProgram, UserGroupProgramDto>.NewConfig();
            TypeAdapterConfig<UserGroupProgramForCreationDto, UserGroupProgram>.NewConfig();
            TypeAdapterConfig<UserGroupProgramForUpdateDto, UserGroupProgram>.NewConfig();
            TypeAdapterConfig<UserGroupScope, UserGroupScopeDto>.NewConfig();
            TypeAdapterConfig<UserGroupScopeForCreationDto, UserGroupScope>.NewConfig();
            TypeAdapterConfig<VwStandardReferenceItem, VwStandardReferenceItemDto>.NewConfig();
            TypeAdapterConfig<WorkItem, WorkItemDto>.NewConfig();
            TypeAdapterConfig<WorkItemForCreationDto, WorkItem>.NewConfig();
            TypeAdapterConfig<WorkItemForUpdateDto, WorkItem>.NewConfig();
            TypeAdapterConfig<WorkNotice, WorkNoticeDto>.NewConfig();
            TypeAdapterConfig<WorkNoticeForCreationDto, WorkNotice>.NewConfig();
            TypeAdapterConfig<WorkNoticeForUpdateDto, WorkNotice>.NewConfig();
            TypeAdapterConfig<WorkTeam, WorkTeamDto>.NewConfig();
            TypeAdapterConfig<WorkTeamForCreationDto, WorkTeam>.NewConfig();
            TypeAdapterConfig<WorkTeamForUpdateDto, WorkTeam>.NewConfig();
            TypeAdapterConfig<WorkTeamCapability, WorkTeamCapabilityDto>.NewConfig();
            TypeAdapterConfig<WorkTeamCapabilityForCreationDto, WorkTeamCapability>.NewConfig();
            TypeAdapterConfig<WorkTeamCapabilityForUpdateDto, WorkTeamCapability>.NewConfig();
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
