using Entities.Models;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    /// <summary>
    /// Repository khusus untuk operasi atomic Generate Number Engine.
    /// Repository ini bertanggung jawab membuat connection + transaction sendiri
    /// agar Service.Shell tidak perlu referensi ke Repository project.
    ///
    /// Dua method terpisah (GetOrCreate + Increment) masih tersedia untuk
    /// kasus di mana caller sudah memiliki transaction aktif.
    /// </summary>
    public interface IAutoNumberGenerateRepository
    {
        /// <summary>
        /// Ambil counter yang sudah ada DENGAN ROW LOCK, atau buat baru jika belum ada.
        /// WAJIB dipanggil di dalam transaction aktif.
        /// </summary>
        Task<AutoNumberCounter> GetOrCreateCounterWithLockAsync(
            long templateId,
            long? companyId,
            long? companyOfficeId,
            long? organizationUnitId,
            int? yearNo,
            int? monthNo,
            int? dayNo,
            IDbTransaction transaction);

        /// <summary>
        /// Increment LastNumber counter.
        /// WAJIB dipanggil di dalam transaction aktif.
        /// </summary>
        Task<int> IncrementCounterAsync(
            long counterId,
            int newValue,
            IDbTransaction transaction);

        /// <summary>
        /// Buat koneksi baru yang siap dipakai untuk BeginTransaction.
        /// Digunakan oleh service agar tidak perlu akses RepositoryContext langsung.
        /// </summary>
        System.Data.IDbConnection CreateConnection();
    }
}
