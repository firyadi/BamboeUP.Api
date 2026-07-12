using Shared.DataTransferObjects;
using System.Globalization;

namespace Service.Shell;

internal static class ReportParameterValidator
{
    public static Dictionary<string, string?> ValidateAndNormalize(
        ReportRunRequestDto request,
        IReadOnlyList<ReportParameterDefinitionDto> schemaFields,
        bool isDocPrint = false)
    {
        if (schemaFields.Count == 0)
            return isDocPrint ? NormalizeContextualPrint(request) : NormalizeLegacy(request);

        var allowed = schemaFields
            .ToDictionary(f => f.ParameterName, f => f, StringComparer.OrdinalIgnoreCase);

        foreach (var key in request.Parameters.Keys)
        {
            if (!allowed.ContainsKey(key))
                throw new ArgumentException($"Parameter '{key}' is not allowed for this report.");
        }

        var normalized = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in schemaFields)
        {
            if (field.ControlType == "ReadonlyText")
            {
                normalized[field.ParameterName] = field.ParameterName switch
                {
                    "CompanyId" => request.CompanyId?.ToString(),
                    "CompanyOfficeId" => request.CompanyOfficeId?.ToString(),
                    _ => request.Parameters.TryGetValue(field.ParameterName, out var ro) ? ro : null
                };
                continue;
            }

            string? raw = null;
            if (field.ParameterName.Equals("DateFrom", StringComparison.OrdinalIgnoreCase))
                raw = request.DateFrom?.ToString("O") ?? GetRaw(request, field.ParameterName);
            else if (field.ParameterName.Equals("DateTo", StringComparison.OrdinalIgnoreCase))
                raw = request.DateTo?.ToString("O") ?? GetRaw(request, field.ParameterName);
            else
                raw = GetRaw(request, field.ParameterName);

            if (field.IsRequired && string.IsNullOrWhiteSpace(raw))
                throw new ArgumentException($"{field.DisplayLabel} is required.");

            if (string.IsNullOrWhiteSpace(raw))
                continue;

            ValidateDataType(field, raw);
            normalized[field.ParameterName] = raw.Trim();
        }

        if (normalized.TryGetValue("DateFrom", out var fromRaw) &&
            normalized.TryGetValue("DateTo", out var toRaw) &&
            DateTime.TryParse(fromRaw, out var from) &&
            DateTime.TryParse(toRaw, out var to) &&
            to < from)
        {
            throw new ArgumentException("Valid To must be on or after Valid From.");
        }

        return normalized;
    }

    public static object MaskForLog(
        IReadOnlyDictionary<string, string?> parameters,
        IReadOnlyList<ReportParameterDefinitionDto> schemaFields)
    {
        var sensitive = schemaFields
            .Where(f => f.IsSensitive)
            .Select(f => f.ParameterName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (sensitive.Count == 0)
            return parameters;

        return parameters.ToDictionary(
            kv => kv.Key,
            kv => sensitive.Contains(kv.Key) ? "***" : kv.Value,
            StringComparer.OrdinalIgnoreCase);
    }

    private static string? GetRaw(ReportRunRequestDto request, string parameterName)
        => request.Parameters.TryGetValue(parameterName, out var value) ? value : null;

    private static Dictionary<string, string?> NormalizeContextualPrint(ReportRunRequestDto request)
    {
        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var kv in request.Parameters)
        {
            if (!string.IsNullOrWhiteSpace(kv.Value))
                result[kv.Key] = kv.Value.Trim();
        }

        if (request.CompanyId.HasValue && !result.ContainsKey("CompanyId"))
            result["CompanyId"] = request.CompanyId.Value.ToString();
        if (request.CompanyOfficeId.HasValue && !result.ContainsKey("CompanyOfficeId"))
            result["CompanyOfficeId"] = request.CompanyOfficeId.Value.ToString();

        return result;
    }

    private static Dictionary<string, string?> NormalizeLegacy(ReportRunRequestDto request)
    {
        if (!request.DateFrom.HasValue || !request.DateTo.HasValue)
            throw new ArgumentException("Valid From and Valid To are required.");

        if (request.DateTo < request.DateFrom)
            throw new ArgumentException("Valid To must be on or after Valid From.");

        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["DateFrom"] = request.DateFrom.Value.ToString("O"),
            ["DateTo"] = request.DateTo.Value.ToString("O")
        };

        foreach (var kv in request.Parameters)
            result[kv.Key] = kv.Value;

        return result;
    }

    private static void ValidateDataType(ReportParameterDefinitionDto field, string raw)
    {
        switch (field.DataType.ToLowerInvariant())
        {
            case "datetime":
            case "date":
                if (!DateTime.TryParse(raw, out _))
                    throw new ArgumentException($"{field.DisplayLabel} must be a valid date.");
                break;
            case "int":
                if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                    throw new ArgumentException($"{field.DisplayLabel} must be a valid integer.");
                break;
            case "long":
                if (!long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                    throw new ArgumentException($"{field.DisplayLabel} must be a valid number.");
                break;
            case "decimal":
                if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                    throw new ArgumentException($"{field.DisplayLabel} must be a valid decimal.");
                break;
            case "bool":
            case "boolean":
                if (!bool.TryParse(raw, out _))
                    throw new ArgumentException($"{field.DisplayLabel} must be true or false.");
                break;
        }
    }
}
