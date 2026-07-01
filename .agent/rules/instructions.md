---
trigger: always_on
---

# Agent Entry Point

This file is a lightweight entry point for IDE agent rules.

`AgentSkills/` is the single source of truth for skills, agents, memory,
prerequisites, checklists, and project rules used by all coding agents and
AI-enabled IDEs in this repository.

## Start Here

1. Read `AgentSkills/OPERATING.md`.
2. Follow skill routing in `AgentSkills/skills/INDEX.md`.
3. Load role definitions from `AgentSkills/agents/` only when the task requires them.

Do not duplicate canonical instructions in this file. Edit shared guidance in
`AgentSkills/`.
