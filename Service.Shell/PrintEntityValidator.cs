using Contracts;
using Shared.DataTransferObjects;

namespace Service.Shell;

internal static class PrintEntityValidator
{
    public static async Task ValidateAsync(
        IRepositoryManager repository,
        string sourceProgramCode,
        string? entityId,
        IReadOnlyDictionary<string, string?>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(sourceProgramCode))
            throw new ArgumentException("Source program code is required.");

        var entityGuid = ResolveEntityGuid(entityId, parameters);
        if (!entityGuid.HasValue)
            throw new ArgumentException("A saved record is required before printing.");

        var code = sourceProgramCode.Trim();
        switch (code)
        {
            case "02.09.04":
                var bank = await repository.Bank.GetBankAsync(entityGuid.Value, trackChanges: false);
                if (bank is null)
                    throw new ArgumentException("Bank record not found or is not available for print.");
                break;
        }
    }

    private static Guid? ResolveEntityGuid(string? entityId, IReadOnlyDictionary<string, string?>? parameters)
    {
        if (!string.IsNullOrWhiteSpace(entityId) && Guid.TryParse(entityId.Trim(), out var fromEntityId))
            return fromEntityId;

        if (parameters is null)
            return null;

        if (parameters.TryGetValue("EntityGuid", out var entityGuidRaw)
            && !string.IsNullOrWhiteSpace(entityGuidRaw)
            && Guid.TryParse(entityGuidRaw.Trim(), out var entityGuid))
        {
            return entityGuid;
        }

        foreach (var kv in parameters)
        {
            if (!kv.Key.EndsWith("Guid", StringComparison.OrdinalIgnoreCase))
                continue;

            if (kv.Key.Equals("CompanyGuid", StringComparison.OrdinalIgnoreCase)
                || kv.Key.Equals("CompanyOfficeGuid", StringComparison.OrdinalIgnoreCase)
                || kv.Key.Equals("ProgramGuid", StringComparison.OrdinalIgnoreCase)
                || kv.Key.Equals("UserGuid", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(kv.Value) && Guid.TryParse(kv.Value.Trim(), out var guid))
                return guid;
        }

        return null;
    }
}
