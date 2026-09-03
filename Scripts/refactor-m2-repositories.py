#!/usr/bin/env python3
"""Batch-refactor M2 master-data repositories to use ISqlDialect."""
import re
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1] / "Repository"

M2_FILES = [
    "AssetRepository.cs",
    "TrainingRepository.cs",
    "HolidayRepository.cs",
    "MeetingRoomRepository.cs",
    "RoomBookingRepository.cs",
    "WorkItemRepository.cs",
    "CostCenterRepository.cs",
    "CostCenterScopeRepository.cs",
    "CostCenterAssignmentRepository.cs",
    "OrganizationUnitRepository.cs",
    "OrganizationUnitScopeRepository.cs",
    "CompanyRepository.cs",
    "CompanyOfficeRepository.cs",
    "CountryRepository.cs",
    "ProvinceRepository.cs",
    "CityRepository.cs",
    "DistrictRepository.cs",
    "SubDistrictRepository.cs",
    "PostalCodeRepository.cs",
    "EmployeeTrainingRepository.cs",
    "EmployeeCertificationRepository.cs",
    "EmployeeAssetRepository.cs",
    "EmployeeContractRepository.cs",
    "FileStorageRepository.cs",
    "AwardRepository.cs",
    "AwardCriteriaRepository.cs",
    "AwardCriteriaRuleRepository.cs",
    r"Custom\CompanyRepository.cs",
    r"Custom\CompanyOfficeRepository.cs",
    r"Custom\CostCenterAssignmentRepository.cs",
]


def detect_table(content: str) -> tuple[str, str] | None:
    m = re.search(r"\[(app|core)\]\.\[(\w+)\]", content)
    if not m:
        return None
    return m.group(1), m.group(2)


def add_usings(content: str) -> str:
    if "using Repository.Database;" not in content:
        if "using Repository.Extensions;" in content:
            content = content.replace(
                "using Repository.Extensions;",
                "using Repository.Database;\nusing Repository.Extensions;",
            )
        elif "using System.Data;" in content:
            content = content.replace(
                "using System.Data;",
                "using Repository.Database;\nusing System.Data;",
            )
    if "using Contracts;" not in content and "ParameterContext" in content:
        # ensure ParameterContext resolves when we drop Contracts. prefix
        if "namespace Repository" in content:
            pass  # ParameterContext is in Contracts, usually already imported
    return content


def inject_primary_ctor(content: str) -> str:
    if "ISqlDialect dialect" in content:
        return content

    content = re.sub(
        r"public partial class (\w+Repository)\(RepositoryContext context\) :",
        r"public partial class \1(RepositoryContext context, ISqlDialect dialect) :",
        content,
    )
    content = re.sub(
        r"public partial class (\w+Repository)\(RepositoryContext context, IAuditService (\w+)\) :",
        r"public partial class \1(RepositoryContext context, IAuditService \2, ISqlDialect dialect) :",
        content,
    )
    return content


def inject_class_ctor(content: str, schema: str, table: str) -> str:
    if "ISqlDialect" in content:
        return content

    class_match = re.search(
        r"public class (\w+Repository)[^{]+\{", content, re.DOTALL
    )
    if not class_match:
        return content

    id_col = f"{table}Id"

    # Add fields after _auditService field if present
    field_block = f"""
        private readonly ISqlDialect _dialect;
        private readonly string _table;
        private readonly int _maxRows = ParameterContext.MaxResultRecord;
"""

    if "private readonly ISqlDialect _dialect" not in content:
        if "private readonly IAuditService _audit" in content:
            content = content.replace(
                "private readonly IAuditService _auditService;",
                "private readonly IAuditService _auditService;"
                + field_block,
            )
        elif "private readonly RepositoryContext _context;" in content:
            content = content.replace(
                "private readonly RepositoryContext _context;",
                "private readonly RepositoryContext _context;" + field_block,
            )

    # Constructor injection
    content = re.sub(
        r"public (\w+Repository)\(RepositoryContext context, IAuditService auditService\)\s*\{",
        r"public \1(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)\n        {",
        content,
    )
    content = re.sub(
        r"(_context = context;\s*\n\s*_auditService = auditService;)",
        r"\1\n            _dialect = dialect;\n            _table = dialect.Qualify(\"" + schema + "\", \"" + table + "\");",
        content,
    )

    # Replace _context usages in SQL - use _dialect for BuildFilter
    content = content.replace("SqlFilterHelper.BuildFilter(", "SqlFilterHelper.BuildFilter(")
    content = re.sub(
        r"SqlFilterHelper\.BuildFilter\(([^)]+), parameters, \"([^\"]+)\", (\w+)\)",
        r"SqlFilterHelper.BuildFilter(\1, parameters, \"\2\", \3, _dialect)",
        content,
    )

    return transform_sql(content, schema, table, id_col, "dialect", "_table", "_maxRows", class_style=True)


def inject_partial_fields(content: str, schema: str, table: str) -> str:
    id_col = f"{table}Id"
    marker = f'partial class {table}Repository' if False else None

    if "private readonly string _table" in content:
        return transform_sql(content, schema, table, id_col, "dialect", "_table", "_maxRows")

    # Insert fields after class opening line
    pattern = r"(public partial class \w+Repository[^{]+\{)\s*\n"
    replacement = (
        r"\1\n"
        f'        private readonly string _table = dialect.Qualify("{schema}", "{table}");\n'
        f"        private readonly int _maxRows = ParameterContext.MaxResultRecord;\n\n"
    )
    content = re.sub(pattern, replacement, content, count=1)
    return transform_sql(content, schema, table, id_col, "dialect", "_table", "_maxRows")


def transform_sql(
    content: str,
    schema: str,
    table: str,
    id_col: str,
    dialect_var: str,
    table_var: str,
    max_rows_var: str,
    class_style: bool = False,
) -> str:
    d = dialect_var if not class_style else "_dialect"
    t = table_var
    m = max_rows_var

    # SELECT TOP -> SelectLimited
    content = re.sub(
        r"SELECT TOP \(\{Contracts\.ParameterContext\.MaxResultRecord\}\)\s*(\S+)",
        rf"{{SqlDialectQueryHelper.SelectLimited({d}, {m}, \"\\1\")}}",
        content,
    )

    # Schema-qualified table names
    content = re.sub(
        rf"\[{schema}\]\.\[{table}\]",
        "{" + t + "}",
        content,
    )

    # Also replace any remaining same schema/table refs in joins (same table)
    # SCOPE_IDENTITY
    content = re.sub(
        r"SELECT CAST\(SCOPE_IDENTITY\(\)\s+as\s+bigint\);",
        "{" + d + '.ReturnIdentitySuffix("' + id_col + '")}',
        content,
        flags=re.IGNORECASE,
    )
    content = re.sub(
        r"SELECT CAST\(SCOPE_IDENTITY\(\)\s+AS\s+BIGINT\);",
        "{" + d + '.ReturnIdentitySuffix("' + id_col + '")}',
        content,
        flags=re.IGNORECASE,
    )

    # const sql -> var sql = $ for dialect interpolation
    content = content.replace("const string sql = @\"", "var sql = $@\"")

    # ORDER BY ... DESC"; with limit for postgres
    content = re.sub(
        r"(ORDER BY a\.\w+ DESC)\";",
        rf"\1{{{d}.AppendLimit({m})}}\";",
        content,
    )

    # BuildFilter dialect
    filter_d = d
    content = re.sub(
        r"SqlFilterHelper\.BuildFilter\(([^)]+), parameters, \"([^\"]+)\", (\w+)\)",
        rf"SqlFilterHelper.BuildFilter(\1, parameters, \"\2\", \3, {filter_d})",
        content,
    )

    # ISNULL -> CoalesceEmpty for display columns (optional, common in company repos)
    content = re.sub(
        r"ISNULL\(([^,]+),\s*''\)",
        rf"{{SqlDialectQueryHelper.CoalesceEmpty({d}, \\1)}}",
        content,
    )

    return content


def refactor_file(path: Path) -> bool:
    content = path.read_text(encoding="utf-8")
    if "ISqlDialect" in content and "SqlDialectQueryHelper.SelectLimited" in content:
        return False

    detected = detect_table(content)
    if not detected:
        print(f"SKIP (no table): {path.name}")
        return False

    schema, table = detected
    original = content
    content = add_usings(content)
    content = inject_primary_ctor(content)

    is_class_style = "public class " in content and "public partial class " not in content
    if is_class_style:
        content = inject_class_ctor(content, schema, table)
    else:
        content = inject_partial_fields(content, schema, table)

    if content != original:
        path.write_text(content, encoding="utf-8")
        print(f"OK: {path.relative_to(REPO_ROOT)}")
        return True

    print(f"UNCHANGED: {path.name}")
    return False


def main() -> None:
    changed = 0
    for rel in M2_FILES:
        path = REPO_ROOT / rel
        if path.exists():
            if refactor_file(path):
                changed += 1
        else:
            print(f"MISSING: {rel}")
    print(f"\nRefactored {changed} files.")


if __name__ == "__main__":
    main()
