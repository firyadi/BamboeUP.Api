using System;

namespace Shared.DataTransferObjects
{
    /// <summary>
    /// DTO Request untuk Generate Number Engine.
    /// Gunakan TemplateGuid (bukan Id) sesuai pola BamboeUp yang expose Guid ke API.
    /// </summary>
    public class GenerateNumberRequestDto
    {
        /// <summary>Guid dari AutoNumberTemplate yang akan digunakan.</summary>
        public Guid TemplateGuid { get; set; }

        /// <summary>CompanyId scope (nullable — sesuai TemplateScopeType).</summary>
        public long? CompanyId { get; set; }

        /// <summary>CompanyOfficeId scope (nullable).</summary>
        public long? CompanyOfficeId { get; set; }

        /// <summary>OrganizationUnitId scope (nullable).</summary>
        public long? OrganizationUnitId { get; set; }

        /// <summary>
        /// Tanggal transaksi untuk menentukan Year/Month/Day komponen.
        /// Jika null, menggunakan DateTime.UtcNow.
        /// </summary>
        public DateTime? TransactionDate { get; set; }

        /// <summary>UserId yang melakukan generate (untuk audit log).</summary>
        public long CreatedById { get; set; }
    }
}
