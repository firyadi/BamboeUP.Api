using Contracts;
using Entities.Models;
using Mapster;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System.Text;

namespace Service.Shell
{
    /// <summary>
    /// Implementasi Generate Number Engine.
    ///
    /// Alur kerja (atomic dalam 1 transaction):
    ///   1. Resolve TemplateGuid  → AutoNumberTemplate
    ///   2. Load AutoNumberComponent[] (SeqNo ASC)
    ///   3. Tentukan YearNo/MonthNo/DayNo dari component IsResetKey
    ///   4. GetOrCreate counter WITH (UPDLOCK, ROWLOCK) — atomic, thread-safe
    ///   5. Increment counter
    ///   6. BuildNumber — loop component, format tiap bagian
    ///   7. Insert AutoNumberLog
    ///   8. Commit &amp; return GenerateNumberResultDto
    ///
    /// Connection dibuat via IAutoNumberGenerateRepository.CreateConnection()
    /// sehingga Service.Shell tidak perlu referensi langsung ke Repository project.
    /// </summary>
    public class AutoNumberGenerateService : IAutoNumberGenerateService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager     _logger;

        public AutoNumberGenerateService(
            IRepositoryManager repository,
            ILoggerManager     logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<GenerateNumberResultDto> GenerateNumberAsync(GenerateNumberRequestDto request)
        {
            var now = request.TransactionDate?.ToUniversalTime() ?? DateTime.UtcNow;

            // ── 1. Resolve Template ──────────────────────────────────────────────
            var template = await _repository.AutoNumberTemplate
                .GetAutoNumberTemplateAsync(request.TemplateGuid, trackChanges: false);

            if (template is null)
                throw new KeyNotFoundException(
                    $"AutoNumberTemplate dengan Guid '{request.TemplateGuid}' tidak ditemukan.");

            // ── 2. Load Components (ordered by SeqNo) ────────────────────────────
            var components = (await _repository.AutoNumberComponent
                .GetAllByAutoNumberTemplateGuidAsync(request.TemplateGuid, trackChanges: false))
                .OrderBy(c => c.SeqNo)
                .ToList();

            if (!components.Any())
                throw new InvalidOperationException(
                    $"Template '{template.TemplateName}' tidak memiliki component. " +
                    "Tambahkan minimal 1 component bertipe COUNTER sebelum generate.");

            // ── 3. Tentukan dimensi waktu (reset granularity) ────────────────────
            //   IsResetKey = true pada sebuah component menentukan kapan counter reset.
            //   Hierarki: DAY ⊃ MONTH ⊃ YEAR
            int? yearNo  = null;
            int? monthNo = null;
            int? dayNo   = null;

            var resetTypes = components
                .Where(c => c.IsResetKey)
                .Select(c => c.ComponentType.ToUpperInvariant())
                .ToHashSet();

            if (resetTypes.Contains("YEAR") || resetTypes.Contains("MONTH") || resetTypes.Contains("DAY"))
                yearNo = now.Year;

            if (resetTypes.Contains("MONTH") || resetTypes.Contains("DAY"))
                monthNo = now.Month;

            if (resetTypes.Contains("DAY"))
                dayNo = now.Day;

            // ── 4–8. Semua operasi DB dalam 1 transaction ────────────────────────
            // CreateConnection() via repository — tanpa menyentuh Repository project langsung
            using var conn = _repository.AutoNumberGenerate.CreateConnection();
            await ((System.Data.Common.DbConnection)conn).OpenAsync();
            using var trx  = conn.BeginTransaction();

            try
            {
                // 4. Get/Create counter WITH UPDLOCK, ROWLOCK
                var counter = await _repository.AutoNumberGenerate.GetOrCreateCounterWithLockAsync(
                    templateId        : template.AutoNumberTemplateId,
                    companyId         : request.CompanyId,
                    companyOfficeId   : request.CompanyOfficeId,
                    organizationUnitId: request.OrganizationUnitId,
                    yearNo            : yearNo,
                    monthNo           : monthNo,
                    dayNo             : dayNo,
                    transaction       : trx);

                // 5. Increment
                var newCounterValue = counter.LastNumber + 1;
                await _repository.AutoNumberGenerate.IncrementCounterAsync(
                    counterId  : counter.AutoNumberCounterId,
                    newValue   : newCounterValue,
                    transaction: trx);

                // 6. Build number string
                var generatedNumber = BuildNumber(components, now, newCounterValue);

                // 7. Insert audit log
                var log = new AutoNumberLog
                {
                    AutoNumberLogGuid    = Guid.NewGuid(),
                    AutoNumberTemplateId = template.AutoNumberTemplateId,
                    GeneratedNumber      = generatedNumber,
                    CounterValue         = newCounterValue,
                    Status               = "Used",
                    CompanyId            = request.CompanyId,
                    CompanyOfficeId      = request.CompanyOfficeId,
                    OrganizationUnitId   = request.OrganizationUnitId,
                    YearNo               = yearNo,
                    MonthNo              = monthNo,
                    DayNo                = dayNo,
                    CreatedById          = request.CreatedById,
                    CreatedTime          = now
                };

                await _repository.AutoNumberLog.CreateAutoNumberLogAsync(log, trx);

                // 8. Commit
                trx.Commit();

                _logger.LogInfo(
                    $"[AutoNumber] Generated '{generatedNumber}' " +
                    $"(Template={template.TemplateName}, Counter={newCounterValue}, " +
                    $"Company={request.CompanyId}, Office={request.CompanyOfficeId})");

                return new GenerateNumberResultDto
                {
                    GeneratedNumber   = generatedNumber,
                    CounterValue      = newCounterValue,
                    AutoNumberLogGuid = log.AutoNumberLogGuid,
                    AutoNumberLogId   = log.AutoNumberLogId
                };
            }
            catch (Exception ex)
            {
                trx.Rollback();
                _logger.LogError(
                    $"[AutoNumber] Generate FAILED for Template='{template.TemplateName}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Ambil riwayat log generate number untuk template tertentu.
        /// </summary>
        public async Task<IEnumerable<AutoNumberLogDto>> GetLogsByTemplateGuidAsync(Guid templateGuid)
        {
            var template = await _repository.AutoNumberTemplate
                .GetAutoNumberTemplateAsync(templateGuid, trackChanges: false);

            if (template is null)
                throw new KeyNotFoundException(
                    $"AutoNumberTemplate dengan Guid '{templateGuid}' tidak ditemukan.");

            var logs = await _repository.AutoNumberLog
                .GetLogsByTemplateIdAsync(template.AutoNumberTemplateId, trackChanges: false);

            return logs.Adapt<IEnumerable<AutoNumberLogDto>>();
        }

        /// <summary>
        /// Membangun string nomor dari daftar komponen (sudah diurutkan SeqNo ASC).
        ///
        /// ComponentType yang didukung:
        ///   STATIC / PREFIX → StaticValue  (separator "/", prefix "INV", kode dept, dll.)
        ///   YEAR            → date.ToString(Format ?? "yyyy")
        ///   MONTH           → date.ToString(Format ?? "MM")
        ///   DAY             → date.ToString(Format ?? "dd")
        ///   COUNTER         → counter.PadLeft(CounterLength ?? 4, '0')
        ///
        /// Tips konfigurasi untuk "INV/2026/05/0001":
        ///   SeqNo=1  STATIC   "INV"         IsResetKey=false
        ///   SeqNo=2  STATIC   "/"           IsResetKey=false
        ///   SeqNo=3  YEAR     Format="yyyy" IsResetKey=false
        ///   SeqNo=4  STATIC   "/"           IsResetKey=false
        ///   SeqNo=5  MONTH    Format="MM"   IsResetKey=true   ← counter reset tiap bulan
        ///   SeqNo=6  STATIC   "/"           IsResetKey=false
        ///   SeqNo=7  COUNTER  Length=4      IsResetKey=false
        /// </summary>
        private static string BuildNumber(
            IEnumerable<AutoNumberComponent> components,
            DateTime date,
            int counter)
        {
            var sb = new StringBuilder();

            foreach (var c in components)
            {
                var type = c.ComponentType?.ToUpperInvariant() ?? string.Empty;

                switch (type)
                {
                    case "STATIC":
                    case "PREFIX":
                        sb.Append(c.StaticValue ?? string.Empty);
                        break;

                    case "YEAR":
                        sb.Append(date.ToString(c.Format ?? "yyyy"));
                        break;

                    case "MONTH":
                        sb.Append(date.ToString(c.Format ?? "MM"));
                        break;

                    case "DAY":
                        sb.Append(date.ToString(c.Format ?? "dd"));
                        break;

                    case "COUNTER":
                        var length = c.CounterLength ?? 4;
                        sb.Append(counter.ToString().PadLeft(length, '0'));
                        break;

                    // Unknown type — dilewati (tidak melempar exception agar backward compatible)
                }
            }

            return sb.ToString();
        }
    }
}
