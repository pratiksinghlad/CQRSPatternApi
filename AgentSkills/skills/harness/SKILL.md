---
name: harness
description: >
  Load when changing agent instructions, validation scripts, CI workflows,
  hooks, or repository operational guardrails.
---

# Harness Engineering

Harness engineering is the operational layer around the model:

```text
Agent = Model + Harness
```

Use this skill when adding or changing guide rails, validation, recovery, or feedback loops.

## Controls

| Control | Purpose | Repo examples |
|---|---|---|
| Guides | Feedforward controls that steer work before action | `AGENTS.md`, `AgentSkills/skills/INDEX.md`, `AgentSkills/agents/` |
| Sensors | Feedback controls that observe output after action | `tools/Harness/validate.ps1`, `.github/workflows/dotnet.yml`, `.editorconfig` |

## Rules

- Keep `AgentSkills/` as the canonical source of truth.
- Tool-specific folders may contain lightweight entry points only.
- Prefer one shared validation command over duplicated CI, hook, and README commands.
- Sensors must be deterministic first: restore, build, test, format.
- Use inferential review only after computational checks pass.
- Keep harness changes small, readable, and easy to remove.

## Validation

Run the shared sensor before finishing changes:

```powershell
pwsh ./tools/Harness/validate.ps1
```

For quick local loops:

```powershell
pwsh ./tools/Harness/validate.ps1 -Configuration Debug -SkipFormat
```
