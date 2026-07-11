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