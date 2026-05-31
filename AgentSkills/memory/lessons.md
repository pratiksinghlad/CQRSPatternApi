# Agent Memory & Lessons Learned

This file is the persistent memory space for all AI agents (Claude, Codex, Copilot, Antigravity, etc.) working on this repository. 

> [!IMPORTANT]
> **Agent Instructions:**
> 1. **Read this file** at the start of every task to load past context, project-specific quirks, and avoid repeating previous mistakes.
> 2. **Update this file** at the end of a task if you made a mistake, encountered an unexpected bug, or spent time correcting a self-inflicted issue. Add a new entry under [Lessons Logged](#lessons-logged).

---

## Logging Schema for New Memories
When recording a new lesson/mistake, add it using the following format:
```markdown
### [YYYY-MM-DD] [Brief descriptive title of the issue]
- **Mistake/Symptom:** What went wrong? What errors were shown?
- **Root Cause:** Why did it happen? (e.g., circular redirect, wrong package version, missing configuration).
- **Prevention/Rule:** How do we avoid it? (Write a concrete rule that future agents can read and follow).
```

---

## Lessons Logged

### 2026-05-31 Agent Mirror Redirect Setup
- **Mistake/Symptom:** Overwrote canonical skill files in `AgentSkills/` with redirect stubs, creating circular redirects pointing to themselves, and accidentally deleted the agent files from `AgentSkills/agents/`.
- **Root Cause:** Did not verify paths carefully before writing redirect files, assuming `AgentSkills/` files were already populated and only `.github/` files were duplicates.
- **Prevention/Rule:** Never delete or overwrite files in `AgentSkills/` without verifying they are backed up elsewhere. `AgentSkills/` is the single source of truth; all mirror folders (`.github/skills/`, `.codex/skills/`, `.copilot/skills/`) are redirects pointing to it.

### 2026-05-31 CQRS Implementation
- **Mistake/Symptom:** Business logic or EF Core queries placed directly in Controllers or MediatR Handlers.
- **Root Cause:** Bypassing Domain Entities and repositories in favor of fast endpoint creation.
- **Prevention/Rule:** Controllers only dispatch commands/queries via MediatR. Handlers handle request translation/persistence boundaries. All business rules belong inside Domain Entities or Domain Services.
