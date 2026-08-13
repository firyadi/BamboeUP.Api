using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeDto
    {
        public long EmployeeId { get; set; }
        public Guid EmployeeGuid { get; init; }
        public long PersonId { get; set; }
        public string EmployeeNo { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public long SrEmploymentStatus { get; set; }
        public long SrEmploymentType { get; set; }
        public long? SrRemunerationType { get; set; }
public string? PersonName { get; set; }
public string? EmploymentStatusName { get; set; }
public string? EmploymentTypeName { get; set; }
public string? RemunerationTypeName { get; set; }

public string? PersonFileStorageId { get; set; }
public string? BirthDate { get; set; }

                public IEnumerable<EmployeeAssignmentDto>? EmployeeAssignments { get; set; }
                public IEnumerable<EmployeeCareerDto>? EmployeeCareers { get; set; }
                public IEnumerable<EmployeeCertificationDto>? EmployeeCertifications { get; set; }
                public IEnumerable<EmployeeContractDto>? EmployeeContracts { get; set; }
                public IEnumerable<EmployeeLanguageDto>? EmployeeLanguages { get; set; }
                public IEnumerable<EmployeeSkillDto>? EmployeeSkills { get; set; }
                public IEnumerable<EmployeeTrainingDto>? EmployeeTrainings { get; set; }
        // ##HeaderDetailCollections##
        public IEnumerable<EmployeeAssetDto>? EmployeeAssets { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record EmployeeForCreationDto
    {
        public long PersonId { get; set; }
        public string EmployeeNo { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public long SrEmploymentStatus { get; set; }
        public long SrEmploymentType { get; set; }
        public long? SrRemunerationType { get; set; }
                public IEnumerable<EmployeeAssignmentForCreationDto>? EmployeeAssignments { get; set; }
                public IEnumerable<EmployeeCareerForCreationDto>? EmployeeCareers { get; set; }
                public IEnumerable<EmployeeCertificationForCreationDto>? EmployeeCertifications { get; set; }
                public IEnumerable<EmployeeContractForCreationDto>? EmployeeContracts { get; set; }
                public IEnumerable<EmployeeLanguageForCreationDto>? EmployeeLanguages { get; set; }
                public IEnumerable<EmployeeSkillForCreationDto>? EmployeeSkills { get; set; }
                public IEnumerable<EmployeeTrainingForCreationDto>? EmployeeTrainings { get; set; }
        // ##HeaderDetailCreateCollections##
        public IEnumerable<EmployeeAssetForCreationDto>? EmployeeAssets { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record EmployeeForUpdateDto
    {
        public long PersonId { get; set; }
        public string EmployeeNo { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public long SrEmploymentStatus { get; set; }
        public long SrEmploymentType { get; set; }
        public long? SrRemunerationType { get; set; }
                public IEnumerable<EmployeeAssignmentForUpdateDto>? EmployeeAssignments { get; set; }
                public IEnumerable<EmployeeCareerForUpdateDto>? EmployeeCareers { get; set; }
                public IEnumerable<EmployeeCertificationForUpdateDto>? EmployeeCertifications { get; set; }
                public IEnumerable<EmployeeContractForUpdateDto>? EmployeeContracts { get; set; }
                public IEnumerable<EmployeeLanguageForUpdateDto>? EmployeeLanguages { get; set; }
                public IEnumerable<EmployeeSkillForUpdateDto>? EmployeeSkills { get; set; }
                public IEnumerable<EmployeeTrainingForUpdateDto>? EmployeeTrainings { get; set; }
        // ##HeaderDetailUpdateCollections##
        public IEnumerable<EmployeeAssetForUpdateDto>? EmployeeAssets { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record EmployeeForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class EmployeeSearchDto
    {
        public string? EmployeeNo { get; set; }
        public SearchType EmployeeNoSearchType { get; set; } = SearchType.Contains;

        public string? HireDate { get; set; }
        public SearchType HireDateSearchType { get; set; } = SearchType.Contains;

        public string? ProbationEndDate { get; set; }
        public SearchType ProbationEndDateSearchType { get; set; } = SearchType.Contains;

        public string? ConfirmationDate { get; set; }
        public SearchType ConfirmationDateSearchType { get; set; } = SearchType.Contains;

        public string? TerminationDate { get; set; }
        public SearchType TerminationDateSearchType { get; set; } = SearchType.Contains;
public string? PersonName { get; set; }
public string? EmploymentStatusName { get; set; }
public string? EmploymentTypeName { get; set; }
public string? RemunerationTypeName { get; set; }
public SearchType PersonNameSearchType { get; set; } = SearchType.Contains;
public SearchType EmploymentStatusNameSearchType { get; set; } = SearchType.Contains;
public SearchType EmploymentTypeNameSearchType { get; set; } = SearchType.Contains;
public SearchType RemunerationTypeNameSearchType { get; set; } = SearchType.Contains;

    }
}
