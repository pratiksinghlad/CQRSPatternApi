# AgentSkills: Unified Skill & Memory Workspace

This directory is the single, shared source of truth for all AI agents (Claude, Codex, Copilot, Antigravity, etc.) interacting with this repository.

---

## Folder Structure

```
AgentSkills/
├── skill/                  ← Custom developer skills (best practices, guidelines)
│   ├── INDEX.md            ← Index of all skills and when to load them
│   ├── core/               ← Core principles (think before coding, checklist)
│   ├── code-standards/     ← Coding standards (no magic values, naming, DRY)
│   ├── design/             ← SOLID, KISS, YAGNI, and design pattern rules
│   ├── dotnet-best-practices/ ← General .NET and EF Core best practices
│   ├── csharp-xunit/       ← Unit testing guidelines using xUnit
│   ├── mcp_dotnet/         ← Model Context Protocol tool rules
│   └── dotnet-api/         ← RESTful API URL structure, pipeline, and OpenAPI rules
│
├── agents/                 ← Custom agent definition personas
│   ├── README.md           ← Mirror redirect instructions
│   ├── architect.agent.md  ← Structure and design planning agent
│   └── developer.agent.md  ← C# / .NET implementation agent
│
└── memory/                 ← Persistent agent memories
    └── lessons.md          ← Log of past mistakes, bugs, and rules to prevent them
```

---

## How It Works

1. **Task Initialization**:
   - The agent reads [AgentSkills/memory/lessons.md](file:///d:/Code/API/CQRSPatternApi/AgentSkills/memory/lessons.md) to load past lessons learned from mistakes on this specific project.
   - The agent reads [AgentSkills/skill/INDEX.md](file:///d:/Code/API/CQRSPatternApi/AgentSkills/skill/INDEX.md) to load the appropriate skills for the current task.

2. **Core Checklist**:
   - Before finishing any task, the agent runs the checklist in [AgentSkills/skill/core/SKILL.md](file:///d:/Code/API/CQRSPatternApi/AgentSkills/skill/core/SKILL.md).

3. **Memory Update**:
   - If the agent makes a mistake, runs into a build/test issue, or corrects an implementation error, it logs the incident in [lessons.md](file:///d:/Code/API/CQRSPatternApi/AgentSkills/memory/lessons.md) to prevent repeat occurrences.

4. **Integration Hooks**:
   - A post-tool-use hook is configured in `.github/hooks/build_test_lint.json` to automatically compile, run unit tests, and perform style formatting checks/fixes after any code modification.
