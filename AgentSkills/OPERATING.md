# Coding Agent Operating Contract

This file is the canonical prerequisite and completion workflow for every coding
agent and AI-enabled IDE working in this repository.

Tool-specific files such as `AGENTS.md`, `CLAUDE.md`, `.github/copilot-instructions.md`,
`.agent/rules/instructions.md`, `.copilot/`, and `.codex/` are lightweight entry
points only. They must route agents here and must not duplicate skill, agent,
memory, checklist, or project-rule content.

## Applies To

This contract applies to Codex, GitHub Copilot, Claude, Cursor, Antigravity,
local agent harnesses, CI hooks, and any future coding agent integration.

## Task Start Prerequisites

Before changing files or producing a final answer, every agent must complete this
bootstrap sequence:

1. Read `AgentSkills/memory/index.md`, then load only the domain lesson file(s)
   that match the task.
2. Read `AgentSkills/skills/INDEX.md`.
3. Always load `AgentSkills/skills/core/SKILL.md`.
4. Load any task-specific skill listed in the skill index.
5. Load `AgentSkills/skills/harness/SKILL.md` when changing instructions,
   validation scripts, CI workflows, hooks, or operational guardrails.
6. Load agent definitions from `AgentSkills/agents/` only when the task needs a
   role-specific agent:
   - `AgentSkills/agents/architect.agent.md` for design, CQRS structure, SQL
     schema, or MCP contract planning.
   - `AgentSkills/agents/developer.agent.md` for .NET C# implementation work.
7. Inspect the local repository context before editing, including relevant files
   and current worktree state.
8. Define the verification path for the task before making changes.

If the request is ambiguous and a safe, local assumption is not possible, ask a
short clarifying question before editing.

## Working Rules

- Keep `AgentSkills/` as the single source of truth for skills, agents, memory,
  prerequisites, project rules, and completion gates.
- Use repo-relative paths in shared documentation and configuration.
- Keep changes scoped to the user request and the files required to satisfy it.
- Preserve user changes already present in the worktree.
- Prefer the repository's existing patterns over new abstractions.
- Update public documentation only when behavior, setup, commands, or usage
  changes.
- Record a memory lesson only when a mistake was made or a durable
  project-specific rule was discovered.

## Completion Gate

Before marking work complete, every coding agent must:

1. Run the pre-submit checklist in `AgentSkills/skills/core/SKILL.md`.
2. Run the shared harness sensor when code, project files, instructions, CI,
   hooks, or validation behavior changed:

   ```powershell
   pwsh ./tools/Harness/validate.ps1
   ```

3. For quick local loops during development, the harness may be run in Debug
   mode with formatting skipped, but the final pass should use the full command
   unless the task is documentation-only or the environment cannot run it:

   ```powershell
   pwsh ./tools/Harness/validate.ps1 -Configuration Debug -SkipFormat
   ```

4. If the full harness cannot be run, report the exact command that was skipped
   or failed and the reason.
5. Do not claim completion when required checks fail. List failures clearly and
   leave the work in a reviewable state.

## Final Response

The final response should state:

- What changed.
- Which validation commands were run.
- Any failures, skipped checks, or follow-up risks.

