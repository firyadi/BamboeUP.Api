#!/usr/bin/env python3
"""Refactor M4 platform repositories to ISqlDialect."""
from __future__ import annotations

import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1] / "Repository"

FILES = [
    "UserRepository.cs",
    "UserGroupRepository.cs",
    "UserGroupProgramRepository.cs",
    "UserGroupScopeRepository.cs",
    "UserCompanyScopeRepository.cs",
    "UserScopeRepository.cs",
    "ParameterRepository.cs",
    "ParameterScopeRepository.cs",
    "StandardReferenceRepository.cs",
    "StandardReferenceItemRepository.cs",
    "StandardReferenceScopeRepository.cs",
    "StandardReferenceScopeItemRepository.cs",
    "StandardReferenceDisplayRepository.cs",
    "VwStandardReferenceItemRepository.cs",
    "ProgramRepository.cs",
    "AutoNumberRepository.cs",
    "AutoNumberTemplateRepository.cs",
    "AutoNumberCounterRepository.cs",
    "AutoNumberComponentRepository.cs",
    "AutoNumberLogRepository.cs",
    "AutoNumberGenerateRepository.cs",
    "DocumentNumberRequestRepository.cs",
    "Approval/ApprovalTemplateRepository.cs",
    "Approval/ApprovalTemplateLevelRepository.cs",
    "Approval/ApprovalStepRepository.cs",
    "Approval/ApprovalRequestRepository.cs",
    "Approval/ApprovalHistoryRepository.cs",
    "Approval/ApprovalDelegationRepository.cs",
]

SELECT_TOP = re.compile(
    r"SELECT TOP \(\{Contracts\.ParameterContext\.MaxResultRecord\}\) (\*|[A-Za-z_]\w*\.\*)"
)
SELECT_TOP_1 = re.compile(r"SELECT TOP 1 (\*|\w+)")
QUALIFIED = re.compile(r"\[(app|core|apv)\]\.\[(\w+)\]")
SCOPE_CAST = re.compile(
    r";\s*SELECT CAST\(SCOPE_IDENTITY\(\)\s+as\s+bigint\);",
    re.IGNORECASE,
)
SCOPE_BARE = re.compile(r"SELECT SCOPE_IDENTITY\(\);", re.IGNORECASE)
OUTPUT = re.compile(
    r"\n[ \t]*OUTPUT INSERTED\.(\w+)[ \t]*\n([ \t]*VALUES)",
    re.IGNORECASE,
)
ISNULL = re.compile(r"ISNULL\(([^,]+),\s*''\)")
BUILD_FILTER = re.compile(
    r"SqlFilterHelper\.BuildFilter\((.*?),\s*parameters,\s*\"([^\"]+)\",\s*(\w+)\)",
    re.DOTALL,
)
INSERT_TABLE = re.compile(r"INSERT INTO \[(app|core|apv)\]\.\[(\w+)\]")


def add_using(content: str) -> str:
    if "using Repository.Database;" in content:
        return content
    if "using Repository.Extensions;" in content:
        return content.replace(
            "using Repository.Extensions;",
            "using Repository.Database;\nusing Repository.Extensions;",
        )
    # insert after last using
    usings = list(re.finditer(r"^using .+;\s*$", content, re.M))
    if not usings:
        return "using Repository.Database;\n" + content
    last = usings[-1]
    return content[: last.end()] + "\nusing Repository.Database;" + content[last.end() :]


def inject_ctor(content: str) -> str:
    if "ISqlDialect dialect" in content or "ISqlDialect _dialect" in content:
        return content

    if "private readonly ISqlDialect _dialect" not in content:
        # after last private readonly field
        fields = list(
            re.finditer(r"        private readonly .+;\n", content)
        )
        field_line = "        private readonly ISqlDialect _dialect;\n        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;\n"
        if fields:
            last = fields[-1]
            content = content[: last.end()] + field_line + content[last.end() :]
        else:
            content = re.sub(
                r"(public class \w+[^{]*\{\n)",
                r"\1        private readonly ISqlDialect _dialect;\n        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;\n",
                content,
                count=1,
            )

    # primary ctor
    content = content.replace(
        "(RepositoryContext context, IAuditService audit) :",
        "(RepositoryContext context, IAuditService audit, ISqlDialect dialect) :",
    )
    content = content.replace(
        "(RepositoryContext context) :",
        "(RepositoryContext context, ISqlDialect dialect) :",
    )

    def add_param_assign(match: re.Match) -> str:
        prefix, closing, rest = match.group(1), match.group(2), match.group(3)
        if "ISqlDialect" in prefix:
            return match.group(0)
        return f"{prefix}, ISqlDialect dialect){closing}{rest}\n            _dialect = dialect;"

    content = re.sub(
        r"(public \w+\(RepositoryContext context(?:, IAuditService \w+)?)\)(\s*\{)(\s*_context = context;)",
        add_param_assign,
        content,
    )
    return content


def replace_output_inserts(content: str) -> str:
    def repl(m: re.Match) -> str:
        col = m.group(1)
        values = m.group(2)
        return f"\n{values}"

    content, n = OUTPUT.subn(repl, content)
    if n == 0:
        return content

    # Add ReturnIdentitySuffix after each VALUES (...) that no longer has OUTPUT.
    # Match VALUES block ending with ";
    def add_suffix(match: re.Match) -> str:
        body = match.group(0)
        if "ReturnIdentitySuffix" in body:
            return body
        # find corresponding OUTPUT column from nearby — already removed.
        return body

    return content


def preceding_insert_id(content: str, index: int) -> str:
    chunk = content[max(0, index - 800) : index]
    m = list(INSERT_TABLE.finditer(chunk))
    if m:
        return m[-1].group(2) + "Id"
    m2 = list(re.finditer(r'Qualify\("(app|core|apv)", "(\w+)"\)', chunk))
    if m2:
        return m2[-1].group(2) + "Id"
    return "Id"


def replace_scope(content: str) -> str:
    def repl_cast(m: re.Match) -> str:
        col = preceding_insert_id(content, m.start())
        return '{_dialect.ReturnIdentitySuffix("' + col + '")}'

    def repl_bare(m: re.Match) -> str:
        col = preceding_insert_id(content, m.start())
        return '{_dialect.ReturnIdentitySuffix("' + col + '")}'

    content = SCOPE_CAST.sub(repl_cast, content)
    content = SCOPE_BARE.sub(repl_bare, content)
    return content


def add_identity_after_values_missing_output(content: str, original: str) -> str:
    """If OUTPUT was stripped, append ReturnIdentitySuffix using original OUTPUT column."""
    cols = re.findall(r"OUTPUT INSERTED\.(\w+)", original, flags=re.I)
    if not cols:
        return content
    # For each VALUES (...)"  without ReturnIdentitySuffix immediately after, add the next unused col.
    # Simpler: replace pattern VALUES\n                (...params...)"  when ReturnIdentity not present
    idx = 0

    def repl(m: re.Match) -> str:
        nonlocal idx
        if "ReturnIdentitySuffix" in m.group(0):
            return m.group(0)
        if idx >= len(cols):
            return m.group(0)
        col = cols[idx]
        idx += 1
        return m.group(1) + '{_dialect.ReturnIdentitySuffix("' + col + '")}' + m.group(2)

    # VALUES ... (@x, @y)"   or  (...); "
    return re.sub(
        r"(VALUES\s*\n\s*\([^;]*?\))(\s*\";)",
        repl,
        content,
        flags=re.IGNORECASE,
    )


def refactor(content: str) -> str:
    original = content
    content = add_using(content)
    content = inject_ctor(content)
    content = SELECT_TOP.sub(
        lambda m: '{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "'
        + m.group(1)
        + '")}',
        content,
    )
    content = SELECT_TOP_1.sub(
        lambda m: '{SqlDialectQueryHelper.SelectLimited(_dialect, 1, "'
        + m.group(1)
        + '")}',
        content,
    )
    content = replace_scope(content)
    content = replace_output_inserts(content)
    content = QUALIFIED.sub(
        lambda m: '{_dialect.Qualify("' + m.group(1) + '", "' + m.group(2) + '")}',
        content,
    )
    content = content.replace("const string sql = @\"", "var sql = $@\"")
    content = content.replace('const string sql = """', 'var sql = $"""')
    content = content.replace("const string sql = \"", "var sql = $\"")
    content = ISNULL.sub(
        lambda m: '{SqlDialectQueryHelper.CoalesceEmpty(_dialect, "'
        + m.group(1).strip()
        + '")}',
        content,
    )
    content = BUILD_FILTER.sub(
        lambda m: 'SqlFilterHelper.BuildFilter('
        + re.sub(r"\s+", " ", m.group(1).strip())
        + f', parameters, "{m.group(2)}", {m.group(3)}, _dialect)',
        content,
    )
    content = content.replace("GETUTCDATE()", "{_dialect.UtcNowExpression}")
    content = content.replace("SYSUTCDATETIME()", "{_dialect.UtcNowExpression}")
    content = content.replace("NEWID()", "{_dialect.NewGuidExpression}")
    content = content.replace(" WITH (UPDLOCK, ROWLOCK)", "{_dialect.TableLockHint}")
    content = re.sub(
        r"(ORDER BY a\.\w+ DESC)(\";)",
        r"\1{_dialect.AppendLimit(_maxRows)}\2",
        content,
    )
    content = add_identity_after_values_missing_output(content, original)
    return content


def main() -> None:
    for name in FILES:
        path = ROOT / name
        original = path.read_text(encoding="utf-8")
        if "ISqlDialect dialect" in original and "SelectLimited" in original:
            print(f"SKIP {name}")
            continue
        updated = refactor(original)
        path.write_text(updated, encoding="utf-8")
        print(f"OK   {name}")


if __name__ == "__main__":
    main()
