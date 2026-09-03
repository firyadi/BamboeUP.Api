#!/usr/bin/env python3
"""Refactor Person/Employee repositories to ISqlDialect. Safer than M2 script."""
from __future__ import annotations

import re
from pathlib import Path

REPO = Path(__file__).resolve().parents[1] / "Repository"

FILES = [
    "PersonRepository.cs",
    "PersonAddressRepository.cs",
    "PersonContactRepository.cs",
    "PersonEducationRepository.cs",
    "PersonFamilyRepository.cs",
    "PersonIdentificationRepository.cs",
    "PersonEmergencyContactRepository.cs",
    "PersonBankAccountRepository.cs",
    "PersonWorkExperienceRepository.cs",
    "PersonPhysicalCharacteristicRepository.cs",
    "EmployeeRepository.cs",
    "EmployeeAssignmentRepository.cs",
    "EmployeeCareerRepository.cs",
    "EmployeeSkillRepository.cs",
    "EmployeeLanguageRepository.cs",
]

OUTER_APPLY = re.compile(
    r"OUTER APPLY \(\s*"
    r"SELECT StandardReferenceItemName\s*"
    r"FROM \[app\]\.\[fn_GetStandardReferenceItems\]\([^,]+,\s*[^,]+,\s*[^,]+,\s*'([^']+)'\)\s*"
    r"WHERE StandardReferenceItemId = a\.(\w+)\s*"
    r"\) (\w+)",
    re.IGNORECASE | re.DOTALL,
)

ISNULL = re.compile(r"ISNULL\(([^,]+),\s*''\)")

SELECT_TOP = re.compile(
    r"SELECT TOP \(\{Contracts\.ParameterContext\.MaxResultRecord\}\) (\S+)"
)

QUALIFIED = re.compile(r"\[(app|core)\]\.\[(\w+)\]")

BUILD_FILTER = re.compile(
    r"SqlFilterHelper\.BuildFilter\((.*?),\s*parameters,\s*\"([^\"]+)\",\s*(\w+)\)",
    re.DOTALL,
)

SCOPE_IDENTITY = re.compile(
    r";\s*SELECT CAST\(SCOPE_IDENTITY\(\)\s+as\s+bigint\);",
    re.IGNORECASE,
)


def detect_primary(content: str) -> tuple[str, str]:
    m = re.search(r"INSERT INTO \[(app|core)\]\.\[(\w+)\]", content)
    if m:
        return m.group(1), m.group(2)
    m = re.search(r"FROM \[(app|core)\]\.\[(\w+)\] a", content)
    if m:
        return m.group(1), m.group(2)
    raise ValueError("Could not detect primary table")


def add_using(content: str) -> str:
    if "using Repository.Database;" in content:
        return content
    return content.replace(
        "using Repository.Extensions;",
        "using Repository.Database;\nusing Repository.Extensions;",
    )


def inject_ctor_and_fields(content: str, schema: str, table: str) -> str:
    content = content.replace(
        "(RepositoryContext context, IAuditService audit) :",
        "(RepositoryContext context, IAuditService audit, ISqlDialect dialect) :",
    )
    if "private readonly string _table" in content:
        return content
    content = re.sub(
        r"(public partial class \w+Repository\([^)]+\) : \w+\s*\{\s*\n)",
        (
            r"\1"
            f'        private readonly string _table = dialect.Qualify("{schema}", "{table}");\n'
            "        private readonly int _maxRows = ParameterContext.MaxResultRecord;\n\n"
        ),
        content,
        count=1,
    )
    return content


def replace_outer_apply(match: re.Match) -> str:
    initial, fk, alias = match.group(1), match.group(2), match.group(3)
    return (
        "{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup("
        + f'dialect, "{alias}", "{fk}", "{initial}"'
        + ")}"
    )


def replace_isnull(match: re.Match) -> str:
    expr = match.group(1).strip()
    return '{SqlDialectQueryHelper.CoalesceEmpty(dialect, "' + expr + '")}'


def replace_select_top(match: re.Match) -> str:
    cols = match.group(1)
    return '{SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "' + cols + '")}'


def replace_qualified(match: re.Match, schema: str, table: str) -> str:
    s, t = match.group(1), match.group(2)
    if s == schema and t == table:
        return "{_table}"
    return "{dialect.Qualify(\"" + s + "\", \"" + t + "\")}"


def replace_build_filter(match: re.Match) -> str:
    prefix, key, value = match.group(1), match.group(2), match.group(3)
    prefix = re.sub(r"\s+", " ", prefix.strip())
    return (
        f'SqlFilterHelper.BuildFilter({prefix}, parameters, "{key}", {value}, dialect)'
    )


def refactor(content: str) -> str:
    schema, table = detect_primary(content)
    id_col = table + "Id"

    content = add_using(content)
    content = inject_ctor_and_fields(content, schema, table)
    content = OUTER_APPLY.sub(replace_outer_apply, content)
    content = ISNULL.sub(replace_isnull, content)
    content = SELECT_TOP.sub(replace_select_top, content)
    content = QUALIFIED.sub(lambda m: replace_qualified(m, schema, table), content)
    content = SCOPE_IDENTITY.sub(
        "{dialect.ReturnIdentitySuffix(\"" + id_col + "\")}", content
    )
    content = content.replace('const string sql = @"', 'var sql = $@"')
    content = BUILD_FILTER.sub(replace_build_filter, content)
    content = re.sub(
        r"(ORDER BY a\.\w+ DESC)(\";)",
        r"\1{dialect.AppendLimit(_maxRows)}\2",
        content,
    )
    return content


def main() -> None:
    for name in FILES:
        path = REPO / name
        original = path.read_text(encoding="utf-8")
        if "ISqlDialect dialect" in original and "SqlDialectQueryHelper.SelectLimited" in original:
            print(f"SKIP {name}")
            continue
        updated = refactor(original)
        path.write_text(updated, encoding="utf-8")
        print(f"OK   {name}")


if __name__ == "__main__":
    main()
