using Dapper;

namespace Repository.Extensions
{
    public static class SqlFilterHelper
    {
        public static string BuildFilter(
            string columnName,
            string parameterName,
            string? searchType,
            DynamicParameters parameters,
            string key,
            string? value)
        {
            if (string.IsNullOrEmpty(value))
                return "1=1"; // tidak perlu disaring

            string sqlCondition = searchType?.ToLower() switch
            {
                "contains" => $"{columnName} LIKE {parameterName}",
                "startswith" => $"{columnName} LIKE {parameterName}",
                "endswith" => $"{columnName} LIKE {parameterName}",
                "equals" => $"{columnName} = {parameterName}",
                _ => $"{columnName} LIKE {parameterName}" // default: contains
            };

            string? paramValue = searchType?.ToLower() switch
            {
                "contains" => $"%{value}%",
                "startswith" => $"{value}%",
                "endswith" => $"%{value}",
                "equals" => value,
                _ => $"%{value}%"
            };

            parameters.Add(key, paramValue);
            return sqlCondition;
        }
    }
}