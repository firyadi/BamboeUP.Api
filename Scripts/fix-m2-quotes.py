#!/usr/bin/env python3
from pathlib import Path

REPO = Path(__file__).resolve().parents[1] / "Repository"

for path in REPO.rglob("*.cs"):
    text = path.read_text(encoding="utf-8")
    original = text
    text = text.replace('parameters, \\"', 'parameters, "')
    text = text.replace('dialect.Qualify(\\"app"', 'dialect.Qualify("app"')
    text = text.replace('dialect.Qualify(\\"core"', 'dialect.Qualify("core"')
    text = text.replace(', dialect, dialect)', ', dialect)')
    text = text.replace(', _dialect, _dialect)', ', _dialect)')
    if text != original:
        path.write_text(text, encoding="utf-8")
        print(path.relative_to(REPO))

print("done")
