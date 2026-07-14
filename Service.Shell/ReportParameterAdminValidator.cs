using Shared.DataTransferObjects;
using System.Text.RegularExpressions;

namespace Service.Shell;

internal static class ReportParameterAdminValidator
{
    private static readonly HashSet<string> AllowedControlTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "ReadonlyText", "DatePicker", "DateTimePicker", "TextBox", "ComboBox", "CheckBox", "NumberBox"
    };

    private static readonly HashSet<string> AllowedDataTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "string", "datetime", "date", "int", "long", "decimal", "bool", "boolean"
    };

    private static readonly Regex ParameterNamePattern = new(@"^[A-Za-z][A-Za-z0-9_]*$", RegexOptions.Compiled);

    public static void ValidateReplaceBatch(IReadOnlyList<ReportParameterForUpsertDto> parameters)
    {
        if (parameters.Count > 50)
            throw new ArgumentException("A report definition cannot have more than 50 parameters.");

        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in parameters.OrderBy(p => p.SortOrder))
        {
            var name = row.ParameterName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Parameter name is required.");

            if (name.Length > 100)
                throw new ArgumentException($"Parameter name '{name}' exceeds 100 characters.");

            if (!ParameterNamePattern.IsMatch(name))
                throw new ArgumentException($"Parameter name '{name}' is invalid. Use letters, digits, and underscore; must start with a letter.");

            if (!names.Add(name))
                throw new ArgumentException($"Duplicate parameter name '{name}'.");

            var label = row.DisplayLabel?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException($"Display label is required for parameter '{name}'.");

            if (label.Length > 200)
                throw new ArgumentException($"Display label for '{name}' exceeds 200 characters.");

            var controlType = row.ControlType?.Trim() ?? string.Empty;
            if (!AllowedControlTypes.Contains(controlType))
                throw new ArgumentException($"Control type '{controlType}' is not supported for parameter '{name}'.");

            var dataType = row.DataType?.Trim() ?? string.Empty;
            if (!AllowedDataTypes.Contains(dataType))
                throw new ArgumentException($"Data type '{dataType}' is not supported for parameter '{name}'.");

            if (row.ColumnGroup is < 1 or > 3)
                throw new ArgumentException($"Column group for '{name}' must be 1, 2, or 3.");

            if (row.ColumnSpan is < 1 or > 12)
                throw new ArgumentException($"Column span for '{name}' must be between 1 and 12.");

            if (row.SortOrder < 0)
                throw new ArgumentException($"Sort order for '{name}' cannot be negative.");

            if (string.Equals(controlType, "ComboBox", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(row.LookupType))
            {
                throw new ArgumentException($"Lookup type is required for ComboBox parameter '{name}'.");
            }
        }
    }
}
