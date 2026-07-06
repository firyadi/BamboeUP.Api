using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IHospitalRepository
    {
        Task<Hospital> GetHospitalAsync(Guid hospitalGuid, bool trackChanges);
        Task<IEnumerable<Hospital>> GetAllHospitalsAsync(bool trackChanges);

        Task CreateHospitalAsync(Hospital hospital, IDbTransaction? transaction = null);
        Task UpdateHospitalAsync(Hospital hospital, IDbTransaction? transaction = null);
        Task DeleteHospitalAsync(Guid hospitalGuid, IDbTransaction? transaction = null);
        Task SoftDeleteHospitalAsync(Hospital hospital, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Hospital>> SearchHospitalAsync(
            string? hospitalName,
            string? hospitalNameSearchType,
            string? hospitalCode,
            string? hospitalCodeSearchType,
            string? shortName,
            string? shortNameSearchType,
            string? licenseNo,
            string? licenseNoSearchType,
            string? hospitalType,
            string? hospitalTypeSearchType,
            string? hospitalClass,
            string? hospitalClassSearchType,
            string? phoneNo,
            string? phoneNoSearchType,
            string? email,
            string? emailSearchType,
            string? website,
            string? websiteSearchType,
            IDbTransaction? transaction = null);
    }
}
