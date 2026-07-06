using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace BamboeUp.Api.Controllers
{
    /// <summary>
    /// Generate Number Engine — endpoint untuk menghasilkan nomor dokumen
    /// secara atomik (thread-safe, dengan locking &amp; audit log).
    /// </summary>
    [Route("api/autoNumber/generate")]
    [ApiController]
    public class AutoNumberGenerateController : ControllerBase
    {
        private readonly IServiceShellManager _service;

        public AutoNumberGenerateController(IServiceShellManager service)
        {
            _service = service;
        }

        /// <summary>Generate nomor dokumen</summary>
        /// <remarks>
        /// Menghasilkan nomor dokumen secara atomik berdasarkan template yang dikonfigurasi.
        ///
        /// **Alur proses:**
        /// 1. Ambil template + components berdasarkan TemplateGuid
        /// 2. Get/Create counter dengan row lock (thread-safe)
        /// 3. Increment counter
        /// 4. Build string nomor sesuai komponen (STATIC, YEAR, MONTH, COUNTER, dll.)
        /// 5. Simpan ke AutoNumberLog
        /// 6. Return nomor yang dihasilkan
        ///
        /// **Contoh hasil:** `INV/2026/05/0001`
        /// </remarks>
        [HttpPost]
        [SwaggerOperation(
            Summary     = "Generate Nomor Dokumen",
            Description = "Menghasilkan nomor dokumen secara atomik berdasarkan template. " +
                          "Proses dilakukan dalam 1 database transaction dengan row locking " +
                          "untuk memastikan tidak ada nomor duplikat meski dipanggil secara concurrent.")]
        [SwaggerResponse(200, "Berhasil generate nomor", typeof(GenerateNumberResultDto))]
        [SwaggerResponse(404, "Template tidak ditemukan")]
        [SwaggerResponse(400, "Request tidak valid atau template tidak memiliki component")]
        public async Task<IActionResult> Generate([FromBody] GenerateNumberRequestDto request)
        {
            if (request.TemplateGuid == Guid.Empty)
                return BadRequest(new { message = "TemplateGuid tidak boleh kosong." });

            var result = await _service.AutoNumberGenerateService.GenerateNumberAsync(request);
            return Ok(result);
        }

        /// <summary>Get riwayat log generate number berdasarkan Template Guid</summary>
        [HttpGet("{templateGuid:guid}/logs")]
        [SwaggerOperation(
            Summary     = "Get Log by Template",
            Description = "Mengambil riwayat semua nomor yang pernah di-generate untuk template tertentu.")]
        [SwaggerResponse(200, "Daftar log", typeof(IEnumerable<AutoNumberLogDto>))]
        [SwaggerResponse(404, "Template tidak ditemukan")]
        public async Task<IActionResult> GetLogs(Guid templateGuid)
        {
            var logs = await _service.AutoNumberGenerateService
                .GetLogsByTemplateGuidAsync(templateGuid);
            return Ok(logs);
        }
    }
}
