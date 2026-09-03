#!/usr/bin/env python3
"""Finish M3 cleanup: UDF leftover, brackets, missing Employee JOIN, identity suffix."""
from __future__ import annotations

import re
from pathlib import Path

REPO = Path(__file__).resolve().parents[1] / "Repository"

M3_FILES = [
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
    "EmployeeContractRepository.cs",
    "EmployeeTrainingRepository.cs",
    "EmployeeCertificationRepository.cs",
    "EmployeeAssetRepository.cs",
]

OUTER_APPLY = re.compile(
    r"OUTER APPLY \(\s*"
    r"SELECT StandardReferenceItemName\s*"
    r"FROM \[app\]\.\[fn_GetStandardReferenceItems\]\([^,]+,\s*[^,]+,\s*[^,]+,\s*'([^']+)'\)\s*"
    r"WHERE StandardReferenceItemId = a\.(\w+)\s*"
    r"\) (\w+)",
    re.IGNORECASE | re.DOTALL,
)

QUALIFIED = re.compile(r"\[(app|core)\]\.\[(\w+)\]")
BRACKET = re.compile(r"\[(\w+)\]")
IDENTITY_SEMI = re.compile(
    r"\);\s*\n(\s*)\{dialect\.ReturnIdentitySuffix\(",
)

EMPLOYEE_JOIN = (
    'LEFT JOIN {dialect.Qualify("app", "Employee")} j_EmployeeId '
    "ON a.EmployeeId = j_EmployeeId.EmployeeId"
)


def replace_outer_apply(match: re.Match) -> str:
    initial, fk, alias = match.group(1), match.group(2), match.group(3)
    return (
        "{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup("
        + f'dialect, "{alias}", "{fk}", "{initial}"'
        + ")}"
    )


def replace_qualified(match: re.Match) -> str:
    return '{dialect.Qualify("' + match.group(1) + '", "' + match.group(2) + '")}'


def add_employee_join(content: str) -> str:
    if "j_EmployeeId" not in content:
        return content
    if "j_EmployeeId ON a.EmployeeId" in content:
        return content

    def inject(match: re.Match) -> str:
        prefix = match.group(0)
        if EMPLOYEE_JOIN in prefix:
            return prefix
        return prefix + EMPLOYEE_JOIN + "\n"

    return re.sub(r"FROM \{\_table\} a\n", inject, content)


def cleanup(content: str) -> str:
    content = content.replace('Qualify("core", "rovince")', 'Qualify("core", "Province")')
    content = OUTER_APPLY.sub(replace_outer_apply, content)
    content = QUALIFIED.sub(replace_qualified, content)
    content = BRACKET.sub(r"\1", content)
    content = IDENTITY_SEMI.sub(r"){dialect.ReturnIdentitySuffix(", content)
    content = add_employee_join(content)
    return content


def main() -> None:
    for name in M3_FILES:
        path = REPO / name
        original = path.read_text(encoding="utf-8")
        updated = cleanup(original)
        if updated != original:
            path.write_text(updated, encoding="utf-8")
            print(f"UPDATED {name}")
        else:
            print(f"OK      {name}")


if __name__ == "__main__":
    main()
