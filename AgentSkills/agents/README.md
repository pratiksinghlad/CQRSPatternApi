# Agents

This folder is the canonical source of truth for agent definitions used by all AI tools and IDEs in this repository.

Do not duplicate agent content under `.github/`, `.copilot/`, `.codex/`, or other tool-specific folders.
Those tools should load agent definitions from this folder.

## Agent Locations (canonical paths)

| Agent | Canonical Path |
|---|---|
| architect | `AgentSkills/agents/architect.agent.md` |
| developer | `AgentSkills/agents/developer.agent.md` |

All agents and tools (Copilot, Codex, Claude, Cursor, Antigravity) load agents from `AgentSkills/agents/`.
