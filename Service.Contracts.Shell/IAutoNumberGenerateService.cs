using Shared.DataTransferObjects;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    /// <summary>
    /// Engine utama untuk generate nomor dokumen secara atomik.
    /// Mengambil template, component, mengelola counter (dengan locking),
    /// membangun string nomor, dan mencatat audit log — semua dalam 1 transaction.
    /// </summary>
    public interface IAutoNumberGenerateService
    {
        /// <summary>
        /// Generate nomor dokumen berdasarkan template yang ditentukan.
        /// </summary>
        Task<GenerateNumberResultDto> GenerateNumberAsync(GenerateNumberRequestDto request);

        /// <summary>
        /// Ambil riwayat log semua nomor yang pernah di-generate untuk template tertentu.
        /// </summary>
        Task<IEnumerable<AutoNumberLogDto>> GetLogsByTemplateGuidAsync(Guid templateGuid);
    }
}
