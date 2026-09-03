using Dapper;
using Repository.Database;
using Shared.Settings;

namespace Repository.Extensions;

public static class SqlFilterHelper
{
    public static string BuildFilter(
        string columnName,
        string parameterName,
        string? searchType,
        DynamicParameters parameters,
        string key,
        string? value,
        ISqlDialect? dialect = null)
    {
        if (string.IsNullOrEmpty(value))
            return "1=1";

        var likeOperator = dialect?.Provider == DatabaseProvider.PostgreSql ? "ILIKE" : "LIKE";

        string sqlCondition = searchType?.ToLower() switch
        {
            "contains" => $"{columnName} {likeOperator} {parameterName}",
            "startswith" => $"{columnName} {likeOperator} {parameterName}",
            "endswith" => $"{columnName} {likeOperator} {parameterName}",
            "equals" => $"{columnName} = {parameterName}",
            _ => $"{columnName} {likeOperator} {parameterName}"
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
