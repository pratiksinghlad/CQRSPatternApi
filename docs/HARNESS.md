# Harness Setup

This repo uses a small model harness:

```text
Agent = Model + Harness
```

## Guides

- `AGENTS.md` is the root entry point for Codex and others.
- `AgentSkills/skills/INDEX.md` routes tasks to the right skills.
- `AgentSkills/agents/` defines the architect and developer agents.
- `AgentSkills/memory/index.md` routes lessons learned by domain.

## Sensors

Run the shared validation sensor before merging:

```powershell
pwsh ./tools/Harness/validate.ps1
```

It restores packages, builds with warnings as errors, runs tests, and verifies formatting.

## Optimized Loop

Use this during active development:

```powershell
pwsh ./tools/Harness/validate.ps1 -Configuration Debug -SkipFormat
```

Use the full command before the final handoff or pull request.

## Improvement Backlog

- Add coverage thresholds after the existing test suite is stable.
- Add architecture tests for MCP tool exposure and CQRS boundaries.
- Add inferential review as a separate step after deterministic checks pass.
