using Dapper;
using System;
using System.Collections.Generic;

namespace Repository.Extensions
{
    public static class SqlScopeHelper
    {
        /// <summary>
        /// Menghasilkan klausa WHERE untuk filter scope CompanyGuid & CompanyOfficeGuid,
        /// sesuai dengan schema yang digunakan (app = 3 kondisi, selain itu = 2 kondisi).
        /// </summary>
        /// <param name="schema">Nama schema (misalnya: "app", "core", dll)</param>
        /// <param name="columnNames">Daftar kolom dari tabel target</param>
        /// <param name="parameters">Parameter SQL (akan ditambahkan CompanyGuid dan OfficeGuid)</param>
        /// <param name="companyGuid">CompanyGuid dari context pengguna</param>
        /// <param name="companyOfficeGuid">CompanyOfficeGuid dari context pengguna</param>
        /// <returns>Klausa SQL WHERE (tanpa kata WHERE)</returns>
        public static string? BuildScopeClause(string schema, List<string> columnNames, DynamicParameters parameters, Guid companyGuid, Guid companyOfficeGuid)
        {
            bool hasCompanyGuid = columnNames.Contains("CompanyGuid");
            bool hasCompanyOfficeGuid = columnNames.Contains("CompanyOfficeGuid");

            if (!hasCompanyGuid || !hasCompanyOfficeGuid)
                return null;

            parameters.Add("CompanyGuid", companyGuid);
            parameters.Add("CompanyOfficeGuid", companyOfficeGuid);

            if (schema.Equals("app", StringComparison.OrdinalIgnoreCase))
            {
                return @"
                (
                    (a.CompanyGuid = @CompanyGuid AND a.CompanyOfficeGuid = @CompanyOfficeGuid)
                    OR (a.CompanyGuid = @CompanyGuid AND a.CompanyOfficeGuid = '00000000-0000-0000-0000-000000000000')
                    OR (a.CompanyGuid = '00000000-0000-0000-0000-000000000000' AND a.CompanyOfficeGuid = '00000000-0000-0000-0000-000000000000')
                )";
            }
            else
            {
                return @"
                (
                    (a.CompanyGuid = @CompanyGuid AND a.CompanyOfficeGuid = @CompanyOfficeGuid)
                    OR (a.CompanyGuid = @CompanyGuid AND a.CompanyOfficeGuid = '00000000-0000-0000-0000-000000000000')
                )";
            }
        }
    }
}
