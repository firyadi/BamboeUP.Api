using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ICertificationRepository
    {
        Task<Certification?> GetCertificationAsync(Guid certificationGuid, bool trackChanges);
        Task<IEnumerable<Certification>> GetAllCertificationsAsync(bool trackChanges);

        Task CreateCertificationAsync(Certification certification, IDbTransaction? transaction = null);
        Task UpdateCertificationAsync(Certification certification, IDbTransaction? transaction = null);
        Task DeleteCertificationAsync(Guid certificationGuid, IDbTransaction? transaction = null);
        Task SoftDeleteCertificationAsync(Certification certification, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Certification>> SearchCertificationAsync(
            string? certificationCode,
            string? certificationCodeSearchType,
            string? certificationName,
            string? certificationNameSearchType,
            string? certificationShortName,
            string? certificationShortNameSearchType,
            string? defaultValidityMonth,
            string? defaultValidityMonthSearchType,
            string? isLifetime,
            string? isLifetimeSearchType,
            string? isRenewable,
            string? isRenewableSearchType,
            string? description,
            string? descriptionSearchType,
            string? isActive,
            string? isActiveSearchType,
string? certificationCategoryName, string? certificationCategoryNameSearchType,
string? certificationProviderName, string? certificationProviderNameSearchType,

            IDbTransaction? transaction = null);
    }
}
