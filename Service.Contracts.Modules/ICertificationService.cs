using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface ICertificationService
    {
        Task<IEnumerable<CertificationDto>> GetAllCertificationsAsync(bool trackChanges);
        Task<CertificationDto?> GetCertificationByGuidAsync(Guid certificationGuid, bool trackChanges);
        Task<CertificationDto> CreateCertificationAsync(CertificationForCreationDto input);
        Task UpdateCertificationAsync(Guid certificationGuid, CertificationForUpdateDto input, bool trackChanges);
        Task DeleteCertificationAsync(Guid certificationGuid, CertificationForDeleteDto input, bool trackChanges);
        Task DeleteCertificationByAdminAsync(Guid certificationGuid, bool trackChanges);

        Task<IEnumerable<CertificationDto>> SearchCertificationAsync(
            string? certificationCode, string? certificationCodeSearchType, string? certificationName, string? certificationNameSearchType, string? certificationShortName, string? certificationShortNameSearchType, string? defaultValidityMonth, string? defaultValidityMonthSearchType, string? isLifetime, string? isLifetimeSearchType, string? isRenewable, string? isRenewableSearchType, string? description, string? descriptionSearchType, string? isActive, string? isActiveSearchType
, string? certificationCategoryName, string? certificationCategoryNameSearchType
, string? certificationProviderName, string? certificationProviderNameSearchType

        );
    }
}
