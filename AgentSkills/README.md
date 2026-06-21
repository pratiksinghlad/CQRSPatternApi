# AgentSkills: Unified Skill & Memory Workspace

This directory is the single, shared source of truth for all AI agents (Claude, Codex, Copilot, Antigravity, etc.) interacting with this repository.

---

## Folder Structure

```
AgentSkills/
├── skills/                 ← Custom developer skills (best practices, guidelines)
│   ├── INDEX.md            ← Index of all skills and when to load them
│   ├── core/               ← Core principles (think before coding, checklist)
│   ├── code-standards/     ← Coding standards (no magic values, naming, DRY)
│   ├── design/             ← SOLID, KISS, YAGNI, and design pattern rules
│   ├── dotnet-best-practices/ ← General .NET and EF Core best practices
│   ├── csharp-xunit/       ← Unit testing guidelines using xUnit
│   ├── mcp_dotnet/         ← Model Context Protocol tool rules
│   ├── dotnet-api/         ← RESTful API URL structure, pipeline, and OpenAPI rules
│   └── harness/            ← Guide and sensor setup for agent operations
│
├── agents/                 ← Custom agent definition personas
│   ├── README.md           ← Agent index and canonical path instructions
│   ├── architect.agent.md  ← Structure and design planning agent
│   └── developer.agent.md  ← C# / .NET implementation agent
│
└── memory/                 ← Persistent agent memories
    ├── index.md            ← Router: always load this first; maps task type to domain file
    ├── schema.md           ← Lesson logging format (load only when writing a lesson)
    └── lessons/
        ├── api.md          ← CQRS, MediatR, commands, queries, controllers
        ├── db.md           ← EF Core, migrations, schema, repositories
        ├── infra.md        ← Build, paths, CI, tooling, portability
        ├── auth.md         ← Authentication, authorisation, JWT
        ├── testing.md      ← xUnit, mocking, coverage
        ├── csharp.md       ← C# language features, naming, async/await
        ├── design.md       ← SOLID, CQRS, patterns, architecture decisions
        └── mcp.md          ← MCP server tools, transports, contracts
```

---

## How It Works

1. **Task Initialization**:
   - The agent reads [AgentSkills/memory/index.md](memory/index.md) to identify relevant domains, then loads only the matching domain file(s) from `memory/lessons/`.
   - The agent reads [AgentSkills/skills/INDEX.md](skills/INDEX.md) to load the appropriate skills for the current task.

2. **Core Checklist**:
   - Before finishing any task, the agent runs the checklist in [AgentSkills/skills/core/SKILL.md](skills/core/SKILL.md).

3. **Memory Update**:
   - If the agent makes a mistake or discovers a project-specific rule, it reads [memory/schema.md](memory/schema.md) for the one-line format, appends to the correct domain file, and increments the count in `index.md`.

4. **Tool Entry Points**:
   - Codex reads `AGENTS.md`.
   - Claude reads `CLAUDE.md`.
   - GitHub Copilot reads `.github/copilot-instructions.md`.
   - Tool-specific files point back to `AgentSkills/`; skill and agent content is not duplicated.

5. **Integration Hooks**:
   - A post-tool-use hook is configured in `.github/hooks/build_test_lint.json` to run the shared harness sensor at `tools/Harness/validate.ps1`.
