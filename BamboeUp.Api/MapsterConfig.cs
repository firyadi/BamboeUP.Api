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