#!/usr/bin/env python3
"""Fix broken regex output from refactor-m2-repositories.py"""
from pathlib import Path
import re

REPO_ROOT = Path(__file__).resolve().parents[1] / "Repository"

def fix_file(path: Path) -> bool:
    text = path.read_text(encoding="utf-8")
    original = text

    # Broken capture group literal
    text = text.replace(
        '{SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, \\"\\1\\")}',
        '{SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}',
    )
    text = text.replace(
        '{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, \\"\\1\\")}',
        '{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}',
    )

    # Broken ORDER BY closing quote
    text = re.sub(
        r"\{dialect\.AppendLimit\(_maxRows\)\}\\\";",
        r'{dialect.AppendLimit(_maxRows)}";',
        text,
    )
    text = re.sub(
        r"\{_dialect\.AppendLimit\(_maxRows\)\}\\\";",
        r'{_dialect.AppendLimit(_maxRows)}";',
        text,
    )

    # Broken CoalesceEmpty backrefs
    text = re.sub(
        r'\{SqlDialectQueryHelper\.CoalesceEmpty\((dialect|_dialect), \\1\)\}',
        r'{SqlDialectQueryHelper.CoalesceEmpty(\1, \1)}',  # wrong - need manual
        text,
    )

    if text != original:
        path.write_text(text, encoding="utf-8")
        return True
    return False


def main():
    count = 0
    for path in REPO_ROOT.rglob("*.cs"):
        if fix_file(path):
            count += 1
            print(path.relative_to(REPO_ROOT))
    print(f"Fixed {count} files")


if __name__ == "__main__":
    main()
